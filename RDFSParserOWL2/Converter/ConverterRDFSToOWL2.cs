﻿using RDFSParserOWL2.Common;
using RDFSParserOWL2.Generator;
using RDFSParserOWL2.Model;
using RDFSParserOWL2.Model.Settings;
using RDFSParserOWL2.Parser;
using System;
using System.Collections.Generic;
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
		/// <summary>
		/// Parser for file in RDFS 
		/// </summary>
		private RDFSXMLParser rdfsParser;

		/// <summary>
		/// Generator for OWL2
		/// </summary>
		private OWL2Generator generator;
		private GeneratorSettings ge;
		private OWL2XMLParser owlParser;

		public ConverterRDFSToOWL2()
		{
			rdfsParser = new RDFSXMLParser();
			owlParser = new OWL2XMLParser();
		}

		public ConverterRDFSToOWL2(string path)
		{
			rdfsParser = new RDFSXMLParser(path);
		}

		public ConverterRDFSToOWL2(string path, GeneratorSettings genSet)
		{
			rdfsParser = new RDFSXMLParser(path);
			ge = genSet;
		}




		/// <summary>
		/// Method for converting file from RDFS format to OWL2 format 
		/// </summary>
		public void Convert()
		{
			if (ge != null)
			{
				rdfsParser.ParseProfile();
				Profile profile = rdfsParser.Profile;
				profile.RemoveElementsWithStereotypes(InputOutput.LoadStereotypesToSkip());

				if (ge.IsSpecialOntology)
				{
					owlParser = new OWL2XMLParser(InputOutput.CreatePathForGeneratedOWL(InputOutput.CreateOWLFilename(ge.NameOfOntology)));
					owlParser.ParseProfile();
					Profile entsoProfile = owlParser.Profile;

					if (entsoProfile != null)
					{
						entsoProfile.IsOwlProfile = true;
						profile.ProcessSpecialStereotypeElements(entsoProfile, ge.NameOfOntology);
						entsoProfile.PopulateObjectReferences();
						generator = new OWL2Generator(entsoProfile, ge);
						generator.GenerateProfile();
					}

				}

				generator = new OWL2Generator(profile, ge);
				generator.GenerateProfile();
			}
		}
	}
}
