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

	/// <summary>
	/// Class that converts file in RDFS to  OWL2 format 
	/// </summary>
    public  class ConverterRDFSToOWL2
    {
		/// <summary>
		/// Parser for file in RDFS 
		/// </summary>
        private RDFSXMLParser rdfsParser;
		/// <summary>
		/// Generator for OWL2
		/// </summary>
        private OWL2Generator generator;
		private bool isSpecialOntology;
		private OWL2XMLParser owlParser;

		private string nameOfOntology;



        public ConverterRDFSToOWL2() 
        {
            rdfsParser = new RDFSXMLParser();
			owlParser = new OWL2XMLParser();
           // generator = new  OWL2Generator();
        }

        public ConverterRDFSToOWL2(string path) 
        {
            rdfsParser = new RDFSXMLParser(path);

            //generator = new  OWL2Generator();
        }

		public ConverterRDFSToOWL2(string path,bool isSpecial,string nameOfOnt)
		{
			rdfsParser = new RDFSXMLParser(path);
			nameOfOntology = nameOfOnt;
			isSpecialOntology = isSpecial;
			//generator = new  OWL2Generator();
		}



		public bool IsSpecialOntology
		{
			get { return isSpecialOntology; }
			set { isSpecialOntology = value; }
		}

		public string NameOfOntology
		{
			get { return nameOfOntology; }
			set { nameOfOntology = value; }
		}


        public void  Convert() 
        {
            rdfsParser.ParseProfile();
            Profile profile = rdfsParser.Profile;
			profile.RemoveElementsWithStereotypes(InputOutput.LoadStereotypesToSkip());
			//bool exists = InputOutput.CheckIfFileExists(InputOutput.CreatePathForGeneratedOWL(InputOutput.CreateOWLFilename("entsoe")));
			Profile entsoProfile = InputOutput.LoadEntsoProfile();
			
			if (profile.ProcessEntsoeElements(entsoProfile))
			{
				entsoProfile.PopulateObjectReferences();
				generator = new OWL2Generator(entsoProfile);
				generator.GenerateProfile();
			}

			generator = new OWL2Generator(profile);
            generator.GenerateProfile();
        }
    }
}
