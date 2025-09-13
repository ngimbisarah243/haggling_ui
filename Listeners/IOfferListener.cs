// Listener-Interface für Angebote im Haggling-System
// Dieses Interface definiert die Struktur für Listener, die auf verschiedene Events rund um Angebote reagieren können.
// Listener helfen, die Event-Verarbeitung zu modularisieren und z.B. Logging, UI-Updates oder Benachrichtigungen zu ermöglichen.
//
// Autor: Team
// Datum: 13.09.2025

using haggling_interfaces;

namespace haggling_ui.Listeners
{
    /// <summary>
    /// Interface für Listener, die auf Angebots-Events reagieren.
    /// Implementierungen können z.B. auf neue Angebote, akzeptierte Angebote oder abgebrochene Verhandlungen reagieren.
    /// </summary>
    public interface IOfferListener
    {
        /// <summary>
        /// Wird aufgerufen, wenn ein neues Angebot erstellt wurde.
        /// </summary>
        /// <param name="offer">Das neue Angebot.</param>
        void OnOfferCreated(IOffer offer);

        /// <summary>
        /// Wird aufgerufen, wenn ein Angebot akzeptiert wurde.
        /// </summary>
        /// <param name="offer">Das akzeptierte Angebot.</param>
        void OnOfferAccepted(IOffer offer);

        /// <summary>
        /// Wird aufgerufen, wenn die Verhandlung gestoppt wurde.
        /// </summary>
        /// <param name="offer">Das Angebot, bei dem die Verhandlung gestoppt wurde.</param>
        void OnNegotiationStopped(IOffer offer);
    }
}
