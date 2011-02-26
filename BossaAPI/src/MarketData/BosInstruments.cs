using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	public class BosInstruments
	{
		private List<BosInstrument> list = new List<BosInstrument>();

		public int Count
		{
			get { return list.Count; }
		}

		public BosInstrument this[int index]
		{
			get { return list[index]; }
		}

	}
}
