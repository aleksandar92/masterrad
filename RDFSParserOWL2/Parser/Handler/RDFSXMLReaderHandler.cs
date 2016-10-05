using RDFSParserOWL2.Generator.Helper;
using RDFSParserOWL2.Manager;
using RDFSParserOWL2.Model;
using RDFSParserOWL2.Parser.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Parser
{
	class RDFSXMLReaderHandler : RDFXMLHandler
	{
		#region fields

		private const string documentError = "Processing aborted: document doesn't have a CIM-RDFS structure!";

		protected bool documentIdentifiedLikeRDFS = false;


		private const string cimsNamespace = "cims:";
		private const string cimsClassCategoryElement = "cims:ClassCategory";
		
		private const string cimsBelongsToCategory = "cims:belongsToCategory";
		private const string cimsDataType = "cims:dataType";
		private const string cimsInverseRoleName = "cims:inverseRoleName";
		private const string cimsMultiplicity = "cims:multiplicity";
		private const string cimsIsAggregate = "cims:isAggregate"; // text
	

		#endregion

		#region IHandler Members



		override
		public void StartDocument(string filePath)
		{
			base.StartDocument(filePath);
			documentIdentifiedLikeRDFS = false;
			//stereotypes = new List<string>();
		}

		override
		public void StartElement(string localName, string qName, SortedList<string, string> atts)
		{
			if (!abort)
			{

				if (qName.Equals(cimsStereotype))
				{
					string val;
					atts.TryGetValue(rdfResource, out val);
					if (!string.IsNullOrEmpty(val))
						stereotypes.Add(val);

				}
				else
				{
					base.StartElement(localName, qName, atts);
				}

				checkedElementsCount++;
				if (qName.StartsWith(rdfsNamespace, StringComparison.OrdinalIgnoreCase) || (qName.StartsWith(cimsNamespace, StringComparison.OrdinalIgnoreCase)))
				{
					documentIdentifiedLikeRDFS = true;
				}
				if ((!documentIdentifiedLikeRDFS) && (checkedElementsCount >= 70))
				{
					this.profile = null;
					//occurredError = new ExtendedParseError(new Exception(documentError));
					abort = true;
				}
			}
		}

		public override bool IsEndElement(string qName)
		{
			return base.IsEndElement(qName) || qName.Equals(cimsClassCategoryElement);
		}

		override
		public void EndElement(string localName, string qName)
		{
			if (!abort)
			{
				base.EndElement(localName, qName);
				//if (qName.Equals(rdfProfileElement) || qName.Equals(cimsClassCategoryElement)
				//	|| qName.Equals(rdfsClassElement) || qName.Equals(rdfPropertyElement)) //end of element    
				//{
				//	//novo
				//	if (prop != null)
				//	{
				//		string type;
				//		prop.TryGetValue(rdfType, out type);

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
				//				if ((pp.Key.Equals(cimsBelongsToCategory)) && (str != null))
				//				{
				//					cs.BelongsToCategory = str;
				//					AddBelongingInformation(cs, cs.BelongsToCategory);
				//				}
				//				else if ((pp.Key.Equals(cimsMultiplicity)) && (str != null))
				//				{
				//					cs.MultiplicityAsString = ExtractSimpleNameFromResourceURI(str);
				//				}
				//				else if ((pp.Key.Contains(cimsStereotype)) && (str != null))
				//				{
				//					cs.AddStereotype(str);
				//				}
				//				else if ((pp.Key.Contains(rdfsSubClassOf)) && (str != null))
				//				{
				//					cs.SubClassOf = str;
				//				}
				//				else if ((pp.Key.Equals(rdfType)) && (str != null))
				//				{
				//					cs.Type = str;
				//				}
				//				else if ((pp.Key.Equals(rdfProfileElement)) && (str != null))
				//				{
				//					cs.URI = StringManipulationManager.ExtractAllWithSeparator(str, StringManipulationManager.SeparatorSharp);
				//				}
				//			}

				//			foreach (string s in stereotypes)
				//			{
				//				cs.AddStereotype(s);
				//			}
				//			ProccessCommentsAndLabels(cs);
				//			AddProfileElement(ProfileElementTypes.Class, cs);
				//		}
				//		else if (ExtractSimpleNameFromResourceURI(type) == "Property")
				//		{
				//			Property pr = new Property();
				//			foreach (KeyValuePair<string, string> pp in prop)
				//			{
				//				string str = pp.Value;
				//				if ((pp.Key.Equals(cimsDataType)) && (str != null))
				//				{
				//					pr.DataType = str;
				//				}
				//				else if ((pp.Key.Equals(cimsMultiplicity)) && (str != null))
				//				{
				//					pr.MultiplicityAsString = str;
				//				}
				//				else if ((pp.Key.Equals(rdfProfileElement)) && (str != null))
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
				//				else if ((pp.Key.Contains(cimsStereotype)) && (str != null))
				//				{
				//					pr.AddStereotype(str);
				//				}
				//				else if ((pp.Key.Equals(rdfsRange)) && (str != null))
				//				{
				//					pr.Range = StringManipulationManager.ExtractAllWithSeparator(str, StringManipulationManager.SeparatorSharp);
				//				}


				//			}

				//			foreach (string s in stereotypes)
				//			{
				//				pr.AddStereotype(s);
				//			}
				//			ProccessCommentsAndLabels(pr);
				//			AddProfileElement(ProfileElementTypes.Property, pr);
				//		}
				//		else
				//		{
				//			EnumMember en = new EnumMember();
				//			foreach (KeyValuePair<string, string> pp in prop)
				//			{
				//				string str = pp.Value;

				//				if ((pp.Key.Equals(rdfType)) && (str != null))
				//				{
				//					en.Type = str;
				//				}
				//				else if ((pp.Key.Equals(rdfProfileElement)) && (str != null))
				//				{
				//					en.URI = str;
				//				}
				//				else if ((pp.Key.Equals(cimsMultiplicity)) && (str != null))
				//				{
				//					en.MultiplicityAsString = ExtractSimpleNameFromResourceURI(str);
				//				}
				//			}
				//			ProccessCommentsAndLabels(en);
				//			AddProfileElement(ProfileElementTypes.Unknown, en);
				//		}

				//		prop.Clear();
				//		values.Clear();
				//		stereotypes.Clear();
				//		commentsAndLabels.Clear();

				//	}
				//}

				//else if (qName.Equals(rdfsLabel)) //// end of label subelement
				//{
				//	content = content.Trim();
				//	if (!string.IsNullOrEmpty(content))
				//	{
				//		if (commentAndLabelAtts == null)
				//		{
				//			commentAndLabelAtts = new Dictionary<string, string>();
				//		}

				//		AddComplexTagToCommentsAndLabels(TextType.Label, new ComplexTag(content, commentAndLabelAtts));
				//	}
				//	content = string.Empty;
				//	commentAndLabelAtts.Clear();
				//}
				//else if (qName.Equals(rdfsComment)) //// end of comment subelement
				//{
				//	content = content.Trim();
				//	if (!string.IsNullOrEmpty(content))
				//	{
				//		if (commentAndLabelAtts == null)
				//		{
				//			commentAndLabelAtts = new Dictionary<string, string>();
				//		}

				//		AddComplexTagToCommentsAndLabels(TextType.Comment, new ComplexTag(content, commentAndLabelAtts));
				//	}
				//	content = string.Empty;
				//	commentAndLabelAtts.Clear();
				//}
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
				//else 
				//if (qName.Equals(cimsStereotype))
				//{
				//	content = content.Trim();
				//	if (!string.IsNullOrEmpty(content))
				//	{
				//		stereotypes.Add(content);
				//	}
				//}
			}
		}

		protected override void PopulateEnumMemberAttribute(EnumMember em, string attrVal, string attr,string localName)
		{
			if ((attr.Equals(rdfProfileElement)) && (attrVal != null))
			{
				em.URI = attrVal;
			}
			else if ((attr.Equals(cimsMultiplicity)) && (attrVal != null))
			{
				em.MultiplicityAsString = ExtractSimpleNameFromResourceURI(attrVal);
			}
			else
			{
				base.PopulateEnumMemberAttribute(em, attrVal, attr,localName);
			}
		}

		protected override void PopulateClassAttribute(Class cs, string attrVal, string attr,string localName)
		{
			if ((attr.Equals(cimsBelongsToCategory)) && (attrVal != null))
			{
				cs.BelongsToCategory = attrVal;
				AddBelongingInformation(cs, cs.BelongsToCategory);
			}
			else if ((attr.Equals(cimsMultiplicity)) && (attrVal != null))
			{
				cs.MultiplicityAsString = ExtractSimpleNameFromResourceURI(attrVal);
			}
			else if ((attr.Contains(cimsStereotype)) && (attrVal != null))
			{
				cs.AddStereotype(attrVal);
			}
			else if ((attr.Equals(rdfType)) && (attrVal != null))
			{
				cs.Type = attrVal;
			}
			else if ((attr.Equals(rdfProfileElement)) && (attrVal != null))
			{
				cs.URI = StringManipulationManager.ExtractAllWithSeparator(attrVal, StringManipulationManager.SeparatorSharp);
			}
			else
			{
				base.PopulateClassAttribute(cs, attrVal, attr,localName);
			}

		}


		protected override void PopulatePropertyAttribute(Property pr, string attrVal, string attr,string localName)
		{
			if ((attr.Equals(cimsDataType)) && (attrVal != null))
			{
				pr.DataType = attrVal;
			}
			else if ((attr.Equals(cimsMultiplicity)) && (attrVal != null))
			{
				pr.MultiplicityAsString = attrVal;
			}
			else if ((attr.Equals(rdfProfileElement)) && (attrVal != null))
			{
				pr.URI = StringManipulationManager.ExtractAllWithSeparator(attrVal, StringManipulationManager.SeparatorSharp);
			}
			else if ((attr.Equals(rdfType)) && (attrVal != null))
			{
				pr.Type = attrVal;
			}
			else if ((attr.Contains(cimsStereotype)) && (attrVal != null))
			{
				pr.AddStereotype(attrVal);
			}
			else if ((attr.Equals(rdfsRange)) && (attrVal != null))
			{
				pr.Range = StringManipulationManager.ExtractAllWithSeparator(attrVal, StringManipulationManager.SeparatorSharp);
			}
			else
			{

				base.PopulatePropertyAttribute(pr, attrVal, attr,localName);
			}
		}

		override
		public void StartPrefixMapping(string prefix, string uri)
		{
			throw new NotImplementedException();
		}

		//override
		//public void Characters(string text)
		//{
		//	if (!string.IsNullOrEmpty(text))
		//	{
		//		content = text;
		//	}
		//	else
		//	{
		//		content = string.Empty;
		//	}
		//}

		override
		public void EndDocument()
		{
			if (profile != null)
			{
				profile.ProfileMap = allByType;
				ProcessProfile();
			}
		}


		#endregion




	}
}
