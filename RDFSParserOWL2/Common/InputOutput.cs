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

		//public static const string entsoFilepath = @"";

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


		//public static Profile GenerateEntsoProfile() 
		//{
		//	OWL2Generator 
		
		//}


	}
}
