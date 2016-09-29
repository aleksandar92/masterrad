using RDFSParserOWL2.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Generator.Helper
{
    public static class OWL2Namespace
    {
        public static string rdfAbout = "about";
        public static string rdfRDF = "RDF";
        public static string rdfResource = "resource";
        public static string rdfPrefix = "rdf";
        public static string rdfDatatype = "datatype";

        public static string rdfsPrefix = "rdfs";
        public static string rdfsDomain = "domain";
        public static string rdfsRange = "range";
        public static string rdfsComment = "comment";
        public static string rdfsLabel = "label";
        public static string rdfsSubclasOf = "subClassOf";

        public static string owlPrefix = "owl";
        public static string DatatypeProperty = "DatatypeProperty";
        public static string owlDatatypeProperty = owlPrefix + StringManipulationManager.SeparatorColon + DatatypeProperty;
        public static string ObjectProperty = "ObjectProperty";
        public static string owlObjectProperty = owlPrefix + StringManipulationManager.SeparatorColon + ObjectProperty;
        public static string Class = "Class";
        public static string owlClass = owlPrefix + StringManipulationManager.SeparatorColon + Class;
        public static string owlRestriction = "Restriction";
        public static string OnProperty = "onProperty";
        public static string owlOnProperty = owlPrefix + StringManipulationManager.SeparatorColon + OnProperty;
        public static string AllValuesFrom = "allValuesFrom";
        public static string owlAllValuesFrom=owlPrefix + StringManipulationManager.SeparatorColon + AllValuesFrom;

        public static string MaxQualified = "maxQualifiedCardinality";
        public static string owlMaxQualified = owlPrefix + StringManipulationManager.SeparatorColon + MaxQualified;
        public static string OnClass = "onClass";
        public static string owlOnClass =owlPrefix + StringManipulationManager.SeparatorColon + OnClass ;
        public static string MinQualified = "minQualifiedCardinality";
        public static string owlMinQualified = owlPrefix + StringManipulationManager.SeparatorColon + MinQualified;
        public static string Qualified = "qualifiedCardinality";
		public static string owlQualified = owlPrefix + StringManipulationManager.SeparatorColon + Qualified;
        public static string OnDataRange = "onDataRange";
        public static string owlOnDataRange = owlPrefix + StringManipulationManager.SeparatorColon + OnDataRange;
        public static string owlOntology = "Ontology";

        public static string Import = "imports";

        public static string owlImport =owlPrefix + StringManipulationManager.SeparatorColon + Import;

		public static string multiplcityNs = "http://iec.ch/TC57/1999/rdf-schema-extensions-19990926#M:";
      

        public static string nonNegativeInteger = "nonNegativeInteger";

		public static string Entsoe = "Entsoe";
		public static string EntsoeOwl = "entsoe.owl";

        public static string Description = "Description";

        public static string EqProperty = "equivalentProperty";

        public static string owlEqProperty = owlPrefix + StringManipulationManager.SeparatorColon + EqProperty;
		public static string xmlLang = "xml:lang";
		public static string xmlPrefix="xml";
		public static string Lang = "lang";
		public static string EnLang = "en";
		public static string EqClass = "equivalentClass";
		public static string OneOf = "oneOf";
		public static string ParseType = "parseType";
		public static string Collection = "Collection";
		public static string NamedIndividual = "NamedIndividual";
		public static string IsDefinedBy = "isDefinedBy";		
 

        #region helpers

        public static bool IsElementOWL(string qname)
        {
            string pfx = StringManipulationManager.TrimAfterLastSeparator(qname, StringManipulationManager.SeparatorColon);
            string sufx = StringManipulationManager.ExtractAllAfterSeparator(qname, StringManipulationManager.SeparatorColon);
            if (pfx != null && pfx.Equals(OWL2Namespace.owlPrefix))
            {
                if (sufx != null && (sufx.Equals(OWL2Namespace.Class) || sufx.Equals(OWL2Namespace.DatatypeProperty) || sufx.Equals(OWL2Namespace.ObjectProperty )|| sufx.Equals(OWL2Namespace.owlOntology)))
                    return true;

            }

            return false;
        }




        #endregion
    }
}
