using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	public class BosOffers
	{
		private List<BosOffer> list = new List<BosOffer>();

		public int Count
		{
			get { return list.Count; }
		}

		public BosOffer this[int index]
		{
			get { return list[index]; }
		}
	}
}
