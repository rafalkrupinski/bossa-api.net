using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI.DTO
{
	/// <summary>
	/// Obiekt transportowy do przekazywania (między modułami biblioteki)
	/// podstawowych danych nt. konkretnego rachunku klienta. 
	/// </summary>
	public class AccountData
	{
		public string Number;
		public Paper[] Papers;
		public decimal AvailableCash;
		public decimal AvailableFunds;
		public decimal? DepositValue;
		public decimal? DepositBlocked;
		public decimal? DepositDeficit;
		public decimal PortfolioValue;
	}
}
