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
    private readonly List<Offer> _offers = new List<Offer>();
    private int _lastRenderHeight = 0;

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

      _offers.Add(offer);

      RenderOffers();
    }

    private void RenderOffers()
    {
      // Tabelle erstellen
      var table = new Table();
      table.Border = TableBorder.Rounded;
      table.AddColumn("Status");
      table.AddColumn("Produkt");
      table.AddColumn("Bieter");
      table.AddColumn("Preis");


      foreach (var o in _offers)
      {
        table.AddRow(o.Product.Name, $"{o.Price:0.00} €");
      }

      // Wenn wir vorher eine Tabelle gezeichnet haben, Cursor nach oben bewegen
      if (_lastRenderHeight > 0)
      {
        AnsiConsole.Cursor.MoveUp(_lastRenderHeight);
      }

      // Tabelle rendern
      AnsiConsole.Render(table);

      // Höhe der Tabelle merken, um beim nächsten Mal den Cursor wieder korrekt zu setzen
      _lastRenderHeight = table.Rows.Count + 4; // +3 für Spaltenüberschrift & Rand
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