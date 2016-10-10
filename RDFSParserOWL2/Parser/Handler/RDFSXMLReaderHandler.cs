﻿using RDFSParserOWL2.Generator.Helper;
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

				//if (qName.Equals(cimsStereotype))
				//{
				//	string val;
				//	atts.TryGetValue(rdfResource, out val);
				//	if (!string.IsNullOrEmpty(val))
				//		stereotypes.Add(val);

				//}
				//else
				//{
					base.StartElement(localName, qName, atts);
				//}

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
			if ((attr.ToLower().Equals(cimsBelongsToCategory.ToLower())) && (attrVal != null))
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
			else if ((attr.ToLower().Equals(rdfProfileElement.ToLower())) && (attrVal != null))
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
			if ((attr.ToLower().Equals(cimsDataType.ToLower())) && (attrVal != null))
			{
				pr.DataType = attrVal;
			}
			else if ((attr.Equals(cimsMultiplicity)) && (attrVal != null))
			{
				pr.MultiplicityAsString = attrVal;
			}
			else if ((attr.ToLower().Equals(rdfProfileElement.ToLower())) && (attrVal != null))
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
		    else if((attr.Equals(cimsInverseRoleName)) && !string.IsNullOrEmpty(attrVal)) 
			{
				pr.InverseRoleName = StringManipulationManager.ExtractAllWithSeparator(attrVal, StringManipulationManager.SeparatorSharp);
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
