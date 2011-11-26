using System;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Interfejs pozwalający na lepszą separację podstawowych elementów biblioteki i ułatwiający np. pisanie testów.
	/// Domyślna implementacja w klasie "BossaApi" - używanej też wewnętrznie przez główną klasę biblioteki: "Bossa".
	/// </summary>
	public interface IBossaApi
	{
		bool Connected { get; }

		IBosClient Connection { get; }

		BosAccounts Accounts { get; }

		BosInstruments Instruments { get; }

		event EventHandler OnUpdate;


		void Connect(IBosClient client);

		void Disconnect();

		void Clear();
	}
}
