using RDFSParserOWL2.Common;
using RDFSParserOWL2.Model;
using RDFSParserOWL2.Parser.Handler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RDFSParserOWL2.Parser
{
	public class OWL2XMLParser
	{
		private OWL2RDFXMLHandler handler;
        private string path;
        private Profile profile;


        public Profile Profile
        {
            get { return profile; }
            set { profile = value; }
        }


        public OWL2XMLParser()
        {
            handler = new OWL2RDFXMLHandler();

        }

		public OWL2XMLParser(string path)
        {
            handler = new OWL2RDFXMLHandler();
            this.path = path;
        }

        public void ParseProfile()
        {

			if (!InputOutput.CheckIfFileExists(path))
			{
				Profile = new Profile(path);
			}
			else
			{
				using (FileStream fs = new FileStream(path, FileMode.Open))
				{
					bool succes;
					TimeSpan ts;
					try
					{
						XMLParser.DoParse(handler, fs, path, out succes, out ts);
						Profile = handler.Profile;
					}catch(XmlException xe) 
					{
						Profile = new Profile(path);
					}
					
				}
			}
        }



	}
}
