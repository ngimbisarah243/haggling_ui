using haggling_ui.Views;
using Spectre.Console;


namespace haggling_ui
{
  public interface IUi
  {
    void ShowProducts(IEnumerable<Product> products, Vendor vendor, Customer customer);
    void ShowOffer(Offer offer, Vendor vendor, Customer customer);
  }

  public class HagglingUI : IUi
  {
    public void ShowOffer(Offer offer, Vendor vendor, Customer customer)
    {
      if (offer == null)
      {
        AnsiConsole.MarkupLine("[red]Kein Angebot verfügbar. [/]");
        return;
      }
      if (vendor == null)
      {
        throw new ArgumentNullException("Keinen VERKÄUFER angegeben.");
      }
      if (customer == null)
      {
        throw new ArgumentNullException("Keinen KUNDEN angegeben");
      }



      throw new NotImplementedException();
    }

    public void ShowProducts(IEnumerable<Product> products, Vendor vendor, Customer customer)
    {
      if (products == null)
      {
        AnsiConsole.MarkupLine("[red]Keine Produkte verfügbar.[/]");
        return;
      }

      if (vendor == null)
        throw new ArgumentNullException("Keinen VERKÄUFER angegeben.");
      if (customer == null)
        throw new ArgumentNullException("Keinen KUNDE angegeben");

      var table = new Table();
      table.Border = TableBorder.Rounded;
      table.AddColumn("Name");
      table.AddColumn("Typ");
      table.AddColumn("Seltenheit");
      foreach (var product in products)
      {
        if (product.Rarity > 100 || product.Rarity < 0)
          throw new ArgumentOutOfRangeException("Die Seltenheit muss zwischen 0 und 100 liegen.");
        table.AddRow(product.Name, product.Type.ToString(), product.Rarity.Value.ToString() + '%');
      }
      AnsiConsole.Write(table);
      AnsiConsole.MarkupLine($"[green]{customer.Name}[/] sieht die Produkte von [blue]{vendor.Name}[/]");
    }
  }
}

/*
using (var live = AnsiConsole.Live(table))
{
    live.Start();

    foreach (var offer in offers)
    {
        table.AddRow(offer.Product.Name, $"{offer.Price:0.00} €", offer.Vendor.Name, offer.Quantity.ToString());
        live.Refresh(); // Tabelle wird sofort aktualisiert
        Thread.Sleep(500); // optional: kleine Pause für Demo
    }
}
*/