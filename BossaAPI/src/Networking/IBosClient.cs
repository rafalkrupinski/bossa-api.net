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
		/// <summary>
		/// Zdarzenie informujące o aktualizacji stanu notowań rynkowych.
		/// </summary>
		event Action<MarketData[]> MarketUpdateEvent;

		/// <summary>
		/// Utworzenie nowego zlecenia.
		/// </summary>
		/// <param name="data">Obiekt z danymi nowego zlecenia.
		/// Wypełnić należy: Nr rachunku i komplet informacji o zleceniu (MainData).</param>
		/// <returns>Zwraca ClientId, jakie zostało przypisane przez bibliotekę do tego zlecenia.</returns>
		string OrderCreate(OrderData data);
		/// <summary>
		/// Modyfikacja wcześniejszego zlecenia.
		/// </summary>
		/// <param name="data">Obiekt ze zmodyfikowanymi danymi zlecenia.
		/// Wypełnić należy: Nr rachunku, obecne Id zlecenia i nowy komplet informacji o nim (MainData).</param>
		void OrderReplace(OrderData data);
		/// <summary>
		/// Anulowanie podanego zlecenia.
		/// </summary>
		/// <param name="data">Obiekt z danymi zlecenia do anulowania.
		/// Wypełnić należy: Nr rachunku, obecne Id zlecenia i jego podstawowe dane (instrument, strona, ilość).</param>
		void OrderCancel(OrderData data);

		/// <summary>
		/// Ustawienie "filtra" papierów wartościowych, dla których chcemy otrzymywać bieżące notowania rynkowe.
		/// </summary>
		/// <param name="instruments">Wykaz interesujących nas papierów wartościowych, null - wyłącza subskrypcję.</param>
		void MarketUpdatesSubscription(Instrument[] instruments);
	}
}
