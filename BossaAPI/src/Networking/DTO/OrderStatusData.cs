using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI.DTO
{
	/// <summary>
	/// Obiekt transportowy do przekazywania (między modułami biblioteki)
	/// informacji nt. aktualnego statusu konkretnego zlecenia na rachunku klienta.
	/// Używany jako "podobiekt" w OrderData (tam precyzujemy, o które zlecenie chodzi).
	/// </summary>
	public class OrderStatusData
	{
		public BosOrderStatus Status;
		public uint Quantity;
		public decimal NetValue;
		public decimal Commission;
	}
}
