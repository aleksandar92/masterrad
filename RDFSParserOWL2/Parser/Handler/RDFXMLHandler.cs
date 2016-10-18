using RDFSParserOWL2.Common.Comparers;
using RDFSParserOWL2.Generator.Helper;
using RDFSParserOWL2.Manager;
using RDFSParserOWL2.Model;
using RDFSParserOWL2.Reporter.Interfaces;
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

		private IParserReporter reporter;

		protected const string cimsStereotype = "cims:stereotype";

		protected Dictionary<TextType, List<ComplexTag>> commentsAndLabels;

		protected const string xmlBase = "xml:base";

		protected const string xmlLang = "xml:lang";

		protected Dictionary<string, string> commentAndLabelAtts;

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

		public IParserReporter Reporter
		{
			get { return reporter; }
			set { reporter = value; }
		}

		//public abstract void StartDocument(string filePath);

		public virtual void StartDocument(string filePath)
		{
			profile = new Profile();
			profile.SourcePath = filePath;
			stereotypes = new List<string>();
			allByType = new SortedDictionary<ProfileElementTypes, List<ProfileElement>>();
			commentsAndLabels = new Dictionary<TextType, List<ComplexTag>>();
			checkedElementsCount = 0;
			commentAndLabelAtts = new Dictionary<string, string>();
			abort = false;
		}

		public virtual void StartElement(string localName, string qName, SortedList<string, string> atts)
		{
			/**
			* Deo neophodan za proveru ako postoji xml:base jer tada elementi, bar vecina nema nista pre #
			*/
			if (atts.ContainsKey(xmlBase))
			{
				profile.BaseNS = atts[xmlBase];
				Console.WriteLine(profile.BaseNS);
			}
			else if (qName.ToLower().Equals(cimsStereotype.ToLower()))
			{
				string val;
				atts.TryGetValue(rdfResource, out val);
				if (!string.IsNullOrEmpty(val))
					stereotypes.Add(val.Trim().ToLower());
			}
			else if (qName.Equals(rdfsComment) || qName.Equals(rdfsLabel))
			{
				if (commentsAndLabels == null)
				{
					commentAndLabelAtts = new Dictionary<string, string>();
				}
				foreach (KeyValuePair<string, string> at in atts)
				{
					commentAndLabelAtts.Add(at.Key, at.Value);
				}

			}
			//novo
			else
			{
				string ls;
				prop.TryGetValue(qName, out ls);

				foreach (KeyValuePair<string, string> at in atts)
				{
					if (ls != null)
					{
						int i = 0;
						do
						{
							ls = null; ;
							prop.TryGetValue(qName + (++i), out ls);
						} while (ls != null);
						ls = at.Value;
						prop.Add(qName + i, ls);
					}
					else
					{
						ls = at.Value;
						prop.Add(qName, ls);
					}
				}
			}


		}



		public virtual bool IsEndElement(string qName)
		{

			return qName.ToLower().Equals(rdfProfileElement.ToLower())
	|| qName.Equals(rdfsClassElement) || qName.Equals(rdfPropertyElement);
		}

		public virtual void EndElement(string localName, string qName)
		{
			if (IsEndElement(qName)) //end of element    
			{
				//novo
				if (prop != null)
				{
					string type;
					GetType(out type, localName);

					//if (ExtractSimpleNameFromResourceURI(type) == "ClassCategory")
					//{
					//	//ClassCategory cs = new ClassCategory();
					//	//foreach (KeyValuePair<string, string> pp in prop)
					//	//{
					//	//	string str = pp.Value;
					//	//	if ((pp.Key.Equals(cimsBelongsToCategory)) && (str != null))
					//	//	{
					//	//		cs.BelongsToCategory = str;
					//	//		AddBelongingInformation(cs, cs.BelongsToCategory);
					//	//	}
					//	//	else if ((pp.Key.Equals(rdfsComment)) && (str != null))
					//	//	{
					//	//		cs.Comment = str;
					//	//	}
					//	//	else if ((pp.Key.Equals(rdfsLabel)) && (str != null))
					//	//	{
					//	//		cs.Label = str;
					//	//	}
					//	//	else if ((pp.Key.Equals(rdfType)) && (str != null))
					//	//	{
					//	//		cs.Type = str;
					//	//	}
					//	//	else if ((pp.Key.Equals(rdfProfileElement)) && (str != null))
					//	//	{
					//	//		cs.URI = str;
					//	//	}
					//	//	else if ((pp.Key.Equals(cimsMultiplicity)) && (str != null))
					//	//	{
					//	//		cs.MultiplicityAsString = ExtractSimpleNameFromResourceURI(str);
					//	//	}
					//	//}
					//	//AddProfileElement(ProfileElementTypes.ClassCategory, cs);
					//}
					//else
                    if(IsClassCategory(type)) 
                    {
                        PopulateClassCategory(localName);
                        reporter.AddtoEntityCountByType(EntityTypesReporter.ClassCategory,1);
                    }else 
					if (IsClass(type))
					{
						PopulateClass(localName);
						reporter.AddtoEntityCountByType(EntityTypesReporter.Class, 1);
					}
					else if (IsProperty(type))
					{
						PopulateProperty(localName);
						reporter.AddtoEntityCountByType(EntityTypesReporter.Property, 1);
					}
					else
					{
						PopulateEnumMember(localName);
						reporter.AddtoEntityCountByType(EntityTypesReporter.Unknown, 1);
					}

					prop.Clear();
					values.Clear();
					stereotypes.Clear();
					commentsAndLabels.Clear();

				}
			}

			else if (qName.Equals(rdfsLabel)) //// end of label subelement
			{
				content = content.Trim();
				if (!string.IsNullOrEmpty(content))
				{
					if (commentAndLabelAtts == null)
					{
						commentAndLabelAtts = new Dictionary<string, string>();
					}

					AddComplexTagToCommentsAndLabels(TextType.Label, new ComplexTag(content, commentAndLabelAtts));
				}
				content = string.Empty;
				commentAndLabelAtts.Clear();
			}
			else if (qName.Equals(rdfsComment)) //// end of comment subelement
			{
				content = content.Trim();
				if (!string.IsNullOrEmpty(content))
				{
					if (commentAndLabelAtts == null)
					{
						commentAndLabelAtts = new Dictionary<string, string>();
					}
					///Dodavanje novog komentara u kolekciju koja ce kasnije biti ispraznjena 
					AddComplexTagToCommentsAndLabels(TextType.Comment, new ComplexTag(content, commentAndLabelAtts));
				}
				content = string.Empty;
				commentAndLabelAtts.Clear();
			}
			//else if (qName.Equals(cimsIsAggregate)) //// end of isAggregate subelement
			//{
			//	content = content.Trim();
			//	if (!string.IsNullOrEmpty(content))
			//	{
			//		bool paresedValue;

			//		//novo
			//		string ls;
			//		prop.TryGetValue(qName, out ls);
			//		if (ls == null)
			//		{
			//			if (bool.TryParse((string)content.Clone(), out paresedValue))
			//			{
			//				ls = paresedValue.ToString();
			//			}
			//			prop.Add(qName, ls);
			//		}
			//		content = string.Empty;
			//	}
			//}
			else if (IsStereotype(qName))
			{
               // qName.Equals(cimsStereotype)
				content = content.Trim();
				if (!string.IsNullOrEmpty(content))
				{
					stereotypes.Add(content.Trim().ToLower());
				}
				content = string.Empty;
			}
			else
			{
				content = content.Trim();
				if (!string.IsNullOrEmpty(content))
				{
					if(!values.ContainsKey(qName))
					values.Add(qName, content.Trim().ToLower());
				}
				content = string.Empty;

			} 


		}

        public abstract bool IsStereotype(string qName);

		public abstract void StartPrefixMapping(string prefix, string uri);


		protected virtual void GetType(out string type, string localName)
		{
			prop.TryGetValue(rdfType, out type);
		}


        protected virtual bool IsClassCategory(string type) 
        {
            return ExtractSimpleNameFromResourceURI(type).Equals("ClassCategory");
        }

		protected virtual bool IsClass(string type)
		{
			return ExtractSimpleNameFromResourceURI(type).Equals("Class") || ExtractSimpleNameFromResourceURI(type).Equals("class");
		}

		protected virtual bool IsProperty(string type)
		{
			return ExtractSimpleNameFromResourceURI(type).Equals("Property") || ExtractSimpleNameFromResourceURI(type).Equals("property");
		}

		public virtual void Characters(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				content = text;
			}
			else
			{
				content = string.Empty;
			}
		}


		protected void PopulateUri(ProfileElement pe,string uriValue) 
		{
			if (pe != null)
			{
				if (uriValue.Contains(StringManipulationManager.SeparatorSharp))
				{
					pe.URI = StringManipulationManager.ExtractAllWithSeparator(uriValue, StringManipulationManager.SeparatorSharp);
				}

				else if (StringManipulationManager.IsBlankNode(uriValue))
				{
					reporter.AddtoEntityCountByType(EntityTypesReporter.BlankId, 1);
					pe.URI = StringManipulationManager.ExtractAllWithSeparator(uriValue, StringManipulationManager.SeparatorBlankNode);
				}
			}
		}

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
			//allByType.Remove(tp);
			//HashSet<ProfileElement> hsElementsOfSameType = new HashSet<ProfileElement>(elementsOfSameType);
			//if (!hsElementsOfSameType.Contains(el, new ProfileElementComparer()))
			//{
			//	hsElementsOfSameType.Add(el);
			//}
			elementsOfSameType.Add(el);
			//HashSet<ProfileElement> hsElementsOfSameType = new HashSet<ProfileElement>(elementsOfSameType);
			//if (!hsElementsOfSameType.Contains(el, new ProfileElementComparer()))
			//{
			//	hsElementsOfSameType.Add(el);
			//}
			//elementsOfSameType.Add(el);
			allByType.Add(tp, elementsOfSameType.ToList());
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
			if (profile != null && profile.ProfileMap != null)
			{
				List<ProfileElement> moveFromUnknownToEnumElement = new List<ProfileElement>();
				foreach (ProfileElementTypes type in profile.ProfileMap.Keys)
				{
					switch (type)
					{
                        case ProfileElementTypes.ClassCategory:
                            {
                                ProcessClassCategory();
                                break;
                            }
						case ProfileElementTypes.Class:
							{
								ProcessClass();
								break;
							}
						case ProfileElementTypes.Property:
							{
								ProcessProperty();
								break;
							}
						case ProfileElementTypes.Unknown:
							{

								List<EnumMember> list = profile.ProfileMap[type].Cast<EnumMember>().ToList();
								foreach (EnumMember element in list)
								{
									Class enumElement = (Class)profile.FindProfileElementByUri(element.Type);
									if (enumElement != null)
									{
										element.EnumerationObject = enumElement;
										enumElement.AddToMyEnumerationMembers(element);
										moveFromUnknownToEnumElement.Add(element);
										reporter.AddtoEntityCountByType(EntityTypesReporter.EnumMembers, 1);
										reporter.RemoveFromEntityCountByType(EntityTypesReporter.Unknown, 1);
									}
								}
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
		/// Populate class attributes
		/// </summary>
		/// <param name="atts"></param>
		protected virtual void PopulateClass(string localName)
		{

			Class cs = new Class();
			OperationsForPopulatingClass(cs, localName);
			AddProfileElement(ProfileElementTypes.Class, cs);

		}

		/// <summary>
		/// Puts all procedures for populating class at one place 
		/// </summary>
		/// <param name="cs">Class to be populated  </param>
		/// <param name="localName">Local name of tag that is being processed</param>
		protected virtual void OperationsForPopulatingClass(Class cs, string localName)
		{
			PopulateClassAttributes(cs, localName);
			PopulateStereotypes(cs);
			ProccessCommentsAndLabels(cs);
		}

		protected virtual void PopulateStereotypes(Class cs)
		{
			if (stereotypes != null)
			{
				foreach (string s in stereotypes)
				{
					cs.AddStereotype(s);
				}
			}
		}


		/// <summary>
		/// Method for populating class category
		/// </summary>
		/// <param name="localName"></param>
		protected virtual void PopulateClassCategory(string localName) 
		{
			ClassCategory csCat = new ClassCategory();
			PopulateClassCategoryAttributes(csCat,localName);
			ProccessCommentsAndLabels(csCat);
            AddProfileElement(ProfileElementTypes.ClassCategory, csCat);
		}


		protected virtual void PopulateClassCategoryAttributes(ClassCategory csCat,string localName)
		{
			foreach (KeyValuePair<string, string> pp in prop)
			{
				//string str = pp.Value;
				PopulateClassCategoryAttribute(csCat, pp.Value, pp.Key, localName);
			}

			foreach (KeyValuePair<string, string> val in values)
			{
				PopulateClassCategoryAttribute(csCat, val.Value, val.Key, localName);
			}


		}

		protected virtual void PopulateClassCategoryAttribute(ClassCategory csCat, string attrVal, string attr, string localName)
		{
            if ((attr.Equals(rdfType)) && (attrVal != null))
            {
                csCat.Type = attrVal;
                   // StringManipulationManager.ExtractAllWithSeparator(attrVal, StringManipulationManager.SeparatorSharp);
            }

			//if ((attr.ToLower().Contains(rdfsSubClassOf.ToLower())) && (attrVal != null))
			//{
			//	cs.SubClassOf = StringManipulationManager.ExtractAllWithSeparator(attrVal, StringManipulationManager.SeparatorSharp);
			//}

		}


		protected virtual void PopulateClassAttributes(Class cs, string localName)
		{
			foreach (KeyValuePair<string, string> pp in prop)
			{
				//string str = pp.Value;
				PopulateClassAttribute(cs, pp.Value, pp.Key, localName);
			}

			foreach (KeyValuePair<string, string> val in values)
			{
				PopulateClassAttribute(cs, val.Value, val.Key, localName);
				//string str = val.Value;
			}


		}


		protected virtual void PopulateProperty(string localName)
		{
			Property pr = new Property();
			foreach (KeyValuePair<string, string> pp in prop)
			{
				//string str = pp.Value;
				PopulatePropertyAttribute(pr, pp.Value, pp.Key, localName);
			}

			foreach (KeyValuePair<string, string> val in values)
			{
				PopulatePropertyAttribute(pr, val.Value, val.Key, localName);
				//string str = val.Value;
			}

			if (stereotypes != null)
			{
				foreach (string s in stereotypes)
				{
					pr.AddStereotype(s);
				}
			}
			ProccessCommentsAndLabels(pr);
			AddProfileElement(ProfileElementTypes.Property, pr);
		}


		protected virtual void PopulateEnumMember(string localName)
		{
			EnumMember en = new EnumMember();
			foreach (KeyValuePair<string, string> pp in prop)
			{
				string str = pp.Value;
				PopulateEnumMemberAttribute(en, pp.Value, pp.Key, localName);
			}
			ProccessCommentsAndLabels(en);
			AddProfileElement(ProfileElementTypes.Unknown, en);
		}


		protected virtual void PopulateEnumMemberAttribute(EnumMember em, string attrVal, string attr, string localName)
		{
			if ((attr.Equals(rdfType)) && (attrVal != null))
			{
				em.Type = StringManipulationManager.ExtractAllWithSeparator(attrVal, StringManipulationManager.SeparatorSharp);
			}

		}

		/// <summary>
		/// Populate class aattribute 
		/// </summary>
		/// <param name="cs">Class for population </param>
		/// <param name="attrVal">Value of attribute to be inserted </param>
		/// <param name="attr">Attribute to be populated</param>
		protected virtual void PopulateClassAttribute(Class cs, string attrVal, string attr, string localName)
		{
			if ((attr.ToLower().Contains(rdfsSubClassOf.ToLower())) && (attrVal != null))
			{
				cs.SubClassOf = StringManipulationManager.ExtractAllWithSeparator(attrVal, StringManipulationManager.SeparatorSharp);
			}

		}


		protected virtual void PopulatePropertyAttribute(Property pr, string attrVal, string attr, string localName)
		{
			if ((attr.Equals(rdfsDomain)) && (attrVal != null))
			{
				pr.Domain = StringManipulationManager.ExtractAllWithSeparator(attrVal, StringManipulationManager.SeparatorSharp);
				AddBelongingInformation(pr, pr.Domain);
			}

		}


        #region Methods for processing entities after parsing

        
        /// <summary>
        /// Method for processing unknown entites after parsing
        /// </summary>
        private void ProcessUnknown() 
        {
        
        }


        /// <summary>
        /// Method for processing class category after parsing
        /// </summary>
        private void ProcessClassCategory() 
        {
            ProfileElementTypes type = ProfileElementTypes.ClassCategory;
            List<ClassCategory> list = profile.ProfileMap[type].Cast<ClassCategory>().ToList();
            foreach (ClassCategory element in list)
            {
                //// search for classes of class categories
                if ((belongingMap != null) && (belongingMap.ContainsKey(element.URI)))
                {
                    Stack<ProfileElement> stack = belongingMap[element.URI];
                    ProfileElement classInPackage;
                    while (stack.Count > 0)
                    {
                        classInPackage = stack.Pop();
                        if (ExtractSimpleNameFromResourceURI(classInPackage.Type).Equals("Class"))
                        {
                            Class cl = (Class)classInPackage;
                            element.AddToMembersOfClassCategory(cl);
                            cl.BelongsToCategoryAsObject = element;
                        }
                        else
                        {
                            ClassCategory cl = (ClassCategory)classInPackage;
                            element.AddToMembersOfClassCategory(cl);
                            cl.BelongsToCategoryAsObject = element;
                        }
                    }
                }
            }
        
        }

		/// <summary>
		/// Method for processing class after parsing 
		/// </summary>
		private void ProcessClass()
		{
			ProfileElementTypes type = ProfileElementTypes.Class;
			if (profile != null && profile.ProfileMap != null && profile.ProfileMap.ContainsKey(type))
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

		}

		/// <summary>
		/// Method for processing  property entity after parsing 
		/// </summary>
		private void ProcessProperty()
		{
			ProfileElementTypes type = ProfileElementTypes.Property;
			if (profile != null && profile.ProfileMap != null && profile.ProfileMap.ContainsKey(type))
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
					if (!string.IsNullOrEmpty(element.InverseRoleName))
					{
						element.InverseRoleNameAsObject = profile.FindProfileElementByUri(element.InverseRoleName);
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

		}


        #endregion

        protected void AddComplexTagToCommentsAndLabels(TextType type, ComplexTag ct)
		{
			if (commentsAndLabels == null)
			{
				commentsAndLabels = new Dictionary<TextType, List<ComplexTag>>();
			}

			if (!commentsAndLabels.ContainsKey(type))
			{
				commentsAndLabels[type] = new List<ComplexTag>();
			}

			commentsAndLabels[type].Add(ct);

		}

		/// <summary>
		/// Method for processing parsed  comments and labels in profile element
		/// </summary>
		/// <param name="pe"></param>
		protected void ProccessCommentsAndLabels(ProfileElement pe)
		{
			if (commentsAndLabels != null && pe != null)
			{
				foreach (TextType type in commentsAndLabels.Keys)
				{
					List<ComplexTag> tags = null;
					switch (type)
					{
						case TextType.Comment:
							tags = new List<ComplexTag>(commentsAndLabels[TextType.Comment]);
							pe.Comments = tags;
							break;
						case TextType.Label:
							tags = new List<ComplexTag>(commentsAndLabels[TextType.Label]);
							pe.Labels = tags;
							break;
					}

				}
			}
		}


		//protected PopulateClass()
		//{

		//}


		#endregion






	}
}
