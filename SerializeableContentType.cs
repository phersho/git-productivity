using System;
using System.Net.Mime;

namespace api.Core.Mailer
{
    [Serializable]
    internal class SerializeableContentType
    {
        private readonly String Boundary;
        private readonly String CharSet;
        private readonly String MediaType;
        private readonly String Name;
        private readonly SerializeableCollection Parameters;

        public SerializeableContentType(ContentType contentType)
        {
            Boundary = contentType.Boundary;
            CharSet = contentType.CharSet;
            MediaType = contentType.MediaType;
            Name = contentType.Name;
            Parameters = new SerializeableCollection(contentType.Parameters);
        }

        public ContentType GetContentType()
        {
            var sct = new ContentType()
                          {
                              Boundary = Boundary,
                              CharSet = CharSet,
                              MediaType = MediaType,
                              Name = Name,
                          };

            Parameters.CopyTo(sct.Parameters);

            return sct;
        }
    }
}