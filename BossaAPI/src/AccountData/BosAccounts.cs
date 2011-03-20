using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Lista rachunków danego klienta.
	/// </summary>
	public class BosAccounts : IEnumerable<BosAccount>
	{
		/// <summary>
		/// Liczba dostępnych na tę chwilę rachunków.
		/// </summary>
		public int Count
		{
			get { return list.Count; }
		}

		/// <summary>
		/// Dostęp do konkretnego rachunku z listy, po jego indeksie (licząc od zera).
		/// </summary>
		public BosAccount this[int index]
		{
			get { return list[index]; }
		}

		/// <summary>
		/// Dostęp do konkretnego rachunku, po jego numerze.
		/// Można podać fragment numeru (np. tylko początek "00-55" dla rachunku akcyjnego,
		/// "00-22" dla kontraktowego... albo też samą końcówkę) - zakładając tylko, że będzie
		/// to unikalny fragment numeru, biblioteka zwróci ten odpowiedni rachunek (o ile istnieje). 
		/// Jeśli podaliśmy pełen numer rachunku (w formacie "xx-xx-xxxxx"), a taki na naszej
		/// liście jeszcze nie istnieje - zostanie automatycznie utworzony dla niego nowy obiekt.
		/// </summary>
		public BosAccount this[string number]
		{
			get { return GetAccount(number, true); }
		}

		/// <summary>
		/// Dostęp do konkretnego rachunku, odpowiedniego dla danego typu instrumentów.
		/// </summary>
		public BosAccount this[BosInstrumentType type]
		{
			get { return GetAccount(type); }
		}

		#region Generic list stuff

		private List<BosAccount> list = new List<BosAccount>();

		// IEnumerable<BosAccount>
		public IEnumerator<BosAccount> GetEnumerator()
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

		#region Internal library stuff

		// wywoływane z Bossa.Reset() - usuwa z pamięci wszystkie rachunki i ich zawartość
		internal void Clear()
		{
			list.Clear();
		}

		#endregion

		#region Private stuff

		// odszukanie rachunku o podanym numerze, ewentualne utworzenie nowego
		private BosAccount GetAccount(string number, bool tryCreate)
		{
			var account = list.Where(a => a.Number.Contains(number)).SingleOrDefault();
			if (account == null)
			{
				if (!tryCreate)
					throw new ArgumentException("Account matching '" + number + "' not found!");
				if (!Regex.IsMatch(number, @"\d\d-\d\d-\d+"))
					throw new ArgumentException("Invalid account number: " + number);
				account = new BosAccount(number);
				list.Add(account);
			}
			return account;
		}

		// odszukanie rachunku odpowiedniego dla danego typu instrumentów
		private BosAccount GetAccount(BosInstrumentType type)
		{
			// Zakładam (nie wiem, czy słusznie...), że w DM BOŚ numery rachunków są zawsze przyznawane 
			// wg. takich właśnie zasad jak poniżej (akcyjne: 00-55-....., kontraktowe: 00-22-.....)
			// Jeśli jest inaczej - bardzo proszę mnie oświecić! :-)
			switch (type)
			{
				case BosInstrumentType.Default: return GetAccount("00-55-", false);
				case BosInstrumentType.Futures: return GetAccount("00-22-", false);
				default: throw new ArgumentException("Invalid instrument type");
			}
		}

		#endregion
	}
}
