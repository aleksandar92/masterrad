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

        private bool isExtensionOntology;
        private bool isCommonOntology;
        private string nameOfOntology;
        private string commonOntology;
        private string extensionOntologyNS;
		private string commonOntologyNS;
		private bool isEnumMembersInstances;
		private string defaultNamespace;
		private string metaURI;

        #endregion

        #region CONSTRUCTORS

        public GeneratorSettings(bool isROnt,bool isSpeOnt,string naOfOnt,string roofOnt,string ext,string roofOntNS,bool isEnumMemInstance,string defaultNS,string mURI) 
        {
            isExtensionOntology = isSpeOnt;
            isCommonOntology = isROnt;
            commonOntology=roofOnt;
            extensionOntologyNS=ext;
            nameOfOntology = naOfOnt;
			commonOntologyNS = roofOntNS;
			isEnumMembersInstances = isEnumMemInstance;
			defaultNamespace = defaultNS;
			metaURI = mURI;

        }

        public GeneratorSettings() 
        {
            
        }

        #endregion

        #region PROPERTIES
        public bool IsExtensionOntology
        {
            get { return isExtensionOntology; }
            set { isExtensionOntology = value; }
        }

        public string NameOfOntology
        {
            get { return nameOfOntology; }
            set { nameOfOntology = value; }
        }

        public string CommonOntology
        {
            get { return commonOntology; }
            set { commonOntology = value; }
        }

        public bool IsCommonOntology
        {
            get { return isCommonOntology; }
            set { isCommonOntology = value; }
        }

        public string ExtensionOntologyNS
        {
            get { return extensionOntologyNS; }
            set { extensionOntologyNS = value; }
        }

		public string CommonOntologyNS
		{
			get { return commonOntologyNS; }
			set { commonOntologyNS = value; }
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

			if(!StringManipulationManager.IsValidURI(extensionOntologyNS) && isExtensionOntology) 
			{
				sb.Append("Extension ontology base URI " + extensionOntologyNS + "   is not valid");
				result = false;
			}

			if (!StringManipulationManager.IsValidURI(commonOntologyNS) && isCommonOntology)
			{
				sb.Append("Common ontology base URI " + commonOntologyNS + "   is not valid");
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
