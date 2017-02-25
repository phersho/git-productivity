using Blitz.Core.BusinessLogic.Dynamic;
using Blitz.Core.Enums;
using Blitz.Core.Infrastructure.Connectors;
using Blitz.Core.Model.Connections;
using Blitz.Core.Model.Dynamic;
using Blitz.Core.Model.Enum;
using Blitz.Core.Repository.Interface.Dynamic;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blitz.Core.Infrastructure
{
    public static class ControllerHelper
    {
        public static RecordLevelSecurityEnum GetAccessType(string roleCode)
        {
            if (roleCode == RolesCodes.TA.ToString())
            {
                return RecordLevelSecurityEnum.NoRestriction;
            }
            else if (roleCode == RolesCodes.SM.ToString())
            {
                return RecordLevelSecurityEnum.Subordinates;
            }
            // This is for RolesCodes.SS.ToString()
            else
            {
                return RecordLevelSecurityEnum.Self;
            }
        }

        public static RecordLevelSecurityEnum GetAccessType(string roleCode, SchemaView view, List<DynamicRecord> permissions)
        {
            try
            {
                var viewAccessCode = permissions.Where(p => p.ContainsKey(roleCode) && (bool)p[roleCode] && p["View"]?.ToString() == view.ViewFullName)
                            .Select(p => (string)p[string.Format("{0}_AccessType", roleCode)]?.ToString())   //TA_AccessType / SP_AccessType
                            .FirstOrDefault();
                if (viewAccessCode != null)
                {
                    return (RecordLevelSecurityEnum)Enum.Parse(typeof(RecordLevelSecurityEnum), viewAccessCode);
                }
                else {
                    return GetAccessType(roleCode);
                }
            }
            catch
            {
                return GetAccessType(roleCode);
            }
        }

        public static byte[] GetFileBytes(string schemaName, SchemaView view)
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(schemaName);

                IEnumerable<dynamic> headers = view.Attributes.Where(a => a.Value.IsVisible == true).Select((a, i) => new { Index = i + 1, ColumnName = a.Value.Title, AttributeName = a.Key, Format = "string" });
                headers.ToList().ForEach(h => ws.Cells[1, h.Index].Value = h.ColumnName);

                //format columns  (TODO: analudg. when format exists at the Column Schema level)
                headers.Where(h => h.Format == "Numeric").ToList().ForEach(h =>
                {
                    ws.Cells[2, h.Index].Style.Numberformat.Format = "#,##0.00";
                    ws.Cells[2, h.Index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                });
                return pck.GetAsByteArray();
            }
        }

        public static ObjectId DeepCreate(DynamicRecord record, SchemaView view, ObjectId tenantId, ObjectId loggedUser, string roleCode, IDynamicService<DynamicRecord> dynamicService, ISchemaService schemaService, bool allowUpdate = false)
        {
            return DeepCreate(null, record, view, tenantId, loggedUser, roleCode, dynamicService, schemaService, allowUpdate);
        }

        public static ObjectId DeepCreate(DynamicRecord parentRecord, DynamicRecord record, SchemaView view, ObjectId tenantId, ObjectId loggedUser, string roleCode, IDynamicService<DynamicRecord> dynamicService, ISchemaService schemaService, bool allowUpdate = false)
        {
            var complexRecords = record.Values().Where(v => view.Schema.Attributes.ContainsKey(v.Key) && (view.Schema.Attributes[v.Key].IsComplex || view.Schema.Attributes[v.Key].IsSubCatalog));

            complexRecords.ToList().ForEach(r =>
            {
                var subcatalogView = schemaService.GetView(view.Schema.Attributes[r.Key].DataType, ViewCategory.Create, tenantId, roleCode);  //this should go to cache all of the time, it would be expensive not to.

                // When manually creating an invoice, we need to check if Count > 0, otherwise FindExisting throws an exception
                if (r.Value is Dictionary<string, object> && ((Dictionary<string, object>)r.Value).Count > 0)
                {
                    var subRecord = new DynamicRecord().Initialize<DynamicRecord>((Dictionary<string, object>)r.Value, null, ViewCategory.Create);
                    bool isValidInput = true;
                    subRecord._id = FindExisting(subRecord, subcatalogView, tenantId, loggedUser, dynamicService, out isValidInput);
                    if (subcatalogView != null && subcatalogView.Schema.AllowQuickCreate && subRecord._id == ObjectId.Empty && isValidInput)
                    {
                        var fieldId = DeepCreate(parentRecord ?? record, subRecord, subcatalogView, tenantId, loggedUser, roleCode, dynamicService, schemaService);
                        var field = ((Dictionary<string, object>)r.Value);
                        var arrayOfKeys = field.Keys.ToArray();
                        foreach (var k in arrayOfKeys)
                        {
                            if (subRecord.ContainsKey(k))
                            {
                                field[k] = subRecord[k];
                            }
                        }
                    }
                    else if (subRecord._id != ObjectId.Empty)
                    {
                        ((Dictionary<string, object>)r.Value)["_id"] = subRecord._id;
                    }
                }
                else if (subcatalogView?.Schema != null && subcatalogView.Schema.AllowQuickCreate && r.Value is List<Dictionary<string, object>> && ((List<Dictionary<string, object>>)r.Value).Count > 0)
                {
                    ((List<Dictionary<string, object>>)r.Value).ForEach(v =>
                    {
                        var subRecord = new DynamicRecord().Initialize<DynamicRecord>(v, subcatalogView, ViewCategory.Create);
                        var fieldId = DeepCreate(parentRecord ?? record, subRecord, subcatalogView, tenantId, loggedUser, roleCode, dynamicService, schemaService, true);
                        var field = v;
                        var arrayOfKeys = field.Keys.ToArray();
                        foreach (var k in arrayOfKeys)
                        {
                            if (subRecord.ContainsKey(k))
                            {
                                field[k] = subRecord[k];
                            }
                        }
                    });
                }
            });

            if (record._id != ObjectId.Empty && allowUpdate)
            {
                record.ParentRecord = parentRecord;
                dynamicService.Update(record, view.Schema.CollectionName, view.Schema.Name, tenantId, loggedUser, roleCode);
                return record._id;
            }
            else
            {
                var isValidInput = false;
                record._id = FindExisting(record, view, tenantId, loggedUser, dynamicService, out isValidInput);
                if (record._id == ObjectId.Empty)
                {
                    record.ParentRecord = parentRecord;
                    return dynamicService.Add(record, view.Schema.Name, tenantId, loggedUser, roleCode);
                }
                else if (allowUpdate)
                {
                    record.ParentRecord = parentRecord;
                    dynamicService.Update(record, view.Schema.CollectionName, view.Schema.Name, tenantId, loggedUser, roleCode);
                    return record._id;
                }
                else
                {
                    return ObjectId.Empty;
                }
            }
        }

        public static ObjectId FindExisting(DynamicRecord record, SchemaView view, ObjectId tenantId, ObjectId loggedUser, IDynamicService<DynamicRecord> dynamicService, out bool isValidInput)
        {
            isValidInput = true;
            //check if the current record exists... only check for quick create ones
            if ((record._id == null || record._id == ObjectId.Empty) && view != null && view.Schema.UniqueIndex != null)
            {
                var filter = view.Schema.UniqueIndex.ToDictionary(v => v, v =>
                    v.Contains("$") ?
                        //v.Split('$')[0] = View  (View$Name)
                        //v.Split('$')[1] = Name
                        (record[v.Split('$')[0]] as IDictionary<string, object>)[v.Split('$')[1]].ToString() :
                        record[v]?.ToString()   
                );

                // If all filters are empty that's because we're receiving an object without a unique index,
                // which can't be compared
                if (filter.All(x => string.IsNullOrEmpty(x.Value)))
                {
                    // We received something without a valid uniqueIndex
                    isValidInput = false;
                }
                else
                {
                    var existingRecords = dynamicService.GetAll(view, tenantId, loggedUser, RecordLevelSecurityEnum.NoRestriction).ToList();
                    var existingRecord = existingRecords.Where(e => e.Contains(filter)).ToList();

                    if (existingRecord != null && existingRecord.Count() > 0)
                    {
                        return existingRecord.First()._id;
                    }
                }
            }
            return ObjectId.Empty;
        }

        public static ObjectId DeepCreate(DynamicRecord record, SchemaView view, ObjectId tenantId, ObjectId loggedUser, IDynamicRepository dynamicRepository, Dictionary<string, SchemaView> views, bool allowUpdate = false)
        {
            return DeepCreate(null, record, view, tenantId, loggedUser, dynamicRepository, views, allowUpdate);
        }

        public static ObjectId DeepCreate(DynamicRecord parentRecord, DynamicRecord record, SchemaView view, ObjectId tenantId, ObjectId loggedUser, IDynamicRepository dynamicRepository, Dictionary<string, SchemaView> views, bool allowUpdate = false)
        {
            var complexRecords = record.Values().Where(v => view.Schema.Attributes.ContainsKey(v.Key) && (view.Schema.Attributes[v.Key].IsComplex || view.Schema.Attributes[v.Key].IsSubCatalog));

            complexRecords.ToList().ForEach(r =>
            {
                var subcatalogView = views.FirstOrDefault(x => x.Key == string.Format("{0}.{1}.{2}", view.Schema.Attributes[r.Key].DataType, ViewCategory.Create.ToString(), "Default")).Value;  //this should go to cache all of the time, it would be expensive not to.

                // When manually creating an invoice, we need to check if Count > 0, otherwise FindExisting throws an exception
                if (r.Value is Dictionary<string, object> && ((Dictionary<string, object>)r.Value).Count > 0)
                {
                    var subRecord = new DynamicRecord().Initialize<DynamicRecord>((Dictionary<string, object>)r.Value, null, ViewCategory.Create);
                    bool isValidInput = true;
                    subRecord._id = FindExisting(subRecord, subcatalogView, tenantId, loggedUser, dynamicRepository, out isValidInput);
                    if (subcatalogView.Schema.AllowQuickCreate && subRecord._id == ObjectId.Empty && isValidInput)
                    {
                        ((Dictionary<string, object>)r.Value)["_id"] = DeepCreate(parentRecord ?? record, subRecord, subcatalogView, tenantId, loggedUser, dynamicRepository, views);
                    }
                    else if (subRecord._id != ObjectId.Empty)
                    {
                        ((Dictionary<string, object>)r.Value)["_id"] = subRecord._id;
                    }
                }
                else if (r.Value is List<Dictionary<string, object>> && ((List<Dictionary<string, object>>)r.Value).Count > 0)
                {
                    ((List<Dictionary<string, object>>)r.Value).ForEach(v =>
                    {
                        var newId = DeepCreate(parentRecord ?? record, new DynamicRecord().Initialize<DynamicRecord>(v, subcatalogView, ViewCategory.Create), subcatalogView, tenantId, loggedUser, dynamicRepository, views, true);
                        v["_id"] = newId;
                    });
                }
            });

            if (record._id != ObjectId.Empty && allowUpdate)
            {
                record.ParentRecord = parentRecord;
                dynamicRepository.Edit(record, view.Schema.Name, tenantId, loggedUser, views);
                return record._id;
            }
            else
            {
                var isValidInput = false;
                record._id = FindExisting(record, view, tenantId, loggedUser, dynamicRepository, out isValidInput);
                if (record._id == ObjectId.Empty)
                {
                    record.ParentRecord = parentRecord;
                    var returnId = dynamicRepository.AddReturnId(record, view.Schema.Name, tenantId, loggedUser, views);
                    return new ObjectId(returnId);
                }
                else if (allowUpdate)
                {
                    record.ParentRecord = parentRecord;
                    dynamicRepository.Edit(record, view.Schema.Name, tenantId, loggedUser, views);
                    return record._id;
                }
                else
                {
                    return ObjectId.Empty;
                }
            }
        }

        public static ObjectId FindExisting(DynamicRecord record, SchemaView view, ObjectId tenantId, ObjectId loggedUser, IDynamicRepository dynamicRepository, out bool isValidInput)
        {
            isValidInput = true;
            //check if the current record exists... only check for quick create ones
            if ((record._id == null || record._id == ObjectId.Empty) && view.Schema.UniqueIndex != null)
            {
                var filter = view.Schema.UniqueIndex.ToDictionary(v => v, v => record[v]?.ToString());

                // If all filters are empty that's because we're receiving an object without a unique index,
                // which can't be compared
                if (filter.All(x => string.IsNullOrEmpty(x.Value)))
                {
                    // We received something without a valid uniqueIndex
                    isValidInput = false;
                }
                else
                {
                    var existingRecords = dynamicRepository.GetAll<DynamicRecord>(view.Schema.Name, tenantId, null);
                    var existingRecord = existingRecords.Where(e => e.Contains(filter)).ToList();

                    if (existingRecord != null && existingRecord.Count() > 0)
                    {
                        return existingRecord.First()._id;
                    }
                }
            }
            return ObjectId.Empty;
        }

        public static Dictionary<string, object> SetComplexValue(object value, ColumnSchema attribute, ObjectId tenantId, ObjectId userId, 
            string roleCode, IDynamicRepository dynamicRepository, IDynamicService<DynamicRecord> dynamicService, ISchemaService schemaService)
        {
            var complexAttributeView = schemaService.GetView(attribute.DataType, ViewCategory.Create, tenantId, roleCode);  //TODO: gutierfe.  I think it should be .Catalog.
            var complexAttributeSchema = complexAttributeView.Schemas[attribute.DataType];
            var isValid = true;
            //Create complex field value
            var fieldValue = new Dictionary<string, object>() { { complexAttributeSchema.UniqueIndex.Count() == 1 ? complexAttributeSchema.UniqueIndex.First() : attribute.ComplexFieldName, value } };   //assumming it has only one unique index, otherwise, use complex value field
            var dynamicRecordValue = new DynamicRecord().Initialize<DynamicRecord>(fieldValue as Dictionary<string, object>, null, ViewCategory.Catalog);
            ThrowExceptionIfViewIsNull(complexAttributeView);
            var id = ControllerHelper.FindExisting(dynamicRecordValue, complexAttributeView, tenantId, userId, dynamicService, out isValid);
            if (id != ObjectId.Empty)
            {
                dynamicRecordValue = dynamicRepository.Get<DynamicRecord>(attribute.DataType, id, tenantId);
            }
            return dynamicRecordValue.Values();
        }

        public static void ThrowExceptionIfViewIsNull(SchemaView view)
        {
            if (view == null)
            {
                throw new Exception("This view is not accesible");
            }
        }

        public static bool SendDocumentToSign(ObjectId tenantId, IDynamicRepository dynamicRepository, string fromEmail, string toEmail, string filePath)
        {
            var configuration = Utilities.Instance.GetConfiguration();

            var subscriptionType = SubscriptionTypes.SignNow.ToString();

            //  Retrieve subscription by type...and user in the current tenant
            var subscription = dynamicRepository.GetAll<Subscription>(SchemaNames.Subscription.ToString(), tenantId).Select(r => (Subscription)new Subscription().Initialize<Subscription>(r.Values()))
                .FirstOrDefault(x => x.IsActive && x.Type.Name == subscriptionType);

            //  Update subscription
            var accessToken = subscription.Token;
            var refreshToken = subscription.Secret;

            ISignNowService signNowService = new SignNowClient(tenantId, configuration.SignNowApiBaseUrl, accessToken);

            #region Retrieve user and extract email

            var userResponse = signNowService.RetrieveUser();
            if (userResponse == null)
                return false;

            var email = string.Empty;
            var userJson = JObject.Parse(userResponse);
            var emails = userJson.SelectTokens("emails").FirstOrDefault();
            if (emails == null)
            {
                email = fromEmail;
            }
            else
            {
                email = (string)emails.FirstOrDefault();
            }

            #endregion

            #region Document Signature Information

            dynamic documentSignatureInfo = new
            {
                fields = new[]
                {
                    new
                    {
                        x = 370,
                        y = 708,
                        width = 208,
                        height = 75,
                        page_number = 0,
                        role = "Customer",
                        required = true,
                        type = "signature"
                    }
                }
            };

            #endregion

            #region Invite Information

            dynamic inviteInfo = new
            {
                to = new[]
                {
                    new
                    {
                        email = toEmail,
                        role_id = "",
                        role = "Customer",
                        order = 1,
                    }
                },
                from = email,
                subject = configuration.SignNowInviteSubject,
                message = configuration.SignNowInviteMessage
            };

            #endregion

            return signNowService.SendRoleBasedInvite(filePath, documentSignatureInfo, inviteInfo);
        }
    }
}
