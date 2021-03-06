﻿using RDFSParserOWL2.Reporter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Reporter
{
	class RDFXMLParserReporter:IParserReporter
	{

		private Dictionary<EntityTypesReporter, int> reporterMap;


		public RDFXMLParserReporter() 
		{
			reporterMap = new Dictionary<EntityTypesReporter, int>();

		}

		
		public int EntityCountByType(EntityTypesReporter type)
		{
			int count;
			reporterMap.TryGetValue(type,out count);
			return count;
		}


		public void AddtoEntityCountByType(EntityTypesReporter type, int number)
		{
			if(!reporterMap.ContainsKey(type)) 
			{
				reporterMap.Add(type,0);
			}

			reporterMap[type]+=number;
			//throw new NotImplementedException();
		}

		public void RemoveFromEntityCountByType(EntityTypesReporter type, int number)
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
			sb.AppendLine("----------RDFXML Parser report----------");
			sb.AppendLine(String.Format("Total number of  entities:{0}",EntityCount));
            sb.AppendLine("Total number per type:");
			foreach(EntityTypesReporter type in Enum.GetValues(typeof(EntityTypesReporter))) 
			{
				switch(type) 
				{
					case EntityTypesReporter.Class:
						sb.AppendLine(String.Format("\t\t\t-parsed classes:{0}", EntityCountByType(type)));
						break;
                    case EntityTypesReporter.ClassCategory:
                        sb.AppendLine(String.Format("\t\t\t-parsed class categories:{0}", EntityCountByType(type)));
                        break;
					case EntityTypesReporter.Property:
						sb.AppendLine(String.Format("\t\t\t-parsed properties:{0}", EntityCountByType(type)));
						break;
					case EntityTypesReporter.EnumMembers:
						sb.AppendLine(String.Format("\t\t\t-parsed enum members:{0}", EntityCountByType(type)));
						break;
					case EntityTypesReporter.Unknown:
                        sb.AppendLine(String.Format("\t\t\t-parsed unknown entities:{0}", EntityCountByType(type)));
						break;
					case EntityTypesReporter.BlankId:
						sb.AppendLine(String.Format("Total number of parsed  entities with blank ids:{0}", EntityCountByType(type)));
						break;
				}
			}
			sb.AppendLine("-----------------------------------------");
			return sb.ToString();
		}

		public int EntityCount
		{
			get 
			{
				int count = 0;
				int numberOfBlankIDs;
				reporterMap.TryGetValue(EntityTypesReporter.BlankId,out numberOfBlankIDs);

				foreach(int value in reporterMap.Values) 
				{
					count += value;
				}
				count -= numberOfBlankIDs;

				return count;
			}
		}


	}
}
