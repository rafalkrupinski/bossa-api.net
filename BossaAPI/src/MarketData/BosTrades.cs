using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	public class BosTrades
	{
		private List<BosTrade> list = new List<BosTrade>();

		public int Count
		{
			get { return list.Count; }
		}

		public BosTrade this[int index]
		{
			get { return list[index]; }
		}
	}
}
