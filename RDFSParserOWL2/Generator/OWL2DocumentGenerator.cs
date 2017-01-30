using RDFSParserOWL2.Common;
using RDFSParserOWL2.Generator.Helper;
using RDFSParserOWL2.Generator.Interfaces;
using RDFSParserOWL2.Manager;
using RDFSParserOWL2.Model;
using RDFSParserOWL2.Model.Settings;
using RDFSParserOWL2.Model.Storage;
using RDFSParserOWL2.Reporter;
using RDFSParserOWL2.Reporter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RDFSParserOWL2.Generator
{
    public  class OWL2DocumentGenerator:IDocumentGenerator
    {
        private XmlWriter writer;
        private const string path = @"../../Resources/OWL2Generated/";

        public void GenerateDocument(Model.Profile profileForGenerating)
        {
            OWL2GeneratorReporter reporter = new OWL2GeneratorReporter();
            string filename;
            StringManipulationManager.GenerateNameForFile(profileForGenerating.FileName, InputOutput.LoadWordsToSkip(), out filename);
            WriteStartDocument(profileForGenerating.BaseNS,filename, profileForGenerating.ImportNamespaces, profileForGenerating.UsedNamespaces);
            foreach (ProfileElement pe in profileForGenerating.GetAllElements())
            {
                WriteElement(pe, profileForGenerating.BaseNS, profileForGenerating.EnumKind);
                reporter.AddToEntityCountByTypes(pe.StatusForOWL2(),1);       
            }
            profileForGenerating.AddToReport(reporter.GenerateReport());
            if (profileForGenerating.IsReportToBeWrittenToFile)
            {
                string convertReport = String.Format("Converted from file in CIM RDFS  {0} to file in OWL 2 {1}", profileForGenerating.FileName, filename + ".owl");
                profileForGenerating.AddToReport(convertReport);
                InputOutput.WriteReportToFile(StringManipulationManager.TrimAfterStringIncludinString(profileForGenerating.FileName, ".rdf") + DateTime.Now.Ticks.ToString(), profileForGenerating.ProcessingReport.ToString());
            }
            WriteEndDocument();
        }

        public void WriteStartDocument(string baseAddress, string documentName, List<string> importNamespaces, List<Namespace> additionalNamespaces)
        {
            NamespaceStorage namespaces = new NamespaceStorage(InputOutput.LoadPredefinedNamespaces());
            namespaces.ProcessNamespaces(baseAddress,additionalNamespaces);
            if (!documentName.EndsWith(".owl")) 
            {
                documentName += ".owl";
            }
            SetWriter(path + documentName);
            WriteStartElement(namespaces);
            WriteOntologyElement(importNamespaces, baseAddress);
        }

        private void WriteOntologyElement(List<string> importNamespaces, string baseAddress)
        {
            writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.owlOntology, null);
            writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfAbout, null, baseAddress);
            WriteOwlImports(importNamespaces);
            writer.WriteEndElement();
        }

        private void WriteOwlImports(List<string> importNamespaces)
        {
            if (importNamespaces != null)
            {
                foreach (string ns in importNamespaces)
                {
                    writer.WriteStartElement(OWL2Namespace.owlPrefix, OWL2Namespace.Import, null);
                    writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, ns);
                    writer.WriteEndElement();
                }
            }
        }

        public void WriteEndDocument()
        {
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }


        private void SetWriter(string path)
        {
            XmlTextWriter xtw = new XmlTextWriter(path, null);
            xtw.Formatting = Formatting.Indented;
            xtw.Indentation = 4;
            writer = xtw;
        }

        private void WriteStartElement(NamespaceStorage namespaces)
        {
            writer.WriteStartDocument();
            string rdfVal = namespaces.GetNamespaceValue("rdf"); ;
            writer.WriteStartElement(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfRDF, rdfVal);
            foreach (Namespace n in namespaces.Namespaces)
            {
                if (!n.IsNotToBeWritten)
                    writer.WriteAttributeString(n.Ns, n.Prefix, null, n.Value);
            }
        }

        public void WriteElement(ProfileElement element, string baseUri, EnumRepresentationKindOWL2 enumOpt)
        {
            element.ToRDFXML(writer, baseUri, enumOpt);
        }


    }


}
