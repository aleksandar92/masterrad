using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Reporter.Interfaces
{
	public enum EntityTypesGeneratorReporter
	{
		Class,
		DatatypeProperty,
		ObjectProperty,
		NamedIndividual,
		NotGenerated,
		ClassCategory,
		BlankId
	}


	 public interface IGeneratorReporter:IReporter
	{
		int EntityCount { get; }
		int EntityCountByType(EntityTypesGeneratorReporter type);
		void AddtoEntityCountByType(EntityTypesGeneratorReporter type, int number);
		void RemoveFromEntityCountByType(EntityTypesGeneratorReporter type, int number);

	}
}
