using RDFSParserOWL2.Generator.Helper;
using RDFSParserOWL2.Manager;
using RDFSParserOWL2.Model;
using RDFSParserOWL2.Reporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Parser.Handler
{
	class OWLRDFXMLHandler : RDFXMLHandler
	{ 

		/// <summary>
		/// Kolekcija za restrikcije u profilu 
		/// </summary>
		private Dictionary<string, string> res;

		private List<string> enumMembers;

		override
		public void StartDocument(string filePath)
		{
			base.StartDocument(filePath);
			res = new Dictionary<string, string>();
			enumMembers = new List<string>();
			Reporter = new RDFXMLParserReporter();
			//throw new NotImplementedException();
		}

		override
		public void StartElement(string localName, string qName, SortedList<string, string> atts)
		{
			if (!abort)
			{
				if (localName.Equals(OWL2Namespace.OneOf))
				{
					enumMembers.Clear();
				}
				else if (qName.Equals(rdfProfileElement))
				{
					string id;
					atts.TryGetValue(rdfAbout, out id);
					if (!string.IsNullOrEmpty(id))
						enumMembers.Add(id);
				}
				else
				{

					base.StartElement(localName, qName, atts);
				}
				//checkedElementsCount++;
				//if (qName.StartsWith(rdfsNamespace, StringComparison.OrdinalIgnoreCase) || (qName.StartsWith(cimsNamespace, StringComparison.OrdinalIgnoreCase)))
				//{
				//    documentIdentifiedLikeRDFS = true;
				//}
				//if ((!documentIdentifiedLikeRDFS) && (checkedElementsCount >= 70))
				//{
				//    this.profile = null;
				//    //occurredError = new ExtendedParseError(new Exception(documentError));
				//    abort = true;
				//}
			}

			//throw new NotImplementedException();
		}


        public override bool IsStereotype(string qName)
        {
            return qName.Equals(MetaNamespace.MetaSteroe);
        }

		public override bool IsEndElement(string qName)
		{
			return OWL2Namespace.IsElementOWL(qName);
		}

		protected override bool IsProperty(string type)
		{
			return ExtractSimpleNameFromResourceURI(type).Equals(OWL2Namespace.DatatypeProperty) || ExtractSimpleNameFromResourceURI(type).Equals(OWL2Namespace.ObjectProperty);
		}

		protected override void GetType(out string type, string localName)
		{
            prop.TryGetValue(rdfType,out type);
            if (string.IsNullOrEmpty(type)) 
            {
                type = localName;
            }
            //if (!prop.ContainsKey(rdfType)) 
			
		}


		override
		public void EndElement(string localName, string qName)
		{
			if (!abort)
			{
				base.EndElement(localName, qName);
				//if (OWL2Namespace.IsElementOWL(qName)) //end of element    
				//{
				//	//novo
				//	if (prop != null)
				//	{
				//		string type = localName;

				//		//prop.TryGetValue(rdfType, out type);

				//		if (ExtractSimpleNameFromResourceURI(type) == "ClassCategory")
				//		{
				//			//ClassCategory cs = new ClassCategory();
				//			//foreach (KeyValuePair<string, string> pp in prop)
				//			//{
				//			//	string str = pp.Value;
				//			//	if ((pp.Key.Equals(cimsBelongsToCategory)) && (str != null))
				//			//	{
				//			//		cs.BelongsToCategory = str;
				//			//		AddBelongingInformation(cs, cs.BelongsToCategory);
				//			//	}
				//			//	else if ((pp.Key.Equals(rdfsComment)) && (str != null))
				//			//	{
				//			//		cs.Comment = str;
				//			//	}
				//			//	else if ((pp.Key.Equals(rdfsLabel)) && (str != null))
				//			//	{
				//			//		cs.Label = str;
				//			//	}
				//			//	else if ((pp.Key.Equals(rdfType)) && (str != null))
				//			//	{
				//			//		cs.Type = str;
				//			//	}
				//			//	else if ((pp.Key.Equals(rdfProfileElement)) && (str != null))
				//			//	{
				//			//		cs.URI = str;
				//			//	}
				//			//	else if ((pp.Key.Equals(cimsMultiplicity)) && (str != null))
				//			//	{
				//			//		cs.MultiplicityAsString = ExtractSimpleNameFromResourceURI(str);
				//			//	}
				//			//}
				//			//AddProfileElement(ProfileElementTypes.ClassCategory, cs);
				//		}
				//		else if (ExtractSimpleNameFromResourceURI(type) == "Class")
				//		{
				//			Class cs = new Class();
				//			foreach (KeyValuePair<string, string> pp in prop)
				//			{
				//				string str = pp.Value;
				//				//if ((pp.Key.Equals(cimsBelongsToCategory)) && (str != null))
				//				//{
				//				//    cs.BelongsToCategory = str;
				//				//    AddBelongingInformation(cs, cs.BelongsToCategory);
				//				//}
				//				//else 
				//				//if ((pp.Key.Equals(rdfsComment)) && (str != null))
				//				//{

				//				//	cs.Comment.Attributes[rdfParseType] = str;
				//				//	cs.Comment.Value = values[rdfsComment];
				//				//}
				//				//else if ((pp.Key.Equals(rdfsLabel)) && (str != null))
				//				//{
				//				//	cs.Label.Attributes[xmlLang] = str;
				//				//	cs.Label.Value = values[rdfsLabel];
				//				//	//cs.Label = str;
				//				//}
				//				//else if ((pp.Key.Equals(cimsMultiplicity)) && (str != null))
				//				//{
				//				//	cs.MultiplicityAsString = ExtractSimpleNameFromResourceURI(str);
				//				//}
				//				//else if ((pp.Key.Contains(cimsStereotype)) && (str != null))
				//				//{
				//				//	cs.AddStereotype(str);
				//				//}
				//				// else 
				//				if ((pp.Key.Contains(rdfsSubClassOf)) && (str != null))
				//				{
				//					cs.SubClassOf = StringManipulationManager.ExtractAllWithSeparator(str, StringManipulationManager.SeparatorSharp);
				//				}
				//				else if ((pp.Key.Equals(rdfType)) && (str != null))
				//				{
				//					cs.Type = str;
				//				}
				//				else if ((pp.Key.Equals(OWL2Namespace.owlClass)) && (str != null))
				//				{
				//					cs.URI = StringManipulationManager.ExtractAllWithSeparator(str, StringManipulationManager.SeparatorSharp);
				//				}
				//			}
				//			ProccessCommentsAndLabels(cs);
				//			AddProfileElement(ProfileElementTypes.Class, cs);
				//		}
				//		else if (ExtractSimpleNameFromResourceURI(type).Equals(OWL2Namespace.DatatypeProperty) || ExtractSimpleNameFromResourceURI(type).Equals(OWL2Namespace.ObjectProperty))
				//		{
				//			Property pr = new Property();
				//			foreach (KeyValuePair<string, string> pp in prop)
				//			{
				//				string str = pp.Value;
				//				//if ((pp.Key.Equals(cimsDataType)) && (str != null))
				//				//{
				//				//    pr.DataType = str;
				//				//}
				//				//else if ((pp.Key.Equals(cimsMultiplicity)) && (str != null))
				//				//{
				//				//    pr.MultiplicityAsString = str;
				//				//}
				//				//else 
				//				if ((pp.Key.Equals(OWL2Namespace.owlDatatypeProperty) || pp.Key.Equals(OWL2Namespace.owlObjectProperty)) && (str != null))
				//				{
				//					pr.URI = StringManipulationManager.ExtractAllWithSeparator(str, StringManipulationManager.SeparatorSharp);
				//				}
				//				else if ((pp.Key.Equals(rdfType)) && (str != null))
				//				{
				//					pr.Type = str;
				//				}
				//				else if ((pp.Key.Equals(rdfsDomain)) && (str != null))
				//				{
				//					pr.Domain = StringManipulationManager.ExtractAllWithSeparator(str, StringManipulationManager.SeparatorSharp);
				//					AddBelongingInformation(pr, pr.Domain);
				//				}
				//				else if ((pp.Key.Equals(rdfsRange)) && (str != null))
				//				{
				//					if (localName.Equals(OWL2Namespace.DatatypeProperty))
				//						pr.DataType = StringManipulationManager.ExtractAllWithSeparator(str, StringManipulationManager.SeparatorSharp);
				//					else
				//						pr.Range = StringManipulationManager.ExtractAllWithSeparator(str, StringManipulationManager.SeparatorSharp);
				//				}
				//			}
				//			AddProfileElement(ProfileElementTypes.Property, pr);
				//		}
				//		else
				//		{
				//			//EnumMember en = new EnumMember();
				//			//foreach (KeyValuePair<string, string> pp in prop)
				//			//{
				//			//	string str = pp.Value;
				//			//	if ((pp.Key.Equals(rdfsComment)) && (str != null))
				//			//	{
				//			//		en.Comment = str;
				//			//	}
				//			//	else if ((pp.Key.Equals(rdfsLabel)) && (str != null))
				//			//	{
				//			//		en.Label = str;
				//			//	}
				//			//	else if ((pp.Key.Equals(rdfType)) && (str != null))
				//			//	{
				//			//		en.Type = str;
				//			//	}
				//			//	else if ((pp.Key.Equals(rdfProfileElement)) && (str != null))
				//			//	{
				//			//		en.URI = str;
				//			//	}
				//			//	else if ((pp.Key.Equals(cimsMultiplicity)) && (str != null))
				//			//	{
				//			//		en.MultiplicityAsString = ExtractSimpleNameFromResourceURI(str);
				//			//	}
				//			//}
				//			//AddProfileElement(ProfileElementTypes.Unknown, en);
				//		}

				//		commentsAndLabels.Clear();
				//		prop.Clear();
				//		values.Clear();


				//	}
				//}

				//else if (qName.Equals(rdfsLabel)) //// end of label subelement
				//{
				//	content = content.Trim();
				//	if (!string.IsNullOrEmpty(content))
				//	{
				//		//novo
				//		string ls;
				//		prop.TryGetValue(qName, out ls);
				//		if (ls == null)
				//		{
				//			ls = (string)content.Clone();
				//			prop.Add(qName, ls);

				//		}
				//		values.Add(qName, content);
				//		content = string.Empty;
				//	}
				//}
				//else

				if (localName.Equals(OWL2Namespace.owlRestriction))
				{
					if (prop != null)
					{
						string onProperty, cardinality;
						prop.TryGetValue(OWL2Namespace.owlOnProperty, out onProperty);
						cardinality = GetCardinality();
						if (onProperty != null && cardinality != null)
						{
							if (prop.ContainsKey(OWL2Namespace.owlOnProperty))
							{
								prop.Remove(OWL2Namespace.owlOnProperty);
							}
                             
							if (cardinality.Equals(OWL2Namespace.owlMaxQualified) || cardinality.Equals(OWL2Namespace.owlMinQualified) || cardinality.Equals(OWL2Namespace.owlQualified))
							{
								if (prop.ContainsKey(OWL2Namespace.owlOnDataRange))
									prop.Remove(OWL2Namespace.owlOnDataRange);
								if (prop.ContainsKey(OWL2Namespace.owlOnClass))
									prop.Remove(OWL2Namespace.owlOnClass);
							}
							AddRestriction(StringManipulationManager.ExtractAllWithSeparator(onProperty, StringManipulationManager.SeparatorSharp), cardinality);

						}
					}

				}
				//else if (qName.Equals(rdfsComment)) //// end of comment subelement
				//{
				//	content = content.Trim();
				//	if (!string.IsNullOrEmpty(content))
				//	{
				//		//novo
				//		string ls;
				//		prop.TryGetValue(qName, out ls);
				//		if (ls == null)
				//		{
				//			ls = (string)content.Clone();
				//			prop.Add(qName, ls);

				//		}
				//		values.Add(qName, content);
				//		content = string.Empty;
				//	}
				//}
				//else if (qName.Equals(cimsIsAggregate)) //// end of isAggregate subelement
				//{
				//    content = content.Trim();
				//    if (!string.IsNullOrEmpty(content))
				//    {
				//        bool paresedValue;

				//        //novo
				//        string ls;
				//        prop.TryGetValue(qName, out ls);
				//        if (ls == null)
				//        {
				//            if (bool.TryParse((string)content.Clone(), out paresedValue))
				//            {
				//                ls = paresedValue.ToString();
				//            }
				//            prop.Add(qName, ls);
				//        }
				//        content = string.Empty;
				//    }
				//}
			}
		}

		override
		public void StartPrefixMapping(string prefix, string uri)
		{
			//throw new NotImplementedException();
		}


		/// <summary>
		/// Method for populating class 
		/// </summary>
		/// <param name="localName"></param>
		protected override void PopulateClass(string localName)
		{

			///Checking if class is for populating
			if (prop.Keys.Contains(OWL2Namespace.owlClass))
			{
				base.PopulateClass(localName);
			}
		}

		protected override void OperationsForPopulatingClass(Class cs, string localName)
		{
			base.OperationsForPopulatingClass(cs, localName);
			AddEnumMembersToClass(cs);
		}

		private void AddEnumMembersToClass(Class cs)
		{
			if (cs != null)
			{
				foreach (string uri in enumMembers)
				{
					ProfileElement pe = new EnumMember() { URI = StringManipulationManager.ExtractAllWithSeparator(uri, StringManipulationManager.SeparatorSharp) };
					cs.AddToMyEnumerationMembers(pe);
				}
			}
			enumMembers.Clear();

		}

		protected override void PopulateEnumMemberAttribute(EnumMember em, string attrVal, string attr, string localName)
		{
			if ((attr.Equals(OWL2Namespace.owlNamedIndividual)) && (attrVal != null))
			{
				PopulateUri(em,attrVal);
			}
			else
			{
				base.PopulateEnumMemberAttribute(em, attrVal, attr, localName);
			}
		}

		protected override void PopulateClassAttribute(Class cs, string attrVal, string attr, string localName)
		{
            if ((attr.Equals(MetaNamespace.MetaFixed)) && (attrVal != null))
            {
                cs.IsFixed = attrVal;
            }
            else
                if ((attr.Equals(MetaNamespace.MetaBelongs)) && (attrVal != null))
                {
                    cs.BelongsToCategory = attrVal;
                    AddBelongingInformation(cs, cs.BelongsToCategory);
                }
            else
			if ((attr.Equals(rdfType)) && (attrVal != null))
			{
				cs.Type = attrVal;
			}
			else if ((attr.Equals(OWL2Namespace.owlClass)) && (attrVal != null))
			{
				PopulateUri(cs, attrVal);
			}
			else
			{
				base.PopulateClassAttribute(cs, attrVal, attr, localName);
			}
		}


        protected override void PopulateClassCategoryAttribute(ClassCategory csCat, string attrVal, string attr, string localName)
        {
            if ((attr.Equals(OWL2Namespace.owlNamedIndividual)) && (attrVal != null))
            {
				PopulateUri(csCat, attrVal);
            }
                else
                {

                    base.PopulateClassCategoryAttribute(csCat, attrVal, attr, localName);
                }
        }

		protected override void PopulatePropertyAttribute(Property pr, string attrVal, string attr, string localName)
		{
            if ((attr.Equals(MetaNamespace.MetaFixed)) && (attrVal != null))
            {
                pr.IsFixed = attrVal;
            }else
			if ((attr.Equals(OWL2Namespace.owlDatatypeProperty) || attr.Equals(OWL2Namespace.owlObjectProperty)) && (attrVal != null))
			{
				PopulateUri(pr, attrVal);

			}
			else if ((attr.Equals(rdfType)) && (attrVal != null))
			{
				pr.Type = attrVal;
			}
			else if ((attr.Equals(OWL2Namespace.owlInverseOf)) && !string.IsNullOrEmpty(attrVal))
			{
				pr.InverseRoleName = StringManipulationManager.ExtractAllWithSeparator(attrVal, StringManipulationManager.SeparatorSharp);
			}
			else if ((attr.Equals(rdfsRange)) && (attrVal != null))
			{
				if (localName.Equals(OWL2Namespace.DatatypeProperty))
					pr.DataType = StringManipulationManager.ExtractAllWithSeparator(attrVal, StringManipulationManager.SeparatorSharp);
				else
					pr.Range = StringManipulationManager.ExtractAllWithSeparator(attrVal, StringManipulationManager.SeparatorSharp);
			}
			else
			{
				base.PopulatePropertyAttribute(pr, attrVal, attr, localName);
			}
		}

		override
		public void Characters(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				content = text;
			}
			else
			{
				content = string.Empty;
			}
			//throw new NotImplementedException();
		}

		override
		public void EndDocument()
		{
			if (profile != null)
			{
				profile.ProfileMap = allByType;
				ProcessEnumerations();
				/// funkcionalnost za povezivanje kardinaliteta
				ConvertCardinality();
				res.Clear();
				ProcessProfile();
			}
		}

		private void ProcessEnumerations()
		{
			if (profile.ProfileMap != null && profile.ProfileMap.ContainsKey(ProfileElementTypes.Class) && profile.ProfileMap.ContainsKey(ProfileElementTypes.Unknown))
			{
				List<ProfileElement> elements = profile.ProfileMap[ProfileElementTypes.Class];
				List<ProfileElement> unknown = profile.ProfileMap[ProfileElementTypes.Unknown];
				if (elements != null && unknown != null)
				{
					List<Class> classes = elements.Cast<Class>().ToList();
					foreach (Class cs in classes)
					{
						if (cs.MyEnumerationMembers != null)
						{
							foreach (EnumMember em in cs.MyEnumerationMembers)
							{
								ProfileElement pe = unknown.Where(x => x.URI.Equals(em.URI)).SingleOrDefault();
								if (pe != null)
									pe.Type = cs.URI;
							}
						}
					}
				}

			}
		}

		private void AddRestriction(string key, string value)
		{
			res.Add(key, value);
		}

		private string GetCardinality()
		{
			string result = string.Empty, allValuesFrom = string.Empty, maxQualified = string.Empty, minQualified = string.Empty, qualified = string.Empty;


			if (prop != null)
			{

				prop.TryGetValue(OWL2Namespace.owlAllValuesFrom, out allValuesFrom);
				prop.TryGetValue(OWL2Namespace.owlMaxQualified, out maxQualified);
				prop.TryGetValue(OWL2Namespace.owlMinQualified, out minQualified);
				prop.TryGetValue(OWL2Namespace.owlQualified, out qualified);

				if (allValuesFrom != null && allValuesFrom != string.Empty)
					result = OWL2Namespace.AllValuesFrom;
				else if (maxQualified != null && maxQualified != string.Empty)
					result = OWL2Namespace.MaxQualified;
				else if (minQualified != null && minQualified != string.Empty)
					result = OWL2Namespace.MinQualified;
				else if (qualified != null && qualified != string.Empty)
					result = OWL2Namespace.Qualified;

				if (result != null && result != string.Empty && prop.ContainsKey(OWL2Namespace.owlPrefix + StringManipulationManager.SeparatorColon + result))
					prop.Remove(OWL2Namespace.owlPrefix + StringManipulationManager.SeparatorColon + result);

			}
			return result;
		}


		private void ConvertCardinality()
		{

			if (res != null && allByType != null && allByType.Keys.Contains(ProfileElementTypes.Property))
			{
				List<Property> properties = new List<Property>(allByType[ProfileElementTypes.Property].Cast<Property>().ToList());

				if(properties!=null && properties.Count>0)
				foreach (string ped in res.Keys)
				{
					Property p = properties.Where(x => x.URI.Equals(ped)).SingleOrDefault();
					if (p != null)
						p.MultiplicityAsString = Property.ProcessOwlMultiplicityToString(res[ped]);
				}

			}
		}


	}
}
