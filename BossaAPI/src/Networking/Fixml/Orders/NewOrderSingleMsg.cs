using System;
using System.Xml;
using System.Text;

namespace pjank.BossaAPI.Fixml
{
    public class NewOrderSingleMsg : FixmlMsg
    {
        private static uint nextId = 0;

        public string ClientOrderId;        // ID zlecenia, definiowane przez nas
        public DateTime TradeDate;          // data sesji, na którą składamy zlecenie
        public string Account;              // numer rachunku
        public uint? MinimumQuantity;       // ilość minimalna
        public uint? DisplayQuantity;       // ilość ujawniona
        public FixmlInstrument Instrument;  // interesujący nas papier wartościowy
        public OrderSide? Side;             // rodzaj zlecenia: kupno/sprzedaż
        public DateTime CreateTime;         // data+godz utworzenia zlecenia przez klienta
        public uint Quantity;               // ilość papierów w zleceniu
        public OrderType Type;              // typ zlecenia (limit, PKC, stoploss itp.)
        public decimal? Price;              // cena zlecenia
        public decimal? StopPrice;          // limit aktywacji
        public string Currency;             // waluta
        public OrdTimeInForce TimeInForce;  // rodzaj ważności zlecenia
        public DateTime? ExpireDate;        // data ważności zlecenia
        public char? TriggerType;           // rodzaj triggera (4 - DDM+ po zmianie ceny)
        public char? TriggerAction;         // akcja triggera (1 - aktywacja zlecenia DDM+)
        public decimal? TriggerPrice;       // cena uaktywnienia triggera (DDM+)
        public char? TriggerPriceType;      // rodzaj ceny uakt. (2 = cena ost. transakcji)
        public char? DeferredPaymentType;   // OTP (odroczony termin płatności) = T/P

        public NewOrderSingleMsg()
        {
            ClientOrderId = (++nextId).ToString();
            TradeDate = DateTime.Now;
            CreateTime = DateTime.Now;
            Type = OrderType.Limit;
            Currency = "PLN";
            TimeInForce = OrdTimeInForce.Day;
        }

        protected override void PrepareXmlMessage(string name)
        {
            base.PrepareXmlMessage("Order");
            xml.SetAttribute("ID", ClientOrderId);
            if (TradeDate != null) xml.SetAttribute("TrdDt", FixmlUtil.WriteDate((DateTime)TradeDate));
            if (Account != null) xml.SetAttribute("Acct", Account);
            if (MinimumQuantity != null) xml.SetAttribute("MinQty", FixmlUtil.WriteDecimal(MinimumQuantity));
            if (DisplayQuantity != null)
                AddElement(xml, "DsplyInstr").SetAttribute("DisplayQty", FixmlUtil.WriteDecimal(DisplayQuantity));
            if (Instrument != null) Instrument.Write(xmlDoc, xml, "Instrmt");
            if (Side != null) xml.SetAttribute("Side", OrderSideUtil.Write(Side));
            xml.SetAttribute("TxnTm", FixmlUtil.WriteDateTime(CreateTime));
            AddElement(xml, "OrdQty").SetAttribute("Qty", Quantity.ToString());
            xml.SetAttribute("OrdTyp", OrderTypeUtil.Write(Type));
            if (Price != null) xml.SetAttribute("Px", FixmlUtil.WriteDecimal(Price));
            if (StopPrice != null) xml.SetAttribute("StopPx", FixmlUtil.WriteDecimal(StopPrice));
            if (Currency != null) xml.SetAttribute("Ccy", Currency);
            xml.SetAttribute("TmInForce", OrdTmInForceUtil.Write(TimeInForce));
            if (ExpireDate != null) xml.SetAttribute("ExpireDt", FixmlUtil.WriteDate((DateTime)ExpireDate));
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
            sb.Append(string.Format("[{0}:{1}] ", Xml.Name, ClientOrderId));
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
