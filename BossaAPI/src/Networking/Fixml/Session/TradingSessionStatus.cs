using System;
using System.Collections.Generic;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
	public enum TradingSessionStatus
	{
		Unknown = 0,            // nieznany
		Halted = 1,             // wstrzymany
		Open = 2,               // otwarcie
		Closed = 3,             // zamknięcie
		PreOpen = 4,            // przed otwarciem
		PreClose = 5,           // przed zamknięciem
		RequestRejected = 6     // odrzucenie żądania
	}

	internal static class TrdgSesStatusUtil
	{
		private static Dictionary<string, TradingSessionStatus> dict =
			new Dictionary<string, TradingSessionStatus> {
				// tu dopisać inne, nienumeryczne statusy zwracane przez NOLa
			};

		public static TradingSessionStatus Read(XmlElement xml, string name)
		{
			string str = FixmlUtil.ReadString(xml, name, true);
			if (str == null) return TradingSessionStatus.Unknown;
			uint number;
			if (uint.TryParse(str, out number))
				if (Enum.IsDefined(typeof(TradingSessionStatus), (TradingSessionStatus)number))
					return (TradingSessionStatus)number;
			if (!dict.ContainsKey(str))
				FixmlUtil.Error(xml, name, str, "- unknown TradingSessionStatus");
			return dict[str];
		}
	}
}
