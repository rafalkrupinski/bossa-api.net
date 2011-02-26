using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	public class BosPapers
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

	}
}
