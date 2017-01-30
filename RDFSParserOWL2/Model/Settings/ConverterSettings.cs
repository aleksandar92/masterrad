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
    public class ConverterSettings
    {
        #region FIELDS

        private bool isExtensionOntology;
        private bool isCommonOntology;
        private string extensionOntologyName;
        private string commonOntologyName;
        private string extensionOntologyURI;
		private string commonOntologyURI;
        private Options option;

        public Options Option
        {
            get { return option; }
            set { option = value; }
        }
        private bool isEnumMembersClosed;
		private string defaultNamespace;
		private string metaURI;

        #endregion

        #region CONSTRUCTORS

        public ConverterSettings(bool isROnt,bool isSpeOnt,string naOfOnt,string roofOnt,string ext,string roofOntNS,bool isEnumMemInstance,string defaultNS,string mURI, Options opt) 
        {
            isExtensionOntology = isSpeOnt;
            isCommonOntology = isROnt;
            commonOntologyName=roofOnt;
            extensionOntologyURI=ext;
            extensionOntologyName = naOfOnt;
			commonOntologyURI = roofOntNS;
			isEnumMembersClosed = isEnumMemInstance;
			defaultNamespace = defaultNS;
			metaURI = mURI;
            option = opt;
        }

        public ConverterSettings() 
        {
            
        }

        #endregion

        #region PROPERTIES
        public bool IsExtensionOntology
        {
            get { return isExtensionOntology; }
            set { isExtensionOntology = value; }
        }

        public string ExtensionOntologyName
        {
            get { return extensionOntologyName; }
            set { extensionOntologyName = value; }
        }

        public string CommonOntologyName
        {
            get { return commonOntologyName; }
            set { commonOntologyName = value; }
        }

        public bool IsCommonOntology
        {
            get { return isCommonOntology; }
            set { isCommonOntology = value; }
        }

        public string ExtensionOntologyURI
        {
            get { return extensionOntologyURI; }
            set { extensionOntologyURI = value; }
        }

		public string CommonOntologyURI
		{
			get { return commonOntologyURI; }
			set { commonOntologyURI = value; }
		}

		public bool IsEnumMembersClosed
		{
			get { return isEnumMembersClosed; }
			set { isEnumMembersClosed = value; }
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

			if(!StringManipulationManager.IsValidURI(extensionOntologyURI) && isExtensionOntology) 
			{
				sb.Append("Extension ontology base URI " + extensionOntologyURI + "   is not valid");
				result = false;
			}

			if (!StringManipulationManager.IsValidURI(commonOntologyURI) && isCommonOntology)
			{
				sb.Append("Common ontology base URI " + commonOntologyURI + "   is not valid");
				result = false;
			}

            if (!string.IsNullOrEmpty(metaURI) && !StringManipulationManager.IsValidRelativeURI(metaURI))
            {
                sb.Append("Meta ontology base URI " + MetaURI + "   is not valid");
                result = false;
            }

			report = sb.ToString();
			return result;
		
		} 

		#endregion
	}
}
