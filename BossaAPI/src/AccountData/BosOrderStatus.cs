using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	public enum BosOrderStatus
	{
		/// <summary>
		/// Zlecenie w trakcie modyfikacji... a póki co jest (chyba?) wciąż aktywne.
		/// </summary>
		PendingReplace,
		/// <summary>
		/// Zlecenie w trakcie anulaty... a póki co jest (chyba?) wciąż aktywne.
		/// </summary>
		PendingCancel,

		/// <summary>
		/// Przyjęte nowe zlecenie, oczekujące w arkuszu ofert na realizację.
		/// </summary>
		Active,
		/// <summary>
		/// Zlecenie częściowo zrealizowane, reszta wciąż oczekuje w arkuszu ofert.
		/// </summary>
		ActiveFilled,
		/// <summary>
		/// Zlecenie anulowane, nie zdążyło się wcale zrealizować.
		/// </summary>
		Cancelled,
		/// <summary>
		/// Zlecenie częściowo zrealizowane, reszta została anulowana.
		/// </summary>
		CancelledFilled,
		/// <summary>
		/// Zlecenie w całości zrealizowane.
		/// </summary>
		Filled,
		/// <summary>
		/// Zlecenie archiwalne (nie zrealizowane, któremu skończył się termin ważności).
		/// </summary>
		Expired,
		/// <summary>
		/// Zlecenie odrzucone przez system (DM lub Giełdę).
		/// </summary>
		Rejected,
	}
}
