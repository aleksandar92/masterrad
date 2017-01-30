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
	public class Class : ProfileElement
	{
		protected string belongsToCategory;
		protected ProfileElement belongsToCategoryAsObject;
		protected List<ProfileElementStereotype> stereotypes;
		protected string subClassOf;

		protected List<ProfileElement> myProperties;

		protected List<ProfileElement> mySubclasses;
		protected ProfileElement subClassOfAsObject;

		protected bool isEnumeration = false;
		protected List<ProfileElement> myEnumerationMembers;  //// if class is enumeration        

		protected bool isAggregate;

		public Class()
			: base("From Derived")
		{
			subClassOf = string.Empty;
		}


		#region Class specifics

 

		/// <summary>
		/// Class Specific property.
		/// <para>Gets and sets the URI string of parent package i.e. parent class category.</para>
		/// </summary>
		public string BelongsToCategory
		{
			get
			{
				return belongsToCategory;
			}
			set
			{
				belongsToCategory = value;
			}
		}

		public List<ProfileElement> MySubclasses
		{
			get { return mySubclasses; }
		}

		/// <summary>
		/// Gets stereotypes of profile element (element can have more than one stereotype)
		/// </summary>
		public List<ProfileElementStereotype> Stereotypes
		{
			get
			{
				return stereotypes;
			}
		}

		public bool IsEnumeration
		{
			get
			{
				return isEnumeration;
			}
		}

		/// <summary>
		/// Class Specific property.
		/// <para>Gets and sets the URI string of super class(base class).</para>
		/// </summary>
		public string SubClassOf
		{
			get
			{
				return subClassOf;
			}
			set
			{
				subClassOf = value;
			}
		}

		//// Created from schema
		/// <summary>
		/// Class Specific property.
		/// <para>Gets and sets the super(base) class ProfileElement.</para>
		/// </summary>
		public ProfileElement SubClassOfAsObject
		{
			get
			{
				return subClassOfAsObject;
			}
			set
			{
				subClassOfAsObject = value;
			}
		}

		/// <summary>
		/// Class Specific property.
		/// <para>Gets and sets the parent package i.e. parent class category. </para>
		/// </summary>
		public ProfileElement BelongsToCategoryAsObject
		{
			get
			{
				return belongsToCategoryAsObject;
			}
			set
			{
				belongsToCategoryAsObject = value;
			}
		}

		/// <summary>
		/// Class Specific property.
		/// <para>Gets the list of profile elements which are the properties of given profile class.</para>
		/// </summary>
		public List<ProfileElement> MyProperties
		{
			get
			{
				return myProperties;
			}
		}

		/// <summary>
		/// Class Specific property.
		/// <para>Gets the list of profile elements which are the properties of given profile class,</para>
		/// <para>as well as all the inherited properties.</para>  
		/// </summary>
		public List<ProfileElement> MyAndInheritedProperties
		{
			get
			{
				List<ProfileElement> allProperties = myProperties;

				return allProperties;
			}
		}

		/// <summary>
		/// Method creates new instance of ProfileElementStereotype and adds it to ProfileElement's
		/// stereotypes list if it doesn't already exists.
		/// </summary>
		/// <param fullName="fullStereotypeString">the full stereotype fullName</param>
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
		/// Method for checking if class has certain  stereotype
		/// </summary>
		/// <param name="stereotypeName">Stereotype for checking existence </param>
		/// <returns> True if class has sterotype otherwise false </returns>
        public bool HasStereotype(string stereotypeName)
        {
            bool hasStereotype = false;
            if (stereotypes != null)
            {
                foreach (ProfileElementStereotype stereotype in stereotypes)
                {
                    if ((string.Compare(stereotype.Name, stereotypeName, StringComparison.OrdinalIgnoreCase) == 0) || (string.Compare(stereotype.ShortName.ToLower(), stereotypeName.ToLower(), StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        hasStereotype = true;
                        break;
                    }
                }
            }
            return hasStereotype;
        }


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

        public override bool ContainsStereotype(string stereotype) 
        {
            return HasStereotype(stereotype);
        }


		public List<ProfileElementStereotype> GetUndefinedStereotypes()
		{
			List<ProfileElementStereotype> undefinedStereotypes = new List<ProfileElementStereotype>();

			if (stereotypes != null)
			{
				foreach (ProfileElementStereotype stereotype in stereotypes)
				{
					if (!stereotype.Name.Equals(ProfileElementStereotype.StereotypeConcrete) && !stereotype.Name.Equals(ProfileElementStereotype.StereotypeCompound) && !stereotype.Name.Equals(ProfileElementStereotype.StereotypeEnumeration) && !stereotype.Name.Equals(ProfileElementStereotype.StereotypeAttribute) && !stereotype.Name.Equals(ProfileElementStereotype.StereotypeByReference) && !stereotype.Name.Equals(ProfileElementStereotype.StereotypeOfAggregate) && !stereotype.Name.Equals(ProfileElementStereotype.StereotypeAggregateOf) && !stereotype.Name.Equals(ProfileElementStereotype.StereotypeCompositeOf))
					{
						undefinedStereotypes.Add(stereotype);
					}
				}
			}

			return undefinedStereotypes;
		}

		/// <summary>
		/// Method adds given ProfileElement to the MyEnumerationMembers list. 
		/// </summary>
		/// <param fullName="enumerationMemeber"></param>
		public void AddToMyEnumerationMembers(ProfileElement enumerationMemeber)
		{
			if (enumerationMemeber != null)
			{
				if (!isEnumeration)
				{
					isEnumeration = true;
				}

				if (myEnumerationMembers == null)
				{
					myEnumerationMembers = new List<ProfileElement>();
				}

				if (!myEnumerationMembers.Contains(enumerationMemeber))
				{
					myEnumerationMembers.Add(enumerationMemeber);
					//myEnumerationMembers.Sort(CIMComparer.ProfileElementComparer);
				}
			}
		}

		/// <summary>
		/// Class Specific property.
		/// <para>If profile element is enumeration class, property gets all enumeration elements.</para>
		/// </summary>
		public List<ProfileElement> MyEnumerationMembers
		{
			get
			{
				return myEnumerationMembers;
			}

			set 
			{
				myEnumerationMembers = value;
			}
		}

		/// <summary>
		/// Method adds given ProfileElement to the MySubclasses list.
		/// </summary>
		/// <param fullName="subclass"></param>
		public void AddToMySubclasses(ProfileElement subclass)
		{
			if (mySubclasses == null)
			{
				mySubclasses = new List<ProfileElement>();
			}

			if (!mySubclasses.Contains(subclass))
			{
				mySubclasses.Add(subclass);
				//mySubclasses.Sort(CIMComparer.ProfileElementComparer);
			}
		}

		/// <summary>
		/// Method adds given ProfileElement to the MyProperties list. 
		/// </summary>
		/// <param fullName="property"></param>
		public void AddToMyProperties(ProfileElement property)
		{
			if (myProperties == null)
			{
				myProperties = new List<ProfileElement>();
			}

			if (!myProperties.Contains(property))
			{
				myProperties.Add(property);
				//myProperties.Sort(CIMComparer.ProfileElementComparer);
			}
		}

		public ProfileElement GetFromMyProperties(string name)
		{
			if (myProperties != null)
			{
				foreach (ProfileElement property in myProperties)
				{
					if (property.Name.ToLower().Equals(name.ToLower()))
					{
						return property;
					}
				}
			}

			return null;
		}

        #region Methods for handling equivalence with another element

        public override void EquivalenceWithAnotherElement(string uriOfEquivalent)
        {
            if (!string.IsNullOrEmpty(uriOfEquivalent) && !IsBlankNode && !HasDifferentStereotype(ConfigurationFiles.StereotypesToSkip))
            {
                UriOfEquivalentElement = String.Format("{0}{1}",uriOfEquivalent, uri);
            }
        }


        public override bool EquvialenceWithAnotherElementWithStereotype(string baseUriOfElement, string stereotype)
        {
            bool result = false;

            if (!IsBlankNode  && HasStereotype(stereotype))
            {
                UriOfEquivalentElement = String.Format("{0}{1}{2}", baseUriOfElement, stereotype, uri);
                result = true;
            }

            return result;
        }


        #endregion


		

		#endregion Class specifics

        #region RDFXML representation

        protected override bool IsNotToBeWritten() 
        {
            return IsNotToBeGenerated;
        }


        protected override void WriteSpecificElementsToRDFXML(XmlWriter writer, string baseAddress, EnumRepresentationKindOWL2 enumOpt)
        {
            WriteBelongsToCategoryToRDFXML(writer,baseAddress);
            WriteSubclassToRDFXML(writer, baseAddress);
            WriteStereotypesToRDFXML(stereotypes,writer);
            WriteEnumerationToRDFXML(writer, baseAddress,enumOpt);
            WriteRestrictionsToRDFXML(writer,baseAddress);

        }

        private void WriteBelongsToCategoryToRDFXML(XmlWriter writer, string baseAddress)
        {
            if (!string.IsNullOrEmpty(BelongsToCategory) && belongsToCategoryAsObject != null)
            {
                ClassCategory cat = BelongsToCategoryAsObject as ClassCategory;
                if (cat != null)
                {
                    string baseURI;
                    writer.WriteStartElement(MetaNamespace.MetaPrefix, MetaNamespace.Belongs, null);
                    if (!cat.IsBasePackage)
                    {
                        baseURI = baseAddress + BelongsToCategory;
                    }
                    else
                    {
                        baseURI = cat.URI;
                    }
                    WriteURIToRDFXMLWithFullURI(baseURI, OWL2Namespace.rdfResource, writer);
                    writer.WriteEndElement();
                }
            }
        }


        private void WriteRestrictionsToRDFXML(XmlWriter writer, string baseAddress) 
        {
            if (MyProperties != null)
            {
                foreach (ProfileElement pe in MyProperties)
                {
                    Property prop = pe as Property;
                    if (prop != null)
                        prop.WriteRestrictionToRDFXML(writer, baseAddress);
                }
            }
        }

        private void WriteSubclassToRDFXML(XmlWriter writer, string baseAddress) 
        {
            if (!string.IsNullOrEmpty(subClassOf) && SubClassOfAsObject != null)
            {
                writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsSubclasOf, null);
                WriteURIToRDFXML(SubClassOf, baseAddress, IsBlankNode, OWL2Namespace.rdfResource, OWL2Namespace.rdfResource, writer);
                writer.WriteEndElement();
            }
        }

        private void WriteEnumerationToRDFXML(XmlWriter writer, string baseAddress, EnumRepresentationKindOWL2 enumOpt) 
        {
            if ( IsEnumeration && writer != null && enumOpt==EnumRepresentationKindOWL2.CLOSED)
            {
                writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.EqClass, null);
                writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.Class, null);
                writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.OneOf, null);
                writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.ParseType, null, OWL2Namespace.Collection);
                WriteEnumMembersToRDFXML(writer,baseAddress,MyEnumerationMembers);
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        private void WriteEnumMembersToRDFXML(XmlWriter writer,string baseAdress,List<ProfileElement> enumMembers) 
        {
            if (enumMembers != null )
            {
                foreach (ProfileElement em in enumMembers)
                {
                    WriteDescriptionToRDFXML(writer,em.IsBlankNode, baseAdress, em.URI);
                }
            }
        }

        protected override void WriteEquivalenceToRDFXML(XmlWriter writer, string baseAdress) 
        {
            WriteEquivalentClassToRDFXML(writer,baseAdress);

        }

        private void WriteEquivalentClassToRDFXML(XmlWriter writer, string baseAdress) 
        {
            if (!string.IsNullOrEmpty(UriOfEquivalentElement))
            {
                writer.WriteStartElement(OWL2Namespace.rdfPrefix, OWL2Namespace.Description, null);
                WriteURIToRDFXML(StringManipulationManager.ExtractAllWithSeparator(URI, StringManipulationManager.SeparatorSharp), baseAdress, IsBlankNode, OWL2Namespace.rdfAbout, OWL2Namespace.NodeId, writer);
                writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.EqClass, null);
                writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, UriOfEquivalentElement);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
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
                reportStatus.Add(EntityTypesGeneratorReporter.Class);
            }

            return reportStatus;
        }

        protected override string OWLRDFXMLType()
        {
            return OWL2Namespace.Class;
        }


        # endregion

	}
}
