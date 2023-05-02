using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.Serialization.Advanced;

namespace CodeGeneration.ImporterExtensions
{
    public class SoapTypeExtension : SchemaImporterExtension
    {
        //readonly Dictionary<XmlSchemaType, string> generatedTypes = new Dictionary<XmlSchemaType, string>();

        public override string ImportSchemaType(
            string name,
            string ns,
            XmlSchemaObject context,
            XmlSchemas schemas,
            XmlSchemaImporter importer,
            CodeCompileUnit compileUnit,
            CodeNamespace mainNamespace,
            CodeGenerationOptions options,
            CodeDomProvider codeProvider)
        {
            if (ns != "http://www.w3.org/2001/XMLSchema")
                return null;

            switch (name)
            {
                case "anyURI":     return "System.Uri"; 
                case "gDay":       return "System.Runtime.Remoting.Metadata.W3cXsd2001.SoapDay";
                case "gMonth":     return "System.Runtime.Remoting.Metadata.W3cXsd2001.SoapMonth";
                case "gMonthDay":  return "System.Runtime.Remoting.Metadata.W3cXsd2001.SoapMonthDay";
                case "gYear":      return "System.Runtime.Remoting.Metadata.W3cXsd2001.SoapYear"; 
                case "gYearMonth": return "System.Runtime.Remoting.Metadata.W3cXsd2001.SoapYearMonth";
                case "duration":   return "System.Runtime.Remoting.Metadata.W3cXsd2001.SoapDuration"; 
                default: return null;
            }
        }

    }
}
