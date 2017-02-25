using System;
using System.IO;
using System.Net.Mail;
using System.Text;

namespace api.Core.Mailer
{
    [Serializable]
    public class SerializeableAttachment
    {
        private readonly String ContentId;
        private readonly SerializeableContentDisposition ContentDisposition;
        private readonly SerializeableContentType ContentType;
        private readonly Stream ContentStream;
        private readonly System.Net.Mime.TransferEncoding TransferEncoding;
        private readonly String Name;
        private readonly Encoding NameEncoding;

        public SerializeableAttachment(Attachment attachment)
        {
            ContentId = attachment.ContentId;
            ContentDisposition = new SerializeableContentDisposition(attachment.ContentDisposition);
            ContentType = new SerializeableContentType(attachment.ContentType);
            Name = attachment.Name;
            TransferEncoding = attachment.TransferEncoding;
            NameEncoding = attachment.NameEncoding;

            if (attachment.ContentStream != null)
            {
                byte[] bytes = new byte[attachment.ContentStream.Length];
                attachment.ContentStream.Read(bytes, 0, bytes.Length);

                ContentStream = new MemoryStream(bytes);
            }
        }

        public Attachment GetAttachment()
        {
            var attachment = new Attachment(ContentStream, Name)
                                 {
                                     ContentId = ContentId,
                                     ContentType = ContentType.GetContentType(),
                                     Name = Name,
                                     TransferEncoding = TransferEncoding,
                                     NameEncoding = NameEncoding,
                                 };

            ContentDisposition.CopyTo(attachment.ContentDisposition);

            return attachment;
        }
    }
}