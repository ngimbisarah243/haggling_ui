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
        public void Start()
        {
            // Task wird gestartet, um Events zu generieren
            Task.Run(async () =>
            {
                while (true)
                {
                    var offer = GenerateRandomOffer();
                    await _offerChannel.Writer.WriteAsync(offer);
                    await Task.Delay(_random.Next(500, 2000)); // zufällige Pause
                }
            });
        }

        // Erzeugt ein zufälliges IOffer-Objekt
        private IOffer GenerateRandomOffer()
        {
            // Generiere ein zufälliges Produkt
            var product = new ProductMock
            {
                Name = $"Produkt{_random.Next(1, 10)}",
                Type = (haggling_interfaces.ProductType)_random.Next(Enum.GetValues(typeof(haggling_interfaces.ProductType)).Length),
                Rarity = _random.Next(0, 100)
            };

            // Generiere ein zufälliges Angebot
            return new OfferMock
            {
                Status = (haggling_interfaces.OfferStatus)_random.Next(Enum.GetValues(typeof(haggling_interfaces.OfferStatus)).Length),
                Product = product,
                Price = _random.Next(10, 100),
                OfferedBy = (haggling_interfaces.PersonType)_random.Next(2)
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
