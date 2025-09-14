using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
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

        // Startet die UI und zeigt Events live als Tabelle an
        public async Task RunAsync()
        {
            AnsiConsole.MarkupLine("[bold yellow]Starte Haggling-Event-UI...[/]");

            // Tabelle anlegen (Spalten: Angebot, Von, Status, Emotion)
            var table = new Table()
                .AddColumn("[bold magenta]Angebot[/]")
                .AddColumn("[bold magenta]Von[/]")
                .AddColumn("[bold magenta]Status[/]")
                .AddColumn("[bold magenta]Emotion[/]");

            table.Border = TableBorder.Rounded;
            table.BorderColor(Color.Fuchsia);
            table.Title("[green]Live-Angebote[/]");

            // Wir behalten nur die letzten N Einträge, damit die Konsole nicht überläuft
            const int maxRows = 20;

            // Live-Ausgabe: bei jedem neuen Event die Tabelle aktualisieren
            while (await _offerChannel.Reader.WaitToReadAsync())
            {
                while (_offerChannel.Reader.TryRead(out var offer))
                {
                    var produktName = offer.Product?.Name ?? "(kein Produkt)";
                    var preis = offer.Price;
                    var status = offer.Status;
                    var von = offer.OfferedBy.ToString();

                    var (emotion, reason) = DetectEmotion(offer);
                    var emoji = GetEmojiFor(emotion);

                    var basePrice = GetBasePrice(offer.Product);
                    var diff = preis - basePrice;

                    var angebotCell = Markup.Escape($"{produktName} — {preis:0.00} EUR (Basis: {basePrice:0.00}, Diff: {diff:+0.00;-0.00})");
                    var vonCell = Markup.Escape(von);
                    var statusCell = Markup.Escape(status.ToString());
                    var emotionCell = Markup.Escape($"{emoji} {emotion} — {reason}");

                    table.AddRow(angebotCell, vonCell, statusCell, emotionCell);

                    // Begrenze Anzahl der Zeilen
                    if (table.Rows.Count > maxRows)
                    {
                        table.Rows.RemoveAt(0);
                    }

                    // Neu zeichnen
                    Console.Clear();
                    AnsiConsole.Write(table);

                    // _dealSuccessful setzen wie zuvor
                    if (offer.Status == OfferStatus.Accepted)
                        _dealSuccessful = true;
                    else if (offer.Status == OfferStatus.Stopped)
                        _dealSuccessful = false;
                }
            }

            // Nach Ende: Ergebnis anzeigen
            Console.WriteLine();
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

        private decimal GetBasePrice(IProduct? product)
        {
            if (product == null)
                return 0;
            return product.Rarity.Value * 0.5m;
        }

        private (Emotion emotion, string reason) DetectEmotion(IOffer offer)
        {
            var basePrice = GetBasePrice(offer.Product);

            if (offer.Status == OfferStatus.Accepted)
            {
                if (offer.OfferedBy == PersonType.Customer)
                {
                    if (offer.Price <= basePrice)
                        return (Emotion.Happy, $"Kunde freut sich: {offer.Price} EUR ist günstig (Basis {basePrice})");
                    else
                        return (Emotion.Neutral, $"Kunde akzeptiert, aber zahlt mehr als gedacht (Basis {basePrice})");
                }
                else if (offer.OfferedBy == PersonType.Vendor)
                {
                    if (offer.Price >= basePrice)
                        return (Emotion.Happy, $"Verkäufer freut sich: {offer.Price} EUR bringt Gewinn (Basis {basePrice})");
                    else
                        return (Emotion.Neutral, $"Verkäufer akzeptiert zähneknirschend, Preis unter Basis {basePrice}");
                }
            }

            if (offer.Status == OfferStatus.Stopped)
            {
                if (offer.Price < basePrice * 0.5m)
                    return (Emotion.Angry, $"Verhandlung abgebrochen – Preis {offer.Price} war VIEL zu niedrig (Basis {basePrice})");
                if (offer.Price > basePrice * 2)
                    return (Emotion.Angry, $"Verhandlung abgebrochen – Preis {offer.Price} war unrealistisch hoch (Basis {basePrice})");

                return (Emotion.Neutral, "Verhandlung abgebrochen ohne Einigung 😐");
            }

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
