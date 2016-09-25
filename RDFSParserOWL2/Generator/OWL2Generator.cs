using RDFSParserOWL2.Generator.Helper;
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
    public class OWL2Generator
    {
        private const string pathToFileNamespaces = @"../../Resources/DefaultNamespaces.xml";
        private const string pathToWordsToSkip = @"../../Resources/WordsToSkip.xml";
        private const string path = @"../../Resources/OWL2Generated/";


        private string fileName;
        private string shortName;
        private List<Namespace> predefinedNamespaces;
        private string baseAdress;
        private List<string> words;


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


        public OWL2Generator(Profile profile)
        {
            LoadWordsToSkip();
            LoadPredefinedNamespaces();
            fileName = profile.FileName;
            GenerateNameForFile(fileName);
            GenerateNamespaces();

            BaseAddress = predefinedNamespaces.Where(x => x.IsToBeDefault == true).Single().Value;
            //GenerateProfile(null);
        }

        public void LoadPredefinedNamespaces()
        {
            XMLNamespaceReaderHandler reader = new XMLNamespaceReaderHandler();
            bool succes;
            TimeSpan ts;
            using (FileStream fs = new FileStream(pathToFileNamespaces, FileMode.Open))
            {
                XMLParser.DoParse(reader, fs, null, out succes, out ts);
            }
            predefinedNamespaces = reader.Namespaces;
        }

        public void LoadWordsToSkip()
        {
            WordsXMLHandler reader = new WordsXMLHandler();
            bool succes;
            TimeSpan ts;
            using (FileStream fs = new FileStream(pathToWordsToSkip, FileMode.Open))
            {
                XMLParser.DoParse(reader, fs, null, out succes, out ts);
            }
            words = reader.Words;
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

        public void GenerateProfile(Model.Profile profile)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(path).Append(shortName).Append(".owl");
            XmlTextWriter xtw = new XmlTextWriter(sb.ToString(), null);
            xtw.Formatting = Formatting.Indented;
            xtw.Indentation = 4;
            XmlWriter writer = xtw;
            GenerateStartElement(ref writer);
            GenerateOntologyTag(ref writer);
            foreach (ProfileElementTypes pet in profile.ProfileMap.Keys)
            {
                foreach (ProfileElement pe in profile.ProfileMap[pet])
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
                    if (p != null)
                        GenerateProperty(p, ref writer);
                    break;
            }

        }

        private void GenerateClass(Class cls, ref XmlWriter writer)
        {

            writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.Class, null);
            writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfAbout, null, baseAdress + cls.URI);

            if (cls.SubClassOf != null && cls.SubClassOf != string.Empty && cls.SubClassOfAsObject != null)
            {
                writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsSubclasOf, null);
                writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, baseAdress + cls.SubClassOfAsObject.URI);
                writer.WriteEndElement();
            }

            if (cls.MyProperties != null)
                foreach (ProfileElement pe in cls.MyProperties)
                {
                    GeneratePropertyForClass(pe as Property, ref writer);
                }

            GenerateProfileElement(cls, ref writer);

            writer.WriteEndElement();


        }

        private void GeneratePropertyForClass(Property property, ref XmlWriter writer)
        {
            if (property != null && property.MultiplicityAsString != null)
            {
                writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsSubclasOf, null);
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
                    writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.AllValuesFrom, null);
                    writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, baseAdress + property.RangeAsObject.URI);
                    writer.WriteEndElement();
                }
                else
                {
                    string qCardinality = string.Empty;
                    string xsd = DefaultNamespaces.Where(x => x.Prefix != null && x.Prefix.Equals("xsd")).Single().Value;
                    string value = String.Format("{0}{1}", xsd, OWL2Namespace.nonNegativeInteger);

                    if (ct == CardinaltyType.ZEROTOONE)
                    {
                        qCardinality = OWL2Namespace.owlMaxQualified;
                    }
                    else if (ct == CardinaltyType.ONETOMANY)
                    {
                        qCardinality = OWL2Namespace.owlMinQualified;
                    }
                    else if (ct == CardinaltyType.ONE)
                    {
                        qCardinality = OWL2Namespace.owlQualified;
                    }

                    writer.WriteStartElement(OWL2Namespace.owlPrefix, qCardinality, null);
                    writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfDatatype, null, value);
                    writer.WriteValue(1);
                    writer.WriteEndElement();


                    if (property.IsObjectProperty())
                    {
                        writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.owlOnClass, null);
                        writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, baseAdress + property.RangeAsObject.URI);
                        writer.WriteEndElement();
                    }
                    else
                    {
                        writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.owlOnDataRange, null);
                        writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, xsd + property.ProcessDatatype());
                        writer.WriteEndElement();
                    }

                }

            }
        }

        private void GenerateProperty(Property property, ref XmlWriter writer)
        {
            string propertyType;
            string rangeValue;
            if (property.IsObjectProperty())
            {
                propertyType = OWL2Namespace.ObjectProperty;
                rangeValue = String.Format("{0}{1}", BaseAddress, property.RangeAsObject.URI);
            }
            else
            {
                propertyType = OWL2Namespace.DatatypeProperty;
                string xsd = DefaultNamespaces.Where(x => x.Prefix != null && x.Prefix.Equals("xsd")).Single().Value;
                rangeValue = String.Format("{0}{1}", xsd, property.ProcessDatatype());
            }

            writer.WriteStartElement(OWL2Namespace.owlPrefix, propertyType, null);
            writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfAbout, null, baseAdress + property.URI);


            writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsDomain, null);
            writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, baseAdress + property.DomainAsObject.URI);
            writer.WriteEndElement();

            writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsRange, null);
            writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, rangeValue);
            writer.WriteEndElement();

            GenerateProfileElement(property, ref writer);


            writer.WriteEndElement();
        }


        private void GenerateProfileElement(ProfileElement pe, ref XmlWriter writer)
        {
            if (pe.Comment.Value != string.Empty)
            {
                writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsComment, null);
                writer.WriteValue(pe.Comment.Value);
                writer.WriteEndElement();
            }

            if (pe.Label.Value != string.Empty)
            {
                writer.WriteStartElement(OWL2Namespace.rdfsPrefix, OWL2Namespace.rdfsLabel, null);
                writer.WriteValue(pe.Label.Value);
                writer.WriteEndElement();
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




    }
}
