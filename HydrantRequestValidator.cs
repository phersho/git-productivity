using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Data;
using Data.Interfaces;
using models;

namespace api.Core
{
    public class HydrantRequestValidator
    {
        private readonly IFlowRateRepository _flowRateRepository;
        private readonly IWaterSourceRepository _waterSourceRepository;
        private readonly IHydrantRepository _hydrantRepository;

        public HydrantRequestValidator(IFlowRateRepository flowRateRepository,
                                        IWaterSourceRepository waterSourceType,
                                        IHydrantRepository hydrantRepository)
        {
            _flowRateRepository = flowRateRepository;
            _waterSourceRepository = waterSourceType;
            _hydrantRepository = hydrantRepository;
        }

        public async Task<List<string>> Validate(HydrantRequest hydrant)
        {
            var requiredFieldsErrors = GetMandatoryFieldErrors(hydrant);

            if (requiredFieldsErrors.Any())
            {
                return requiredFieldsErrors;
            }

            var formatErrors = IsValidHydrant(hydrant);

            if (formatErrors.Any())
            {
                return formatErrors;
            }

            var refereceErrors = await GetReferencedFieldErrors(hydrant);

            if (refereceErrors.Any())
            {
                return refereceErrors;
            }

            return new List<string>();
        }

        public async Task<bool> IsValidHydrantID(long HydrantID)
        {
            var HydrantsCollection = await _hydrantRepository.GetHydrantById(HydrantID);
            if (HydrantsCollection == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }



        public List<string> IsValidHydrant(HydrantRequest hydrant)
        {
            List<string> InvalidTypes = new List<string>();
            string[] OutletsNumber = { "1", "2", "3", "4", "4+" };
            int[] DirectionToOpen = { 1, 2 };           
            bool IsValidDirectionToOpen = false;
            bool IsValidOutLets = false;
            bool isIncompleteGeography = false;
            bool isEmptyGeography = false;
            if (!hydrant.IsUpdated)
            {
                isIncompleteGeography = hydrant.Latitude == 0 ||
                                             hydrant.Longitude == 0;
                isEmptyGeography = hydrant.Latitude == 0 &
                                        hydrant.Longitude == 0;
            }


            #region Mandatory fields format


            if (!(hydrant.HydrantID.GetType() == typeof(long)))
            {
                InvalidTypes.Add("Invalid Hydrant ID.");
            }
            if (!(hydrant.SubscriberID.GetType() == typeof(long)))
            {
                InvalidTypes.Add("Invalid Subscriber ID.");
            }

            if (!(hydrant.Status.GetType() == typeof(bool)))
            {
                InvalidTypes.Add("Invalid Status.");
            }
            if (!(hydrant.FlowRateID.GetType() == typeof(long)))
            {
                InvalidTypes.Add("Invalid FlowRateID.");
            }
        

            if ((isEmptyGeography) || (isIncompleteGeography)) 
            {                
                if (hydrant.Longitude == 0)
                {
                    InvalidTypes.Add("Invalid Longitude.");
                }
                if (hydrant.Latitude== 0)
                {
                    InvalidTypes.Add("Invalid Latitude.");
                }                
            }

            if (hydrant.Latitude == 0 & hydrant.Longitude == 0 && !hydrant.IsUpdated) 
            {              
                InvalidTypes.Add("Invalid Longitude.");
                InvalidTypes.Add("Invalid Latitude.");
            }
            

            if (!(hydrant.StreetNumber.GetType() == typeof(string)))
            {
                InvalidTypes.Add("Invalid Street Number.");
            }
            if (!(hydrant.StreetName.GetType() == typeof(string)))
            {
                InvalidTypes.Add("Invalid Street Name.");
            }
            if (!(hydrant.City.GetType() == typeof(string)))
            {
                InvalidTypes.Add("Invalid City.");
            }
            if (!(hydrant.State.GetType() == typeof(string)))
            {
                InvalidTypes.Add("Invalid State.");
            }

            #endregion

            #region Non-mandatory fields format

            if (hydrant.WaterSourceType != null)
            {
                if (hydrant.WaterSourceType.SubscriberID.GetType() != typeof(long)) InvalidTypes.Add("Subscriber id must be a number.");

                if (hydrant.WaterSourceType.WaterSourceTypeID.GetType() != typeof(long)) InvalidTypes.Add("Water source type id must be a number.");

                if (hydrant.WaterSourceType.Name.GetType() != typeof(string)) InvalidTypes.Add("Name must be text.");

            }

            if (hydrant.WaterSourceTypeID != null && !(hydrant.WaterSourceTypeID.GetType() == typeof(long)))
            {
                InvalidTypes.Add("Invalid Water Source Type ID.");
            }

            if (hydrant.Name != null && !(hydrant.Name.GetType() == typeof(string)))
            {
                InvalidTypes.Add("Invalid Name.");
            }

            if (hydrant.OutletsNumber != null)
            {
                foreach (string options in OutletsNumber)
                {
                    if (hydrant.OutletsNumber == options)
                    {
                        IsValidOutLets = true;
                        break;
                    }
                }

                if (!IsValidOutLets)
                {
                    InvalidTypes.Add("Invalid Outlets Number.");
                }
            }

            if (hydrant.OutletSize != null)
            {
                if (hydrant.OutletSize != null && hydrant.OutletSize == "")
                {
                    InvalidTypes.Add("Invalid Outlet Size.");
                }
            }

            if (hydrant.ThreadType != null && !(hydrant.ThreadType.GetType() == typeof(string)))
            {
                InvalidTypes.Add("Invalid Thread Type.");
            }

            if (hydrant.DirectionToOpen != null)
            {
                foreach (int options in DirectionToOpen)
                {
                    if (hydrant.DirectionToOpen == options)
                    {
                        IsValidDirectionToOpen = true;
                        break;
                    }
                }
                if (!IsValidDirectionToOpen)
                {
                    InvalidTypes.Add("Invalid Direction to Open.");
                }
            }

            if (hydrant.Notes != null && !(hydrant.Notes.GetType() == typeof(string)))
            {
                InvalidTypes.Add("Invalid Notes");
            }

            if (hydrant.ClosestCrossStreet != null && !(hydrant.ClosestCrossStreet.GetType() == typeof(string)))
            {
                InvalidTypes.Add("Invalid Closest Cross Street.");
            }

            #endregion

            return InvalidTypes;
        }

        public List<string> GetMandatoryFieldErrors(HydrantRequest hydrant)
        {
            List<string> errors = new List<string>();

            if(hydrant.SubscriberID == null) errors.Add("Invalid SubscriberId");

            if (hydrant.Status == null) errors.Add("Invalid hydrant status");

            if (hydrant.FlowRateID == null) errors.Add("Invalid flow rate");

            if (hydrant.Latitude == null) errors.Add("Invalid latitude");

            if (hydrant.Longitude == null) errors.Add("Invalid longitude");

            if (hydrant.StreetNumber == null) errors.Add("Invald street number");

            if (hydrant.StreetName == null) errors.Add("Invalid street name");

            if (hydrant.City == null) errors.Add("Invalid city");

            if (hydrant.State == null) errors.Add("Invalid state");

            if (hydrant.WaterSourceType != null)
            {
                if (hydrant.WaterSourceType.Name == null) errors.Add("Invalid new water source type name");

                if (hydrant.WaterSourceType.SubscriberID == null) errors.Add("Invalid subscriber id for water source type");

                if (hydrant.WaterSourceType.WaterSourceTypeID == null) errors.Add("Invalid water source type id");
            }

            return errors;
        }

        public async Task<List<string>> GetReferencedFieldErrors(HydrantRequest hydrant)
        {
            var errors = new List<string>();

            if (hydrant.WaterSourceType == null && hydrant.WaterSourceTypeID != null)
            {
                if (!(from w in await _waterSourceRepository.GetWaterSourceTypes(hydrant.SubscriberID)
                      where w.WaterSourceTypeID == hydrant.WaterSourceTypeID
                      select w).Any())
                {
                    errors.Add("Invalid water source type");
                    return errors;
                }
            }

            if (!(from f in await _flowRateRepository.GetFlowRates()
                  where f.FlowRateID == hydrant.FlowRateID
                  select f).Any())
            {
                errors.Add("Invalid flow rate");
            }

            return errors;
        }
    }
}