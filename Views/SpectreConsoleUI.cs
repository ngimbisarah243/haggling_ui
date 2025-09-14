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

            // Tabelle anlegen (Spalten: Angebot, Von, Status, Emotion) mit rosa Header-Hintergrund
            var table = new Table()
                .AddColumn("[bold white on magenta]   Angebot   [/]")
                .AddColumn("[bold white on magenta]   Von   [/]")
                .AddColumn("[bold white on magenta]   Status   [/]")
                .AddColumn("[bold white on magenta]   Emotion   [/]");

            table.Border = TableBorder.Rounded;
            table.BorderColor(Color.Fuchsia);
            table.Title("[bold green]Live-Angebote[/]");

            // Wir behalten nur die letzten N Einträge, damit die Konsole nicht überläuft
            const int maxRows = 20;

            // Live-Ausgabe: bei jedem neuen Event die Tabelle aktualisieren
            await AnsiConsole.Live(table)
                .StartAsync(async ctx =>
                {
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

                            // 🎨 Farbige Zellen erstellen
                            var angebotCell = CreateColoredOfferCell(produktName, preis, basePrice, diff);
                            var vonCell = CreateColoredPersonCell(von);
                            var statusCell = CreateColoredStatusCell(status.ToString());
                            var emotionCell = CreateColoredEmotionCell(emoji, emotion.ToString(), reason);

                            table.AddRow(angebotCell, vonCell, statusCell, emotionCell);

                            // Begrenze Anzahl der Zeilen
                            if (table.Rows.Count > maxRows)
                            {
                                table.Rows.RemoveAt(0);
                            }

                            // Live-Update ohne Console.Clear()
                            ctx.UpdateTarget(table);
                            await Task.Delay(500); // Kurze Pause zwischen Updates

                            // _dealSuccessful setzen wie zuvor
                            if (offer.Status == OfferStatus.Accepted)
                                _dealSuccessful = true;
                            else if (offer.Status == OfferStatus.Stopped)
                                _dealSuccessful = false;
                        }
                    }
                });

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
            Emotion.Happy   => "^_^",  // Freude
            Emotion.Neutral => "-_-",  // Neutral
            Emotion.Annoyed => ">:/",  // Genervt (bleibt gleich)
            Emotion.Angry   => ">:[",  // Böse
            Emotion.Excited => "^_^",  // Aufgeregt (gleich wie Happy)
            _ => "-_-"                 // Default auf Neutral
        };

        // 🎨 Farbige Zellen-Erstellungsmethoden
        private string CreateColoredOfferCell(string produktName, decimal preis, decimal basePrice, decimal diff)
        {
            // Produktname: Bold für alle Produkte (gleichfarbig)
            var coloredProduktName = $"[bold]{Markup.Escape(produktName)}[/]";

            // Preis nach Logik einfärben
            string coloredPreis;
            if (preis < basePrice * 0.8m)
                coloredPreis = $"[green]{preis:0.00} EUR[/]";  // Günstig
            else if (preis > basePrice * 1.5m)
                coloredPreis = $"[red]{preis:0.00} EUR[/]";    // Teuer
            else
                coloredPreis = $"[yellow]{preis:0.00} EUR[/]"; // Fair

            // Differenz einfärben
            string coloredDiff;
            if (diff < 0)
                coloredDiff = $"[green]{diff:+0.00;-0.00}[/]";  // Günstiger als Basis
            else if (diff > basePrice * 0.5m)
                coloredDiff = $"[red]{diff:+0.00;-0.00}[/]";    // Viel teurer
            else
                coloredDiff = $"[yellow]{diff:+0.00;-0.00}[/]"; // Moderat teurer

            return $"{coloredProduktName} — {coloredPreis}\n[dim](Basis: {basePrice:0.00}, Diff: {coloredDiff})[/]";
        }

        private string CreateColoredPersonCell(string person)
        {
            return person switch
            {
                "Customer" => $"[cyan]{Markup.Escape(person)}[/]",
                "Vendor" => $"[orange3]{Markup.Escape(person)}[/]",
                _ => Markup.Escape(person)
            };
        }

        private string CreateColoredStatusCell(string status)
        {
            return status switch
            {
                "Accepted" => $"[green]{Markup.Escape(status)}[/]",
                "Stopped" => $"[red]{Markup.Escape(status)}[/]",
                "Ongoing" => $"[blue]{Markup.Escape(status)}[/]",
                _ => Markup.Escape(status)
            };
        }

        private string CreateColoredEmotionCell(string emoji, string emotion, string reason)
        {
            // Emoticons farbig gestalten
            string coloredEmoji = emotion switch
            {
                "Happy" => $"[bold green]{Markup.Escape(emoji)}[/]",
                "Angry" => $"[bold red]{Markup.Escape(emoji)}[/]",
                "Neutral" => $"[bold yellow]{Markup.Escape(emoji)}[/]",
                "Annoyed" => $"[bold orange3]{Markup.Escape(emoji)}[/]",
                "Excited" => $"[bold green]{Markup.Escape(emoji)}[/]",
                _ => $"[bold]{Markup.Escape(emoji)}[/]"
            };

            return $"{coloredEmoji} [bold]{Markup.Escape(emotion)}[/] — {Markup.Escape(reason)}";
        }
    }
}
