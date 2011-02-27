using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace pjank.BossaAPI
{
	public class BosAccounts : IEnumerable<BosAccount>
	{
		private List<BosAccount> list = new List<BosAccount>();

		public int Count
		{
			get { return list.Count; }
		}

		public BosAccount this[int index]
		{
			get { return list[index]; }
		}

		public BosAccount this[string number]
		{
			get { return GetAccount(number); }
		}

		public IEnumerator<BosAccount> GetEnumerator()
		{
			for (int i = 0; i < Count; i++)
				yield return this[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private BosAccount GetAccount(string number)
		{
			var account = list.Where(a => a.Number.Contains(number)).SingleOrDefault();
			if (account == null)
			{
				if (!Regex.IsMatch(number, @"\d\d-\d\d-\d+"))
					throw new ArgumentException("Invalid account number: " + number);
				account = new BosAccount(number);
				list.Add(account);
			}
			return account;
		}

		internal void Clear()
		{
			list.Clear();
		}
	}
}
