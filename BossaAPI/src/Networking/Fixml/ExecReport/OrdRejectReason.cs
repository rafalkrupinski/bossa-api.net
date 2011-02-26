using System;
using System.Xml;

namespace pjank.BossaAPI.Fixml
{
	public enum OrdRejectReason
	{
		/*
		BrokerOption = 0,
		UnknownSymbol = 1,
		ExchangeClosed = 2,
		OrderExceedsLimit = 3,
		TooLateToEnter = 4,
		UnknownOrder = 5,
		DuplicateOrder = 6,
		DuplicateVerbOrder = 7,
		StaleOrder = 8,
		TradeAlongRequired = 9,
		InvalidInvestorID = 10,
		UnsupportedOrder = 11,
		SurveillenceOption = 12,
		IncorrectQuantity = 13,
		IncorrectAllocQty = 14,
		UnknownAccount = 15,
		PriceExceedsBand = 16,
		InvalidPriceInc = 18, 
		 */
		Other = 99
	}

	internal static class OrderRejRsnUtil
	{
		public static OrdRejectReason? Read(XmlElement xml, string name, bool optional)
		{
			int? number = FixmlUtil.ReadInt(xml, name, optional);
			if (number == null) return null;
			if (!Enum.IsDefined(typeof(OrdRejectReason), (OrdRejectReason)number))
				FixmlUtil.Error(xml, name, number, "- unknown OrderRejectReason");
			return (OrdRejectReason)number;
		}
	}
}
