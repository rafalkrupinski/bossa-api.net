using System;
using System.Text;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
    public class MarketDataRequestMsg : FixmlMsg
    {

        private static uint nextId = 0;

        public uint Id;
        public SubscriptionRequestType Type;
        public byte MarketDepth;
        public MDEntryType[] EntryTypes;
        public FixmlInstrument[] Instruments;

        public MarketDataRequestMsg()
        {
            Id = nextId++;
        }

        protected override void PrepareXmlMessage(string name)
        {
            base.PrepareXmlMessage("MktDataReq");
            xml.SetAttribute("ReqID", Id.ToString());
            xml.SetAttribute("SubReqTyp", Type.ToString("d"));
            if (Type == SubscriptionRequestType.CancelSubscription) return;
            xml.SetAttribute("MktDepth", MarketDepth.ToString());
            if (EntryTypes != null)
            {
                foreach (MDEntryType type in EntryTypes)
                    AddElement("req").SetAttribute("Typ", ((char)type).ToString());
            }
            if ((Instruments != null) && (Instruments.Length > 0))
            {
                XmlElement elem = AddElement("InstReq");
                foreach (FixmlInstrument instr in Instruments)
                    instr.Write(xmlDoc, elem, "Instrmt");
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("[{0}:{1}] {2}", Xml.Name, Id, Type));
            if (EntryTypes != null)
                sb.Append(string.Format(" (EntryTypes: {0})", EntryTypes.ToString()));
            if (Instruments != null)
                foreach (FixmlInstrument instr in Instruments)
                    sb.Append("\n " + instr);
            return sb.ToString();
        }

    }
}
