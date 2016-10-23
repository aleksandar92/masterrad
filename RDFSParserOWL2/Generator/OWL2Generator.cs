using RDFSParserOWL2.Common;
using RDFSParserOWL2.Generator.Helper;
using RDFSParserOWL2.Manager;
using RDFSParserOWL2.Model;
using RDFSParserOWL2.Model.Settings;
using RDFSParserOWL2.Parser;
using RDFSParserOWL2.Parser.Handler;
using RDFSParserOWL2.Reporter;
using RDFSParserOWL2.Reporter.Interfaces;
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
		private GeneratorSettings settings;
		private Profile profileForGenerating;
		private HashSet<string> stereotypesToSkip;
		private IGeneratorReporter reporter;

		#endregion

		public Profile ProfileForGenerating
		{
			get { return profileForGenerating; }
			//set { profileForGenerating = value; }
		}



		public string ShortName
		{
			get { return shortName; }
			set { shortName = value; }
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


		public HashSet<string> StereotypesToSkip
		{
			get { return stereotypesToSkip; }
			set { stereotypesToSkip = value; }
		}


		public IGeneratorReporter Reporter
		{
			get { return reporter; }
			set { reporter = value; }
		}


		//public OWL2Generator(Profile profile)
		//{
		//	words = InputOutput.LoadWordsToSkip();
		//	predefinedNamespaces = InputOutput.LoadPredefinedNamespaces();
		//	fileName = profile.FileName;
		//	GenerateNameForFile(fileName);
		//	GenerateNamespaces();
		//	profileForGenerating = profile;
		//	importNamespaces = InputOutput.LoadImportNamespaces();
		//	BaseAddress = predefinedNamespaces.Where(x => x.IsToBeDefault == true).Single().Value;
		//	stereotypesToSkip = new HashSet<string>(InputOutput.LoadFixedStereotypes());
		//}

		public OWL2Generator(Profile profile, GeneratorSettings ge)
		{
			words = InputOutput.LoadWordsToSkip();
			predefinedNamespaces = InputOutput.LoadPredefinedNamespaces();
			importNamespaces = new List<Namespace>();
			settings = ge;
			profileForGenerating = profile;
			GenerateDefaultNamespace(predefinedNamespaces, ge, profile.IsOwlProfile);
			AddToImportNamespaces();
			fileName = profile.FileName;
			GenerateNameForFile(fileName);
			GenerateNamespaces();
			reporter = new OWL2GeneratorReporter();
			BaseAddress = predefinedNamespaces.Where(x => x.IsToBeDefault == true).Single().Value;
			stereotypesToSkip = new HashSet<string>(InputOutput.LoadFixedStereotypes());
		}



		private void AddToImportNamespaces()
		{
			if (profileForGenerating != null && !profileForGenerating.IsOwlProfile)
			{
				if (settings.IsSpecialOntology)
				{
					if (!string.IsNullOrEmpty(settings.ExtractionOntologyNS))
					{
						importNamespaces.Add(new Namespace(String.Format("{0}{1}", settings.ExtractionOntologyNS, settings.NameOfOntology), settings.NameOfOntology));
					}
					else
					{
						importNamespaces.Add(new Namespace(predefinedNamespaces.Where(x => x.IsToBeDefault == true).Single().Value + settings.NameOfOntology, settings.NameOfOntology));
					}
				}

				if (settings.IsRoofOntology)
				{
					if (!string.IsNullOrEmpty(settings.RoofOntologyNS.Trim()))
					{
						importNamespaces.Add(new Namespace(String.Format("{0}{1}", settings.RoofOntologyNS, settings.RoofOntology), settings.RoofOntology));
					}
					else
					{
						importNamespaces.Add(new Namespace(predefinedNamespaces.Where(x => x.IsToBeDefault == true).Single().Value + settings.RoofOntology, settings.RoofOntology));
					}
					//	importNamespaces.Add(new Namespace(predefinedNamespaces.Where(x => x.IsToBeDefault == true).Single().Value + settings.RoofOntology, settings.RoofOntology));
				}


			}

		}

		private void GenerateDefaultNamespace(List<Namespace> defaultNamespaces, GeneratorSettings ge, bool isOWl)
		{
			if (defaultNamespaces != null && ge != null)
			{
				if (!string.IsNullOrEmpty(ge.MetaURI))
				{
					Namespace n = defaultNamespaces.Where(x => !string.IsNullOrEmpty(x.Prefix) && x.Prefix.Equals("meta")).SingleOrDefault();
					if (n != null)
					{
						n.Value = ge.MetaURI;
					}
				}

				if (isOWl == true && ge.IsSpecialOntology && !string.IsNullOrEmpty(ge.ExtractionOntologyNS))
				{
					string ns = String.Format("{0}", ge.ExtractionOntologyNS);
					Namespace n = defaultNamespaces.Where(x => x.IsToBeDefault).SingleOrDefault();
					if (n != null)
					{
						n.Value = ns;
					}
				}
				else if (!string.IsNullOrEmpty(ge.DefaultNamespace))
				{
					string ns = String.Format("{0}", ge.DefaultNamespace);
					Namespace n = defaultNamespaces.Where(x => x.IsToBeDefault).SingleOrDefault();
					if (n != null)
					{
						n.Value = ns;
					}
				}
			}
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
			GenerateStartElement(writer);
			GenerateOntologyTag(writer);
			foreach (ProfileElementTypes pet in profileForGenerating.ProfileMap.Keys)
			{
				foreach (ProfileElement pe in profileForGenerating.ProfileMap[pet])
				{
					GenerateElement(pet, pe, writer);
				}
			}

			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Close();
		}

		public void GenerateOntologyTag(XmlWriter writer)
		{
			writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.owlOntology, null);
			writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfAbout, null, baseAdress);
			GenerateImports(writer);
			writer.WriteEndElement();
		}

		private void GenerateElement(ProfileElementTypes type, ProfileElement pe, XmlWriter writer)
		{
            if (pe.IsBlankNode) 
            {
                reporter.AddtoEntityCountByType(EntityTypesGeneratorReporter.BlankId,1);
            }

			switch (type)
			{
               

				case ProfileElementTypes.ClassCategory:
					ClassCategory clsCat = pe as ClassCategory;
					if (clsCat != null)
						GenerateClassCategory(clsCat, writer);
					break;
				case ProfileElementTypes.Class:
					Class cls = pe as Class;
					if (cls != null)
						GenerateClass(cls, writer);
					break;

				case ProfileElementTypes.Property:
					Property p = pe as Property;
					GenerateProperty(p, ref writer);
					break;

				case ProfileElementTypes.EnumerationElement:
					EnumMember em = pe as EnumMember;
					GenerateEnumMember(em, writer);
					break;

				case ProfileElementTypes.Unknown:
					reporter.AddtoEntityCountByType(EntityTypesGeneratorReporter.NotGenerated, 1);
					break;
			}

		}


		private void GenerateURI(string uri, string baseURI,bool isBlank, XmlWriter writer, string startingTag,string blankNodeTag)
		{
			if (!isBlank)
			{
				writer.WriteAttributeString(OWL2Namespace.rdfPrefix, startingTag, null, baseURI + uri);
			}
			else
			{
				
				writer.WriteAttributeString(OWL2Namespace.rdfPrefix,blankNodeTag, null, uri);
			}
		}

		/// <summary>
		/// Generate class category to OWL2 format
		/// </summary>
		/// <param name="csCat">Class category to be represented in xml</param>
		/// <param name="writer">Writer to which xml representation of class category is being written</param>
		private void GenerateClassCategory(ClassCategory csCat, XmlWriter writer)
		{
			if (csCat != null && writer != null)
			{
                if (!csCat.IsBasePackage)
                {
                    writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.NamedIndividual, null);
                    GenerateURI(csCat.URI, baseAdress, csCat.IsBlankNode, writer, OWL2Namespace.rdfAbout, OWL2Namespace.NodeId);
                    //if (!StringManipulationManager.IsBlankNode(csCat.URI))
                    //{
                    //	writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfAbout, null, baseAdress + csCat.URI);
                    //}
                    //else 
                    //{
                    //	writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfAbout, null, baseAdress + csCat.URI);
                    //}

                    if (!string.IsNullOrEmpty(csCat.Type))
                    {
                        writer.WriteStartElement(OWL2Namespace.rdfPrefix, OWL2Namespace.Type, null);
                        writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, csCat.Type);
                        writer.WriteEndElement();
                    }
                    GenerateProfileElement(csCat, writer);
                    writer.WriteEndElement();
                    reporter.AddtoEntityCountByType(EntityTypesGeneratorReporter.NamedIndividual, 1);
                }
                else 
                {
                    reporter.AddtoEntityCountByType(EntityTypesGeneratorReporter.NotGenerated,1);
                }
             }
		}

		/// <summary>
		/// Generate enum member to OWL2 format 
		/// </summary>
		/// <param name="em">Enum member to be generated</param>
		/// <param name="writer"></param>
		private void GenerateEnumMember(EnumMember em, XmlWriter writer)
		{
			if (em != null && writer != null)
			{
				reporter.AddtoEntityCountByType(EntityTypesGeneratorReporter.NamedIndividual, 1);
				writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.NamedIndividual, null);
				GenerateURI(em.URI, baseAdress,em.IsBlankNode, writer,OWL2Namespace.rdfAbout,OWL2Namespace.NodeId);
				//writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfAbout, null, baseAdress + em.URI);
				if (!profileForGenerating.IsOwlProfile && !em.IsBlankNode)
				{
					string nmspace = string.Empty;
					bool found = false;

					Class cls = em.EnumerationObject as Class;

					if (settings.IsSpecialOntology && cls != null && settings.NameOfOntology != null && settings.NameOfOntology != string.Empty && cls.HasStereotype(settings.NameOfOntology))
					{
						found = true;
						nmspace = importNamespaces.Where(x => x.Prefix.Equals(settings.NameOfOntology)).Single().Value + em.URI;
					}
					else if (((cls != null && !cls.HasDifferentStereotype(stereotypesToSkip)) || (cls == null)) && settings.IsRoofOntology && settings.RoofOntology != null && settings.RoofOntology != string.Empty)
					{
						found = true;
						nmspace = importNamespaces.Where(x => x.Prefix.Equals(settings.RoofOntology)).Single().Value + em.URI;
					}

					if (found)
					{
						writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.SameAs, null);
						writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, nmspace);
						writer.WriteEndElement();
					}


				}

				GenerateProfileElement(em, writer);

				if (settings.IsEnumMembersInstances && em.EnumerationObject != null)
				{
					writer.WriteStartElement(OWL2Namespace.rdfPrefix, OWL2Namespace.Type, null);
					GenerateURI(em.EnumerationObject.URI, baseAdress,em.IsBlankNode, writer, OWL2Namespace.rdfResource, OWL2Namespace.rdfResource);
					//writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, baseAdress + em.EnumerationObject.URI);
					writer.WriteEndElement();
				}

				writer.WriteEndElement();
			}
		}


		private void GenerateStereotypes(List<ProfileElementStereotype> sterotypes, XmlWriter writer)
		{
			if (sterotypes != null && writer != null)
			{
				foreach (ProfileElementStereotype pes in sterotypes)
				{
					writer.WriteStartElement(MetaNamespace.MetaPrefix, MetaNamespace.Steroetype, null);
					writer.WriteValue(pes.Name);
					writer.WriteEndElement();
				}
			}
		}


		private void GenerateClass(Class cls, XmlWriter writer)
		{
			if (cls != null && writer != null)
			{
				if (cls.IsNotToBeGenerated)
				{

					reporter.AddtoEntityCountByType(EntityTypesGeneratorReporter.NotGenerated, 1);
				}
				else
				{
					reporter.AddtoEntityCountByType(EntityTypesGeneratorReporter.Class, 1);
					if (!profileForGenerating.IsOwlProfile && !cls.IsBlankNode)
					{
						string ontologyNamespace = string.Empty;
						bool found = false;
						if (settings.IsSpecialOntology && cls.HasStereotype(settings.NameOfOntology))
						{
							ontologyNamespace = importNamespaces.Where(x => x.Prefix.Equals(settings.NameOfOntology)).Single().Value;
							found = true;
						}
						else if (settings.IsRoofOntology && settings.RoofOntology != null && !settings.RoofOntology.Equals(string.Empty) && !cls.HasDifferentStereotype(stereotypesToSkip))
						{
							ontologyNamespace = importNamespaces.Where(x => x.Prefix.Equals(settings.RoofOntology)).Single().Value;
							found = true;
						}
						if (found)
							GenerateEquivalentClass(writer, ontologyNamespace, cls);
					}

					///starting owl class element
					writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.Class, null);
                    GenerateURI(cls.URI, baseAdress, cls.IsBlankNode, writer, OWL2Namespace.rdfAbout, OWL2Namespace.NodeId);
					//writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfAbout, null, baseAdress + cls.URI);

					if (cls.SubClassOf != null && cls.SubClassOf != string.Empty && cls.SubClassOfAsObject != null)
					{
						///subclass owl element
						writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsSubclasOf, null);
                        GenerateURI(cls.SubClassOf, baseAdress, cls.IsBlankNode, writer, OWL2Namespace.rdfResource, OWL2Namespace.rdfResource);
						//writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, baseAdress + cls.SubClassOfAsObject.URI);
						writer.WriteEndElement();
					}


					if (!string.IsNullOrEmpty(cls.BelongsToCategory) && cls.BelongsToCategoryAsObject != null && !profileForGenerating.IsOwlProfile)
					{
						writer.WriteStartElement(MetaNamespace.MetaPrefix, MetaNamespace.Belongs, null);
						string baseURI, uri;
						ClassCategory cat = cls.BelongsToCategoryAsObject as ClassCategory;
						if (cat != null && cat.IsBasePackage)
						{
							baseURI = predefinedNamespaces.Where(x => !string.IsNullOrEmpty(x.Prefix) && x.Prefix.Equals("meta")).Single().Value;
						}
						else
						{
							baseURI = baseAdress;
						}
						uri = cls.BelongsToCategory;
                        GenerateURI(uri, baseURI,false, writer, OWL2Namespace.rdfResource, OWL2Namespace.rdfResource);
						// writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null,baseURI);
						writer.WriteEndElement();
					}

					//if (!profileForGenerating.IsOwlProfile)
					//{
					if (cls.MyProperties != null)
					{
						foreach (ProfileElement pe in cls.MyProperties)
						{
							///restriction for property
							GeneratePropertyForClass(pe as Property, writer);
						}
					}

					GenerateStereotypes(cls.Stereotypes, writer);
					GenerateProfileElement(cls, writer);
					GenerateEnumeration(cls, writer);
					writer.WriteEndElement();
				}
			}
		}

		private void GenerateEnumeration(Class cls, XmlWriter writer)
		{
			if (cls != null && cls.IsEnumeration && writer != null && !settings.IsEnumMembersInstances)
			{
				writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.EqClass, null);
				writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.Class, null);
				writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.OneOf, null);
				writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.ParseType, null, OWL2Namespace.Collection);
				GenerateEnumMembers(cls.MyEnumerationMembers, writer);
				writer.WriteEndElement();
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
		}


		private void GenerateEnumMembers(List<ProfileElement> members, XmlWriter writer)
		{
			if (members != null && writer != null)
			{
				List<EnumMember> enumMembers = members.Cast<EnumMember>().ToList();
				foreach (EnumMember em in enumMembers)
				{
                    GenerateDescription(writer, em.IsBlankNode, BaseAddress, em.URI);
				}
			}
		}

		/// <summary>
		/// Generates rdf description tag in owl2 document 
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="value"></param>
		private void GenerateDescription(XmlWriter writer,bool isBlank, string baseURI, string uri)
		{
			writer.WriteStartElement(OWL2Namespace.rdfPrefix, OWL2Namespace.Description, null);
			GenerateURI(uri, baseURI,isBlank, writer, OWL2Namespace.rdfAbout,OWL2Namespace.NodeId);
			//writer.WritettributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfAbout, null, value);
			writer.WriteEndElement();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="property"></param>
		/// <param name="writer"></param>
		private void GeneratePropertyForClass(Property property, XmlWriter writer)
		{
			if (property != null && property.MultiplicityAsString != null)
			{
				///start of subclass element 
				writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsSubclasOf, null);
				///start of restriction owl element  
				writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.owlRestriction, null);
				writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.OnProperty, null);
                GenerateURI(property.URI, baseAdress,property.IsBlankNode, writer, OWL2Namespace.rdfResource, OWL2Namespace.rdfResource);
				//writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, baseAdress + property.URI);
				writer.WriteEndElement();
				GenerateCardinality(property, writer);
				writer.WriteEndElement();
				writer.WriteEndElement();
			}

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="property"></param>
		/// <param name="writer"></param>
		private void GenerateCardinality(Property property, XmlWriter writer)
		{
			if (property != null)
			{

				CardinaltyType ct = property.ProcessMultiplicity();

				if (ct == CardinaltyType.ZEROTOMANY)
				{
					string value = property.RangeAsObject != null ? baseAdress + property.RangeAsObject.URI : baseAdress + property.Range;
					writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.AllValuesFrom, null);
					if (property.RangeAsObject != null)
					{
                        GenerateURI(property.RangeAsObject.URI, baseAdress, property.IsBlankNode, writer, OWL2Namespace.rdfResource, OWL2Namespace.rdfResource);
					}
					else
					{
						GenerateURI(property.Range, baseAdress,property.IsBlankNode, writer, OWL2Namespace.rdfResource, OWL2Namespace.rdfResource);
					}
					//writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, value);
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
						GenerateURI(rangeUri, baseAdress,property.IsBlankNode, writer, OWL2Namespace.rdfResource, OWL2Namespace.rdfResource);
						//writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, baseAdress + rangeUri);
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

				if (!profileForGenerating.IsOwlProfile && !property.IsBlankNode)
				{
					string ontologyNamespace = string.Empty;
					bool found = false;
					Class cls = property.DomainAsObject as Class;
					if (settings.IsSpecialOntology && (property.HasStereotype(settings.NameOfOntology) || (property.DomainAsObject != null && property.DomainAsObject.HasStereotype(settings.NameOfOntology))))
					{
						ontologyNamespace = importNamespaces.Where(x => x.Prefix.Equals(settings.NameOfOntology)).SingleOrDefault().Value;
						found = true;
					}
					else if ((((cls != null && !cls.HasDifferentStereotype(stereotypesToSkip)) || (cls == null)) && !property.HasDifferentStereotype(stereotypesToSkip)) && settings.IsRoofOntology && settings.RoofOntology != null && !settings.RoofOntology.Equals(string.Empty))
					{
						ontologyNamespace = importNamespaces.Where(x => x.Prefix.Equals(settings.RoofOntology)).SingleOrDefault().Value;
						found = true;
					}
					if (found)
						GenerateEquivalentProperty(writer, ontologyNamespace, property);
				}

				//if (!profileForGenerating.IsOwlProfile && (hasSpecialStereotype && (property.HasStereotype(specialStereotype) || (property.DomainAsObject != null && property.DomainAsObject.HasStereotype(specialStereotype)))))
				//{
				//	string ontologyNamespace = importNamespaces.Where(x => x.Prefix.Equals(specialStereotype)).Single().Value;
				//	GenerateEquivalentProperty(ref writer, ontologyNamespace, property);
				//}

				if (property.IsObjectProperty())
				{
					reporter.AddtoEntityCountByType(EntityTypesGeneratorReporter.ObjectProperty, 1);
					propertyType = OWL2Namespace.ObjectProperty;
					rangeValue = property.RangeAsObject != null ? property.RangeAsObject.URI : property.Range;
					//rangeValue = StringManipulationManager.IsBlankNode(rangeValue) ? rangeValue : BaseAddress + rangeValue;
					//rangeValue = property.RangeAsObject != null ? String.Format("{0}{1}", BaseAddress, property.RangeAsObject.URI) : String.Format("{0}{1}", BaseAddress, property.Range);
				}
				else
				{
					reporter.AddtoEntityCountByType(EntityTypesGeneratorReporter.DatatypeProperty, 1);
					propertyType = OWL2Namespace.DatatypeProperty;
					string xsd = DefaultNamespaces.Where(x => x.Prefix != null && x.Prefix.Equals("xsd")).Single().Value;
					rangeValue = String.Format("{0}{1}", xsd, property.ProcessDatatype());
				}

				domainUri = property.DomainAsObject == null  ? property.Domain :  property.DomainAsObject.URI;
				//domainUri = StringManipulationManager.IsBlankNode(domainUri) ? domainUri : baseAdress + domainUri; 

				writer.WriteStartElement(OWL2Namespace.owlPrefix, propertyType, null);
				GenerateURI(property.URI, baseAdress,property.IsBlankNode, writer, OWL2Namespace.rdfAbout,OWL2Namespace.NodeId);
				//writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfAbout, null, baseAdress + property.URI);

				if (!profileForGenerating.IsOwlProfile || (profileForGenerating.IsOwlProfile && property.DomainAsObject != null))
				{
					writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsDomain, null);
					GenerateURI(domainUri, BaseAddress,property.IsBlankNode, writer, OWL2Namespace.rdfResource, OWL2Namespace.rdfResource);
					//writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, domainUri);
					writer.WriteEndElement();
				}

				writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsRange, null);
				if (property.IsObjectProperty())
				{
					GenerateURI(rangeValue, baseAdress,property.IsBlankNode, writer, OWL2Namespace.rdfResource, OWL2Namespace.rdfResource);
				}
				else
				{
					writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, rangeValue);
				}
				writer.WriteEndElement();


				///deo za generisanje cimsInverseRoleName
				if (!string.IsNullOrEmpty(property.InverseRoleName) && property.InverseRoleNameAsObject != null)
				{
					writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.InverseOf, null);
					GenerateURI(property.InverseRoleNameAsObject.URI, baseAdress,property.IsBlankNode, writer, OWL2Namespace.rdfResource, OWL2Namespace.rdfResource);
					//writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, baseAdress + property.InverseRoleNameAsObject.URI);
					writer.WriteEndElement();
				}

				if (!string.IsNullOrEmpty(property.AssociationUsed))
				{
					writer.WriteStartElement(MetaNamespace.MetaPrefix, MetaNamespace.AsocUsed, null);
					writer.WriteValue(property.AssociationUsed);
					writer.WriteEndElement();
				}

				GenerateStereotypes(property.Stereotypes, writer);
				//if (!profileForGenerating.IsOwlProfile)
				GenerateProfileElement(property, writer);

				writer.WriteEndElement();
			}
		}

		private void GenerateProfileElement(ProfileElement pe, XmlWriter writer)
		{
			if (pe.Comments != null)
			{
				foreach (ComplexTag ct in pe.Comments)
				{
					GenerateCommentElement(writer, ct);
				}
			}

			if (pe.Labels != null)
			{
				foreach (ComplexTag ct in pe.Labels)
				{
					GenerateLabelElement(writer, ct);
				}
			}

			if (!string.IsNullOrEmpty(pe.IsFixed))
			{
				writer.WriteStartElement(MetaNamespace.MetaPrefix, MetaNamespace.Fixed, null);
				writer.WriteValue(pe.IsFixed);
				writer.WriteEndElement();
			}

			writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.IsDefinedBy, null);
			writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, BaseAddress);
			writer.WriteEndElement();
		}

		private void GenerateCommentElement(XmlWriter writer, ComplexTag ct)
		{
			if (ct != null && ct.Value != string.Empty && writer != null)
			{
				writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsComment, null);
				GenerateAttsLabelComment(writer, ct.Attributes);
				writer.WriteValue(ct.Value);
				writer.WriteEndElement();
			}
		}

		private void GenerateLabelElement(XmlWriter writer, ComplexTag ct)
		{
			if (ct != null && ct.Value != string.Empty && writer != null)
			{
				writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsLabel, null);
				GenerateAttsLabelComment(writer, ct.Attributes);
				writer.WriteValue(ct.Value);
				writer.WriteEndElement();
			}
		}

		private void GenerateAttsLabelComment(XmlWriter writer, Dictionary<string, string> atts)
		{
			if (atts != null && atts.ContainsKey(OWL2Namespace.xmlLang))
			{
				writer.WriteAttributeString(OWL2Namespace.xmlPrefix, OWL2Namespace.Lang, null, atts[OWL2Namespace.xmlLang]);
			}
		}


		public void GenerateStartElement(XmlWriter writer)
		{
			writer.WriteStartDocument();
			string rdfVal = DefaultNamespaces.Where(x => x.Prefix != null && x.Prefix.Equals("rdf")).Single().Value;
			writer.WriteStartElement(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfRDF, rdfVal);
			foreach (Namespace n in DefaultNamespaces)
			{
                if(!n.IsNotToBeWritten)
				writer.WriteAttributeString(n.Ns, n.Prefix, null, n.Value);
			}
		}

		private void GenerateImports(XmlWriter writer)
		{
			if (importNamespaces != null && profileForGenerating != null && !profileForGenerating.IsOwlProfile)
			{

				foreach (Namespace n in importNamespaces)
				{

                        writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.Import, null);
                        writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, n.Value);
                        writer.WriteEndElement();
                    

				}
			}
		}

		private void GenerateEquivalentProperty(XmlWriter writer, string baseAddressImport, Property p)
		{
			if (p != null && writer != null)
			{
				writer.WriteStartElement(OWL2Namespace.rdfPrefix, OWL2Namespace.Description, null);
				GenerateURI(StringManipulationManager.ExtractAllWithSeparator(p.URI, StringManipulationManager.SeparatorSharp), BaseAddress,p.IsBlankNode, writer, OWL2Namespace.rdfAbout,OWL2Namespace.NodeId);
				//writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfAbout, null, baseAdress + StringManipulationManager.ExtractAllWithSeparator(p.URI, StringManipulationManager.SeparatorSharp));
				writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.EqProperty, null);
				writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, baseAddressImport + StringManipulationManager.ExtractAllWithSeparator(p.URI, StringManipulationManager.SeparatorSharp));
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
		}

		private void GenerateEquivalentClass(XmlWriter writer, string baseAddressImport, Class c)
		{
			if (c != null && writer != null)
			{
				writer.WriteStartElement(OWL2Namespace.rdfPrefix, OWL2Namespace.Description, null);
				GenerateURI(StringManipulationManager.ExtractAllWithSeparator(c.URI, StringManipulationManager.SeparatorSharp), BaseAddress,c.IsBlankNode, writer, OWL2Namespace.rdfAbout, OWL2Namespace.NodeId);
				//writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfAbout, null, baseAdress + StringManipulationManager.ExtractAllWithSeparator(c.URI, StringManipulationManager.SeparatorSharp));
				//
				writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.EqClass, null);
				writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, baseAddressImport + StringManipulationManager.ExtractAllWithSeparator(c.URI, StringManipulationManager.SeparatorSharp));
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
		}

	}
}
