// Beispiel-Implementierung eines Listeners für Angebote
// Diese Klasse reagiert auf Angebots-Events und gibt Informationen zur Konsole aus.
// Sie kann als Vorlage für Logging, UI-Updates oder andere Event-Verarbeitung dienen.
//
// Autor: Team
// Datum: 13.09.2025

using System;
using haggling_interfaces;

namespace haggling_ui.Listeners
{
    /// <summary>
    /// Listener, der Angebots-Events auf der Konsole ausgibt.
    /// </summary>
    public class ConsoleOfferListener : IOfferListener
    {
        /// <summary>
        /// Wird aufgerufen, wenn ein neues Angebot erstellt wurde.
        /// Gibt die Angebotsdaten auf der Konsole aus.
        /// </summary>
        public void OnOfferCreated(IOffer offer)
        {
            Console.WriteLine($"[Listener] Neues Angebot: Produkt={offer.Product?.Name}, Preis={offer.Price}, Status={offer.Status}, Von={offer.OfferedBy}");
        }

        /// <summary>
        /// Wird aufgerufen, wenn ein Angebot akzeptiert wurde.
        /// Gibt die Angebotsdaten auf der Konsole aus.
        /// </summary>
        public void OnOfferAccepted(IOffer offer)
        {
            Console.WriteLine($"[Listener] Angebot akzeptiert: Produkt={offer.Product?.Name}, Preis={offer.Price}, Von={offer.OfferedBy}");
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Verhandlung gestoppt wurde.
        /// Gibt die Angebotsdaten auf der Konsole aus.
        /// </summary>
        public void OnNegotiationStopped(IOffer offer)
        {
            Console.WriteLine($"[Listener] Verhandlung gestoppt: Produkt={offer.Product?.Name}, Preis={offer.Price}, Von={offer.OfferedBy}");
        }
    }
}
