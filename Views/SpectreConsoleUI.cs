using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using haggling_interfaces;
using Spectre.Console;

namespace haggling_ui.Views
{
    // SpectreConsoleUI: Zeigt Haggling-Events in einer schönen Konsolen-UI an
    public class SpectreConsoleUI
    {
        private readonly Channel<IOffer> _offerChannel;
        private bool? _dealSuccessful = null;

        public SpectreConsoleUI(Channel<IOffer> offerChannel)
        {
            _offerChannel = offerChannel;
        }

        // Startet die UI und zeigt Events live als große Tabelle an
        public async Task RunAsync()
        {
            AnsiConsole.MarkupLine("[bold yellow]Starte Haggling-Event-UI...[/]");
            AnsiConsole.WriteLine();
            
            // Titel einmal am Anfang anzeigen
            AnsiConsole.MarkupLine("[bold green]📋 Live-Angebote[/]");
            AnsiConsole.WriteLine();

            // Tabelle anlegen OHNE eigenen Titel (damit er nicht mehrfach erscheint)
            var table = new Table()
                .AddColumn("[bold white on magenta]   Angebot   [/]")
                .AddColumn("[bold white on magenta]   Von   [/]")
                .AddColumn("[bold white on magenta]   Status   [/]")
                .AddColumn("[bold white on magenta]   Emotion   [/]");

            table.Border = TableBorder.Rounded;
            table.BorderColor(Color.Fuchsia);
            // KEIN table.Title() - das war das Problem!

            const int maxRows = 20;
            
            // Live-Ausgabe mit Console.Clear für saubere Darstellung
            while (await _offerChannel.Reader.WaitToReadAsync())
            {
                while (_offerChannel.Reader.TryRead(out var offer))
                {
                    // Event Details
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

                    // Console leeren und Titel + Tabelle neu zeichnen
                    Console.Clear();
                    AnsiConsole.MarkupLine("[bold yellow]Starte Haggling-Event-UI...[/]");
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine("[bold green]📋 Live-Angebote[/]");
                    AnsiConsole.WriteLine();
                    AnsiConsole.Write(table);
                    
                    if (offer.Status == OfferStatus.Accepted)
                        _dealSuccessful = true;
                    else if (offer.Status == OfferStatus.Stopped)
                        _dealSuccessful = false;
                        
                    // Pause zwischen Updates
                    await Task.Delay(800);
                }
            }

            // Nach Ende: Ergebnis anzeigen
            AnsiConsole.WriteLine();
            if (_dealSuccessful == true)
            {
                AnsiConsole.MarkupLine("[bold green]✅ Verhandlung erfolgreich! Ein Deal wurde abgeschlossen.[/]");
            }
            else if (_dealSuccessful == false)
            {
                AnsiConsole.MarkupLine("[bold red]❌ Verhandlung gescheitert! Kein Deal zustande gekommen.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]⏸️ Verhandlung beendet ohne Ergebnis.[/]");
            }
        }

        // Bestimmt die Emotion basierend auf dem Angebot
        private (string emotion, string reason) DetectEmotion(IOffer offer)
        {
            var basePrice = GetBasePrice(offer.Product);
            var diff = offer.Price - basePrice;
            var diffPercent = (diff / basePrice) * 100;

            return offer.Status switch
            {
                OfferStatus.Accepted when diffPercent > 10 => ("Happy", $"Verkäufer freut sich: {offer.Price} EUR bringt Gewinn (Basis {basePrice:F1})"),
                OfferStatus.Accepted when diffPercent < -10 => ("Happy", $"Kunde freut sich: {offer.Price} EUR ist günstig (Basis {basePrice:F1})"),
                OfferStatus.Accepted => ("Neutral", $"Verkäufer akzeptiert zähneknirschend, Preis unter Basis {basePrice:F1}"),
                OfferStatus.Stopped when Math.Abs(diffPercent) > 50 => ("Angry", $"Verhandlung abgebrochen – Preis {offer.Price} war unrealistisch {(diffPercent > 0 ? "hoch" : "niedrig")} (Basis {basePrice:F1})"),
                OfferStatus.Stopped => ("Angry", "Verhandlung abgebrochen ohne Einigung �"),
                _ when offer.OfferedBy == PersonType.Customer && diffPercent < -30 => ("Annoyed", $"Vendor ist verärgert: Kunde bietet nur {offer.Price} EUR (viel zu wenig für Basis {basePrice:F1})"),
                _ when offer.OfferedBy == PersonType.Vendor && diffPercent > 30 => ("Annoyed", $"Customer ist verärgert: Vendor verlangt {offer.Price} EUR (viel zu teuer für Basis {basePrice:F1})"),
                _ when Math.Abs(diffPercent) < 5 => ("Excited", $"Sehr faires Angebot 🤩 - nah an Basis {basePrice:F1}"),
                _ => ("Neutral", $"Normales Angebot von {offer.OfferedBy}")
            };
        }

