using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI.DTO
{
	/// <summary>
	/// Obiekt transportowy do przekazywania (między modułami biblioteki)
	/// symbolu papieru wartościowego (pełnego, skróconego, ISIN - jaki tam akurat mamy pod ręką...)
	/// </summary>
	public class Instrument
	{
		public string Symbol;
		public string ISIN;
	}
}
