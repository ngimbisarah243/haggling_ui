using System;
using haggling_interfaces;
using Spectre.Console;

namespace haggling_ui.Views
{
    public class PrintScreen
    {
        public static void PrintTitleScreen()
        {
            Console.Clear();
            AnsiConsole.Write(
                new FigletText("THE BASAR")
                    .Centered()
                    .Color(Color.Gold1));

            string[] menuItems = { "Start", "Optionen", "Beenden" };
            int selectedIndex = 0;

            ConsoleKey key;
            do
            {
                DrawMenu(menuItems, selectedIndex);
                var input = Console.ReadKey(true);
                key = input.Key;

                if (key == ConsoleKey.UpArrow)
                    selectedIndex = (selectedIndex - 1 + menuItems.Length) % menuItems.Length;
                else if (key == ConsoleKey.DownArrow)
                    selectedIndex = (selectedIndex + 1) % menuItems.Length;

            } while (key != ConsoleKey.Enter);

            if (selectedIndex == 0)
                RunGame();
            else if (selectedIndex == 1)
                ShowOptions();
            else if (selectedIndex == 2)
                ExitGame();
        }

        private static void DrawMenu(string[] items, int selected)
        {
            Console.SetCursorPosition(0, 10);
            for (int i = 0; i < items.Length; i++)
            {
                if (i == selected)
                    AnsiConsole.MarkupLine($"[yellow]>> {items[i]}[/]");
                else
                    AnsiConsole.MarkupLine($"   {items[i]}");
            }
        }

        private static void RunGame()
        {
            Console.Clear();

            var customer = new Customer { Name = "Alice", Age = 30, Patience = 80 };
            var vendor = new Vendor
            {
                Name = "Bob",
                Age = 45,
                Patience = 70,
                Products = new IProduct[]
                {
            new Product { Name = "Goldring", Type = ProductType.Jewelry, Rarity = 90 }
                }
            };

            var display = new ConsoleDisplay();
            display.ShowProducts(vendor.Products, vendor, customer);

            var product = customer.ChooseProduct(vendor);
            var offer = vendor.GetStartingOffer(product, customer);

            do
            {
                display.ShowOffer(offer, vendor, customer);

                if (offer.OfferedBy == PersonType.Vendor)
                    offer = customer.RespondToOffer(offer, vendor);
                else
                    offer = vendor.RespondToOffer(offer, customer);

            } while (offer.Status == OfferStatus.Ongoing);

            if (offer.Status == OfferStatus.Accepted)
            {
                customer.AcceptTrade(offer);
                vendor.AcceptTrade(offer);
            }
            else
            {
                customer.StopTrade();
                vendor.StopTrade();
            }

            Console.ReadKey(true);
            PrintTitleScreen();
        }



        private static void ShowOptions()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan]Optionen (noch nicht implementiert)[/]");
            Console.ReadKey(true);
            PrintTitleScreen();
        }

        private static void ExitGame()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold red]Spiel beendet.[/]");
            Environment.Exit(0);
        }

    }

public class ConsoleDisplay : IDisplay
{
    public void ShowProducts(IProduct[] products, IVendor vendor, ICustomer customer)
    {
        AnsiConsole.MarkupLine($"[bold]{vendor.Name}[/] bietet folgende Produkte an:");
        foreach (var product in products)
        {
            AnsiConsole.MarkupLine($"- {product.Name} ({product.Type}, Rarity: {product.Rarity.Value}%)");
        }
    }

    public void ShowOffer(IOffer offer, IVendor vendor, ICustomer customer)
    {
        string who = offer.OfferedBy == PersonType.Customer ? customer.Name : vendor.Name;
        AnsiConsole.MarkupLine($"[yellow]{who}[/] bietet [green]{offer.Price}[/] für {offer.Product.Name} an. (Status: {offer.Status})");
    }
}

}
