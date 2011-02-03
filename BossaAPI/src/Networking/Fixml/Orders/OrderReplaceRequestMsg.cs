using System;
using System.Xml;
using System.Text;

namespace pjank.BossaAPI.Fixml
{
    public class OrderReplaceRequestMsg : FixmlMsg
    {
        private static uint nextId = 0;

        public DateTime? TradeDate;         // data sesji, na którą składamy zlecenie
        public string OrderReplaceId;       // ID anulaty, definiowane przez nas
        public string ClientOrderId;        // ID zlecenia do anulowania, nadane przez nas
        public string BrokerOrderId;        // główne id zlecenia nadane w DM
        public string BrokerOrderId2;       // numer zlecenia nadany w DM
        public string Account;              // numer rachunku
        public uint? MinimumQuantity;       // ilość minimalna
        public uint? DisplayQuantity;       // ilość ujawniona
        public FixmlInstrument Instrument;  // nazwa papieru
        public OrderSide? Side;             // rodzaj zlecenia: kupno/sprzedaż
        public DateTime? CreateTime;        // data+godz utworzenia zlecenia
        public uint? Quantity;              // ilość papierów w zleceniu
        public OrderType? Type;             // typ zlecenia (limit, PKC, stoploss itp.)
        public decimal? Price;              // cena zlecenia
        public decimal? StopPrice;          // limit aktywacji
        public string Currency;             // waluta
        public OrdTimeInForce? TimeInForce; // rodzaj ważności zlecenia
        public DateTime? ExpireDate;        // data ważności zlecenia
        public string Text;                 // dowolny tekst
        public char? TriggerType;           // rodzaj triggera (4 - DDM+ po zmianie ceny)
        public char? TriggerAction;         // akcja triggera (1 - aktywacja zlecenia DDM+)
        public decimal? TriggerPrice;       // cena uaktywnienia triggera (DDM+)
        public char? TriggerPriceType;      // rodzaj ceny uakt. (2 = cena ost. transakcji)
        public char? DeferredPaymentType;   // OTP (odroczony termin płatności) = T/P

        public OrderReplaceRequestMsg()
        {
            OrderReplaceId = (++nextId).ToString();
            TradeDate = DateTime.Now;
            CreateTime = DateTime.Now;
            Type = OrderType.Limit;
        }

        protected override void PrepareXmlMessage(string name)
        {
            base.PrepareXmlMessage("OrdCxlRplcReq");
            if (TradeDate != null) xml.SetAttribute("TrdDt", FixmlUtil.WriteDate((DateTime)TradeDate));
            xml.SetAttribute("ID", OrderReplaceId);
            if (ClientOrderId != null) xml.SetAttribute("OrigID", ClientOrderId);
            if (BrokerOrderId != null) xml.SetAttribute("OrdID", BrokerOrderId);
            if (BrokerOrderId2 != null) xml.SetAttribute("OrdID2", BrokerOrderId2);
            if (Account != null) xml.SetAttribute("Acct", Account);
            if (MinimumQuantity != null) xml.SetAttribute("MinQty", FixmlUtil.WriteDecimal(MinimumQuantity));
            if (DisplayQuantity != null)
                AddElement(xml, "DisplyInstr").SetAttribute("DisplayQty", FixmlUtil.WriteDecimal(DisplayQuantity));
            if (Instrument != null) Instrument.Write(xmlDoc, xml, "Instrmt");
            if (Side != null) xml.SetAttribute("Side", OrderSideUtil.Write(Side));
            if (CreateTime != null) xml.SetAttribute("TxnTm", FixmlUtil.WriteDateTime((DateTime)CreateTime));
            if (Quantity != null) 
                AddElement(xml, "OrdQty").SetAttribute("Qty", Quantity.ToString());
            if (Type != null) xml.SetAttribute("OrdTyp", OrderTypeUtil.Write((OrderType)Type));
            if (Price != null) xml.SetAttribute("Px", FixmlUtil.WriteDecimal(Price));
            if (StopPrice != null) xml.SetAttribute("StopPx", FixmlUtil.WriteDecimal(StopPrice));
            if (Currency != null) xml.SetAttribute("Ccy", Currency);
            if (TimeInForce != null) xml.SetAttribute("TmInForce", OrdTmInForceUtil.Write((OrdTimeInForce)TimeInForce));
            if (ExpireDate != null) xml.SetAttribute("ExpireDt", FixmlUtil.WriteDate((DateTime)ExpireDate));
            if (Text != null) xml.SetAttribute("Txt", Text);
            if (TriggerPrice != null)
            {
                XmlElement el = AddElement("TrgrInstr");
                el.SetAttribute("TrgrTyp", TriggerType.ToString());
                el.SetAttribute("TrgrActn", TriggerAction.ToString());
                el.SetAttribute("TrgrPx", FixmlUtil.WriteDecimal(TriggerPrice));
                el.SetAttribute("TrgrPxTyp", TriggerPriceType.ToString());
            }
            if (DeferredPaymentType != null)
                xml.SetAttribute("DefPayTyp", DeferredPaymentType.ToString());
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("[{0}:{1}:{2}] ", Xml.Name, OrderReplaceId, ClientOrderId));
            sb.Append(string.Format("  {0}  {1,-4} {2} x ", Instrument, Side, Quantity));
            if (Price != null) sb.Append(Price);
            else
                if ((Type == OrderType.PKC) || (Type == OrderType.StopLoss)) sb.Append("PKC");
                else sb.Append("PCR/PCRO");
            if (StopPrice != null) sb.Append(string.Format(" @{0}", StopPrice));
            return sb.ToString();
        }

    }
}
