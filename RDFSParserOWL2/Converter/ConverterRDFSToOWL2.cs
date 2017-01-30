using RDFSParserOWL2.Common;
using RDFSParserOWL2.Generator;
using RDFSParserOWL2.Generator.Interfaces;
using RDFSParserOWL2.Manager;
using RDFSParserOWL2.Model;
using RDFSParserOWL2.Model.Settings;
using RDFSParserOWL2.Parser;
using RDFSParserOWL2.Parser.Handler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Converter
{

    /// <summary>
    /// Class that converts file in RDFS to  OWL2 format 
    /// </summary>
    public class ConverterRDFSToOWL2
    {

        private IDocumentGenerator generator;

        public ConverterRDFSToOWL2(IDocumentGenerator docGenerator)
        {
            generator = docGenerator;
        }

        public void Convert(List<string> paths, string baseAdressOfResultingDocument, string baseAdressOfMetaOntology, EnumRepresentationKindOWL2 enumRepresentationInResultingDocument)
        {
            Profiles profiles = new Profiles(ParseDocuments(paths));
            profiles.ProcessElementsInProfiles(baseAdressOfMetaOntology);
            profiles.ProcessProfilesInformationAfterParsing(baseAdressOfResultingDocument, new List<string>(), enumRepresentationInResultingDocument);
            profiles.WriteProfilesToFile(generator);
        }

        public void Convert(List<string> paths, string baseAdressOfResultingDocument, string baseAdressOfMetaOntology, EnumRepresentationKindOWL2 enumRepresentationInResultingDocument, string commonOntologyBaseAddress)
        {
            Profiles profiles = new Profiles(ParseDocuments(paths));
            profiles.ProcessElementsInProfiles(baseAdressOfMetaOntology);
            profiles.ProcessProfilesInformationAfterParsing(baseAdressOfResultingDocument, ProcessImportNamespaces(commonOntologyBaseAddress), enumRepresentationInResultingDocument);
            profiles.ProcessEquivalencesForProfileElementsInCommonOntology(commonOntologyBaseAddress);
            profiles.WriteProfilesToFile(generator);
        }


        public void Convert(List<string> paths, string baseAdressOfResultingDocument, string baseAdressOfMetaOntology, EnumRepresentationKindOWL2 enumRepresentationInResultingDocument, string baseAdressOfExtensionOntology, string stereotype, string commonOntologyBaseAdress)
        {
            Profiles profiles = new Profiles(ParseDocuments(paths));
            profiles.ProcessElementsInProfiles(baseAdressOfMetaOntology);
            profiles.ProcessProfilesInformationAfterParsing(baseAdressOfResultingDocument, ProcessImportNamespaces(baseAdressOfExtensionOntology + stereotype, commonOntologyBaseAdress), enumRepresentationInResultingDocument);
            profiles.ProcessEquivalencesForProfileElementsForCommonAndExtension(baseAdressOfExtensionOntology, stereotype, commonOntologyBaseAdress);
            profiles.WriteProfilesToFile(generator);
            Profile extensionOntologyProfile = profiles.ProcessExtensionOntology(baseAdressOfExtensionOntology, stereotype, enumRepresentationInResultingDocument, baseAdressOfMetaOntology);
            generator.GenerateDocument(extensionOntologyProfile);
        }


        public void Convert(List<string> paths, string baseAdressOfResultingDocument, string baseAdressOfMetaOntology, EnumRepresentationKindOWL2 enumRepresentationInResultingDocument, string baseAdressOfExtensionOntology, string stereotype)
        {
            Profiles profiles = new Profiles(ParseDocuments(paths));
            profiles.ProcessElementsInProfiles(baseAdressOfMetaOntology);
            profiles.ProcessProfilesInformationAfterParsing(baseAdressOfResultingDocument, ProcessImportNamespaces(baseAdressOfExtensionOntology + stereotype), enumRepresentationInResultingDocument);
            profiles.ProcessEquivalencesForProfileElementsInExtensionOntology(baseAdressOfExtensionOntology, stereotype);
            profiles.WriteProfilesToFile(generator);
            Profile extensionOntologyProfile = profiles.ProcessExtensionOntology(baseAdressOfExtensionOntology, stereotype, enumRepresentationInResultingDocument, baseAdressOfMetaOntology);
            generator.GenerateDocument(extensionOntologyProfile);
        }


        #region helper methods



        private List<Profile> ParseDocuments(List<string> paths)
        {
            RDFXMLParser rdfsParser = new RDFXMLParser(new RDFSRDFXMLHandler(), false);
            List<Profile> parsedProfiles = new List<Profile>();

            foreach (string path in paths)
            {
                Profile profile = rdfsParser.ParseProfile(path);
                parsedProfiles.Add(profile);
            }

            return parsedProfiles;
        }


        #endregion
        # region Methods for populating import namespace
        private List<string> ProcessImportNamespaces(string extensionOntologyUri, string commonOntologyUri)
        {
            List<string> result = new List<string>();

            if (!string.IsNullOrEmpty(extensionOntologyUri))
            {
                result.Add(extensionOntologyUri);
            }

            if (!string.IsNullOrEmpty(commonOntologyUri))
            {
                result.Add(commonOntologyUri);
            }

            return result;
        }

        private List<string> ProcessImportNamespaces(string commonOntologyUri)
        {
            List<string> result = new List<string>();
            if (!string.IsNullOrEmpty(commonOntologyUri))
            {
                result.Add(commonOntologyUri);
            }

            return result;
        }

        #endregion

    }
}
