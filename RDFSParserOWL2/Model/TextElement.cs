using RDFSParserOWL2.Generator.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RDFSParserOWL2.Model
{
	public class TextElement
	{
		//protected string name;
		private string value;

		private Dictionary<string, string> attributes;



		public TextElement() 
		{                                        
			attributes = new Dictionary<string, string>();
            value = String.Empty;
		}

		public TextElement(string value)
		{
			attributes = new Dictionary<string, string>();
			this.value = value;
		}

		public TextElement(string value,Dictionary<string,string> attr) 
		{
		    this.attributes = new Dictionary<string,string>(attr);
			this.value = value;		
		}


		public string Value
		{
			get { return this.value; }
			set { this.value = value; }
		}

		public Dictionary<string, string> Attributes
		{
			get { return attributes; }
			set { attributes = value; }
        }

        #region RDFXML representation

        /// <summary>
        /// Write  text element  in RDFXML
        /// </summary>
        /// <param name="writer"></param>
        public void WriteTextElementToRDFXML(XmlWriter writer, string prefix)
        {
                writer.WriteStartElement(OWL2Namespace.rdfsPrefix,prefix, null);
                WriteAttsToRDFXML(Attributes, writer);
                writer.WriteValue(Value);
                writer.WriteEndElement();
        }



        private void WriteAttsToRDFXML(Dictionary<string, string> atts, XmlWriter writer)
        {
            if (atts != null && atts.ContainsKey(OWL2Namespace.xmlLang))
            {
                writer.WriteAttributeString(OWL2Namespace.xmlPrefix, OWL2Namespace.Lang, null, atts[OWL2Namespace.xmlLang]);
            }
        }


        #endregion
    }
}
