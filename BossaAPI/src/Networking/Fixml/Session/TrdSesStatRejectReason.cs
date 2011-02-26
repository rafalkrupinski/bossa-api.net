using System;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
	public enum TrdSesStatRejectReason
	{
		None = 0,
		InvalidSessionId = 1,
		Other = 99
	}

	internal static class TrdSesStatRejRsnUtil
	{
		public static TrdSesStatRejectReason Read(XmlElement xml, string name)
		{
			int? number = FixmlUtil.ReadInt(xml, name, true);
			if (number == null) return TrdSesStatRejectReason.None;
			if (!Enum.IsDefined(typeof(TrdSesStatRejectReason), (TrdSesStatRejectReason)number))
				FixmlUtil.Error(xml, name, number, "- unknown TrdSesStatRejectReason");
			return (TrdSesStatRejectReason)number;
		}
	}
}
