using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pjank.BossaAPI.DTO;

namespace pjank.BossaAPI
{
	public interface IBosClient : IDisposable
	{
		/// <summary>
		/// Zdarzenie informujące o aktualizacji danych rachunku.
		/// </summary>
		event Action<AccountData> AccountUpdateEvent;
		/// <summary>
		/// Zdarzenie informujące o aktualizacji informacji o zleceniu na rachunku.
		/// </summary>
		event Action<OrderData> OrderUpdateEvent;
	}
}
