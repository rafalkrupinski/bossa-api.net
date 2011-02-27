using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace pjank.BossaAPI
{
	public class BosPapers : IEnumerable<BosPaper>
	{
		private List<BosPaper> list = new List<BosPaper>();

		public int Count
		{
			get { return list.Count; }
		}

		public BosPaper this[int index]
		{
			get { return list[index]; }
		}

		public IEnumerator<BosPaper> GetEnumerator()
		{
			for (int i = 0; i < Count; i++)
				yield return this[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		internal void Update(DTO.Paper[] dtoPapers)
		{
			list = dtoPapers.Select(p => new BosPaper(p)).ToList();
		}
	}
}
