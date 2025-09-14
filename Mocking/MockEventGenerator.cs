// REFERENCE: haggling_interfaces.csproj gefunden — benutze Interface-Typen
// Diese Klasse erzeugt Mock-Events für die Haggling-UI.
// Sie verwendet die Interfaces aus dem Projekt haggling_interfaces.

using System.Threading.Channels;
using haggling_interfaces;

namespace Mocking
{
    // MockEventGenerator erzeugt zufällige Events und schreibt sie in einen Channel.
    public class MockEventGenerator
    {
        private readonly Channel<IOffer> _offerChannel;
        private readonly Random _random = new Random();

        // Konstruktor: Channel wird übergeben
        public MockEventGenerator(Channel<IOffer> offerChannel)
        {
            _offerChannel = offerChannel;
        }

        // Startet die Event-Generierung in einem Hintergrund-Task
        public Task Start()
        {
            // Task wird gestartet, um Events zu generieren
            return Task.Run(async () =>
            {
                // Generiere eine begrenzte Anzahl von Events (wie die ursprünglichen Test-Szenarien)
                const int maxEvents = 8; // Weniger Events für übersichtlichere Demo
                
                for (int i = 0; i < maxEvents; i++)
                {
                    var offer = GenerateRandomOffer();
                    await _offerChannel.Writer.WriteAsync(offer);
                    
                    if (i < maxEvents - 1) // Don't delay after the last event
                    {
                        await Task.Delay(_random.Next(1200, 2000)); // Langsamer für bessere Lesbarkeit
                    }
                }
                
                // Channel schließen, damit die UI weiß, dass keine weiteren Events kommen
                _offerChannel.Writer.Complete();
            });
        }

        // Erzeugt ein zufälliges IOffer-Objekt
        private IOffer GenerateRandomOffer()
        {
            // Realistische Produktnamen nach Kategorie
            var productNames = new Dictionary<ProductType, string[]>
            {
                [ProductType.Tools] = new[] { "Stahlschwert", "Eisenhammer", "Silberdolch", "Kriegsaxt", "Rüstung" },
                [ProductType.Food] = new[] { "Gewürzbrot", "Honigwein", "Käselaib", "Räucherfisch", "Apfelwein" },
                [ProductType.Clothing] = new[] { "Seidenkleid", "Ledermantel", "Wollmantel", "Leinenhose", "Samthandschuhe" },
                [ProductType.Electronics] = new[] { "Magischer Kristall", "Leuchtstein", "Kompass", "Fernrohr", "Sanduhr" },
                [ProductType.Furniture] = new[] { "Eichentisch", "Polsterstuhl", "Truhe", "Bücherregal", "Schreibpult" },
                [ProductType.Toys] = new[] { "Holzpferd", "Stoffpuppe", "Würfelset", "Puzzle", "Kreisel" },
                [ProductType.Books] = new[] { "Kochbuch", "Reiseführer", "Märchenbuch", "Geschichtsbuch", "Gedichtband" },
                [ProductType.SportsEquipment] = new[] { "Bogen", "Köcher", "Schild", "Helm", "Stiefel" },
                [ProductType.Jewelry] = new[] { "Goldring", "Silberkette", "Edelstein", "Armband", "Ohrring" },
                [ProductType.BeautyProducts] = new[] { "Parfüm", "Seife", "Spiegel", "Kamm", "Salbe" }
            };

            // Wähle zufälligen Produkttyp und Namen
            var productType = (ProductType)_random.Next(Enum.GetValues(typeof(ProductType)).Length);
            var names = productNames[productType];
            var productName = names[_random.Next(names.Length)];

            // Generiere realistische Basispreise je nach Produkttyp
            var basePrice = productType switch
            {
                ProductType.Tools => _random.Next(25, 80),
                ProductType.Food => _random.Next(5, 25),
                ProductType.Clothing => _random.Next(15, 60),
                ProductType.Electronics => _random.Next(30, 100),
                ProductType.Furniture => _random.Next(40, 120),
                ProductType.Toys => _random.Next(8, 35),
                ProductType.Books => _random.Next(3, 20),
                ProductType.SportsEquipment => _random.Next(20, 75),
                ProductType.Jewelry => _random.Next(50, 200),
                ProductType.BeautyProducts => _random.Next(10, 45),
                _ => _random.Next(10, 50)
            };

            var product = new ProductMock
            {
                Name = productName,
                Type = productType,
                Rarity = _random.Next(20, 90)
            };

            // Generiere realistische Preisvariation (±50% vom Basispreis)
            var priceVariation = _random.NextDouble() * 1.0 - 0.5; // -50% bis +50%
            var finalPrice = Math.Max(1, basePrice + (int)(basePrice * priceVariation));

            // Wähle realistischen Status (mehr Ongoing als andere)
            var status = _random.NextDouble() switch
            {
                < 0.6 => OfferStatus.Ongoing,
                < 0.8 => OfferStatus.Accepted,
                _ => OfferStatus.Stopped
            };

            return new OfferMock
            {
                Status = status,
                Product = product,
                Price = finalPrice,
                OfferedBy = (PersonType)_random.Next(2)
            };
        }
    }

    // Hilfsklasse: Mock-Implementierung von IOffer
    public class OfferMock : IOffer
    {
        // Implementierung aller Properties des IOffer-Interfaces
        // Status des Angebots (z.B. Ongoing, Accepted, Stopped)
        public OfferStatus Status { get; set; }
        // Das Produkt, auf das sich das Angebot bezieht
        public IProduct Product { get; set; } = null!;
        // Der Preis des Angebots
        public decimal Price { get; set; }
        // Wer hat das Angebot gemacht? (Kunde oder Händler)
        public PersonType OfferedBy { get; set; }
    }

    // Hilfsklasse: Mock-Implementierung von IProduct
    public class ProductMock : IProduct
    {
        // Name des Produkts
        public string Name { get; init; } = string.Empty;
        // Produkttyp (z.B. Food, Electronics, ...)
        public ProductType Type { get; init; }
        // Seltenheit des Produkts als Prozentwert
        public Percentage Rarity { get; set; }
    }
}
