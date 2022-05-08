using System;
using System.Xml;
using System.Xml.Schema;

namespace IFP.Modules.PiguIntegration
{
    internal static class PiguXmlValidator
    {
        public static void Validate(XmlDocument doc)
        {
            // ToDo: Make Xml Validation
            //var path = new Uri(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
            //XmlSchemaSet schema = new XmlSchemaSet();
            //schema.Add("", path + "\\input.xsd");
            //XmlReader rd = XmlReader.Create(path + "\\input.xml");
            //XDocument docq = XDocument.Load(rd);
            //docq    .Validate(schema, ValidationEventHandler);
        }
        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            XmlSeverityType type = XmlSeverityType.Warning;
            if (Enum.TryParse("Error", out type))
            {
                if (type == XmlSeverityType.Error) throw new Exception(e.Message);
            }
        }
    }
}
