using System;
using System.Linq;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
	public enum MDEntryType
	{
		Buy = '0',      // oferta kupna
		Sell = '1',     // oferta sprzedaży
		Trade = '2',    // kurs/wolumen/czas transakcji
		Vol = 'B',      // dotychczasowy obrót sesji
		Lop = 'C',      // liczba otwartych pozycji
		Index = '3',    // wartość/obrót indeksu
		Open = '4',     // kurs/obrót na otwarciu
		Close = '5',    // kurs/obrót na zamknięciu
		High = '7',     // kurs maksymalny
		Low = '8',      // kurs minimalny
		Ref = 'r'       // kurs odniesienia
	}

	public static class MDEntryTypes
	{
		public static readonly MDEntryType[] All = {
            MDEntryType.Buy, MDEntryType.Sell, 
            MDEntryType.Trade, MDEntryType.Vol, MDEntryType.Lop, 
            MDEntryType.Index,
            MDEntryType.Open, MDEntryType.Close, 
            MDEntryType.High, MDEntryType.Low, 
            MDEntryType.Ref
        };
		public static readonly MDEntryType[] BasicBook = {
            MDEntryType.Buy, MDEntryType.Sell
        };
		public static readonly MDEntryType[] BasicTrade = {
            MDEntryType.Trade, MDEntryType.Vol, MDEntryType.Lop
        };
		public static readonly MDEntryType[] OpenClose = {
            MDEntryType.Open, MDEntryType.Close
        };

		public static readonly MDEntryType[] HasPrice = All.
			Except(new[]{ 
                MDEntryType.Vol, MDEntryType.Lop 
            }).ToArray();
		public static readonly MDEntryType[] HasCurrency = HasPrice.
			Except(new[]{ 
                MDEntryType.Index 
            }).ToArray();
		public static readonly MDEntryType[] HasSize = {
            MDEntryType.Buy, MDEntryType.Sell, 
            MDEntryType.Trade, MDEntryType.Vol, MDEntryType.Lop, 
        };
		public static readonly MDEntryType[] HasTurnover = {
            MDEntryType.Vol, MDEntryType.Index,
            MDEntryType.Open, MDEntryType.Close
        };

		public static bool In(this MDEntryType type, MDEntryType[] array)
		{
			return array.Contains(type);
		}

		public static bool Compare(this MDEntryType[] array1, MDEntryType[] array2)
		{
			if (array1 == array2) return true;
			if (array1 == null) return false;
			if (array2 == null) return false;
			if (array1.Length != array2.Length) return false;
			foreach (MDEntryType t in array1)
				if (!array2.Contains(t)) return false;
			return true;
		}

		public static string ToString(this MDEntryType[] array)
		{
			if (array.Compare(MDEntryTypes.All)) return "All";
			var a = array.Select(t => t.ToString()).ToArray();
			return string.Join("+", a);
		}

	}

	internal static class MDEntryTypeUtil
	{
		public static MDEntryType Read(XmlElement xml, string name)
		{
			char ch = FixmlUtil.ReadChar(xml, name);
			if (!Enum.IsDefined(typeof(MDEntryType), (MDEntryType)ch))
				FixmlUtil.Error(xml, name, ch, "- unknown MDEntryType");
			return (MDEntryType)ch;
		}
	}
}
