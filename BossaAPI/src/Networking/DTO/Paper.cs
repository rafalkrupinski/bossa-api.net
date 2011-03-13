using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI.DTO
{
	/// <summary>
	/// Obiekt transportowy do przekazywania (między modułami biblioteki) informacji 
	/// o otwartej pozycji (posiadanych papierach wartościowych) na rachunku klienta. 
	/// </summary>
	public class Paper
	{
		public Instrument Instrument;
		public int Account110;
		public int Account120;
	}
}
