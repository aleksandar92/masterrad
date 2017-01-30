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


        //private IParserReporter reporter;
        bool isCheckingForFileExistenceNeeded;
        //public IParserReporter Reporter
        //{
        //    get { return reporter; }
        //    set { reporter = value; }
        //}


        public RDFXMLParser(RDFXMLHandler handl,bool checking)
        {
            handler = handl;
            //this.path = path;
            isCheckingForFileExistenceNeeded = checking;
        }

        public Profile ParseProfile(string path)
        {
            Profile result = null;
             if(isCheckingForFileExistenceNeeded && !InputOutput.CheckIfFileExists(path))
			{
				result = new Profile(path);
                return  result;
			}

				using (FileStream fs = new FileStream(path, FileMode.Open))
				{
					bool succes;
					TimeSpan ts;
					try
					{
						XMLParser.DoParse(handler, fs, path, out succes, out ts);
                        result = handler.Profile;
                        
					}catch(XmlException xe) 
					{
                        if(isCheckingForFileExistenceNeeded)
						result = new Profile(path);
					}

                    return result;
				}
			}

        }
    }

