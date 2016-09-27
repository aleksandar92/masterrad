using RDFSParserOWL2.Manager;
using RDFSParserOWL2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Parser.Handler
{
	abstract class RDFXMLHandler : IHandler
	{
		protected const string rdfId = "rdf:ID";
		protected const string rdfType = "rdf:type";
		protected const string rdfAbout = "rdf:about";
		protected const string rdfResource = "rdf:resource";
		protected const string rdfParseType = "rdf:parseType";

		protected const string rdfProfileElement = "rdf:Description";
		protected const string rdfPropertyElement = "rdf:Property";

		protected const string rdfsNamespace = "rdfs:";
		protected const string rdfsClassElement = "rdfs:Class";
		protected const string rdfsLabel = "rdfs:label";     // text
		protected const string rdfsComment = "rdfs:comment"; // text
		protected const string rdfsRange = "rdfs:range";
		protected const string rdfsDomain = "rdfs:domain";
		protected const string rdfsSubClassOf = "rdfs:subClassOf";

		protected  Dictionary<string,Dictionary<string,string>> commentsAndLabels;

		protected const string xmlBase = "xml:base";

		protected const string xmlLang = "xml:lang";

		protected const string separator = StringManipulationManager.SeparatorSharp;

		protected string content = string.Empty; //// text content of element
		protected Profile profile;
		protected SortedDictionary<ProfileElementTypes, List<ProfileElement>> allByType;
		//// helper map:  parent class uri,   properties
		////   ie.        package uri,      classes
		protected Dictionary<string, Stack<ProfileElement>> belongingMap;
		//private ProfileElement currentElement;


		


		/// <summary>
		/// stereotypes for element
		/// </summary>
		protected List<string> stereotypes;

		//// for checking if document can't be processed as CIM-RDFS

		protected int checkedElementsCount = 0;
		protected bool abort = false;

		//novo
		protected static SortedList<string, string> prop = new SortedList<string, string>();

		protected static SortedList<string, string> values = new SortedList<string, string>();

		/// <summary>
		/// Gets the Profile object which is finall product of parsing RDFS document.
		/// </summary>
		public Profile Profile
		{
			get
			{
				return profile;
			}
		}

		//public abstract void StartDocument(string filePath);

		public virtual void StartDocument(string filePath)
		{
			profile = new Profile();
			profile.SourcePath = filePath;
			stereotypes = new List<string>();
			allByType = new SortedDictionary<ProfileElementTypes, List<ProfileElement>>();
			commentsAndLabels = new Dictionary<string, Dictionary<string, string>>();
			checkedElementsCount = 0;

			abort = false;
		}

		public abstract void StartElement(string localName, string qName, SortedList<string, string> atts);

		public abstract void EndElement(string localName, string qName);

		public abstract void StartPrefixMapping(string prefix, string uri);

		public abstract void Characters(string text);

		public abstract void EndDocument();





		#region Helper methods


		//novo
		protected void AddProfileElement(ProfileElementTypes tp, ProfileElement el)
		{
			List<ProfileElement> elementsOfSameType = null;
			if (allByType.ContainsKey(tp))
			{
				allByType.TryGetValue(tp, out elementsOfSameType);
			}
			if (elementsOfSameType == null)
			{
				elementsOfSameType = new List<ProfileElement>();
			}
			allByType.Remove(tp);
			elementsOfSameType.Add(el);
			allByType.Add(tp, elementsOfSameType);
		}


		protected string ExtractResourceAttributeFromElement(SortedList<string, string> atts)
		{
			string resourceAtt = string.Empty;
			if (atts.ContainsKey(rdfResource))
			{
				resourceAtt = atts[rdfResource];
			}
			return resourceAtt.Trim();
		}

		protected string ExtractSimpleNameFromResourceURI(string resourceUri)
		{
			return StringManipulationManager.ExtractShortestName(resourceUri, separator);
		}

		//// Connect property of class with class and classes with packages.        
		protected void AddBelongingInformation(ProfileElement currentElement, string classUri)
		{
			if (belongingMap == null)
			{
				belongingMap = new Dictionary<string, Stack<ProfileElement>>();
			}
			Stack<ProfileElement> stack;

			if (!belongingMap.ContainsKey(classUri))
			{
				stack = new Stack<ProfileElement>();
			}
			else
			{
				stack = belongingMap[classUri];
			}
			stack.Push(currentElement);

			belongingMap.Remove(classUri);
			belongingMap.Add(classUri, stack);
		}


		protected void ProcessProfile()
		{
			if (profile.ProfileMap != null)
			{
				List<ProfileElement> moveFromUnknownToEnumElement = new List<ProfileElement>();
				foreach (ProfileElementTypes type in profile.ProfileMap.Keys)
				{
					switch (type)
					{
						case ProfileElementTypes.ClassCategory:
							{
								//List<ClassCategory> list = profile.ProfileMap[type].Cast<ClassCategory>().ToList();
								//foreach (ClassCategory element in list)
								//{
								//	//// search for classes of class categories
								//	if ((belongingMap != null) && (belongingMap.ContainsKey(element.URI)))
								//	{
								//		Stack<ProfileElement> stack = belongingMap[element.URI];
								//		ProfileElement classInPackage;
								//		while (stack.Count > 0)
								//		{
								//			classInPackage = stack.Pop();
								//			if (ExtractSimpleNameFromResourceURI(classInPackage.Type).Equals("Class"))
								//			{
								//				Class cl = (Class)classInPackage;
								//				element.AddToMembersOfClassCategory(cl);
								//				cl.BelongsToCategoryAsObject = element;
								//			}
								//			else
								//			{
								//				ClassCategory cl = (ClassCategory)classInPackage;
								//				element.AddToMembersOfClassCategory(cl);
								//				cl.BelongsToCategoryAsObject = element;
								//			}
								//		}
								//	}
								//}
								break;
							}
						case ProfileElementTypes.Class:
							{
								if (profile.ProfileMap.ContainsKey(type))
								{

									List<Class> list = profile.ProfileMap[type].Cast<Class>().ToList();
									foreach (Class element in list)
									{
										if (element.SubClassOf != null)
										{
											Class uppclass = (Class)profile.FindProfileElementByUri(element.SubClassOf);
											element.SubClassOfAsObject = uppclass;

											if (uppclass != null)
											{
												uppclass.AddToMySubclasses(element);
											}
										}

										//// search for attributes of class and classCategory of class
										if ((belongingMap != null) && (belongingMap.ContainsKey(element.URI)))
										{
											Stack<ProfileElement> stack = belongingMap[element.URI];
											Property property;
											while (stack.Count > 0)
											{
												property = (Property)stack.Pop();
												element.AddToMyProperties(property);
												property.DomainAsObject = element;
											}
										}
									}
								}
								break;
							}
						case ProfileElementTypes.Property:
							{
								if (profile.ProfileMap.ContainsKey(type))
								{

									List<Property> list = profile.ProfileMap[type].Cast<Property>().ToList();
									foreach (Property element in list)
									{
										if (!element.IsPropertyDataTypeSimple)
										{
											element.DataTypeAsComplexObject = profile.FindProfileElementByUri(element.DataType);
										}
										if (!string.IsNullOrEmpty(element.Range))
										{
											element.RangeAsObject = profile.FindProfileElementByUri(element.Range);
										}
										//if (!string.IsNullOrEmpty(element.Name) && (Char.IsUpper(element.Name[0]))
										//    && (!element.HasStereotype(ProfileElementStereotype.StereotypeByReference)))
										//{
										//    element.IsExpectedToContainLocalClass = true;
										//    if (element.RangeAsObject != null)
										//    {
										//        element.RangeAsObject.IsExpectedAsLocal = true;
										//    }
										//}
									}
								}
								break;
							}
						case ProfileElementTypes.Unknown:
							{
								//List<EnumMember> list = profile.ProfileMap[type].Cast<EnumMember>().ToList();
								//foreach (EnumMember element in list)
								//{
								//	Class enumElement = (Class)profile.FindProfileElementByUri(element.Type);
								//	if (enumElement != null)
								//	{
								//		element.EnumerationObject = enumElement;
								//		enumElement.AddToMyEnumerationMembers(element);
								//		moveFromUnknownToEnumElement.Add(element);
								//	}
								//}
								break;
							}
					}
				}
				if (moveFromUnknownToEnumElement.Count > 0)
				{
					List<ProfileElement> unknownsList = null;
					List<ProfileElement> enumerationElementsList = null;
					profile.ProfileMap.TryGetValue(ProfileElementTypes.Unknown, out unknownsList);
					profile.ProfileMap.TryGetValue(ProfileElementTypes.EnumerationElement, out enumerationElementsList);
					if (unknownsList != null)
					{
						if (enumerationElementsList == null)
						{
							enumerationElementsList = new List<ProfileElement>();
						}

						foreach (ProfileElement movingEl in moveFromUnknownToEnumElement)
						{
							unknownsList.Remove(movingEl);
							enumerationElementsList.Add(movingEl);
						}

						profile.ProfileMap.Remove(ProfileElementTypes.Unknown);
						if (unknownsList.Count > 0)
						{
							profile.ProfileMap.Add(ProfileElementTypes.Unknown, unknownsList);
						}

						profile.ProfileMap.Remove(ProfileElementTypes.EnumerationElement);
						if (enumerationElementsList.Count > 0)
						{
							//enumerationElementsList.Sort(CIMComparer.ProfileElementComparer);
							profile.ProfileMap.Add(ProfileElementTypes.EnumerationElement, enumerationElementsList);
						}
					}
				}

				if (profile.ProfileMap.ContainsKey(ProfileElementTypes.Property))
				profile.ProfileMap[ProfileElementTypes.Property] = Profile.RepackProperties(profile.ProfileMap[ProfileElementTypes.Property]);
			}
		}

		/// <summary>
		/// Method for adding content of label or comment in dictionary
		/// </summary>
		/// <param name="key">First key repesents type of content ,comment or label  </param>
		/// <param name="attribute"> Attribute id of content </param>
		/// <param name="content">Content of attribute </param>
		protected void  AddToCommentsAndLabels(string key,string attribute,string content) 
		{
			if (key != null && attribute != null && content != null) 
			{
				
			} 
			
		}


		#endregion






	}
}
