using RDFSParserOWL2.Generator.Helper;
using RDFSParserOWL2.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Model
{


	public enum TextType 
	{
		Unknown=0,Comment=1,Label=2
	}

	public enum CardinaltyType
	{
		UNKNOWN = 0, ONETOMANY, ZEROTOMANY, ZEROTOONE, ONE, MANYTOMANY
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
		//// created from shema
		
		private List<ComplexTag> labels;

		private List<ComplexTag> comments;


		//protected string label;
		//protected ComplexTag label;
		//protected ComplexTag comment;

		//protected Dictionary<string,string> label;
		//protected Dictionary<string,string> comment;


		protected string multiplicityAsString = "M:0..1";

		public ProfileElement()
		{
			//label = new Dictionary<string, string>();
			//comment = new Dictionary<string, string>();
			//label = new ComplexTag();
			//comment = new ComplexTag();
			labels = new List<ComplexTag>();
			comments = new List<ComplexTag>();
		}

		public ProfileElement(string uri)
		{
			this.uri = uri;
			labels = new List<ComplexTag>();
			comments = new List<ComplexTag>();
			//label = new ComplexTag();
			//comment = new ComplexTag();
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

		public List<ComplexTag> Labels
		{
			get { return labels; }
			set { labels = value; }
		}

		//public Dictionary<string, string> Label 
		//{
		//	get { return label; }
		//	set { label = value; }
		
		//}
		//public ComplexTag Label
		//{
		//	get { return label; }
		//	set { label = value; }
		//}
		/// 
		//public string Label
		//{
		//	get
		//	{
		//		return label;
		//	}
		//	set
		//	{
		//		label = value;
		//	}
		//}

		/// <summary>
		/// gets and sets the comment atached to the profile element.
		/// </summary>
		/// 
		//public Dictionary<string, string> Comment
		//{
		//	get { return comment; }
		//	set { comment = value; }

		//}


		public List<ComplexTag> Comments
		{
			get { return comments; }
			set { comments = value; }
		}


		//public ComplexTag Comment
		//{
		//	get { return comment; }
		//	set { comment = value; }
		//}

		//public string Comment
		//{
		//	get
		//	{
		//		return comment;
		//	}
		//	set
		//	{
		//		comment = value;
		//	}
		//}

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

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public CardinaltyType ProcessMultiplicity()
		{
			CardinaltyType result=CardinaltyType.UNKNOWN;;
			if (multiplicityAsString != null && multiplicityAsString != null)
			{
				string cardinality = StringManipulationManager.ExtractShortestName(multiplicityAsString, StringManipulationManager.SeparatorColon);
				if (cardinality.Equals("0..1")) 
				{
					result = CardinaltyType.ZEROTOONE;
				}
				else if (cardinality.Equals("1..1") || cardinality.Equals("1")) 
				{
					result = CardinaltyType.ONE;
				}
				else if (cardinality.Equals("1..n")) 
				{
					result = CardinaltyType.ONETOMANY;
				}
				else if (cardinality.Equals("0..n")) 
				{
					result = CardinaltyType.ZEROTOMANY;
				} 

			}

			return result;

		}


        public static string ProcessOwlMultiplicityToString(string card)
        {
            string result=string.Empty;
            if (card != null && card.Equals(OWL2Namespace.AllValuesFrom))
                result = OWL2Namespace.multiplcityNs+"0..n";
            else if (card != null && card.Equals(OWL2Namespace.MinQualified))
				result = OWL2Namespace.multiplcityNs + "1..n";
            else if (card != null && card.Equals(OWL2Namespace.MaxQualified))
				result = OWL2Namespace.multiplcityNs + "0..1";
            else if (card != null && card.Equals(OWL2Namespace.Qualified))
				result = OWL2Namespace.multiplcityNs + "1..1";
            return result;

        }


	}
	
}
