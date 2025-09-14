// REFERENCE: haggling_interfaces.csproj gefunden — benutze Interface-Typen
// Einstiegspunkt für das Mocking-Subsystem.
// Startet den Event-Generator und die UI.

/*using System.Threading.Channels;
using Mocking;
using haggling_ui.Views;
using System.Threading.Tasks;

namespace Mocking
{
    internal class Program
    {
        // Hauptmethode: Initialisiert Channel, Event-Generator und UI
        static async Task Main(string[] args)
        {
            // Channel für Angebote
            var offerChannel = Channel.CreateUnbounded<haggling_interfaces.IOffer>();

            // Event-Generator starten
            var generator = new MockEventGenerator(offerChannel);
            generator.Start();

            // UI starten (jetzt aus Views)
            var ui = new SpectreConsoleUI(offerChannel);
            await ui.RunAsync();
            // Hinweis: Die UI-Komponenten liegen jetzt im Views-Ordner für bessere Trennung.
        }
    }
}
*/