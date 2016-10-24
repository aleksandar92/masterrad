using RDFSParserOWL2.Generator.Helper;
using RDFSParserOWL2.Manager;
using RDFSParserOWL2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Common
{
	public enum CardinaltyType
	{
		UNKNOWN = 0, ONETOMANY, ZEROTOMANY, ZEROTOONE, ONE, MANYTOMANY
	}

	public enum CardinalityNumber { MAX, MIN }

	public class CardinalityHelper
	{



		
		private static Dictionary<CardinalityNumber,string> PopulateCardinalityMap(List<string> cNumb) 
		{
			Dictionary<CardinalityNumber, string> card = new Dictionary<CardinalityNumber, string>();

			if (cNumb!=null && cNumb.Count <= 2 && cNumb.Count>0) 
			{
				card.Add(CardinalityNumber.MIN,cNumb[0]);
				if(cNumb.Count==2) 
				{
					card.Add(CardinalityNumber.MAX, cNumb[1]);
				} 
			}
			return card;
		}

		public static CardinaltyType ProcessMultiplicity(string multiplicityAsString,out Dictionary<CardinalityNumber,string> crdMap)
		{
			List<string> carNumbers = StringManipulationManager.ParseMultiplicity(multiplicityAsString);
			crdMap = PopulateCardinalityMap(carNumbers);
			CardinaltyType result=CardinaltyType.UNKNOWN;
			if (multiplicityAsString != null && multiplicityAsString != null)
			{
				string min,max;
				crdMap.TryGetValue(CardinalityNumber.MIN,out min);
				crdMap.TryGetValue(CardinalityNumber.MAX, out max);

				if (!string.IsNullOrEmpty(min) && string.IsNullOrEmpty(max))
				{
					result = CardinaltyType.ONE;
				}
				else 
				{
					int numberMin,numberMax;
					bool isNumericMin=int.TryParse(min,out numberMin);
					bool isNumericMax=int.TryParse(max,out numberMax);

					if (!isNumericMin)
					{
						result = CardinaltyType.MANYTOMANY;
					}
					else 
					{
						if (numberMin == 0)
						{
							if (isNumericMax)
							{
								result = CardinaltyType.ZEROTOONE;
							}
							else
							{
								result = CardinaltyType.ZEROTOMANY;
							}
						}
						else 
						{
							 result = CardinaltyType.ONETOMANY;
						}
					}		
				}
			}
			return result;
		}

        public static string ProcessOwlMultiplicityToString(string card)
        {
            string result=string.Empty;
            if (card != null && card.Equals(OWL2Namespace.AllValuesFrom))
                result = OWL2Namespace.multiplcityNs+"0..n";
            else if (card != null && card.Equals(OWL2Namespace.MinQualified))
				result = OWL2Namespace.multiplcityNs + "1..n";
            else if (card != null && card.Equals(OWL2Namespace.MaxQualified))
				result = OWL2Namespace.multiplcityNs + "0..1";
            else if (card != null && card.Equals(OWL2Namespace.Qualified))
				result = OWL2Namespace.multiplcityNs + "1..1";
            return result;
        }


	}
}
