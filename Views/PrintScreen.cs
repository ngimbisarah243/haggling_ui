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
            AnsiConsole.MarkupLine("[bold green]Die Verhandlung beginnt![/]");

            var table = new Table();
            table.AddColumn("[bold cyan]Runde[/]");
            table.AddColumn("[bold yellow]Kunde[/]");
            table.AddColumn("[bold green]Verkäufer[/]");

            var rnd = new Random();
            int customerOffer = rnd.Next(10, 20);
            int vendorOffer = rnd.Next(25, 40);

            for (int round = 1; round <= 5; round++)
            {
                table.AddRow(
                    round.ToString(),
                    $"Bob bietet {customerOffer} Coins",
                    $"Bobby fordert {vendorOffer} Coins");

                customerOffer += rnd.Next(1, 4);
                vendorOffer -= rnd.Next(1, 4);
            }

            AnsiConsole.Write(table);

            bool deal = rnd.Next(0, 2) == 1;
            Console.ReadKey();
            PrintEndScreen(deal, (customerOffer + vendorOffer) / 2);

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

        private static void PrintEndScreen(bool deal, int finalPrice)
        {
            Console.Clear();
            if (deal)
            {
                AnsiConsole.MarkupLine("[bold green]Verhandlung erfolgreich![/]");
                AnsiConsole.MarkupLine($"[yellow]Endpreis:[/] {finalPrice} Coins");
                AnsiConsole.MarkupLine("[green]Der Kunde und der Verkäufer sind zufrieden![/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red]Verhandlung gescheitert![/]");
                AnsiConsole.MarkupLine("[red]Der Kunde und der Verkäufer konnten sich nicht einigen.[/]");
            }

            AnsiConsole.MarkupLine("\nDrücke eine beliebige Taste, um zum Hauptmenü zurückzukehren...");
            Console.ReadKey(true);
            PrintTitleScreen();
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
