using RDFSParserOWL2.Common;
using RDFSParserOWL2.Model;
using RDFSParserOWL2.Parser.Handler;
using RDFSParserOWL2.Reporter.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RDFSParserOWL2.Parser
{
    public class RDFXMLParser
    {

            private RDFXMLHandler handler;
        private string path;
        private Profile profile;
		private IParserReporter reporter;
        bool isCheckingForFileExistenceNeeded;
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


        public RDFXMLParser(string path,RDFXMLHandler handl,bool checking)
        {
            handler = new RDFSRDFXMLHandler();
            this.path = path;
            isCheckingForFileExistenceNeeded = checking;
        }

        public void ParseProfile()
        {
             if(isCheckingForFileExistenceNeeded && !InputOutput.CheckIfFileExists(path))
			{
				Profile = new Profile(path);
                return;
			}

				using (FileStream fs = new FileStream(path, FileMode.Open))
				{
					bool succes;
					TimeSpan ts;
					try
					{
						XMLParser.DoParse(handler, fs, path, out succes, out ts);
						Profile = handler.Profile;
                        reporter = handler.Reporter;
					}catch(XmlException xe) 
					{
                        if(isCheckingForFileExistenceNeeded)
						Profile = new Profile(path);
					}
					
				}
			}

        }
    }

