# haggling_ui

Dieses Projekt enthält die UI-Logik für das Haggling-System. Die Struktur ist modular aufgebaut, um spätere Erweiterungen und Anpassungen zu erleichtern.

## Ordnerstruktur

- **Mocking/**
  - Enthält MockEventGenerator und zugehörige Mocks für Tests und Simulationen.
- **Adapters/**
  - UI-Adapter, die zwischen Event-Generator und UI vermitteln.
- **Listeners/**
  - Listener für Events oder UI-Aktionen.
- **Views/**
  - Verschiedene UI-Darstellungen (z.B. Console, Web, etc.).

## Einstiegspunkt

- **Program.cs**
  - Hauptprogramm. Hier wird die Initialisierung und der Start der Komponenten gesteuert.

## Hinweise für Teammitglieder

- Die einzelnen Komponenten können unabhängig entwickelt und getestet werden.
- Kommentare in den jeweiligen .gitkeep-Dateien geben Hinweise zur Nutzung der Ordner.
- Für neue Features einfach einen passenden Ordner/Datei anlegen und im Hauptprogramm einbinden.