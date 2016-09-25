using RDFSParserOWL2.Model;
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


        private RDFSXMLReaderHandler handler;
        private string path;
        private Profile profile;


        public Profile Profile
        {
            get { return profile; }
            set { profile = value; }
        }


        public RDFSXMLParser()
        {
            handler = new RDFSXMLReaderHandler();

        }

        public RDFSXMLParser(string path)
        {
            handler = new RDFSXMLReaderHandler();
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

            }

        }






    }
}
