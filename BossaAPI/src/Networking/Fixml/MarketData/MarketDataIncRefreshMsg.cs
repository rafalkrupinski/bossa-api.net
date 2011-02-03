using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
    public class MarketDataIncRefreshMsg : FixmlMsg
    {
        public const string MsgName = "MktDataInc";

        public int RequestId { get; private set; }
        public MDEntry[] Entries { get; private set; }

        public MarketDataIncRefreshMsg(FixmlMsg m) : base(m) { }

        protected override void ParseXmlMessage(string name)
        {
            base.ParseXmlMessage(MsgName);
            RequestId = FixmlUtil.ReadInt(xml, "MDReqID");
            List<MDEntry> list = new List<MDEntry>();
            foreach (XmlElement inc in xml.GetElementsByTagName("Inc"))
                list.Add(new MDEntry(inc));
            Entries = list.ToArray();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("[{0}:{1}]", Xml.Name, RequestId));
            foreach (MDEntry entry in Entries) 
                sb.Append("\n " + entry.ToString());
            return sb.ToString();
        }

    }
}
