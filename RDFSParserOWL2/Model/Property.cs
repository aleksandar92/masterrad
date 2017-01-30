using RDFSParserOWL2.Common;
using RDFSParserOWL2.Generator.Helper;
using RDFSParserOWL2.Manager;
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

    public class Property : ProfileElement
    {
        protected string domain;
        protected Class domainAsObject;
        protected string dataType;
        protected Type dataTypeAsSimple;
        protected string range;
        protected ProfileElement rangeAsObject;
        bool isDataTypeSimple;
        protected bool isAggregate;
        protected bool isEnumeration = false;
        protected ProfileElement dataTypeAsComplexObject;
        protected string inverseRoleName;
        protected ProfileElement inverseRoleNameAsObject;
        private string associationUsed;



        //protected bool isExpectedToContainLocalClass = false; //// if this property expected to contain inner instance of some class

        private List<ProfileElementStereotype> stereotypes;



        public Property() : base("From Derived") { }

        #region get and set



        public bool IsPropertyDataTypeSimple
        {
            get
            {
                return isDataTypeSimple;
            }
        }

        public string Domain
        {
            get
            {
                return domain;
            }
            set
            {
                domain = value;
            }
        }

        public bool IsAggregate
        {
            get
            {
                return isAggregate;
            }
            set
            {
                isAggregate = value;
            }
        }

        public bool IsEnumeration
        {
            get
            {
                return isEnumeration;
            }
            set
            {
                isEnumeration = value;
            }
        }



        public string AssociationUsed
        {
            get { return associationUsed; }
            set { associationUsed = value; }
        }

        /// <summary>
        /// Property Specific property.
        /// <para>Gets and sets the ProfileElement which is cosidered for data type of this profile property.</para>
        /// <para>Given ProfileElement should be of type ProfileElementTypes.Class.</para>
        /// <para>This can be null if IsPropertyDataTypeSimple has <c>true</c> value.</para>
        /// </summary>
        public ProfileElement DataTypeAsComplexObject
        {
            get
            {
                return dataTypeAsComplexObject;
            }
            set
            {
                dataTypeAsComplexObject = value;
            }
        }


        public List<ProfileElementStereotype> Stereotypes
        {
            get { return stereotypes; }
            set { stereotypes = value; }
        }

        /// <summary>
        /// Property Specific property.
        /// <para>Gets and sets the CIMType which is the data type of this profile property.</para>        
        /// <para>This can be null if IsPropertyDataTypeSimple has <c>false</c> value.</para>
        /// </summary>
        public Type DataTypeAsSimple
        {
            get
            {
                return dataTypeAsSimple;
            }
            set
            {
                dataTypeAsSimple = value;
            }
        }

        /// <summary>
        /// Property Specific property.
        /// <para>Gets and sets the string representation of data type for this profile property.</para>
        /// </summary>
        public string DataType
        {
            get
            {
                return dataType;
            }
            set
            {
                dataType = value;

                if (dataType != null)
                {
                    isDataTypeSimple = true;
                    string shortDT = StringManipulationManager.ExtractShortestName(dataType, Separator);
                    switch (shortDT.ToLower())
                    {
                        case SimpleDataTypeInteger:
                        case SimpleDataTypeInt:
                            {
                                dataTypeAsSimple = System.Type.GetType("System.Int32");
                                break;
                            }
                        case SimpleDataTypeLong:
                            {
                                dataTypeAsSimple = System.Type.GetType("System.Int64");
                                break;
                            }
                        case SimpleDataTypeFloat:
                            {
                                dataTypeAsSimple = System.Type.GetType("System.Single");
                                break;
                            }
                        case SimpleDataTypeDouble:
                            {
                                dataTypeAsSimple = System.Type.GetType("System.Double");
                                break;
                            }
                        case SimpleDataTypeString:
                            {
                                dataTypeAsSimple = System.Type.GetType("System.String");
                                break;
                            }
                        case SimpleDataTypeBoolean:
                        case SimpleDataTypeBool:
                            {
                                dataTypeAsSimple = System.Type.GetType("System.Boolean");
                                break;
                            }
                        case SimpleDataTypeDateTime:
                            {
                                dataTypeAsSimple = System.Type.GetType("System.DateTime");
                                break;
                            }
                        default:
                            {
                                isDataTypeSimple = false;
                                break;
                            }
                    }
                }
            }
        }
        public Class DomainAsObject
        {
            get
            {
                return domainAsObject;
            }
            set
            {
                domainAsObject = value;
            }
        }
        public string Range
        {
            get
            {
                return range;
            }
            set
            {
                range = value;
            }
        }

        public string InverseRoleName
        {
            get
            {
                return inverseRoleName;
            }
            set
            {
                inverseRoleName = value;
            }
        }

        public ProfileElement RangeAsObject
        {
            get
            {
                return rangeAsObject;
            }
            set
            {
                rangeAsObject = value;
            }
        }


        public ProfileElement InverseRoleNameAsObject
        {
            get { return inverseRoleNameAsObject; }
            set { inverseRoleNameAsObject = value; }
        }


        #endregion get and set

        #region stereotype

        public bool HasDifferentStereotype(HashSet<string> fixedStereotypes)
        {
            if (stereotypes != null)
            {
                foreach (ProfileElementStereotype pet in stereotypes)
                {
                    if (!fixedStereotypes.Contains(pet.ShortName.ToLower()))
                        return true;
                }
            }
            return false;

        }

        public void AddStereotype(string fullStereotypeString)
        {
            if (!string.IsNullOrEmpty(fullStereotypeString))
            {
                ProfileElementStereotype stereotype = Profile.FindOrCreateStereotypeForName(fullStereotypeString);

                if (stereotype != null)
                {
                    if (stereotypes == null)
                    {
                        stereotypes = new List<ProfileElementStereotype>();
                    }

                    if (!stereotypes.Contains(stereotype))
                    {
                        stereotypes.Add(stereotype);
                        //stereotypes.Sort(CIMComparer.ProfileElementStereotypeComparer);
                    }

                    if (ProfileElementStereotype.StereotypeEnumeration.Equals(stereotype.Name))
                    {
                        isEnumeration = true;
                    }

                    if (ProfileElementStereotype.StereotypeAggregateOf.Equals(stereotype.Name))
                    {
                        isAggregate = true;
                    }
                }
            }
        }

        /// <summary>
        /// Method checks whether or not given stereotype exist is inside of stereotypes list.
        /// </summary>
        /// <param fullName="stereotype">search for this stereotype</param>
        /// <returns><c>true</c> if stereotype was founded, <c>false</c> otherwise</returns>
        /// 

        public bool HasStereotype(ProfileElementStereotype stereotype)
        {
            bool hasStereotype = false;
            if (stereotypes != null)
            {
                hasStereotype = stereotypes.Contains(stereotype);
            }
            return hasStereotype;
        }

        /// <summary>
        /// Method checks whether or not given stereotype exist is inside of stereotypes list.
        /// </summary>
        /// <param fullName="stereotypeName">search for stereotype with this name</param>
        /// <returns><c>true</c> if stereotype was founded, <c>false</c> otherwise</returns>

        public bool HasStereotype(string stereotypeName)
        {
            bool hasStereotype = false;
            if (stereotypes != null)
            {
                foreach (ProfileElementStereotype stereotype in stereotypes)
                {
                    if ((string.Compare(stereotype.Name, stereotypeName, StringComparison.OrdinalIgnoreCase) == 0) || (string.Compare(stereotype.ShortName, stereotypeName, StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        hasStereotype = true;
                        break;
                    }
                }
            }
            return hasStereotype;
        }

        public override bool ContainsStereotype(string stereotype)
        {
            if (DomainAsObject != null)
                return HasStereotype(stereotype) || DomainAsObject.HasStereotype(stereotype);

            return HasStereotype(stereotype);
        }


        public List<ProfileElementStereotype> GetUndefinedStereotypes()
        {
            List<ProfileElementStereotype> undefinedStereotypes = new List<ProfileElementStereotype>();
            if (stereotypes == null || stereotypes.Count <= 0)
            {
                return null;
            }
            foreach (ProfileElementStereotype stereotype in stereotypes)
            {
                if (!stereotype.Name.Equals(ProfileElementStereotype.StereotypeConcrete) && !stereotype.Name.Equals(ProfileElementStereotype.StereotypeCompound) && !stereotype.Name.Equals(ProfileElementStereotype.StereotypeEnumeration) && !stereotype.Name.Equals(ProfileElementStereotype.StereotypeAttribute) && !stereotype.Name.Equals(ProfileElementStereotype.StereotypeByReference) && !stereotype.Name.Equals(ProfileElementStereotype.StereotypeOfAggregate) && !stereotype.Name.Equals(ProfileElementStereotype.StereotypeAggregateOf) && !stereotype.Name.Equals(ProfileElementStereotype.StereotypeCompositeOf))
                {
                    undefinedStereotypes.Add(stereotype);

                }
            }

            return undefinedStereotypes;
        }

        #endregion

        #region Methods for handling equivalence with another element

        public override void EquivalenceWithAnotherElement(string uriOfEquivalent)
        {
            if (!string.IsNullOrEmpty(uriOfEquivalent) && CheckIfHasDifferentStereotype())
            {
                UriOfEquivalentElement = String.Format("{0}{1}", uriOfEquivalent, uri);
            }
        }


        private bool CheckIfHasDifferentStereotype()
        {
            if (DomainAsObject != null)
            {
                return !IsBlankNode && !HasDifferentStereotype(ConfigurationFiles.StereotypesToSkip) && !DomainAsObject.HasDifferentStereotype(ConfigurationFiles.StereotypesToSkip);
            }
            return !IsBlankNode && !HasDifferentStereotype(ConfigurationFiles.StereotypesToSkip);
        }

        public override bool EquvialenceWithAnotherElementWithStereotype(string baseUriOfElement, string stereotype)
        {
            bool result = false;

            if (!IsBlankNode && ContainsStereotype(stereotype))
            {
                UriOfEquivalentElement = String.Format("{0}{1}{2}", baseUriOfElement, stereotype, uri);
                result = true;
            }

            return result;
        }


        #endregion

        #region RDFXML representation

        protected override string OWLRDFXMLType()
        {
            if (IsObjectProperty())
                return OWL2Namespace.ObjectProperty;
            else
                return OWL2Namespace.DatatypeProperty;
        }

        protected override void WriteEquivalenceToRDFXML(XmlWriter writer, string baseAdress)
        {
            WriteEquivalentPropertyToRDFXML(writer, baseAdress);

        }

        private void WriteEquivalentPropertyToRDFXML(XmlWriter writer, string baseAdress)
        {
            if (!string.IsNullOrEmpty(UriOfEquivalentElement))
            {
                writer.WriteStartElement(OWL2Namespace.rdfPrefix, OWL2Namespace.Description, null);
                WriteURIToRDFXML(StringManipulationManager.ExtractAllWithSeparator(URI, StringManipulationManager.SeparatorSharp), baseAdress, IsBlankNode, OWL2Namespace.rdfAbout, OWL2Namespace.NodeId, writer);
                writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.EqProperty, null);
                writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, UriOfEquivalentElement);
                writer.WriteEndElement();
                writer.WriteEndElement();

            }
        }

        protected override void WriteSpecificElementsToRDFXML(XmlWriter writer, string baseAddress, EnumRepresentationKindOWL2 enumOpt)
        {
            WriteInverseRolenameToRDFXML(writer, baseAddress);
            WriteAssociationUsedToRDFXML(writer);
            WriteStereotypesToRDFXML(stereotypes, writer);
            WriteDomainToRDFXML(writer, baseAddress);
            WriteRangeToRDFXML(writer, baseAddress);
        }

        private void WriteDomainToRDFXML(XmlWriter writer, string baseAddress)
        {
            if (domainAsObject != null && !string.IsNullOrEmpty(Domain))
            {
                string domainUri = DomainAsObject == null ? Domain : DomainAsObject.URI;
                writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsDomain, null);
                WriteURIToRDFXML(domainUri, baseAddress, DomainAsObject.IsBlankNode, OWL2Namespace.rdfResource, OWL2Namespace.rdfResource, writer);
                writer.WriteEndElement();
            }

        }

        private void WriteRangeToRDFXML(XmlWriter writer, string baseAddress)
        {
            string rangeValue;

            if (IsObjectProperty() && rangeAsObject != null && !string.IsNullOrEmpty(Range))
            {
                writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsRange, null);
                rangeValue = RangeAsObject != null ? RangeAsObject.URI : Range;
                bool isBlank = false;
                if (RangeAsObject != null)
                {
                    isBlank = RangeAsObject.IsBlankNode;
                }
                WriteURIToRDFXML(rangeValue, baseAddress, isBlank, OWL2Namespace.rdfResource, OWL2Namespace.rdfResource, writer);
                writer.WriteEndElement();

            }
            else if (!IsObjectProperty())
            {
                writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsRange, null);
                rangeValue = ProcessRangeValueForDatatypeProperty(baseAddress);
                writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, rangeValue);
                writer.WriteEndElement();
            }



        }


        private void WriteAssociationUsedToRDFXML(XmlWriter writer)
        {
            if (!string.IsNullOrEmpty(AssociationUsed))
            {
                writer.WriteStartElement(MetaNamespace.MetaPrefix, MetaNamespace.AsocUsed, null);
                writer.WriteValue(AssociationUsed);
                writer.WriteEndElement();
            }
        }

        private void WriteInverseRolenameToRDFXML(XmlWriter writer, string baseAdress)
        {
            if (!string.IsNullOrEmpty(InverseRoleName) && InverseRoleNameAsObject != null)
            {
                writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.InverseOf, null);
                WriteURIToRDFXML(InverseRoleNameAsObject.URI, baseAdress, InverseRoleNameAsObject.IsBlankNode, OWL2Namespace.rdfResource, OWL2Namespace.rdfResource, writer);
                writer.WriteEndElement();
            }
        }


        #endregion

        #region helperMethods


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
                if (IsObjectProperty())
                    reportStatus.Add(EntityTypesGeneratorReporter.ObjectProperty);
                else
                    reportStatus.Add(EntityTypesGeneratorReporter.DatatypeProperty);
            }

            return reportStatus;
        }

        public bool IsObjectProperty()
        {
            return (domain != null && range != null) || (domainAsObject != null && rangeAsObject != null);
        }

        public string ProcessDatatype()
        {
            string result = null;
            if (dataType != null && dataType != String.Empty)
            {

                result = dataType.Replace(StringManipulationManager.SeparatorSharp, "").ToLower().Trim();
                switch (result)
                {
                    case "negativeInteger":
                        result = "negativeInteger";
                        break;

                    case "nonnegativeinteger":
                        result = "nonNegativeInteger";
                        break;

                    case "nonpositiveinteger":
                        result = "nonPositiveInteger";
                        break;

                    case "positiveinteger":
                        result = "positiveInteger";
                        break;

                    case "unsignedlong":
                        result = "unsignedLong";
                        break;

                    case "unsignedint":
                        result = "unsignedInt";
                        break;

                    case "unsignedshort":
                        result = "unsignedShort";
                        break;

                    case "unsignedbyte":
                        result = "unsignedByte";
                        break;

                    case "datetime":
                        result = "dateTime";
                        break;

                    case "gday":
                        result = "gDay";
                        break;

                    case "gmonth":
                        result = "gMonth";
                        break;



                }

            }

            return result;
        }


        #region RDFXML representation for restrictions

        protected override bool IsNotToBeWritten()
        {
            return IsNotToBeGenerated;
        }

        public void WriteRestrictionToRDFXML(XmlWriter writer, string baseAdress)
        {
            if (!string.IsNullOrEmpty(MultiplicityAsString))
            {
                ///start of subclass element 
                writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsSubclasOf, null);
                ///start of restriction owl element  
                writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.owlRestriction, null);
                writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.OnProperty, null);
                WriteURIToRDFXML(URI, baseAdress, IsBlankNode, OWL2Namespace.rdfResource, OWL2Namespace.rdfResource, writer);
                writer.WriteEndElement();
                WriteCardinalityToRDFXML(writer, baseAdress);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

        }

        private void WriteAllValuesFromTORDFXML(XmlWriter writer, string baseAdress)
        {
            string value = RangeAsObject != null ? baseAdress + RangeAsObject.URI : baseAdress + Range;
            writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.AllValuesFrom, null);
            if (RangeAsObject != null)
            {
                WriteURIToRDFXML(RangeAsObject.URI, baseAdress, RangeAsObject.IsBlankNode, OWL2Namespace.rdfResource, OWL2Namespace.rdfResource, writer);
            }
            else
            {
                WriteURIToRDFXML(Range, baseAdress, false, OWL2Namespace.rdfResource, OWL2Namespace.rdfResource, writer);
            }
            writer.WriteEndElement();
        }


        private string ProcessCardinalityAndCardinalNumber(Dictionary<CardinalityNumber, string> carNumb, CardinaltyType ct, out string cardNumber)
        {
            string qCardinality = string.Empty;
            carNumb.TryGetValue(CardinalityNumber.MIN, out cardNumber);

            if (ct == CardinaltyType.ZEROTOONE)
            {
                qCardinality = OWL2Namespace.MaxQualified;
                carNumb.TryGetValue(CardinalityNumber.MAX, out cardNumber);
            }
            else if (ct == CardinaltyType.ONETOMANY)
            {
                qCardinality = OWL2Namespace.MinQualified;
            }
            else if (ct == CardinaltyType.ONE)
            {
                qCardinality = OWL2Namespace.Qualified;

            }

            return qCardinality;
        }

        private void WriteQualifiedCardinalityToRDFXML(XmlWriter writer, string qCardinality, string value, string cardNumber)
        {
            writer.WriteStartElement(OWL2Namespace.owlPrefix, qCardinality, null);
            writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfDatatype, null, value);

            writer.WriteValue(cardNumber);
            writer.WriteEndElement();
        }

        private void WriteOnClassToRDFXML(XmlWriter writer, string baseAdress)
        {
            string rangeUri = RangeAsObject == null ? Range : RangeAsObject.URI;
            writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.OnClass, null);
            WriteURIToRDFXML(rangeUri, baseAdress, IsBlankNode, OWL2Namespace.rdfResource, OWL2Namespace.rdfResource, writer);
            writer.WriteEndElement();
        }

        private void WriteOnDataRangeToRDFXML(XmlWriter writer, string baseAdress)
        {
            writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.OnDataRange, null);
            writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, ProcessRangeValueForDatatypeProperty(baseAdress));
            writer.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="writer"></param>
        private void WriteCardinalityToRDFXML(XmlWriter writer, string baseAdress)
        {
            Dictionary<CardinalityNumber, string> carNumb;
            CardinaltyType ct = CardinalityHelper.ProcessMultiplicity(MultiplicityAsString, out carNumb);

            if (ct == CardinaltyType.ZEROTOMANY)
            {
                WriteAllValuesFromTORDFXML(writer, baseAdress);
            }
            else
            {

                string xsd = "http://www.w3.org/2001/XMLSchema#";
                string value = String.Format("{0}{1}", xsd, OWL2Namespace.nonNegativeInteger);
                string cardNumber;
                string qCardinality = ProcessCardinalityAndCardinalNumber(carNumb, ct, out cardNumber);

                WriteQualifiedCardinalityToRDFXML(writer, qCardinality, value, cardNumber);

                if (IsObjectProperty())
                {
                    WriteOnClassToRDFXML(writer, baseAdress);
                }
                else
                {
                    WriteOnDataRangeToRDFXML(writer, baseAdress);
                }

            }

        }


        private string ProcessRangeValueForDatatypeProperty(string baseAddress)
        {
            string ns = null;

            Class cls = DataTypeAsComplexObject as Class;

            if (cls != null && !cls.HasStereotype("Primitive"))
            {
                ns = String.Format("{0}{1}", baseAddress, cls.URI);
            }
            else
            {
                ns = String.Format("{0}{1}", "http://www.w3.org/2001/XMLSchema#", ProcessDatatype()); ;
            }

            return ns;
        }



        # endregion



        #endregion

    }

}