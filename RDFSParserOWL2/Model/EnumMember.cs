using RDFSParserOWL2.Common;
using RDFSParserOWL2.Generator.Helper;
using RDFSParserOWL2.Model.Settings;
using RDFSParserOWL2.Reporter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RDFSParserOWL2.Model
{
    public class EnumMember : ProfileElement
    {
        //protected bool isEnumeration = true;
        //// enum specifics
        protected ProfileElement enumerationObject; //// if this ProfileElement is a value in some Enumeration, this is that Enumeration

        public EnumMember()
            : base("From Derived")
        {

        }

        #region Enumeration specifics
        /// <summary>
        /// Class Specific property.
        /// <para>Gets the indicator whether or not given profile element is enumeration class.</para>
        /// </summary>
        //public bool IsEnumeration
        //{
        //    get
        //    {
        //        return isEnumeration;
        //    }
        //}

        /// <summary>
        /// Enumeration member Specific property.
        /// <para>Gets and sets the ProfileElement which is the parent class of this enumeration member element.</para>        
        /// </summary>
        public ProfileElement EnumerationObject
        {
            get
            {
                return enumerationObject;
            }
            set
            {
                enumerationObject = value;
            }
        }

        public override bool ContainsStereotype(string stereotype)
        {
      
            if (EnumerationObject!=null)
                return EnumerationObject.ContainsStereotype(stereotype);

            return false;
        }

        #endregion Enumeration specifics

        #region Methods for handling equivalence with another element

        public override void EquivalenceWithAnotherElement(string uriOfEquivalent)
        {
            Class cls = EnumerationObject as Class;

            if (!string.IsNullOrEmpty(uriOfEquivalent) && !IsBlankNode && cls != null && !cls.HasDifferentStereotype(ConfigurationFiles.StereotypesToSkip))
            {
                UriOfEquivalentElement = String.Format("{0}{1}", uriOfEquivalent, uri);
            }
        }


        public override bool EquvialenceWithAnotherElementWithStereotype(string baseUriOfElement, string stereotype)
        {
            bool result = false;
            Class cls = EnumerationObject as Class;

            if (!IsBlankNode && ContainsStereotype(stereotype))
            {
                UriOfEquivalentElement = String.Format("{0}{1}{2}", baseUriOfElement, stereotype, uri);
                result = true;
            }

            return result;
        }


        #endregion

        #region RDFXML representation

        protected override bool IsNotToBeWritten()
        {
            return IsNotToBeGenerated || kind==ProfileElementTypes.Unknown;
        }

        protected override void WriteEquivalenceToRDFXML(XmlWriter writer, string baseAdress)
        {
            return;

        }

        protected override void WriteSpecificElementsToRDFXML(XmlWriter writer, string baseAddress, EnumRepresentationKindOWL2 enumOpt)
        {
            if (enumOpt == EnumRepresentationKindOWL2.OPENED)
                WriteEnumTypeToRDFXML(writer, baseAddress);
            EquivalenceEnumMemberToRDFXML(writer);
        }

        private void WriteEnumTypeToRDFXML(XmlWriter writer, string baseAddress)
        {
            if (EnumerationObject != null)
            {
                writer.WriteStartElement(OWL2Namespace.rdfPrefix, OWL2Namespace.Type, null);
                WriteURIToRDFXML(EnumerationObject.URI, baseAddress, IsBlankNode, OWL2Namespace.rdfResource, OWL2Namespace.rdfResource, writer);
                writer.WriteEndElement();
            }
        }

        private void EquivalenceEnumMemberToRDFXML(XmlWriter writer) 
        {
            if (!string.IsNullOrEmpty(UriOfEquivalentElement))
            {
                writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.SameAs, null);
                writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, UriOfEquivalentElement);
                writer.WriteEndElement();
            }
        }

        protected override string OWLRDFXMLType()
        {
            return OWL2Namespace.NamedIndividual;
        }

        public override List<EntityTypesGeneratorReporter> StatusForOWL2()
        {
            List<EntityTypesGeneratorReporter> reportStatus = new List<EntityTypesGeneratorReporter>();
            if (IsBlankNode)
            {
                reportStatus.Add(EntityTypesGeneratorReporter.BlankId);
            }

            if (IsNotToBeWritten())
            {
                reportStatus.Add(EntityTypesGeneratorReporter.NotGenerated);
            }
            else
            {
                reportStatus.Add(EntityTypesGeneratorReporter.NamedIndividual);
            }

            return reportStatus;
        }

        #endregion
    }
}
