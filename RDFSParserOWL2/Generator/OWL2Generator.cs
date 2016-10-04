using RDFSParserOWL2.Common;
using RDFSParserOWL2.Generator.Helper;
using RDFSParserOWL2.Manager;
using RDFSParserOWL2.Model;
using RDFSParserOWL2.Parser;
using RDFSParserOWL2.Parser.Handler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace RDFSParserOWL2.Generator
{
	/// <summary>
	/// Class for generating owl2 document from profile 
	/// </summary>
	public class OWL2Generator
	{

		#region fields

		private const string path = @"../../Resources/OWL2Generated/";

		private string fileName;
		private string shortName;
		private List<Namespace> predefinedNamespaces;
		private string baseAdress;
		private List<string> words;
		private List<Namespace> importNamespaces;
		private string specialStereotype;
		private bool hasSpecialStereotype;
		private bool hasRoofOntology;
		private string roofOntology;
		private Profile profileForGenerating;

		#endregion

		public Profile ProfileForGenerating
		{
			get { ChangeOfProfile(); return profileForGenerating; }
			set { profileForGenerating = value; }
		}


		private void ChangeOfProfile()
		{
			GenerateNameForFile(profileForGenerating.FileName);
			fileName = profileForGenerating.FileName;
		}


		public string FileName
		{
			get { return fileName; }
			set { fileName = value; }
		}

		public List<Namespace> DefaultNamespaces
		{
			get { return predefinedNamespaces; }
			set { predefinedNamespaces = value; }
		}

		public string BaseAddress
		{
			get { return baseAdress; }
			set { baseAdress = value; }
		}

		public List<string> Words
		{
			get { return words; }
			set { words = value; }
		}

		public bool HasRoofOntology
		{
			get { return hasRoofOntology; }
			set { hasRoofOntology = value; }
		}

		public string RoofOntology
		{
			get { return roofOntology; }
			set { roofOntology = value; }
		}


		public bool HasSpecialStereotype
		{
			get { return hasSpecialStereotype; }
			set { hasSpecialStereotype = value; }
		}

		public OWL2Generator(Profile profile)
		{
			words = InputOutput.LoadWordsToSkip();
			predefinedNamespaces = InputOutput.LoadPredefinedNamespaces();
			fileName = profile.FileName;
			GenerateNameForFile(fileName);
			GenerateNamespaces();
			profileForGenerating = profile;
			importNamespaces = InputOutput.LoadImportNamespaces();
			BaseAddress = predefinedNamespaces.Where(x => x.IsToBeDefault == true).Single().Value;
		}

		public OWL2Generator(Profile profile, string specialStereo, bool hasSpecStereotype, string roofOnt, bool hasRoofOnt)
		{
			words = InputOutput.LoadWordsToSkip();
			predefinedNamespaces = InputOutput.LoadPredefinedNamespaces();
			importNamespaces = new List<Namespace>();
			specialStereotype = specialStereo;
			roofOntology = roofOnt;
			hasSpecialStereotype = hasSpecStereotype;
			hasRoofOntology = hasRoofOnt;

			if (profile != null && !profile.IsOwlProfile)
			{
				if (hasSpecialStereotype)
				{
					importNamespaces.Add(new Namespace(predefinedNamespaces.Where(x => x.IsToBeDefault == true).Single().Value + specialStereotype, specialStereotype));
				}

				if (hasRoofOnt)
				{
					importNamespaces.Add(new Namespace(predefinedNamespaces.Where(x => x.IsToBeDefault == true).Single().Value + roofOntology, roofOntology));
				}
			}


			fileName = profile.FileName;
			GenerateNameForFile(fileName);
			GenerateNamespaces();
			profileForGenerating = profile;
			BaseAddress = predefinedNamespaces.Where(x => x.IsToBeDefault == true).Single().Value;

		}







		public void GenerateNameForFile(string fileName)
		{
			shortName = fileName;
			if (words != null)
			{
				int minIndex = Int32.MaxValue;
				foreach (string w in words)
				{
					int index = fileName.IndexOf(w);
					if (index != -1 && index < minIndex)
					{
						minIndex = index;
					}
				}
				if (minIndex != Int32.MaxValue)
				{
					shortName = fileName.Substring(0, minIndex);
				}
			}
		}

		public void GenerateNamespaces()
		{
			List<Namespace> namespacesToGenerate = predefinedNamespaces.Where(x => x.IsToBeGenerated == true).ToList();
			string baseAdr = predefinedNamespaces.Where(x => x.IsToBeDefault == true).Single().Value;
			foreach (Namespace n in namespacesToGenerate)
			{
				n.GenerateNamespace(baseAdr, shortName);
			}
		}

		public void GenerateProfile()
		{
			//profileForGenerating.ProcessEntsoeElements(entsoProfile);
			StringBuilder sb = new StringBuilder();
			sb.Append(path).Append(shortName).Append(".owl");
			XmlTextWriter xtw = new XmlTextWriter(sb.ToString(), null);
			xtw.Formatting = Formatting.Indented;
			xtw.Indentation = 4;
			XmlWriter writer = xtw;
			GenerateStartElement(ref writer);
			GenerateOntologyTag(ref writer);
			foreach (ProfileElementTypes pet in profileForGenerating.ProfileMap.Keys)
			{
				foreach (ProfileElement pe in profileForGenerating.ProfileMap[pet])
				{
					GenerateElement(pet, pe, ref writer);
				}
			}

			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Close();
		}



		public void GenerateOntologyTag(ref XmlWriter writer)
		{
			writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.owlOntology, null);
			writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfAbout, null, baseAdress);
			GenerateImports(ref writer);
			writer.WriteEndElement();
		}

		private void GenerateElement(ProfileElementTypes type, ProfileElement pe, ref XmlWriter writer)
		{
			switch (type)
			{
				case ProfileElementTypes.Class:
					Class cls = pe as Class;
					if (cls != null)
						GenerateClass(cls, ref writer);

					break;

				case ProfileElementTypes.Property:
					Property p = pe as Property;
					GenerateProperty(p, ref writer);
					break;

				case ProfileElementTypes.EnumerationElement:
					EnumMember em = pe as EnumMember;
					GenerateEnumMember(em, ref writer);

					break;
			}

		}

		private void GenerateEnumMember(EnumMember em, ref XmlWriter writer)
		{
			if (em != null && writer != null)
			{
				writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.NamedIndividual, null);
				writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfAbout, null, baseAdress + em.URI);
				if (!profileForGenerating.IsOwlProfile) 
				{
					string nmspace=string.Empty;
					bool found = false;

					Class cls = em.EnumerationObject as Class;

					if(hasSpecialStereotype && cls!=null &&   specialStereotype!=null && specialStereotype!=string.Empty && cls.HasStereotype(specialStereotype)) 
					{
						found = true;
						nmspace = importNamespaces.Where(x => x.Prefix.Equals(specialStereotype)).Single().Value + em.URI;
					}
					else if (hasRoofOntology && roofOntology != null && roofOntology != string.Empty)
					{ 
						found=true;
						nmspace=importNamespaces.Where(x => x.Prefix.Equals(roofOntology)).Single().Value + em.URI;

					}
				
					if(found) 
					{
						writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.SameAs, null);
						writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null,nmspace);
						writer.WriteEndElement();
					}

					GenerateProfileElement(em, ref writer);
				} 

				
				writer.WriteEndElement();
			}
		}

		private void GenerateClass(Class cls, ref XmlWriter writer)
		{
			if (cls != null && writer != null)
			{
				if (!profileForGenerating.IsOwlProfile)
				{
					string ontologyNamespace = string.Empty;
					bool found = false;
					if (hasSpecialStereotype && cls.HasStereotype(specialStereotype))
					{
						ontologyNamespace = importNamespaces.Where(x => x.Prefix.Equals(specialStereotype)).Single().Value;
						found = true;
					}
					else if (hasRoofOntology && roofOntology != null && !roofOntology.Equals(string.Empty))
					{
						ontologyNamespace = importNamespaces.Where(x => x.Prefix.Equals(roofOntology)).Single().Value;
						found = true;
					}
					if (found)
						GenerateEquivalentClass(ref writer, ontologyNamespace, cls);
				}

				///starting owl class element
				writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.Class, null);
				writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfAbout, null, baseAdress + cls.URI);

				if (cls.SubClassOf != null && cls.SubClassOf != string.Empty && cls.SubClassOfAsObject != null)
				{
					///subclass owl element
					writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsSubclasOf, null);
					writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, baseAdress + cls.SubClassOfAsObject.URI);
					writer.WriteEndElement();
				}

				if (!profileForGenerating.IsOwlProfile)
				{
					if (cls.MyProperties != null)
					{
						foreach (ProfileElement pe in cls.MyProperties)
						{
							///restriction for property
							GeneratePropertyForClass(pe as Property, ref writer);
						}
					}


					GenerateProfileElement(cls, ref writer);
				
				}
				GenerateEnumeration(cls, ref writer);
				writer.WriteEndElement();
			}
		}

		private void GenerateEnumeration(Class cls, ref XmlWriter writer)
		{
			if (cls != null && cls.IsEnumeration && writer != null)
			{
				writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.EqClass, null);
				writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.Class, null);
				writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.OneOf, null);
				writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.ParseType, null, OWL2Namespace.Collection);
				GenerateEnumMembers(cls.MyEnumerationMembers, ref writer);
				writer.WriteEndElement();
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
		}


		private void GenerateEnumMembers(List<ProfileElement> members, ref XmlWriter writer)
		{
			if (members != null && writer != null)
			{
				List<EnumMember> enumMembers = members.Cast<EnumMember>().ToList();
				foreach (EnumMember em in enumMembers)
				{
					GenerateDescription(ref writer, BaseAddress + em.URI);
				}
			}
		}


		private void GenerateDescription(ref XmlWriter writer, string value)
		{
			writer.WriteStartElement(OWL2Namespace.rdfPrefix, OWL2Namespace.Description, null);
			writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfAbout, null, value);
			writer.WriteEndElement();
		}

		private void GeneratePropertyForClass(Property property, ref XmlWriter writer)
		{
			if (property != null && property.MultiplicityAsString != null)
			{
				///start of subclass element 
				writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsSubclasOf, null);
				///start of restriction owl element  
				writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.owlRestriction, null);
				writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.OnProperty, null);
				writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, baseAdress + property.URI);
				writer.WriteEndElement();

				GenerateCardinality(property, ref writer);

				writer.WriteEndElement();
				writer.WriteEndElement();
			}

		}

		private void GenerateCardinality(Property property, ref XmlWriter writer)
		{
			if (property != null)
			{

				CardinaltyType ct = property.ProcessMultiplicity();

				if (ct == CardinaltyType.ZEROTOMANY)
				{
					string value = property.RangeAsObject != null ? baseAdress + property.RangeAsObject.URI : baseAdress + property.Range;
					writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.AllValuesFrom, null);
					writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, value);
					writer.WriteEndElement();
				}
				else
				{
					string qCardinality = string.Empty;
					string xsd = DefaultNamespaces.Where(x => x.Prefix != null && x.Prefix.Equals("xsd")).Single().Value;
					string value = String.Format("{0}{1}", xsd, OWL2Namespace.nonNegativeInteger);

					if (ct == CardinaltyType.ZEROTOONE)
					{
						qCardinality = OWL2Namespace.MaxQualified;
					}
					else if (ct == CardinaltyType.ONETOMANY)
					{
						qCardinality = OWL2Namespace.MinQualified;
					}
					else if (ct == CardinaltyType.ONE)
					{
						qCardinality = OWL2Namespace.Qualified;
					}

					writer.WriteStartElement(OWL2Namespace.owlPrefix, qCardinality, null);
					writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfDatatype, null, value);
					writer.WriteValue(1);
					writer.WriteEndElement();

					if (property.IsObjectProperty())
					{
						string rangeUri = property.RangeAsObject == null ? property.Range : property.RangeAsObject.URI;
						writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.OnClass, null);
						writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, baseAdress + rangeUri);
						writer.WriteEndElement();
					}
					else
					{
						writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.OnDataRange, null);
						writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, xsd + property.ProcessDatatype());
						writer.WriteEndElement();
					}

				}

			}
		}

		private void GenerateProperty(Property property, ref XmlWriter writer)
		{
			if (property != null && writer != null)
			{

				string propertyType;
				string rangeValue;
				string domainUri;

				if (!profileForGenerating.IsOwlProfile)
				{
					string ontologyNamespace = string.Empty;
					bool found = false;
					if (hasSpecialStereotype && (property.HasStereotype(specialStereotype) || (property.DomainAsObject != null && property.DomainAsObject.HasStereotype(specialStereotype))))
					{
						ontologyNamespace = importNamespaces.Where(x => x.Prefix.Equals(specialStereotype)).Single().Value;
						found = true;
					}
					else if (hasRoofOntology && roofOntology != null && !roofOntology.Equals(string.Empty))
					{
						ontologyNamespace = importNamespaces.Where(x => x.Prefix.Equals(roofOntology)).Single().Value;
						found = true;
					}
					if (found)
						GenerateEquivalentProperty(ref writer, ontologyNamespace, property);
				}

				//if (!profileForGenerating.IsOwlProfile && (hasSpecialStereotype && (property.HasStereotype(specialStereotype) || (property.DomainAsObject != null && property.DomainAsObject.HasStereotype(specialStereotype)))))
				//{
				//	string ontologyNamespace = importNamespaces.Where(x => x.Prefix.Equals(specialStereotype)).Single().Value;
				//	GenerateEquivalentProperty(ref writer, ontologyNamespace, property);
				//}

				if (property.IsObjectProperty())
				{
					propertyType = OWL2Namespace.ObjectProperty;
					rangeValue = property.RangeAsObject != null ? String.Format("{0}{1}", BaseAddress, property.RangeAsObject.URI) : String.Format("{0}{1}", BaseAddress, property.Range);
				}
				else
				{
					propertyType = OWL2Namespace.DatatypeProperty;
					string xsd = DefaultNamespaces.Where(x => x.Prefix != null && x.Prefix.Equals("xsd")).Single().Value;
					rangeValue = String.Format("{0}{1}", xsd, property.ProcessDatatype());
				}

				domainUri = property.DomainAsObject == null ? baseAdress + property.Domain : baseAdress + property.DomainAsObject.URI;

				writer.WriteStartElement(OWL2Namespace.owlPrefix, propertyType, null);
				writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfAbout, null, baseAdress + property.URI);

				if (!profileForGenerating.IsOwlProfile)
				{
					writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsDomain, null);
					writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, domainUri);
					writer.WriteEndElement();
				}

				writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsRange, null);
				writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, rangeValue);
				writer.WriteEndElement();


				if (!profileForGenerating.IsOwlProfile)
					GenerateProfileElement(property, ref writer);

				writer.WriteEndElement();
			}
		}

		private void GenerateProfileElement(ProfileElement pe, ref XmlWriter writer)
		{
			if (pe.Comments != null)
			{
				foreach (ComplexTag ct in pe.Comments)
				{
					GenerateCommentElement(ref writer, ct);
				}
			}

			if (pe.Labels != null)
			{
				foreach (ComplexTag ct in pe.Labels)
				{
					GenerateLabelElement(ref writer, ct);
				}
			}

			writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.IsDefinedBy, null);
			writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, BaseAddress);
			writer.WriteEndElement();
		}

		private void GenerateCommentElement(ref XmlWriter writer, ComplexTag ct)
		{
			if (ct != null && ct.Value != string.Empty && writer != null)
			{
				writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsComment, null);
				GenerateAttsLabelComment(ref writer, ct.Attributes);
				writer.WriteValue(ct.Value);
				writer.WriteEndElement();
			}
		}

		private void GenerateLabelElement(ref XmlWriter writer, ComplexTag ct)
		{
			if (ct != null && ct.Value != string.Empty && writer != null)
			{
				writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsLabel, null);
				GenerateAttsLabelComment(ref writer, ct.Attributes);
				writer.WriteValue(ct.Value);
				writer.WriteEndElement();
			}
		}

		private void GenerateAttsLabelComment(ref XmlWriter writer, Dictionary<string, string> atts)
		{
			if (atts != null && atts.ContainsKey(OWL2Namespace.xmlLang))
			{
				writer.WriteAttributeString(OWL2Namespace.xmlPrefix, OWL2Namespace.Lang, null, atts[OWL2Namespace.xmlLang]);
			}
		}


		public void GenerateStartElement(ref XmlWriter writer)
		{
			writer.WriteStartDocument();
			string rdfVal = DefaultNamespaces.Where(x => x.Prefix != null && x.Prefix.Equals("rdf")).Single().Value;
			writer.WriteStartElement(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfRDF, rdfVal);
			foreach (Namespace n in DefaultNamespaces)
			{
				writer.WriteAttributeString(n.Ns, n.Prefix, null, n.Value);
			}
		}

		public void GenerateImports(ref XmlWriter writer)
		{
			if (importNamespaces != null)
			{

				foreach (Namespace n in importNamespaces)
				{
					writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.Import, null);
					writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, n.Value);
					writer.WriteEndElement();

				}
			}
		}

		private void GenerateEquivalentProperty(ref XmlWriter writer, string baseAddressImport, Property p)
		{
			if (p != null && writer != null)
			{
				writer.WriteStartElement(OWL2Namespace.rdfPrefix, OWL2Namespace.Description, null);
				writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfAbout, null, baseAddressImport + StringManipulationManager.ExtractAllWithSeparator(p.URI, StringManipulationManager.SeparatorSharp));
				writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.EqProperty, null);
				writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, baseAdress + StringManipulationManager.ExtractAllWithSeparator(p.URI, StringManipulationManager.SeparatorSharp));
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
		}

		private void GenerateEquivalentClass(ref XmlWriter writer, string baseAddressImport, Class c)
		{
			if (c != null && writer != null)
			{
				writer.WriteStartElement(OWL2Namespace.rdfPrefix, OWL2Namespace.Description, null);
				writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfAbout, null, baseAddressImport + StringManipulationManager.ExtractAllWithSeparator(c.URI, StringManipulationManager.SeparatorSharp));
				writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.EqClass, null);
				writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, baseAdress + StringManipulationManager.ExtractAllWithSeparator(c.URI, StringManipulationManager.SeparatorSharp));
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
		}

	}
}
