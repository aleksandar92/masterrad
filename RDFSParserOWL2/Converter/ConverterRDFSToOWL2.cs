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
		private bool isRoofOntology;
		private OWL2XMLParser owlParser;
		private string nameOfOntology;
		private string roofOntology;

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

		public ConverterRDFSToOWL2(string path,bool isSpecialOnt,string nameOfOnt)
		{
			rdfsParser = new RDFSXMLParser(path);
			isSpecialOntology = isSpecialOnt;
			nameOfOntology = nameOfOnt;
		}

		public ConverterRDFSToOWL2(string path, bool isSpecialOnt, string nameOfOnt,bool isRoof,string roofOntologyName)
		{
			rdfsParser = new RDFSXMLParser(path);
			isSpecialOntology = isSpecialOnt;
			nameOfOntology = nameOfOnt;
			isRoofOntology = isRoof;
			roofOntology = roofOntologyName;
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

		public string RoofOntology
		{
			get { return roofOntology; }
			set { roofOntology = value; }
		}


		/// <summary>
		/// Method for converting file from RDFS format to OWL2 format 
		/// </summary>
        public void  Convert() 
        {
            rdfsParser.ParseProfile();
            Profile profile = rdfsParser.Profile;
			profile.RemoveElementsWithStereotypes(InputOutput.LoadStereotypesToSkip());

			if (isSpecialOntology)
			{
				owlParser = new OWL2XMLParser(InputOutput.CreatePathForGeneratedOWL(InputOutput.CreateOWLFilename(nameOfOntology)));
				owlParser.ParseProfile();
				Profile entsoProfile = owlParser.Profile;

				if (entsoProfile != null)
				{
					entsoProfile.IsOwlProfile = true;
				}

				if (profile.ProcessSpecialStereotypeElements(entsoProfile,nameOfOntology))
				{
					entsoProfile.PopulateObjectReferences();
					generator = new OWL2Generator(entsoProfile);
					generator.GenerateProfile();
				}
			}

			generator = new OWL2Generator(profile,nameOfOntology,isSpecialOntology,roofOntology,isRoofOntology);
            generator.GenerateProfile();
        }
    }
}
