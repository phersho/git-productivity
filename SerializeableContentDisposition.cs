using System;
using System.Net.Mime;

namespace api.Core.Mailer
{
    [Serializable]
    public class SerializeableContentDisposition
    {
        private readonly  DateTime CreationDate;
        private readonly  String DispositionType;
        private readonly String FileName;
        private readonly Boolean Inline;
        private readonly DateTime ModificationDate;
        private readonly SerializeableCollection Parameters;
        private readonly DateTime ReadDate;
        private readonly long Size;

        public SerializeableContentDisposition(ContentDisposition contentDisposition)
        {
            CreationDate = contentDisposition.CreationDate;
            DispositionType = contentDisposition.DispositionType;
            FileName = contentDisposition.FileName;
            Inline = contentDisposition.Inline;
            ModificationDate = contentDisposition.ModificationDate;
            Parameters = new SerializeableCollection(contentDisposition.Parameters);
            ReadDate = contentDisposition.ReadDate;
            Size = contentDisposition.Size;
        }

        public void CopyTo(ContentDisposition contentDisposition)
        {
            contentDisposition.CreationDate = CreationDate;
            contentDisposition.DispositionType = DispositionType;
            contentDisposition.FileName = FileName;
            contentDisposition.Inline = Inline;
            contentDisposition.ModificationDate = ModificationDate;
            contentDisposition.ReadDate = ReadDate;
            contentDisposition.Size = Size;

            Parameters.CopyTo(contentDisposition.Parameters);
        }
    }
}