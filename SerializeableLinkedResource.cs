using System;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;

namespace api.Core.Mailer
{
    [Serializable]
    public class SerializeableLinkedResource
    {
        private readonly  String ContentId;
        private readonly Uri ContentLink;
        private readonly Stream ContentStream;
        private readonly SerializeableContentType ContentType;
        private readonly TransferEncoding TransferEncoding;

        public SerializeableLinkedResource(LinkedResource linkedResource)
        {
            ContentId = linkedResource.ContentId;
            ContentLink = linkedResource.ContentLink;
            ContentType = new SerializeableContentType(linkedResource.ContentType);
            TransferEncoding = linkedResource.TransferEncoding;

            if (linkedResource.ContentStream != null)
            {
                var bytes = new byte[linkedResource.ContentStream.Length];
                linkedResource.ContentStream.Read(bytes, 0, bytes.Length);
                ContentStream = new MemoryStream(bytes);
            }
        }

        public LinkedResource GetLinkedResource()
        {
            return new LinkedResource(ContentStream)
                       {
                           ContentId = ContentId,
                           ContentLink = ContentLink,
                           ContentType = ContentType.GetContentType(),
                           TransferEncoding = TransferEncoding
                       };
        }
    }
}