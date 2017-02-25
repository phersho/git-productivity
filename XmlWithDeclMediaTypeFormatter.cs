using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace api.Core
{
    public class XmlWithDeclMediaTypeFormatter : XmlMediaTypeFormatter
    {
        public XmlWithDeclMediaTypeFormatter()
        {
            UseXmlSerializer = true;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
            TransportContext transportContext)
        {
            WriteXmlDeclaration(writeStream);
            return base.WriteToStreamAsync(type, value, writeStream, content, transportContext);
        }

        private static void WriteXmlDeclaration(Stream writeStream)
        {
            const string XmlDeclaration = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            var declBytes = Encoding.UTF8.GetBytes(XmlDeclaration);
            writeStream.Write(declBytes, 0, declBytes.Length);
        }
    }
}