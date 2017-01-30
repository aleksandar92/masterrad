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


	public enum TextType 
	{
		Unknown=0,Comment=1,Label=2
	}


	/// <summary>
	/// Possible types of profile elements
	/// </summary>
	public enum ProfileElementTypes
	{
		Unknown = 0, ClassCategory, Property, EnumerationElement, Class, Stereotype
	};

	/// <summary>
	/// ProfileElement class represents one element founded during processing of profile's source file.
	/// <para>See also: <seealso cref="T:Profile"/></para>
	/// <para>@author: Stanislava Selena</para>
	/// </summary>
	public abstract class ProfileElement
	{
		public const string Separator = StringManipulationManager.SeparatorSharp;

		/// <summary> string with value "classcategory" </summary>
		public const string TypeClassCategoryString = "classcategory";
		/// <summary> string with value "class" </summary>
		public const string TypeClassString = "class";
		/// <summary> string with value "property" </summary>
		public const string TypePropertyString = "property";
		/// <summary> string with value "stereotype" </summary>
		public const string TypeStereotypeString = "stereotype";

		/// <summary> "integer" </summary>
		protected const string SimpleDataTypeInteger = "integer";
		/// <summary> "int" </summary>
		protected const string SimpleDataTypeInt = "int";
		/// <summary> "long" </summary>
		protected const string SimpleDataTypeLong = "long";
		/// <summary> "float"  </summary>
		protected const string SimpleDataTypeFloat = "float";
		/// <summary> "double"  </summary>
		protected const string SimpleDataTypeDouble = "double";
		/// <summary> "string" </summary>
		protected const string SimpleDataTypeString = "string";
		/// <summary> "dateTime" </summary>
		protected const string SimpleDataTypeDateTime = "datetime";
		/// <summary> "boolean" </summary>
		protected const string SimpleDataTypeBoolean = "boolean";
		/// <summary> "bool" </summary>
		protected const string SimpleDataTypeBool = "bool";


		protected string uri;
		protected string type;
        protected ProfileElementTypes kind;
		//// created from shema
        private bool isBlankNode;
        private string uriOfEquivalentElement;

        protected string isFixed;
		protected List<TextElement> labels;
		protected List<TextElement> comments;
		/// <summary>
		/// used for generating owl profile to idnicate if element is not going to be generated
		/// </summary>
		private bool isNotToBeGenerated;

		protected string multiplicityAsString = "M:0..1";


		public ProfileElement()
		{
			labels = new List<TextElement>();
			comments = new List<TextElement>();
		}

		public ProfileElement(string uri)
		{
			this.uri = uri;
			labels = new List<TextElement>();
			comments = new List<TextElement>();
            isBlankNode = false;
		}


        public bool IsBlankNode
        {
            get { return isBlankNode; }
            set { isBlankNode = value; }
        }

		/// <summary>
		/// Gets and sets the full URI of profile element.
		/// </summary>
		public string URI
		{
			get
			{
				return uri;
			}
			set
			{
				uri = value;
			}
		}


        /// <summary>
        /// Gets or sets isFixed of profile element
        /// </summary>
        public string IsFixed
        {
            get { return isFixed; }
            set { isFixed = value; }
        }


		/// <summary>
		/// Gets and sets the multiplicityAsString of profile element.
		/// </summary>
		public string MultiplicityAsString
		{
			get
			{
				return multiplicityAsString;
			}
			set
			{
				multiplicityAsString = value;
			}
		}

        /// <summary>
        /// Uri of equivalent element
        /// </summary>
        protected string UriOfEquivalentElement
        {
            get { return uriOfEquivalentElement; }
            set { uriOfEquivalentElement = value; }
        }

		/// <summary>
		/// Gets the part of URI after the '#' simbol
		/// </summary>
		public string UniqueName
		{
			get
			{
				string uniqueName = StringManipulationManager.ExtractShortestName(uri, Separator);
				return uniqueName;
			}
		}


		public bool IsNotToBeGenerated
		{
			get { return isNotToBeGenerated; }
			set { isNotToBeGenerated = value; }
		} 


		/// <summary>
		/// Gets the most simple fullName ( URI.sustring('#').substring('.') )
		/// </summary>
		public string Name
		{
			get
			{
				string name = StringManipulationManager.ExtractShortestName(uri, Separator);
				name = StringManipulationManager.ExtractShortestName(name, StringManipulationManager.SeparatorDot);
				return name;
			}
		}

		/// <summary>
		/// Gets and sets the type of given profile element in string format.
		/// </summary>
		public string Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
			}
		}

		/// <summary>
		/// Gets and sets the labels of profile element.
		/// </summary>
		/// 

		public List<TextElement> Labels
		{
			get { return labels; }
			set { labels = value; }
		}


		public List<TextElement> Comments
		{
			get { return comments; }
			set { comments = value; }
		}

        public ProfileElementTypes Kind 
        {
            get { return kind; }
            set { kind=value; }
        }


		public override bool Equals(object obj)
		{
			bool eq = false;
			if (obj != null)
			{
				if (obj.GetType().Equals(this.GetType()))
				{
					ProfileElement pelObj = (ProfileElement)obj;
					if ((pelObj.URI != null) && (this.URI != null))
					{
						eq = this.URI.Equals(pelObj.URI);
					}
				}
			}
			return eq;
		}

        public void ClearEquivalence() 
        {
            uriOfEquivalentElement = string.Empty;
        }

		public override int GetHashCode()
		{
			return base.GetHashCode();
        }

        #region Methods for handling equivalence with another element
        
        /// <summary>
        /// Method used for equivalent element in another ontology
        /// </summary>
        /// <param name="baseUriOfElement"></param>
        /// <param name="shortNameOfElement"></param>
        public virtual void EquivalenceWithAnotherElement(string uriOfEquivalent) 
        {
            if (!isBlankNode && !string.IsNullOrEmpty(uriOfEquivalent)) 
            {
                UriOfEquivalentElement = String.Format("{0}{1}", uriOfEquivalent, uri);
            }   
        }

        /// <summary>
        /// Method used for eqquvivalence with another element if given element contains given stereotype
        /// </summary>
        /// <param name="baseUriOfElement"></param>
        /// <param name="stereotype"></param>
        public abstract bool EquvialenceWithAnotherElementWithStereotype(string baseUriOfElement, string stereotype);

        public void EquivalenceForAnotherElementCombined(string commonUri,string baseUriOfElementWithStereotype, string stereotype) 
        {
            if (!isBlankNode &&!EquvialenceWithAnotherElementWithStereotype(baseUriOfElementWithStereotype, stereotype))
                EquivalenceWithAnotherElement(commonUri);
        }

        #endregion
        #region RDFXML representation

        protected void WriteURIToRDFXML(string uri, string baseURI, bool isBlank, string startingTag, string blankNodeTag,XmlWriter writer)
        {
            if (!isBlank)
            {
                writer.WriteAttributeString(OWL2Namespace.rdfPrefix, startingTag, null, baseURI + uri);
            }
            else
            {

                writer.WriteAttributeString(OWL2Namespace.rdfPrefix, blankNodeTag, null, StringManipulationManager.SeparatorBlankNode + uri);
            }
        }

        protected void WriteURIToRDFXMLWithFullURI(string uri, string startingTag,XmlWriter writer)
        {               
                writer.WriteAttributeString(OWL2Namespace.rdfPrefix, startingTag, null, uri);
        }

        protected void WriteDescriptionToRDFXML(XmlWriter writer, bool isBlank, string baseURI, string uri)
        {
            writer.WriteStartElement(OWL2Namespace.rdfPrefix, OWL2Namespace.Description, null);
            WriteURIToRDFXML(uri, baseURI, isBlank, OWL2Namespace.rdfAbout, OWL2Namespace.NodeId, writer);
            writer.WriteEndElement();
        }

        protected void WriteStereotypesToRDFXML(List<ProfileElementStereotype> sterotypes, XmlWriter writer)
        {
            if (sterotypes != null && writer != null)
            {
                foreach (ProfileElementStereotype pes in sterotypes)
                {
                    pes.WriteProfileElementStereotype(writer);
                }
            }
        }

        protected abstract string OWLRDFXMLType();

        protected abstract void WriteSpecificElementsToRDFXML(XmlWriter writer, string baseAddress, EnumRepresentationKindOWL2 enumOpt);

        protected abstract bool IsNotToBeWritten();

        public abstract List<EntityTypesGeneratorReporter> StatusForOWL2();

        public  void ToRDFXML(XmlWriter writer,string baseAddress, EnumRepresentationKindOWL2 enumOpt)
        {
            if (IsNotToBeWritten())
                return;

            WriteEquivalenceToRDFXML(writer,baseAddress);
            WriteStartOfElementToRDFXML(writer,baseAddress);
            WriteSpecificElementsToRDFXML(writer,baseAddress,enumOpt);
            WriteIsFixedToRDFXML(writer);
            WriteIsDefinedByToRDFXML(writer,baseAddress);
            WriteCommentsToRDFXML(writer);
            WriteLabelsToRDFXML(writer);
            writer.WriteEndElement();
        }

        protected abstract void WriteEquivalenceToRDFXML(XmlWriter writer,string baseAdress);

        private void WriteIsDefinedByToRDFXML(XmlWriter writer, string baseAdress) 
        {
            writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.IsDefinedBy, null);
            writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, baseAdress);
            writer.WriteEndElement();
        }

        protected void WriteStartOfElementToRDFXML(XmlWriter writer, string baseAddress)
        {
            writer.WriteStartElement(OWL2Namespace.owlPrefix, OWLRDFXMLType(), null);
            WriteURIToRDFXML(URI, baseAddress, IsBlankNode, OWL2Namespace.rdfAbout, OWL2Namespace.NodeId, writer);
        }

        private void WriteIsFixedToRDFXML(XmlWriter writer) 
        {
            if (!string.IsNullOrEmpty(IsFixed))
            {
                writer.WriteStartElement(MetaNamespace.MetaPrefix, MetaNamespace.Fixed, null);
                writer.WriteValue(IsFixed);
                writer.WriteEndElement();
            }
        }

        private void WriteLabelsToRDFXML(XmlWriter writer) 
        {
            if (Labels != null)
            {
                foreach (TextElement ct in Labels)
                {
                    ct.WriteTextElementToRDFXML(writer, OWL2Namespace.rdfsLabel);
                }
            }
        }

        private void WriteCommentsToRDFXML(XmlWriter writer)
        {
            if (Comments != null)
            {
                foreach (TextElement ct in Comments)
                {
                    ct.WriteTextElementToRDFXML(writer, OWL2Namespace.rdfsComment);
                }
            }
        }

           # endregion

        #region working with stereotype

        public abstract bool ContainsStereotype(string stereotype);

        #endregion
    }
	
}
