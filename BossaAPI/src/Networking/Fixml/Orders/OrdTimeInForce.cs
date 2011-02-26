using System;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
	public enum OrdTimeInForce
	{
		Day = '0',      // ważne na dzień
		WiA = '3',      // wykonaj i anuluj (ważne do 1. wykonania)
		WuA = '4',      // wykonaj lub anuluj (w całości albo wcale)
		Date = '6',     // ważne do dnia... (ExpireDate)
		Opening = '2',  // PCRO na otwarciu
		Closing = '7'   // PCRO na zamknięciu
	}

	internal static class OrdTmInForceUtil
	{
		public static OrdTimeInForce? Read(XmlElement xml, string name)
		{
			char? ch = FixmlUtil.ReadChar(xml, name, true);
			if (ch == null) return null;
			if (!Enum.IsDefined(typeof(OrdTimeInForce), (OrdTimeInForce)ch))
				FixmlUtil.Error(xml, name, ch, "- unknown OrdTimeInForce");
			return (OrdTimeInForce)ch;
		}

		public static string Write(OrdTimeInForce value)
		{
			return ((char)value).ToString();
		}
	}
}
