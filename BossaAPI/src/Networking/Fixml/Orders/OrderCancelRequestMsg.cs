using System;
using System.Xml;
using System.Text;

namespace pjank.BossaAPI.Fixml
{
	public class OrderCancelRequestMsg : FixmlMsg
	{
		private static uint nextId = 0;

		public string OrderCancelId;        // ID anulaty, definiowane przez nas
		public string ClientOrderId;        // ID zlecenia do anulowania, nadane przez nas
		public string BrokerOrderId;        // główne id zlecenia nadane w DM
		public string BrokerOrderId2;       // numer zlecenia nadany w DM
		public string Account;              // numer rachunku
		public FixmlInstrument Instrument;  // nazwa papieru
		public OrderSide? Side;             // rodzaj zlecenia: kupno/sprzedaż
		public DateTime? CreateTime;        // data+godz utworzenia zlecenia
		public uint? Quantity;              // ilość papierów w zleceniu
		public string Text;                 // dowolny tekst

		public OrderCancelRequestMsg()
		{
			OrderCancelId = (++nextId).ToString();
			CreateTime = DateTime.Now;
		}

		protected override void PrepareXmlMessage(string name)
		{
			base.PrepareXmlMessage("OrdCxlReq");
			xml.SetAttribute("ID", OrderCancelId);
			if (ClientOrderId != null) xml.SetAttribute("OrigID", ClientOrderId);
			if (BrokerOrderId != null) xml.SetAttribute("OrdID", BrokerOrderId);
			if (BrokerOrderId2 != null) xml.SetAttribute("OrdID2", BrokerOrderId2);
			if (Account != null) xml.SetAttribute("Acct", Account);
			if (Instrument != null) Instrument.Write(xmlDoc, xml, "Instrmt");
			if (Side != null) xml.SetAttribute("Side", OrderSideUtil.Write(Side));
			if (CreateTime != null) xml.SetAttribute("TxnTm", FixmlUtil.WriteDateTime((DateTime)CreateTime));
			if (Quantity != null) AddElement(xml, "OrdQty").SetAttribute("Qty", Quantity.ToString());
			if (Text != null) xml.SetAttribute("Txt", Text);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(string.Format("[{0}:{1}:{2}] ", Xml.Name, OrderCancelId, ClientOrderId));
			sb.Append(string.Format("  {0}", Instrument));
			if (Side != null) sb.Append(string.Format("  {0,-4}", Side));
			if (Quantity != null) sb.Append(string.Format(" x {0} ", Quantity));
			return sb.ToString();
		}

	}
}
