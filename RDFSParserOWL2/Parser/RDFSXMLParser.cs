using RDFSParserOWL2.Model;
using RDFSParserOWL2.Reporter.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Parser
{
    public class RDFSXMLParser
    {


        private RDFSRDFXMLHandler handler;
        private string path;
        private Profile profile;
		private IParserReporter reporter;

		public IParserReporter Reporter
		{
			get { return reporter; }
			set { reporter = value; }
		}

        public Profile Profile
        {
            get { return profile; }
            set { profile = value; }
        }


        public RDFSXMLParser()
        {
            handler = new RDFSRDFXMLHandler();

        }

        public RDFSXMLParser(string path)
        {
            handler = new RDFSRDFXMLHandler();
            this.path = path;
        }

        public void ParseProfile()
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                bool succes;
                TimeSpan ts;
                XMLParser.DoParse(handler, fs, path, out succes, out ts);
                Profile = handler.Profile;
				reporter = handler.Reporter;

            }

        }






    }
}
