using RDFSParserOWL2.Common;
using RDFSParserOWL2.Generator;
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

		private GeneratorSettings settings;
		private List<string> paths;
		

		public ConverterRDFSToOWL2()
		{
			//rdfsParser = new RDFSXMLParser();
			//owlParser = new OWL2XMLParser();
		}

		//public ConverterRDFSToOWL2(string path)
		//{
		//	rdfsParser = new RDFSXMLParser(path);
		//}

		public ConverterRDFSToOWL2(List<string> filePaths) 
		{
			paths = filePaths;
		}

		public ConverterRDFSToOWL2(List<string> filePaths, GeneratorSettings genSet)
		{
			//rdfsParser = new RDFSXMLParser(path);
			paths = filePaths;
			settings = genSet;
		}


		/// <summary>
		/// Method for converting file from RDFS format to OWL2 format 
		/// </summary>
		public void Convert()
		{
			
			if (settings != null)
			{
				

				Profile extensionOntologyProfile = null;
                Profile metaProfile = null;
				if (settings.IsExtensionOntology) 
				{
					RDFXMLParser extensionOntologyParser = new RDFXMLParser(InputOutput.CreatePathForGeneratedOWL(InputOutput.CreateOWLFilename(settings.ExtensionOntologyName)), new OWL2RDFXMLHandler(),true);
					extensionOntologyParser.ParseProfile();
					extensionOntologyProfile = extensionOntologyParser.Profile;
					extensionOntologyProfile.IsOwlProfile = true;	
				}

                    metaProfile = InputOutput.LoadMetaProfile();

				foreach(string path in paths) 
				{
					Stopwatch sw = new Stopwatch();
					sw.Start();
					RDFXMLParser rdfsParser = new RDFXMLParser(path, new RDFSRDFXMLHandler(),false);
					rdfsParser.ParseProfile();
					Profile profile = rdfsParser.Profile;
					profile.MarkElementsWithStereotypes(InputOutput.LoadStereotypesToSkip());
                    if (metaProfile != null)
                    {
                        profile.PopulateClassCategoryReferences(metaProfile);
                        profile.MarkBasePackages(metaProfile);
                    }

					OWL2Generator generator = new OWL2Generator(profile, settings);
					generator.GenerateProfile();
					

					if (settings.IsExtensionOntology && extensionOntologyProfile!=null)
					{

							profile.ProcessElementsWithSpecialStereotype(extensionOntologyProfile, settings.ExtensionOntologyName);
							extensionOntologyProfile.PopulateObjectReferences();
					}

					sw.Stop();
					string timeOfParsing = String.Format("\nTotal time:{0}",sw.Elapsed);
					InputOutput.WriteReportToFile(InputOutput.CreateTxtFilename(generator.ShortName + DateTime.Now.Ticks), rdfsParser.Reporter.GenerateReport() + generator.Reporter.GenerateReport()+timeOfParsing);	
				}

				if (settings.IsExtensionOntology && extensionOntologyProfile != null) 
				{
					OWL2Generator generator = new OWL2Generator(extensionOntologyProfile, settings);
					generator.GenerateProfile();
				}



			}




		}
	}
}
