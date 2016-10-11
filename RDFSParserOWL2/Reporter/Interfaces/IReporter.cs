using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Reporter.Interfaces
{
	/// <summary>
	/// Interface for generating reports
	/// </summary>
	public interface IReporter
	{
		/// <summary>
		/// Method for generating report
		/// </summary>
		/// <returns></returns>
		string GenerateReport();
	}
}
