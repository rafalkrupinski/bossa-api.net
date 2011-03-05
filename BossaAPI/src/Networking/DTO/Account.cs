using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI.DTO
{
	public class Account
	{
		public string Number;
		public Paper[] Papers;
		public decimal AvailableCash;
		public decimal AvailableFunds;
		public decimal? DepositDeficit;
		public decimal? DepositValue;
		public decimal PortfolioValue;
	}
}
