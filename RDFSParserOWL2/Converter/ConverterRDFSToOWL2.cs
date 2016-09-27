using RDFSParserOWL2.Common;
using RDFSParserOWL2.Generator;
using RDFSParserOWL2.Model;
using RDFSParserOWL2.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Converter
{
    public  class ConverterRDFSToOWL2
    {
        private RDFSXMLParser rdfsParser;
        private OWL2Generator generator;

        public ConverterRDFSToOWL2() 
        {
            rdfsParser = new RDFSXMLParser();
           // generator = new  OWL2Generator();
        }

        public ConverterRDFSToOWL2(string path) 
        {
            rdfsParser = new RDFSXMLParser(path);
            //generator = new  OWL2Generator();
        }

        public void  Convert() 
        {
            rdfsParser.ParseProfile();
            Profile profile = rdfsParser.Profile;
			Profile entsoProfile = InputOutput.LoadEntsoProfile();
			if (profile.ProcessEntsoeElements(entsoProfile))
			{
				entsoProfile.PopulateDomainAsObjectProperties();
				generator = new OWL2Generator(entsoProfile);
				generator.GenerateProfile();

			}
			generator = new OWL2Generator(profile);
            generator.GenerateProfile();
        }
    }
}
