using RDFSParserOWL2.Reporter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Reporter
{
	public class OWL2GeneratorReporter:IGeneratorReporter
	{

		private Dictionary<EntityTypesGeneratorReporter, int> reporterMap;

		public OWL2GeneratorReporter() 
		{
			reporterMap = new Dictionary<EntityTypesGeneratorReporter, int>();

		}


		public int EntityCount
		{
			get
			{
				int count = 0;

				foreach (int value in reporterMap.Values)
				{
					count += value;
				}

				return count;
			}
		}

		public int EntityCountByType(EntityTypesGeneratorReporter type)
		{
			int count;
			reporterMap.TryGetValue(type, out count);
			return count;
		}

		public void AddtoEntityCountByType(EntityTypesGeneratorReporter type, int number)
		{
			if (!reporterMap.ContainsKey(type))
			{
				reporterMap.Add(type, 0);
			}

			reporterMap[type] += number;
		}

		public void RemoveFromEntityCountByType(EntityTypesGeneratorReporter type, int number)
		{
			if (!reporterMap.ContainsKey(type))
			{
				reporterMap.Add(type, 0);
			}
			reporterMap[type] -= number;
		}

		public string GenerateReport()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("----------OWL2 Generator report----------");
			sb.AppendLine(String.Format("Total number of  entities:{0}", EntityCount));
			foreach (EntityTypesGeneratorReporter type in Enum.GetValues(typeof(EntityTypesGeneratorReporter)))
			{
				switch (type)
				{
					case EntityTypesGeneratorReporter.Class:
						sb.AppendLine(String.Format("Total number of generated classes:{0}", EntityCountByType(type)));
						break;
					case EntityTypesGeneratorReporter.DatatypeProperty:
						sb.AppendLine(String.Format("Total number of generated datatype properties:{0}", EntityCountByType(type)));
						break;
					case EntityTypesGeneratorReporter.ObjectProperty:
						sb.AppendLine(String.Format("Total number of generated object properties:{0}", EntityCountByType(type)));
						break;
					case EntityTypesGeneratorReporter.NamedIndividual:
						sb.AppendLine(String.Format("Total number of generated named individual:{0}", EntityCountByType(type)));
						break;
					case EntityTypesGeneratorReporter.NotGenerated:
						sb.AppendLine(String.Format("Total number of entities not generated:{0}", EntityCountByType(type)));
						break;
				}
			}
			sb.AppendLine("-----------------------------------------");
			return sb.ToString();
		}
	}
}
