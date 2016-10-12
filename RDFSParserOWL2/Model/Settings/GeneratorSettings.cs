﻿using System;
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




        #endregion

        #region CONSTRUCTORS

        public GeneratorSettings(bool isROnt,bool isSpeOnt,string naOfOnt,string roofOnt,string ext,string roofOntNS,bool isEnumMemInstance,string defaultNS) 
        {
            isSpecialOntology = isSpeOnt;
            isRoofOntology = isROnt;
            roofOntology=roofOnt;
            extractionOntologyNS=ext;
            nameOfOntology = naOfOnt;
			roofOntologyNS = roofOntNS;
			isEnumMembersInstances = isEnumMemInstance;
			defaultNamespace = defaultNS;
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


        #endregion
    }
}