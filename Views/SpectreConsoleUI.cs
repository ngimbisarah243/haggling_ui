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
        private bool? _dealSuccessful = null;

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
                    var produktName = offer.Product?.Name ?? "(kein Produkt)";
                    var preis = offer.Price;
                    var status = offer.Status;
                    var von = offer.OfferedBy;
                    
                    var (emotion, reason) = DetectEmotion(offer);
                    var emoji = GetEmojiFor(emotion);

                    var basePrice = GetBasePrice(offer.Product);
                    var diff = preis - basePrice;

                    AnsiConsole.MarkupLine(
                        $"[green]Angebot:[/] [bold]{produktName}[/] für [yellow]{preis} EUR[/] " +
                        $"(Basis: {basePrice:0.00} EUR, Diff: {diff:+0.00;-0.00}) | " +
                        $"Status: [blue]{status}[/] | Von: [red]{von}[/] | " +
                        $"Emotion: {emoji.EscapeMarkup()} {emotion} [italic]{reason}[/]"
                    );

                    if (offer.Status == OfferStatus.Accepted)
                        _dealSuccessful = true;
                    else if (offer.Status == OfferStatus.Stopped)
                        _dealSuccessful = false;
                }

                
                
            }
            
            if (_dealSuccessful == true)
            {
                AnsiConsole.MarkupLine("[bold green] Verhandlung erfolgreich! Ein Deal wurde abgeschlossen.[/]");
            }
            else if (_dealSuccessful == false)
            {
                AnsiConsole.MarkupLine("[bold red] Verhandlung gescheitert! Kein Deal zustande gekommen.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow] Verhandlung beendet ohne Ergebnis.[/]");
            }

            Console.ReadKey();
        }
        private decimal GetBasePrice(IProduct product)
        {
            // Falls Produkt null ist → 0 zurück
            if (product == null)
                return 0;

            // Beispiel: Rarity bestimmt den Basispreis
            // Rarity 50 → 25 EUR
            return product.Rarity.Value * 0.5m;
        }

        
      private (Emotion emotion, string reason) DetectEmotion(IOffer offer)
{
    var basePrice = GetBasePrice(offer.Product);

    // ✅ Accepted-Status differenzieren
    if (offer.Status == OfferStatus.Accepted)
    {
        if (offer.OfferedBy == PersonType.Customer)
        {
            // Kunde hat akzeptiert → prüfe Preis
            if (offer.Price <= basePrice)
                return (Emotion.Happy, $"Kunde freut sich: {offer.Price} EUR ist günstig (Basis {basePrice})");
            else
                return (Emotion.Neutral, $"Kunde akzeptiert, aber zahlt mehr als gedacht (Basis {basePrice})");
        }
        else if (offer.OfferedBy == PersonType.Vendor)
        {
            // Vendor hat akzeptiert → prüfe Preis
            if (offer.Price >= basePrice)
                return (Emotion.Happy, $"Verkäufer freut sich: {offer.Price} EUR bringt Gewinn (Basis {basePrice})");
            else
                return (Emotion.Neutral, $"Verkäufer akzeptiert zähneknirschend, Preis unter Basis {basePrice}");
        }
    }

    // ❌ Stopped
    if (offer.Status == OfferStatus.Stopped)
    {
        if (offer.Price < basePrice * 0.5m)
            return (Emotion.Angry, $"Verhandlung abgebrochen – Preis {offer.Price} war VIEL zu niedrig (Basis {basePrice})");
        if (offer.Price > basePrice * 2)
            return (Emotion.Angry, $"Verhandlung abgebrochen – Preis {offer.Price} war unrealistisch hoch (Basis {basePrice})");

        return (Emotion.Neutral, "Verhandlung abgebrochen ohne Einigung 😐");
    }

    // Ongoing mit Preisspanne prüfen
    if (offer.Price < basePrice * 0.5m)
        return (Emotion.Annoyed, $"Preis {offer.Price} ist viel zu niedrig (Basis: {basePrice})");

    if (offer.Price > basePrice * 2)
        return (Emotion.Annoyed, $"Preis {offer.Price} ist überzogen hoch (Basis: {basePrice})");

    if (offer.Price < basePrice * 0.8m)
        return (Emotion.Excited, $"Sehr günstiges Angebot 🤩 (Basis {basePrice})");

    return (Emotion.Neutral, "Normales Angebot");
}

        
        private string GetEmojiFor(Emotion emotion) => emotion switch
        {
            Emotion.Happy   => ":D",
            Emotion.Neutral => ":|",
            Emotion.Annoyed => ">:/",
            Emotion.Angry   => ">:[",
            Emotion.Excited => "^_^",
            _ => ":|"
        };

    }
}
