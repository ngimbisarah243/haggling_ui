
// Hauptprogramm für haggling_ui
// Einstiegspunkt für die gesamte Anwendung
// Hier wird die Initialisierung und der Start der Komponenten gesteuert
// Weitere Logik kann über Adapter, Listener und Views ausgelagert werden
//
// Autor: Team
// Datum: 13.09.2025

using System;
using Spectre.Console;
using haggling_interfaces;
using haggling_ui.Views;

namespace haggling_ui
{
    public class Program
    {
        // Main-Methode: Einstiegspunkt für die Anwendung
        public static void Main(string[] args)
        {

            // TODO: Initialisierung von MockEventGenerator, Adaptern, Listenern, Views etc.
            // Beispiel: Starte nur den MockEventGenerator für Tests
            // var generator = new Mocking.MockEventGenerator(...);
            // generator.Start();
            var products = new List<Product>
            {
                new Product { Name = "Schwert", Type = ProductType.Tools, Rarity = 50 },
                new Product { Name = "Heiltrank", Type = ProductType.Food, Rarity = 20 },
                new Product { Name = "Rüstung", Type = ProductType.Clothing, Rarity = 70 }
            };

            // Vendor
            var vendor = new Vendor { Name = "Marktstand" };

            // Customer
            var customer = new Customer { Name = "Held" };

            // Offers
            var offers = new List<Offer>
            {
                new Offer { Product = products[0], Price = 15m },
                new Offer { Product = products[1], Price = 5m},
                new Offer { Product = products[2], Price = 25m}
            };

            // 2️⃣ HagglingUI erstellen
            var ui = new HagglingUI();

            // 3️⃣ Produkte anzeigen
            ui.ShowProducts(products, vendor, customer);

            // 4️⃣ Offers nacheinander anzeigen
            foreach (var offer in offers)
            {
                ui.ShowOffer(offer, vendor, customer);
                Thread.Sleep(1000); // kurze Pause zwischen den Offers
            }

            Console.ReadLine();

            Views.PrintScreen.PrintTitleScreen();
            //AnsiConsole.Markup("[underline red]Hello[/] World!");
            PrintScreen.PrintTitleScreen();
        }
    }

    public class Product : IProduct
    {
        public string Name { get; init; }
        public ProductType Type { get; init; }
        public Percentage Rarity { get; set; }
    }

    public class Offer : IOffer
    {
        public OfferStatus Status { get; set; }
        public IProduct Product { get; set; }
        public decimal Price { get; set; }
        public PersonType OfferedBy { get; set; }
    }

    public class Customer : ICustomer
    {
        public string Name { get; init; }
        public int Age { get; init; }
        public Percentage Patience { get; set; }

        public IProduct ChooseProduct(IVendor vendor)
        {
            // Wählt einfach das erste Produkt
            return vendor.Products[0];
        }

        public IOffer RespondToOffer(IOffer offer, IVendor vendor)
        {
            if (offer.Price > 50)
            {
                return new Offer
                {
                    Status = OfferStatus.Ongoing,
                    Product = offer.Product,
                    Price = offer.Price - 10,
                    OfferedBy = PersonType.Customer
                };
            }

            offer.Status = OfferStatus.Accepted;
            return offer;
        }

        public void AcceptTrade(IOffer offer) =>
            Console.WriteLine($"{Name} hat das Angebot akzeptiert!");

        public void StopTrade() =>
            Console.WriteLine($"{Name} hat den Handel abgebrochen.");
    }

    public class Vendor : IVendor
    {
        public string Name { get; init; }
        public int Age { get; init; }
        public Percentage Patience { get; set; }
        public IProduct[] Products { get; init; }

        public IOffer GetStartingOffer(IProduct product, ICustomer customer)
        {
            return new Offer
            {
                Status = OfferStatus.Ongoing,
                Product = product,
                Price = 100,
                OfferedBy = PersonType.Vendor
            };
        }

        public IOffer RespondToOffer(IOffer offer, ICustomer customer)
        {
            if (offer.Price >= 80)
            {
                offer.Status = OfferStatus.Accepted;
            }
            else
            {
                return new Offer
                {
                    Status = OfferStatus.Ongoing,
                    Product = offer.Product,
                    Price = offer.Price + 5,
                    OfferedBy = PersonType.Vendor
                };
            }
            return offer;
        }

        public void AcceptTrade(IOffer offer) =>
            Console.WriteLine($"{Name} hat den Handel abgeschlossen!");

        public void StopTrade() =>
            Console.WriteLine($"{Name} hat den Handel abgebrochen.");
    }

}