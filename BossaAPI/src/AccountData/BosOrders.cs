using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Lista bieżących zleceń na danym rachunku.
	/// </summary>
	public class BosOrders : IEnumerable<BosOrder>
	{
		/// <summary>
		/// Rachunek, którego dotyczą te zlecenia.
		/// </summary>
		public readonly BosAccount Account;

		/// <summary>
		/// Liczba bieżących zleceń dostępnych na tym rachunku.
		/// </summary>
		public int Count
		{
			get { return list.Count; }
		}

		/// <summary>
		/// Dostęp do konkretnego zlecenia z listy, po jego indeksie (licząc od zera).
		/// </summary>
		public BosOrder this[int index]
		{
			get { return list[index]; }
		}

		#region Generic list stuff

		private List<BosOrder> list = new List<BosOrder>();

		public IEnumerator<BosOrder> GetEnumerator()
		{
			for (int i = 0; i < Count; i++)
				yield return this[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Internal library stuff

		// konstruktor, wywoływany z samej klasy BosAccount
		internal BosOrders(BosAccount account)
		{
			Account = account;
		}
	
		// aktualizacja danych na liście po odebraniu ich z sieci
		internal void Update(DTO.OrderData data)
		{
			var order = list.Where(o => o.Id == data.BrokerId).SingleOrDefault();
			if (order != null)
				order.Update(data);
			else 
				list.Add(new BosOrder(Account, data));
		}

		#endregion
	}
}
