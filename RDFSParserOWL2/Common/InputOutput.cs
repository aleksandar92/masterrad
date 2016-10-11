using RDFSParserOWL2.Generator;
using RDFSParserOWL2.Model;
using RDFSParserOWL2.Parser;
using RDFSParserOWL2.Parser.Handler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Common
{
	public static class InputOutput
	{
		public static  string entsoFilename = "entsoe.owl";
		   //private const string pathToWordsToSkip = @"../../Resources/WordsToSkip.xml";
		public static  string resourceFilepath = @"../../Resources/";
		public static  string owlGeneratedFilePath = @"../../Resources/OWL2Generated/";
		public static string filePathReports = @"../../Resources/Reports/";
        public static string importsFilename = "Imports.xml";
		public static string stereotypeFilename = "StereotypesToSkip.xml";
		private const string filenameWordsToSkip = "WordsToSkip.xml";
		private const string filenameNamespaces = "DefaultNamespaces.xml";
		private const string filenameFixed = "FixedStereotypes.xml";

		//public static const string entsoFilepath = @"";


		private static string ToSafeFileName( string s)
		{
			return s
				.Replace("\\", "")
				.Replace("/", "")
				.Replace("\"", "")
				.Replace("*", "")
				.Replace(":", "")
				.Replace("?", "")
				.Replace("<", "")
				.Replace(">", "")
				.Replace("|", "");
		}

		/// <summary>
		/// Method for loading entso owl profile 
		/// </summary>
		/// <returns>Returns resulting profile parsed from entsoe.owl file </returns>
		public static Profile LoadEntsoProfile() 
		{
			OWLRDFXMLHandler handler = new OWLRDFXMLHandler();
			using (FileStream fs = new FileStream(owlGeneratedFilePath+entsoFilename, FileMode.Open))
			{
				bool su;
				TimeSpan ts;
				XMLParser.DoParse(handler, fs,entsoFilename, out su, out ts);

			}
			return handler.Profile;		
		}


		/// <summary>
		/// Method for loading stereotypes to skip from configuration file
		/// </summary>
		/// <returns>List of stereotypes from file  </returns>
		public static List<string> LoadFixedStereotypes()
		{
			WordsXMLHandler reader = new WordsXMLHandler();
			bool succes;
			TimeSpan ts;
			using (FileStream fs = new FileStream(resourceFilepath + filenameFixed, FileMode.Open))
			{
				XMLParser.DoParse(reader, fs, null, out succes, out ts);
			}
			return reader.Words;
		}


		public static void WriteReportToFile(string fileName,string content) 
		{
			//File.WriteAllText(Path.GetFullPath(filePathReports).ToString()+ fileName, content);
			//string absolutPath = Path.GetFullPath(filePathReports+fileName);

			using (FileStream fs = new FileStream(Path.Combine(filePathReports,ToSafeFileName(fileName)), FileMode.OpenOrCreate))
			{
				using (StreamWriter sw = new StreamWriter(fs))
				{
					sw.Write(content);

				}
			}
		}
		


        /// <summary>
        /// Method for loading import namespaces from configuration file
        /// </summary>
        /// <returns>List of loaded namespaces </returns>
        public static List<Namespace> LoadImportNamespaces()
        {
            XMLNamespaceReaderHandler reader = new XMLNamespaceReaderHandler();
            bool succes;
            TimeSpan ts;
            using (FileStream fs = new FileStream(resourceFilepath+importsFilename, FileMode.Open))
            {
                XMLParser.DoParse(reader, fs, null, out succes, out ts);
            }
            return reader.Namespaces;
        }

		/// <summary>
		/// Method for loading stereotypes to skip from configuration file
		/// </summary>
		/// <returns>List of stereotypes from file  </returns>
		public static List<string> LoadStereotypesToSkip() 
		{
			WordsXMLHandler reader = new WordsXMLHandler();
			bool succes;
			TimeSpan ts;
			using (FileStream fs = new FileStream(resourceFilepath +stereotypeFilename, FileMode.Open))
			{
				XMLParser.DoParse(reader, fs, null, out succes, out ts);
			}
			return reader.Words;

		}

		/// <summary>
		/// Method for loading words to skip from configuration file
		/// </summary>
		/// <returns>List of words from file</returns>
		public  static List<string> LoadWordsToSkip()
		{
			WordsXMLHandler reader = new WordsXMLHandler();
			bool succes;
			TimeSpan ts;
			using (FileStream fs = new FileStream(resourceFilepath+filenameWordsToSkip, FileMode.Open))
			{
				XMLParser.DoParse(reader, fs, null, out succes, out ts);
			}
			return reader.Words;
		}

		public static List<Namespace> LoadPredefinedNamespaces()
		{
			XMLNamespaceReaderHandler reader = new XMLNamespaceReaderHandler();
			bool succes;
			TimeSpan ts;
			using (FileStream fs = new FileStream(resourceFilepath + filenameNamespaces, FileMode.Open))
			{
				XMLParser.DoParse(reader, fs, null, out succes, out ts);
			}
			return reader.Namespaces;
		}

		public static string CreateOWLFilename(string name) 
		{
			return name + ".owl";
		}

		public static string CreateTxtFilename(string name)
		{
			return name + ".txt";
		}

		public static string CreatePathForGeneratedOWL(string fileName) 
		{
			return owlGeneratedFilePath + fileName;	
			
		}


		public static bool CheckIfFileExists(string filePath) 
		{
			bool exists = false;

			if (filePath != null)
			{
				string absolute =  Path.GetFullPath(filePath);
			
				if (File.Exists(absolute))
				{
					exists = true;
				}

			}
			return exists;

		}

	}
}
