using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pjank.BossaAPI.DTO;
using System.Threading;

namespace pjank.BossaAPI
{
	public class BosInstruments : IEnumerable<BosInstrument>
	{
		/// <summary>
		/// Liczba dostępnych na liście instrumentów.
		/// </summary>
		public int Count
		{
			get { return list.Count; }
		}

		/// <summary>
		/// Dostęp do konkretnego instrumentu z listy - po jego indeksie (licząc od zera).
		/// </summary>
		public BosInstrument this[int index]
		{
			get { return list[index]; }
		}

		/// <summary>
		/// Dostęp do konkretnego instrumentu z listy - po jego symbolu
		/// (dokładnie, jak metoda "BySymbol()", ale przy krótszym zapisie).
		/// Jeśli takiego instrumentu nie ma jeszcze na liście, to go utworzy.
		/// </summary>
		public BosInstrument this[string symbol]
		{
			get { return FindBySymbol(symbol); }
		}

		#region Generic list stuff

		private List<BosInstrument> list = new List<BosInstrument>();

		// IEnumerable<BosAccount>
		public IEnumerator<BosInstrument> GetEnumerator()
		{
			for (int i = 0; i < Count; i++)
				yield return this[i];
		}

		// IEnumerable
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Subscription stuff

		private Timer subscriptionTimer;
		private BosInstrument[] newSubscription, oldSubscription;

		// wywoływane po każdej zmianie listy instrumentów
		internal void SubscriptionUpdate()
		{
			SubscriptionUpdate(false);
		}

		// j.w. z opcją wymuszenia ponownej subskrypcji (wywoływane po nawiązaniu połączenia)
		internal void SubscriptionUpdate(bool forceRefresh)
		{
			if (Bossa.client == null) return;
			if (subscriptionTimer == null)
				subscriptionTimer = new Timer(new TimerCallback(SubscriptionTimerProc));
			lock (this)
			{
				if (forceRefresh) oldSubscription = null;
				// kopia listy instrumentów (żeby nie blokować dłużej zmiennej "list")
				newSubscription = list.Where(i => i.UpdatesEnabled).ToArray();
				// restart timera wywołującego rzeczywisty update subskrypcji
				subscriptionTimer.Change(500, Timeout.Infinite);
			}
		}

		// wywołanie rzeczywistego update'a z drobnym opóźnieniem... 
		// (agregujemy kolejne częstsze zmiany do jednego odświeżenia subskrypcji)
		private void SubscriptionTimerProc(object state)
		{
			Instrument[] dtoInstruments;
			lock (this)
			{
				if (oldSubscription == null ||
					newSubscription.Except(oldSubscription).Any() ||
					oldSubscription.Except(newSubscription).Any())
				{
					oldSubscription = newSubscription;
					dtoInstruments = newSubscription.Select(i => i.Convert()).ToArray();
				}
				else return;
			}
			Bossa.client.MarketUpdatesSubscription(dtoInstruments);
		}

		#endregion

		#region Internal library stuff

		// wywoływane z Bossa.Reset() - usuwa z pamięci wszystkie instrumenty, historię transakcji itp.
		internal void Clear()
		{
			list.Clear();
			SubscriptionUpdate();
		}

		// odszukanie pasującego instrumentu na liście lub utworzenie nowego
		internal BosInstrument Find(string isin, string symbol)
		{
			if (isin == null && symbol == null)
				throw new ArgumentNullException("isin", "Both arguments can not be null!");

			// - jeśli podano tylko "symbol": szukamy po Symbolu...
			//		znalazł (być może kilka): zwracamy ostatni z nich
			//		nie znalazł żadnego: zwracamy nowy obiekt (ISIN=null)
			if (isin == null)
				return FindBySymbol(symbol);

			// - jeśli podano tylko "isin": szukamy po ISIN...
			//		znalazł kilka: błąd - to się nie powinno zdarzyć!
			//		znalazł jeden: zwracamy ten obiekt
			//		nie znalazł: zwracamy nowy obiekt (Symbol=null)
			if (symbol == null)
				return FindByISIN(isin);

			// - jeśli podano "isin" oraz "symbol": szukamy po Symbol + ISIN...
			//		znalazł kilka: błąd - to się nie powinno zdarzyć!
			//		znalazł jeden: zwracamy ten obiekt
			//		nie znalazł: 
			//			- szukamy po ISIN, Symbol=null... -> A
			//			- szukamy po Symbol, ISIN=null... -> B
			//			jeśli znalazł A, uzupełniamy w nim m.in. Symbol
			//			jeśli znalazł B, uzupełniamy w nim m.in. ISIN
			//			jeśli znalazł A i B, na liście zostawiamy tylko A
			//			jeśli nie znalazł żadnego, zwracamy nowy obiekt
			var instrument = list.SingleOrDefault(i => i.ISIN == isin && i.Symbol == symbol);
			if (instrument == null)
			{
				instrument = new BosInstrument(isin, symbol);
				var instrA = list.SingleOrDefault(i => i.ISIN == isin);
				var instrB = list.SingleOrDefault(i => i.Symbol == symbol && i.ISIN == null);
				if (instrA != null) instrA.Combine(instrB ?? instrument);
				if (instrB != null) instrB.Combine(instrA ?? instrument);
				if (instrA != null && instrB != null) list.Remove(instrB);
				if (instrA == null && instrB == null) list.Add(instrument);
				else instrument = instrA ?? instrB;
				SubscriptionUpdate();
			}
			return instrument;
		}

		#endregion

		/// <summary>
		/// Dostęp do konkretnego instrumentu z listy - po jego symbolu.
		/// Jeśli takiego instrumentu nie ma jeszcze na liście, to go utworzy.
		/// </summary>
		public BosInstrument FindBySymbol(string symbol)
		{
			var instrument = list.LastOrDefault(i => i.Symbol == symbol);
			if (instrument == null)
			{
				instrument = new BosInstrument(null, symbol);
				list.Add(instrument);
				SubscriptionUpdate();
			}
			return instrument;
		}

		/// <summary>
		/// Dostęp do konkretnego instrumentu z listy - po jego kodzie ISIN.
		/// Jeśli takiego instrumentu nie ma jeszcze na liście, to go utworzy.
		/// </summary>
		public BosInstrument FindByISIN(string isin)
		{
			var instrument = list.SingleOrDefault(i => i.ISIN == isin);
			if (instrument == null)
			{
				instrument = new BosInstrument(isin, null);
				list.Add(instrument);
				SubscriptionUpdate();
			}
			return instrument;
		}
	}
}
