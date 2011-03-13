using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Identyfikuje intrument (papier wartościowy), którego dotyczy jakaś oferta, zlecenie, transakcja.
	/// TODO: ...
	/// </summary>
	public class BosInstrument
	{
		/// <summary>
		/// kod ISIN instrumentu
		/// </summary>
		public string ISIN { get; private set; }

		/// <summary>
		/// standardowy symbol instrumentu
		/// </summary>
		public string Symbol { get; private set; }

		/// <summary>
		/// skrócony symbol instrumentu
		/// </summary>
		public string Sym { get; private set; }

		/// <summary>
		/// tabela ofert kupna 
		/// </summary>
		public BosOffers BuyOffers { get; private set; }

		/// <summary>
		/// tabela ofert sprzedaży
		/// </summary>
		public BosOffers SellOffers { get; private set; }

		/// <summary>
		/// znana historia transakcji
		/// </summary>
		public BosTrades Trades { get; private set; }

		// IsIndex
		// IsFutures

		public override string ToString()
		{
			return Symbol ?? ISIN;
		}

		public static BosInstrument FindByIsin(string isin)
		{
			return null;
		}

		public static BosInstrument FindBySymbol(string symbol)
		{
			return null;
		}

		internal static BosInstrument Find(string isin, string symbol)
		{
			//TODO: globalna lista instrumentów 
			//return (isin != null) ? FindByIsin(isin) : FindBySymbol(symbol);
			return new BosInstrument { ISIN = isin, Symbol = symbol };
		}

		// zwraca instancję dla konkretnego instrumentu - używane wewnętrznie po odebraniu danych z sieci
		internal static BosInstrument Find(DTO.Instrument data)
		{
			return Find(data.ISIN, data.Symbol);
		}
	}
}
