using haggling_interfaces;
using haggling_ui.Views;
using Spectre.Console;

namespace haggling_ui
{

  public class HagglingUI
  {
    private readonly List<IOffer> _offers = new List<IOffer>();
    private int _lastRenderHeight = 0;

    public void ShowOffer(IOffer offer, IVendor vendor, ICustomer customer)
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

      RenderOffers(customer, vendor);
    }

    private void RenderOffers(ICustomer customer, IVendor vendor)
    {
      // Tabelle erstellen
      var table = new Table();
      table.Border = TableBorder.Double; // Doppelte Ränder
      table.BorderColor(Color.Fuchsia); // Fuchsia Rand
      
      // Spalten mit fuchsia Hintergrund, schwarzem Text und fett
      table.AddColumn(new TableColumn("[bold black on fuchsia]   Status   [/]").Centered());
      table.AddColumn(new TableColumn("[bold black on fuchsia]   Produkt   [/]").Centered());
      table.AddColumn(new TableColumn("[bold black on fuchsia]   Bieter   [/]").Centered());
      table.AddColumn(new TableColumn("[bold black on fuchsia]   Preis   [/]").Centered());
      table.AddColumn(new TableColumn("[bold black on fuchsia]   EmotionCustomer   [/]").Centered());
      table.AddColumn(new TableColumn("[bold black on fuchsia]   EmotionVendor   [/]").Centered());

      foreach (var o in _offers)
      {
        // Status mit verschiedenen Farben
        string statusColor = o.Status switch
        {
          OfferStatus.Accepted => "[green]",
          OfferStatus.Stopped => "[red]",
          OfferStatus.Ongoing => "[yellow]",
          _ => "[white]"
        };

        // Bieter with verschiedenen Farben
        string bieterColor = o.OfferedBy switch
        {
          PersonType.Customer => "[cyan1]",
          PersonType.Vendor => "[blue]",
          _ => "[white]"
        };

        string customerEmotion = customer.Patience.Value switch
        {
          <= 10 => "[red]😡 Genervt[/]",
          <= 30 => "[orange1]😟 Unzufrieden[/]",
          <= 70 => "[yellow]😐 Neutral[/]",
          _ => "[green]😊 Glücklich[/]"
        };

        string vendorEmotion = vendor.Patience.Value switch
        {
          <= 10 => "[red]😡 Genervt[/]",
          <= 30 => "[orange1]😟 Unzufrieden[/]",
          <= 70 => "[yellow]😐 Neutral[/]",
          _ => "[green]😊 Glücklich[/]"
        };

        table.AddRow(
          $"{statusColor}{o.Status}[/]", 
          o.Product.Name, 
          $"{bieterColor}{o.OfferedBy}[/]", 
          $"{o.Price,8:0.00} €",
          customerEmotion,
          vendorEmotion
        );
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

    public void ShowProducts(IEnumerable<IProduct> products, IVendor vendor, ICustomer customer)
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
      table.Border = TableBorder.Double; // Doppelte Ränder
      table.BorderColor(Color.Fuchsia); // Fuchsia Rand
      
      // Spalten mit fuchsia Hintergrund, schwarzem Text und fett
      table.AddColumn(new TableColumn("[bold black on fuchsia]   Name   [/]").Centered());
      table.AddColumn(new TableColumn("[bold black on fuchsia]   Typ   [/]").Centered());
      table.AddColumn(new TableColumn("[bold black on fuchsia]   Seltenheit   [/]").Centered());
      
      foreach (var product in products)
      {
        if (product.Rarity > 100 || product.Rarity < 0)
          throw new ArgumentOutOfRangeException("Die Seltenheit muss zwischen 0 und 100 liegen.");
        
        // Normale Zellen ohne rosa Hintergrund
        table.AddRow(
          product.Name, 
          product.Type.ToString(), 
          product.Rarity.Value.ToString() + "%"
        );
      }
      AnsiConsole.Write(table);
      AnsiConsole.MarkupLine($"[cyan1]{customer.Name}[/] sieht die Produkte von [blue]{vendor.Name}[/]");
    }
  }
}