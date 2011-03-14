using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI.DTO
{
	/// <summary>
	/// Obiekt transportowy do przekazywania (między modułami biblioteki)
	/// ogólnych informacji nt. jednego ze zleceń na rachunku klienta. 
	/// Dalsze dane - zależnie od potrzeb - przekazujemy w kolejnych "podobiektach".
	/// </summary>
	public class OrderData
	{
		public string AccountNumber;
		public string BrokerId;
		public string ClientId;
		public OrderMainData MainData;
		public OrderStatusData StatusReport;
		public OrderTradeData TradeReport;
	}
}
