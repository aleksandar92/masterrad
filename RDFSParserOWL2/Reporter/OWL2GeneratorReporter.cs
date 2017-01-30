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
				int numberOfBlankIDs;
				reporterMap.TryGetValue(EntityTypesGeneratorReporter.BlankId, out numberOfBlankIDs);

				foreach (int value in reporterMap.Values)
				{
					count += value;
				}
				count -= numberOfBlankIDs;

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
            sb.AppendLine("Total number per type:");
			foreach (EntityTypesGeneratorReporter type in Enum.GetValues(typeof(EntityTypesGeneratorReporter)))
			{
				switch (type)  
				{
					case EntityTypesGeneratorReporter.Class:
						sb.AppendLine(String.Format("\t\t\t-generated classes:{0}", EntityCountByType(type)));                        
						break;
					case EntityTypesGeneratorReporter.DatatypeProperty:
						sb.AppendLine(String.Format("\t\t\t-generated datatype properties:{0}", EntityCountByType(type)));
						break;
					case EntityTypesGeneratorReporter.ObjectProperty:
						sb.AppendLine(String.Format("\t\t\t-generated object properties:{0}", EntityCountByType(type)));
						break;
					case EntityTypesGeneratorReporter.NamedIndividual:
						sb.AppendLine(String.Format("\t\t\t-generated named individual:{0}", EntityCountByType(type)));
						break;
					case EntityTypesGeneratorReporter.NotGenerated:
						sb.AppendLine(String.Format("\t\t\t-entities not generated:{0}", EntityCountByType(type)));
						break;
					case EntityTypesGeneratorReporter.BlankId:
						sb.AppendLine(String.Format("Total number of entities with blank ids:{0}", EntityCountByType(type)));
						break;
				}
			}
			sb.AppendLine("-----------------------------------------");
			return sb.ToString();
		}


        public void AddToEntityCountByTypes(List<EntityTypesGeneratorReporter> types, int nubmer)
        {
            foreach (EntityTypesGeneratorReporter reptype in types)
            {
                AddtoEntityCountByType(reptype, nubmer);
            }
        }
    }
}
