using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.Serialization;

namespace api.Core.Mailer
{
    [Serializable]
    [DataContract]
    public class SerializeableAlternateView
    {
        private readonly Uri BaseUri;
        private readonly  String ContentId;
        private readonly Stream ContentStream;
        private readonly SerializeableContentType ContentType;
        private readonly IList<SerializeableLinkedResource> LinkedResources = new List<SerializeableLinkedResource>();
        private readonly TransferEncoding TransferEncoding;

        public SerializeableAlternateView(AlternateView alternativeView)
        {
            BaseUri = alternativeView.BaseUri;
            ContentId = alternativeView.ContentId;
            ContentType = new SerializeableContentType(alternativeView.ContentType);
            TransferEncoding = alternativeView.TransferEncoding;

            if (alternativeView.ContentStream != null)
            {
                byte[] bytes = new byte[alternativeView.ContentStream.Length];
                alternativeView.ContentStream.Read(bytes, 0, bytes.Length);
                ContentStream = new MemoryStream(bytes);
            }

            foreach (var lr in alternativeView.LinkedResources)
            {
                LinkedResources.Add(new SerializeableLinkedResource(lr));
            }
        }

        public AlternateView GetAlternateView()
        {
            var sav = new AlternateView(ContentStream)
                          {
                              BaseUri = BaseUri,
                              ContentId = ContentId,
                              ContentType = ContentType.GetContentType(),
                              TransferEncoding = TransferEncoding,
                          };

            foreach (var linkedResource in LinkedResources)
            {
                sav.LinkedResources.Add(linkedResource.GetLinkedResource());
            }

            return sav;
        }
    }
}