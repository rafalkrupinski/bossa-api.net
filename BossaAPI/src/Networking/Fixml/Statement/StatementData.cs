using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
	public class StatementData
	{
		public class PosQuantity
		{
			public int Acc110 { get; private set; }
			public int Acc120 { get; private set; }
			internal PosQuantity(int acc110, int acc120)
			{
				Acc110 = acc110;
				Acc120 = acc120;
			}
		}

		public string AccountNumber { get; private set; }
		public Dictionary<FixmlInstrument, PosQuantity> Positions { get; private set; }
		public Dictionary<StatementFundType, decimal> Funds { get; private set; }

		public StatementData(XmlElement xml)
		{
			AccountNumber = FixmlUtil.ReadString(xml, "Acct");
			XmlNodeList fundsXml = xml.GetElementsByTagName("Fund");
			XmlNodeList positionsXml = xml.GetElementsByTagName("Position");
			Funds = new Dictionary<StatementFundType, decimal>(fundsXml.Count);
			Positions = new Dictionary<FixmlInstrument, PosQuantity>(positionsXml.Count);
			foreach (XmlElement elem in fundsXml)
			{
				StatementFundType key;
				try { key = StatementFundUtil.Read(elem, "name"); }
				catch (FixmlException e) { e.PrintWarning(); continue; }
				decimal value = FixmlUtil.ReadDecimal(elem, "value");
				Funds.Add(key, value);
			}
			foreach (XmlElement elem in positionsXml)
			{
				FixmlInstrument key = FixmlInstrument.FindById(FixmlUtil.ReadString(elem, "Isin"));
				int acc110 = FixmlUtil.ReadInt(elem, "Acc110");
				int acc120 = FixmlUtil.ReadInt(elem, "Acc120", true) ?? 0;
				Positions.Add(key, new PosQuantity(acc110, acc120));
			}
			// nie zaszkodzi się upewnić - czy to, co NOL3 podesłał, stanowi jakąś integralną całość...
			// (i czy ja w ogóle słusznie zakładam, jakie powinny być zależności między tymi wartościami)
			// - dla rachunku akcyjnego:
			if (CheckFundsSum(StatementFundType.CashReceivables, StatementFundType.Cash, StatementFundType.Receivables))
				CheckFundsSum(StatementFundType.PortfolioValue, StatementFundType.CashReceivables, StatementFundType.SecuritiesValue);
			// - dla rachunku kontraktowego:
			if (CheckFundsSum(StatementFundType.Deposit, StatementFundType.DepositBlocked, StatementFundType.DepositFree))
				CheckFundsSum(StatementFundType.PortfolioValue, StatementFundType.Cash, StatementFundType.CashBlocked, StatementFundType.Deposit);
		}

		// sprawdza, czy podane "Fundy" w ogóle istnieją... i jeśli tak - czy pierwszy jest równy sumie pozostałych
		private bool CheckFundsSum(params StatementFundType[] types)
		{
			if (types.All(t => Funds.ContainsKey(t)))
			{
				var values = types.Select(t => Funds[t]);
				if (values.First() == values.Skip(1).Sum()) return true;
				MyUtil.PrintWarning(string.Format("Unexpected '{0}' value!  ({1} != {2})", 
					types.First(), values.First(),
					string.Join(" + ", values.Skip(1).Select(v => v.ToString()).ToArray())));
			}
			return false;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("- " + AccountNumber + " -");
			foreach (var p in Positions)
				sb.Append(string.Format("\n  {1} x {0}", p.Key, p.Value.Acc110 + p.Value.Acc120));
			foreach (var f in Funds.OrderBy(t => t.Key))
				sb.Append(string.Format("\n {0,-20} {1,8}", f.Key, f.Value));
			return sb.ToString();
		}
	}
}
