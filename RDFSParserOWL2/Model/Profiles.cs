using RDFSParserOWL2.Common;
using RDFSParserOWL2.Generator.Interfaces;
using RDFSParserOWL2.Manager;
using RDFSParserOWL2.Model.Settings;
using RDFSParserOWL2.Parser;
using RDFSParserOWL2.Parser.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Model
{
    public  class Profiles
    {
        private List<Profile> profileCollection;

        public Profiles() 
        {
            profileCollection = new List<Profile>();
        }

        public Profiles(List<Profile> profiles) 
        {
            profileCollection = profiles;
        }

        public void WriteProfilesToFile( IDocumentGenerator generator)
        {
            foreach (Profile profile in profileCollection)
                generator.GenerateDocument(profile);
        }



        public void ProcessElementsInProfiles(string metaBaseAddress)
        {
            Profile metaProfile = ParseMetaDocument();
            foreach (Profile profile in profileCollection)
            {
                
                profile.MarkElementsWithStereotypes(InputOutput.LoadStereotypesToSkip());
                profile.AddNamespace(new Namespace(metaBaseAddress,"meta"));
                if (metaProfile != null)
                {
                    profile.MarkBasePackages(metaProfile, metaBaseAddress);
                }
            }
        }

        public void ProcessProfilesInformationAfterParsing(string baseAddressOfDocument, List<string> importNamespaces,EnumRepresentationKindOWL2 enumRepresentation) 
        {
            foreach (Profile profile in profileCollection)
            {
                profile.ProcessProfileInformationAfterParsing(baseAddressOfDocument,importNamespaces,enumRepresentation);
            }
        }



        public void ProcessEquivalencesForProfileElementsInCommonOntology( string commonBaseAddress)
        {
            foreach (Profile profile in profileCollection)
            {
                profile.ProcessEquivalenceForProfileElementsForCommon(commonBaseAddress);
            }
        }


        public void ProcessEquivalencesForProfileElementsInExtensionOntology( string extensionOntologyBaseAddress, string stereotype)
        {
            foreach (Profile profile in profileCollection)
            {
                profile.ProcessEquivalenceForProfileElementsForExtension(extensionOntologyBaseAddress, stereotype);
            }
        }

        public void ProcessEquivalencesForProfileElementsForCommonAndExtension(string extensionOntologyBaseAddress, string stereotype, string commonOntologyBaseAddress)
        {
            foreach (Profile profile in profileCollection)
            {
                profile.ProcessEquivalenceForProfileElementsForExtensionAndCommon(extensionOntologyBaseAddress, stereotype, commonOntologyBaseAddress);
            }
        }


        private Profile ParseMetaDocument()
        {
            return InputOutput.LoadMetaProfile();
        }

        public  Profile ProcessExtensionOntology( string baseAddressExtensionOntology, string stereotype,EnumRepresentationKindOWL2 kind,string metaOntologyBaseUri)
        {
            RDFXMLParser extensionOntologyParser = new RDFXMLParser(new OWL2RDFXMLHandler(), true);
            Profile extensionOntologyProfile = extensionOntologyParser.ParseProfile(InputOutput.CreatePathForGeneratedOWL(InputOutput.CreateOWLFilename(stereotype)));
            foreach (Profile profile in profileCollection)
            {
                profile.ProcessElementsWithSpecialStereotype(extensionOntologyProfile, stereotype);
            }
            //extensionOntologyProfile.FileName = stereotype+".owl";
            extensionOntologyProfile.BaseNS = baseAddressExtensionOntology + stereotype;
            extensionOntologyProfile.EnumKind = kind;
            extensionOntologyProfile.ImportNamespaces = new List<string>();
            extensionOntologyProfile.AddNamespace(new Namespace(metaOntologyBaseUri, "meta"));
            extensionOntologyProfile.PopulateObjectReferences();
            return extensionOntologyProfile;
        }


    }
}
