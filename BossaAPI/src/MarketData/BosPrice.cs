using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	public class BosPrice
	{
		// pozwala łatwo przypisać do obiektu zwykłą liczbę
		public static implicit operator BosPrice(decimal price)
		{
			return new BosPrice(price);
		}

		// stałe używane przy przypisywaniu wartości słownych
		public static readonly BosPrice PKC = new BosPrice("PKC");
		public static readonly BosPrice PCR = new BosPrice("PCR");
		public static readonly BosPrice PCRO = new BosPrice("PCRO");

		// prywatny konstruktor używany przy przypisaniu zwykłej liczby
		private BosPrice(decimal value)
		{
			this.numValue = value;
			this.txtValue = null;
		}

		// prywatny konstruktor używany przy tworzeniu stałych PKC, PCR itp.
		private BosPrice(string value)
		{
			this.numValue = null;
			this.txtValue = value;
		}

		private decimal? numValue;
		private string txtValue;

		/// <summary>
		/// Kwota w standardowej postaci liczbowej.
		/// Jeśli ustawiono cenę typu PKC, PCR - zwraca tutaj null.
		/// </summary>
		public decimal? NumValue
		{
			get { return numValue; }
		}

		/// <summary>
		/// Kwota w postaci tekstowej.
		/// Np. "PKC", "PCR" albo po prostu liczba zamieniona na stringa.
		/// </summary>
		public string TxtValue
		{
			get { return txtValue ?? numValue.ToString(); }
		}


		public override string ToString()
		{
			return TxtValue;
		}

	}
}
