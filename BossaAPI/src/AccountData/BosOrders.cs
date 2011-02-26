using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	public class BosOrders
	{
		private List<BosOrder> list = new List<BosOrder>();

		public int Count
		{
			get { return list.Count; }
		}

		public BosOrder this[int index]
		{
			get { return list[index]; }
		}
	}
}
