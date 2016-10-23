using RDFSParserOWL2.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Model.Settings
{
    /// <summary>
    /// Class that represents parameters for generators
    /// </summary>
    public class GeneratorSettings
    {
        #region FIELDS
        private bool isSpecialOntology;
        private bool isRoofOntology;
        private string nameOfOntology;
        private string roofOntology;
        private string extractionOntologyNS;
		private string roofOntologyNS;
		private bool isEnumMembersInstances;
		private string defaultNamespace;
		private string metaURI;





        #endregion

        #region CONSTRUCTORS

        public GeneratorSettings(bool isROnt,bool isSpeOnt,string naOfOnt,string roofOnt,string ext,string roofOntNS,bool isEnumMemInstance,string defaultNS,string mURI) 
        {
            isSpecialOntology = isSpeOnt;
            isRoofOntology = isROnt;
            roofOntology=roofOnt;
            extractionOntologyNS=ext;
            nameOfOntology = naOfOnt;
			roofOntologyNS = roofOntNS;
			isEnumMembersInstances = isEnumMemInstance;
			defaultNamespace = defaultNS;
			metaURI = mURI;

        }

        public GeneratorSettings() 
        {
            
        }

        #endregion

        #region PROPERTIES
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

        public bool IsRoofOntology
        {
            get { return isRoofOntology; }
            set { isRoofOntology = value; }
        }

        public string ExtractionOntologyNS
        {
            get { return extractionOntologyNS; }
            set { extractionOntologyNS = value; }
        }

		public string RoofOntologyNS
		{
			get { return roofOntologyNS; }
			set { roofOntologyNS = value; }
		}

		public bool IsEnumMembersInstances
		{
			get { return isEnumMembersInstances; }
			set { isEnumMembersInstances = value; }
		}

		public string DefaultNamespace
		{
			get { return defaultNamespace; }
			set { defaultNamespace = value; }
		}

		public string MetaURI
		{
			get { return metaURI; }
			set { metaURI = value; }
		}




        #endregion

		#region HELPERS
		public bool CheckValidity(out string report) 
		{
			bool result = true;
			StringBuilder sb = new StringBuilder();
			if(!StringManipulationManager.IsValidURI(defaultNamespace)) 
			{
				sb.Append("Base URI "+defaultNamespace+"  for generated files is not valid");
				result = false;
			}

			if(!StringManipulationManager.IsValidURI(extractionOntologyNS) && isSpecialOntology) 
			{
				sb.Append("Extension ontology base URI " + extractionOntologyNS + "   is not valid");
				result = false;
			}

			if (!StringManipulationManager.IsValidURI(roofOntologyNS) && isRoofOntology)
			{
				sb.Append("Common ontology base URI " + roofOntologyNS + "   is not valid");
				result = false;
			}

			//if (!string.IsNullOrEmpty(metaURI) && !StringManipulationManager.IsValidURI(roofOntologyNS)) 
			//{
			//	sb.Append("Meta ontology base URI " + roofOntologyNS + "   is not valid");
			//	result = false;
			//}

			report = sb.ToString();
			return result;
		
		} 

		#endregion
	}
}
