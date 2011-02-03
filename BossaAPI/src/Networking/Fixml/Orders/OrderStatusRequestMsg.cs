using System;
using System.Xml;
using System.Text;

namespace pjank.BossaAPI.Fixml
{
    public class OrderStatusRequestMsg : FixmlMsg
    {
        private static uint nextId = 0;

        public string OrderStatusId;        // ID zapytania, definiowane przez nas
        public string ClientOrderId;        // ID zlecenia, nadane przez nas
        public string BrokerOrderId;        // główne id zlecenia nadane w DM
        public string BrokerOrderId2;       // numer zlecenia nadany w DM
        public string Account;              // numer rachunku
        public FixmlInstrument Instrument;  // nazwa papieru
        public OrderSide? Side;             // rodzaj zlecenia: kupno/sprzedaż

        public OrderStatusRequestMsg()
        {
            OrderStatusId = (++nextId).ToString();
        }

        protected override void PrepareXmlMessage(string name)
        {
            base.PrepareXmlMessage("OrdStatReq");
            xml.SetAttribute("StatReqID", OrderStatusId);
            if (ClientOrderId != null) xml.SetAttribute("ID", ClientOrderId);
            if (BrokerOrderId != null) xml.SetAttribute("OrdID", BrokerOrderId);
            if (BrokerOrderId2 != null) xml.SetAttribute("OrdID2", BrokerOrderId2);
            if (Account != null) xml.SetAttribute("Acct", Account);
            if (Instrument != null) Instrument.Write(xmlDoc, xml, "Instrmt");
            if (Side != null) xml.SetAttribute("Side", OrderSideUtil.Write(Side));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("[{0}:{1}:{2}] ", Xml.Name, OrderStatusId, ClientOrderId));
            sb.Append(string.Format(" {0,-4} {1}", Side, Instrument));
            return sb.ToString();
        }

    }
}
