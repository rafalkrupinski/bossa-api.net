using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace pjank.BossaAPI
{
	public class BosPapers : IEnumerable<BosPaper>
	{
		/// <summary>
		/// Rachunek, na którym znajdują się te papiery.
		/// </summary>
		public readonly BosAccount Account;

		/// <summary>
		/// Liczba różnych papierów wartościowych na tym rachunku.
		/// </summary>
		public int Count
		{
			get { return list.Count; }
		}

		/// <summary>
		/// Dostęp do konkretnego papieru wartościowego z listy.
		/// </summary>
		public BosPaper this[int index]
		{
			get { return list[index]; }
		}

		#region Generic list stuff

		private List<BosPaper> list = new List<BosPaper>();

		public IEnumerator<BosPaper> GetEnumerator()
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
		internal BosPapers(BosAccount account)
		{
			Account = account;
		}

		// aktualizacja danych na liście po odebraniu ich z sieci
		internal void Update(DTO.Paper[] dtoPapers)
		{
			list = dtoPapers.Select(p => new BosPaper(Account, p)).ToList();
		}

		#endregion
	}
}
