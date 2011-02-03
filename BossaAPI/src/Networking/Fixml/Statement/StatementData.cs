using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
    public class StatementData
    {
        public string Account { get; private set; }
        public Dictionary<FixmlInstrument, int> Positions { get; private set; }
        public Dictionary<StatementFundType, decimal> Funds { get; private set; }

        public StatementData(XmlElement xml)
        {
            Account = FixmlUtil.ReadString(xml, "Acct");
            XmlNodeList fundsXml = xml.GetElementsByTagName("Fund");
            XmlNodeList positionsXml = xml.GetElementsByTagName("Position");
            Funds = new Dictionary<StatementFundType, decimal>(fundsXml.Count);
            Positions = new Dictionary<FixmlInstrument, int>(positionsXml.Count);
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
                int value = FixmlUtil.ReadInt(elem, "Acc110");
                Positions.Add(key, value);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("- " + Account + " -");
            foreach (var p in Positions)
                sb.Append(string.Format("\n  {1} x {0}", p.Key, p.Value));
            foreach (var f in Funds)
                sb.Append(string.Format("\n {0,-20} {1,8}", f.Key, f.Value));
            return sb.ToString();
        }
    }
}
