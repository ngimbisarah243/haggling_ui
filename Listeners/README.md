# Listener-Architektur im Haggling-UI

Dieser Ordner enthält Listener, die auf verschiedene Events im Haggling-System reagieren können. Listener helfen, die Event-Verarbeitung zu modularisieren und z.B. Logging, UI-Updates oder Benachrichtigungen zu ermöglichen.

## Typische Events
- Neues Angebot wird erstellt
- Angebot wird akzeptiert
- Verhandlung wird gestoppt

## Wie funktioniert ein Listener?
Ein Listener implementiert das Interface `IOfferListener` und reagiert auf die definierten Methoden:
- `OnOfferCreated(IOffer offer)`
- `OnOfferAccepted(IOffer offer)`
- `OnNegotiationStopped(IOffer offer)`

## Beispiel
Der `ConsoleOfferListener` gibt die Angebotsdaten einfach auf der Konsole aus. Weitere Listener können z.B. für Logging, UI-Updates oder externe Benachrichtigungen gebaut werden.

## Erweiterung
Listener können beliebig erweitert und kombiniert werden. So bleibt die Event-Verarbeitung flexibel und übersichtlich.

---
Viele Kommentare in den Dateien helfen Teammitgliedern beim Einstieg und bei der Erweiterung.
