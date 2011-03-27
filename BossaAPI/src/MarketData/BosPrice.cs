using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Klasa reprezentująca cenę podaną w naszym zleceniu lub w arkuszu ofert
	/// (tj. po prostu zwykła kwota *albo* wartość specjalna, jak: PKC, PCR, PCRO)
	/// </summary>
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

		#region Internal stuff

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

		// konstruktor używany przy odczycie danych z obiektów transportowych
		internal static BosPrice Create(DTO.PriceType priceType, decimal? value)
		{
			switch (priceType)
			{
				case DTO.PriceType.PKC: return PKC;
				case DTO.PriceType.PCR: return PCR;
				case DTO.PriceType.PCRO: return PCRO;
				default: return value;
			}
		}

		// konwersja tego obiektu na typ transportowy (poza samym limitem ceny)
		internal DTO.PriceType Type
		{
			get
			{
				if (this == PKC) return DTO.PriceType.PKC;
				if (this == PCR) return DTO.PriceType.PCR;
				if (this == PCRO) return DTO.PriceType.PCRO;
				return DTO.PriceType.Limit;
			}
		}

		private decimal? numValue;
		private string txtValue;

		#endregion

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
			get { return txtValue ?? string.Format("{0:0.00}", numValue); }
		}

		/// <summary>
		/// Standardowa konwersja na string -> dokładnie, jak "TxtValue".
		/// </summary>
		public override string ToString()
		{
			return TxtValue;
		}

	}
}
