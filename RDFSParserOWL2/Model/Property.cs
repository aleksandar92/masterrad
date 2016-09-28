using RDFSParserOWL2.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		//protected bool isExpectedToContainLocalClass = false; //// if this property expected to contain inner instance of some class

		protected List<ProfileElementStereotype> stereotypes;

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

		#endregion get and set

		#region stereotype

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


		#region helperMethods

	


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




		#endregion

	}

}