        // Gibt Emoji für Emotion zurück
        private string GetEmojiFor(string emotion)
        {
            return emotion switch
            {
                "Happy" => "^_^",
                "Excited" => "^_^",
                "Neutral" => "-_-",
                "Annoyed" => ">:/",
                "Angry" => ">:[",
                _ => "-_-"
            };
        }

            // Berechnet Basispreis für ein Produkt
        private decimal GetBasePrice(IProduct? product)
        {
            if (product == null) return 10m;

            var basePrice = product.Type switch
            {
                ProductType.Tools => 35m,
                ProductType.Food => 15m,
                ProductType.Clothing => 25m,
                ProductType.Electronics => 65m,
                ProductType.Furniture => 80m,
                ProductType.Toys => 20m,
                ProductType.Books => 12m,
                ProductType.SportsEquipment => 45m,
                ProductType.Jewelry => 120m,
                ProductType.BeautyProducts => 28m,
                _ => 30m
            };

            // Rarity-Modifier: +0% bis +100% je nach Seltenheit
            var rarityMultiplier = 1 + (product.Rarity.Value / 100m);
            return basePrice * rarityMultiplier;
        }

        // 🎨 Farbige Zellen-Methoden
        private string CreateColoredOfferCell(string produktName, decimal preis, decimal basePrice, decimal diff)
        {
            var diffColor = diff switch
            {
                > 20 => "red",
                > 10 => "orange3",
                > 0 => "yellow",
                > -10 => "lime",
                _ => "green"
            };

            // Escape product name to prevent markup parsing issues
            var escapedProduktName = produktName.Replace("[", "[[").Replace("]", "]]");

            return $"[bold]{escapedProduktName}[/] — [bold green]{preis:F2} EUR[/]\n([dim]Basis: [bold]{basePrice:F2}[/], Diff: [{diffColor}]{(diff >= 0 ? "+" : "")}{diff:F2}[/][/])";
        }

        private string CreateColoredPersonCell(string von)
        {
            var color = von == "Vendor" ? "blue" : "cyan";
            return $"[{color}]{von}[/]";
        }

        private string CreateColoredStatusCell(string status)
        {
            var color = status switch
            {
                "Ongoing" => "yellow",
                "Accepted" => "green",
                "Stopped" => "red",
                _ => "white"
            };
            return $"[{color}]{status}[/]";
        }

        private string CreateColoredEmotionCell(string emoji, string emotion, string reason)
        {
            var color = emotion switch
            {
                "Happy" => "green",
                "Excited" => "green",
                "Neutral" => "yellow",
                "Annoyed" => "orange3",
                "Angry" => "red",
                _ => "white"
            };

            // Escape special characters to prevent markup parsing issues
            var escapedEmoji = emoji.Replace("[", "[[").Replace("]", "]]");
            var escapedReason = reason.Replace("[", "[[").Replace("]", "]]");

            return $"[bold {color}]{escapedEmoji} {emotion}[/] — [dim]{escapedReason}[/]";
        }
    }
}