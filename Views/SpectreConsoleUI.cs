// REFERENCE: haggling_interfaces.csproj gefunden — benutze Interface-Typen
// Diese Klasse stellt eine einfache UI mit Spectre.Console dar,
// die Events aus einem Channel liest und live anzeigt.
//
// Verschoben aus Mocking nach Views für bessere Architektur.
// Autor: Team
// Datum: 13.09.2025

using System.Threading.Channels;
using Spectre.Console;
using haggling_interfaces;

namespace haggling_ui.Views
{
    // SpectreConsoleUI liest Events aus dem Channel und zeigt sie an.
    public class SpectreConsoleUI
    {
        private readonly Channel<IOffer> _offerChannel;

        // Konstruktor: Channel wird übergeben
        public SpectreConsoleUI(Channel<IOffer> offerChannel)
        {
            _offerChannel = offerChannel;
        }

        // Startet die UI und zeigt Events live an
        public async Task RunAsync()
        {
            // Starte die UI-Ausgabe
            AnsiConsole.MarkupLine("[bold yellow]Starte Haggling-Event-UI...[/]");
            while (await _offerChannel.Reader.WaitToReadAsync())
            {
                while (_offerChannel.Reader.TryRead(out var offer))
                {
                    // Zeige die wichtigsten Angebotsdaten an
                    // Produktname, Preis, Status, OfferedBy
                    var produktName = offer.Product?.Name ?? "(kein Produkt)";
                    var preis = offer.Price;
                    var status = offer.Status;
                    var von = offer.OfferedBy;
                    AnsiConsole.MarkupLine($"[green]Angebot:[/] [bold]{produktName}[/] für [yellow]{preis} EUR[/] | Status: [blue]{status}[/] | Von: [red]{von}[/]");
                }
            }
        }
    }
}
