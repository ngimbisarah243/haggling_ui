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
      var table = new Table();
      table.Border = TableBorder.Double;
      table.BorderColor(Color.HotPink);

      table.AddColumn(new TableColumn("[bold black on pink1]Status[/]"));
      table.AddColumn(new TableColumn("[bold black on pink1]Produkt[/]"));
      table.AddColumn(new TableColumn("[bold black on pink1]Bieter[/]"));
      table.AddColumn(new TableColumn("[bold black on pink1]Preis[/]"));
      table.AddColumn(new TableColumn("[bold black on pink1]EmotionCustomer[/]"));
      table.AddColumn(new TableColumn("[bold black on pink1]EmotionVendor[/]"));


      foreach (var o in _offers)
      {
        string statusColor = o.Status switch
        {
          OfferStatus.Accepted => "[green]",
          OfferStatus.Stopped => "[red]",
          OfferStatus.Ongoing => "[yellow]",
          _ => "[white]"
        };

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

        // Emotion für Vendor
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
            $"{o.Price:0.00} €",
            customerEmotion,
            vendorEmotion
        );
      }

      if (_lastRenderHeight > 0)
        AnsiConsole.Cursor.MoveUp(_lastRenderHeight);

      AnsiConsole.Render(table);
      _lastRenderHeight = table.Rows.Count + 4;
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
      table.BorderColor(Color.HotPink); // Pinker Rand

      // Spalten mit rosa Hintergrund, schwarzem Text und fett
      table.AddColumn(new TableColumn("[bold black on pink1]Name[/]"));
      table.AddColumn(new TableColumn("[bold black on pink1]Typ[/]"));
      table.AddColumn(new TableColumn("[bold black on pink1]Seltenheit[/]"));
      table.AddColumn(new TableColumn("[bold black on pink1]Emotion[/]"));


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