using System;
using System.ComponentModel;
using pjank.BossaAPI.DTO;
using System.Collections.Generic;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Podstawowa klasa biblioteki oferująca łatwy dostęp do większości funkcji API, a dokładniej:
	/// wewnętrzna implementacja tego, co zwykle wywołujemy za pośrednictwem statycznej klasy "Bossa".
	/// 
	/// Upubliczniona raczej tylko dla tych, co mają uprzedzenia do klas statycznych i singletonów ;)
	/// A na poważnie: jeśli nie wiesz o co chodzi - śmiało korzystaj z klasy "Bossa".
	/// Jeśli jednak coś stoi na przeszkodzie (potrzebujesz np. zaimplementować testy jednostkowe
	/// lub integracyjne) - możesz z powodzeniem zastąpić klasę "Bossa" własną instancją "BossaApi".
	/// </summary>
	public class BossaApi : IBossaApi
	{
		internal IBosClient connection;


		public bool Connected { get { return (connection != null); } }

		public IBosClient Connection { get { return connection; } }

		public BosAccounts Accounts { get; private set; }

		public BosInstruments Instruments { get; private set; }

		public event EventHandler OnUpdate;


		public BossaApi()
		{
			Accounts = new BosAccounts(this);
			Instruments = new BosInstruments(this);
		}


		public void Connect(IBosClient client)
		{
			if (connection != null)
				throw new InvalidOperationException("Already connected!");

			connection = client;
			connection.AccountUpdateEvent += AccountUpdateHandler;
			connection.MarketUpdateEvent += MarketUpdateHandler;
			connection.OrderUpdateEvent += OrderUpdateHandler;

			Instruments.SubscriptionUpdate(true);
		}

		public void Disconnect()
		{
			connection.Dispose();
			connection = null;
		}

		public void Clear()
		{
			Accounts.Clear();
			Instruments.Clear();
		}


		#region Internal library stuff...

		// aktualizacja stanu jednego z rachunków
		private void AccountUpdateHandler(AccountData acccountData)
		{
			var account = Accounts[acccountData.Number];
			account.Update(acccountData);
			InvokeUpdate(account);
		}

		// aktualizacja informacji o bieżących zleceniach
		private void OrderUpdateHandler(OrderData orderData)
		{
			var account = Accounts[orderData.AccountNumber];
			account.Orders.Update(orderData);
			InvokeUpdate(account);
		}

		// aktualizacja informacji o bieżących notowaniach
		private void MarketUpdateHandler(MarketData[] marketData)
		{
			var updatedInstruments = new HashSet<BosInstrument>();
			foreach (var data in marketData)
			{
				var instrument = BosInstrument.Create(data.Instrument);
				instrument.Update(data);
				updatedInstruments.Add(instrument);
			}
			foreach (var instrument in updatedInstruments)
			{
				InvokeUpdate(instrument);
			}
		}

		// wywołanie handlerów podpiętych pod zdarzenie OnUpdate
		private void InvokeUpdate(object updatedObject)
		{
			if (OnUpdate != null)
			{
				var args = new[] { updatedObject, null };
				foreach (var handler in OnUpdate.GetInvocationList())
				{
					ISynchronizeInvoke invokeTarget = handler.Target as ISynchronizeInvoke;
					if (invokeTarget != null)
						invokeTarget.BeginInvoke(handler, args);
					else
						handler.DynamicInvoke(args);
				}
			}
		}

		#endregion
	}
}
