using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Lista raportów z wykonania kolejnych transakcji dla naszego zlecenia.
	/// </summary>
	public class BosOrderTradeReports : IEnumerable<BosOrderTradeReport>
	{
		private List<BosOrderTradeReport> list = new List<BosOrderTradeReport>();

		public int Count
		{
			get { return list.Count; }
		}

		public BosOrderTradeReport this[int index]
		{
			get { return list[index]; }
		}

		public IEnumerator<BosOrderTradeReport> GetEnumerator()
		{
			for (int i = 0; i < Count; i++)
				yield return this[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		// aktualizacja danych na liście po odebraniu ich z sieci
		internal void Update(DTO.OrderTradeData data)
		{
			list.Add(new BosOrderTradeReport(data));
		}
	}
}
