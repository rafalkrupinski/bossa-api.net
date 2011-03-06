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
		event Action<Account> AccountUpdateEvent;
	}
}
