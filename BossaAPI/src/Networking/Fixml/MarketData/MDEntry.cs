using System;
using System.Linq;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
	public class MDEntry
	{
		public MDUpdateAction UpdateAction { get; private set; }
		public MDEntryType EntryType { get; private set; }
		public FixmlInstrument Instrument { get; private set; }
		// Bid, Offer - cena oferty
		// Trade - cena transakcji
		// Index - wartość indeksu
		// Open/Close/High/Low/Ref - kurs open/close/max/min/odniesienia
		public decimal? Price { get; private set; }   // Price ?? PriceStr
		public string PriceStr { get; private set; }  // np.: PKC, PCR, PCRO
		// Bid, Offer - pozycja w arkuszu
		public uint? Level { get; private set; }
		// Bid, Offer, Trade, Open/Close/High/Low/Ref - waluta
		public string Currency { get; private set; }
		// Bid, Offer - wolumen oferty
		// Trade - wolumen transakcji
		// Volume - wolumen obrotu
		// OpenInt - LOP
		public uint? Size { get; private set; }
		// Bid, Offer - liczba ofert
		public uint? Orders { get; private set; }
		// Trade - data i czas transakcji
		public DateTime DateTime { get; private set; }
		// Volume - wartość obrotu
		// Index - wartość obrotu na indeksie
		// Opening/Closing - wartość obrotu na otwarciu/zamknięciu
		public decimal? Turnover { get; private set; }

		internal MDEntry(XmlElement inc)
		{
			//TODO: na razie nie jest przesyłane??
			//UpdateAction = MDUpdateActionUtil.Read(inc, "UpdtAct");
			EntryType = MDEntryTypeUtil.Read(inc, "Typ");
			Instrument = FixmlInstrument.Read(inc, "Instrmt");
			if (EntryType.In(MDEntryTypes.HasPrice))
			{
				PriceStr = FixmlUtil.ReadString(inc, "Px");
				if (!(new[] { "PKC", "PCR", "PCRO" }).Contains(PriceStr))
					Price = FixmlUtil.ReadDecimal(inc, "Px");
			}
			if (EntryType.In(MDEntryTypes.HasCurrency))
				Currency = FixmlUtil.ReadString(inc, "CCy");
			if (EntryType.In(MDEntryTypes.HasSize))
				try { Size = FixmlUtil.ReadUInt(inc, "Sz"); }
				catch (FixmlException e) { e.PrintWarning(); }
			if (EntryType.In(MDEntryTypes.HasTurnover))
				Turnover = FixmlUtil.ReadDecimal(inc, "Tov", EntryType.In(MDEntryTypes.OpenClose));
			if (EntryType.In(MDEntryTypes.BasicBook))
			{
				Level = FixmlUtil.ReadUInt(inc, "MDPxLvl");
				Orders = FixmlUtil.ReadUInt(inc, "NumOfOrds");
			}
			if (EntryType == MDEntryType.Trade)
				DateTime = FixmlUtil.ReadDateTime(inc, "Dt", "Tm");
		}

		public override string ToString()
		{
			switch (EntryType)
			{
				case MDEntryType.Buy:
					return string.Format("{0} K {1,7} x {2} ({3})", Instrument, PriceStr, Size, Orders);
				case MDEntryType.Sell:
					return string.Format("{0} S {1,7} x {2} ({3})", Instrument, PriceStr, Size, Orders);
				case MDEntryType.Trade:
					return string.Format("{0} - {1,7} x {2}  {3}", Instrument, Price, Size, DateTime);
				case MDEntryType.Vol:
					return string.Format("{0} - Vol = {1,6} [{2:0.0}mln]", Instrument, Size, Turnover / 1000000);
				case MDEntryType.Lop:
					return string.Format("{0} - Lop = {1,6}", Instrument, Size);
				case MDEntryType.Index:
					return string.Format("{0} - {1} [{2:0}mln]", Instrument, Price, Turnover / 1000000);
				case MDEntryType.Open:
				case MDEntryType.Close:
					return string.Format("{0}: {1,-5} = {2,7} {3,10:[0.0mln]}", Instrument, EntryType, Price, Turnover / 1000000);
				case MDEntryType.High:
				case MDEntryType.Low:
				case MDEntryType.Ref:
					return string.Format("{0}: {1,-5} = {2,7}", Instrument, EntryType, Price);
				default:
					return EntryType.ToString();
			}
		}
	}
}
