using System;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
	public enum BizMsgRejectReason
	{
		Other = 0,
		UnknownId = 1,
		UnknownSecurity = 2,
		UnsupportedMessageType = 3,
		ApplicationNotAvailable = 4,
		RequiredFieldMissing = 5,
		NotAuthorized = 6,
		DestinationNotAvailable = 7,
		InvalidPriceIncrement = 18
	}

	internal static class BizRejRsnUtil
	{
		public static BizMsgRejectReason Read(XmlElement xml, string name)
		{
			int number = FixmlUtil.ReadInt(xml, name);
			if (!Enum.IsDefined(typeof(BizMsgRejectReason), (BizMsgRejectReason)number))
				FixmlUtil.Error(xml, name, number, "- unknown BizRejectReason");
			return (BizMsgRejectReason)number;
		}
	}
}
