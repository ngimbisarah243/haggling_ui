using System;
using haggling_interfaces;

namespace haggling_ui
{
    public class TestProduct : IProduct
    {
        public string Name { get; init; }
        public ProductType Type { get; init; }
        public Percentage Rarity { get; set; }

        public TestProduct(string name, ProductType type, Percentage rarity)
        {
            Name = name;
            Type = type;
            Rarity = rarity;
        }
    }

    public class TestOffer : IOffer
    {
        public OfferStatus Status { get; set; }
        public IProduct Product { get; set; }
        public decimal Price { get; set; }
        public PersonType OfferedBy { get; set; }
    }
    
    public class TestCustomer : ICustomer
    {
        public string Name { get; init; }
        public int Age { get; init; }
        public Percentage Patience { get; set; }

        public TestCustomer(string name, int age, Percentage patience)
        {
            Name = name;
            Age = age;
            Patience = patience;
        }

        public IProduct ChooseProduct(IVendor vendor) => vendor.Products[0];

        public IOffer RespondToOffer(IOffer offer, IVendor vendor)
        {
            if (offer.Price > 40)
                return new TestOffer { Product = offer.Product, Price = offer.Price - 10, Status = OfferStatus.Ongoing, OfferedBy = PersonType.Customer };

            if (offer.Price <= 25)
                return new TestOffer { Product = offer.Product, Price = offer.Price, Status = OfferStatus.Accepted, OfferedBy = PersonType.Customer };

            return new TestOffer { Product = offer.Product, Price = offer.Price - 5, Status = OfferStatus.Ongoing, OfferedBy = PersonType.Customer };
        }

        public void AcceptTrade(IOffer offer) =>
            Console.WriteLine($"{Name} hat den Handel akzeptiert!");

        public void StopTrade() =>
            Console.WriteLine($"{Name} hat den Handel abgebrochen.");
    }
    
    public class TestVendor : IVendor
    {
        public string Name { get; init; }
        public int Age { get; init; }
        public Percentage Patience { get; set; }
        public IProduct[] Products { get; init; }

        public TestVendor(string name, int age, Percentage patience, IProduct[] products)
        {
            Name = name;
            Age = age;
            Patience = patience;
            Products = products;
        }

        public IOffer GetStartingOffer(IProduct product, ICustomer customer) =>
            new TestOffer { Product = product, Price = 50, Status = OfferStatus.Ongoing, OfferedBy = PersonType.Vendor };

        public IOffer RespondToOffer(IOffer offer, ICustomer customer)
        {
            if (offer.Price >= 30)
                return new TestOffer { Product = offer.Product, Price = offer.Price, Status = OfferStatus.Accepted, OfferedBy = PersonType.Vendor };

            if (offer.Price < 20)
                return new TestOffer { Product = offer.Product, Price = offer.Price + 5, Status = OfferStatus.Ongoing, OfferedBy = PersonType.Vendor };

            return new TestOffer { Product = offer.Product, Price = offer.Price, Status = OfferStatus.Accepted, OfferedBy = PersonType.Vendor };
        }

        public void AcceptTrade(IOffer offer) =>
            Console.WriteLine($"{Name} hat den Handel abgeschlossen!");

        public void StopTrade() =>
            Console.WriteLine($"{Name} hat den Handel abgebrochen.");
    }
}
