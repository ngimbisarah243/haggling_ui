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

      RenderOffers();
    }

    private Emotion GetEmotion(IOffer o)
    {
      if (o.Status == OfferStatus.Stopped)
        return Emotion.Angry;

      if (o.Status == OfferStatus.Accepted)
        return Emotion.Happy;

      if (o.Status == OfferStatus.Ongoing && o.Price < 10)
        return Emotion.Annoyed;

      return Emotion.Neutral;
    }

    private void RenderOffers()
    {
      // Tabelle erstellen
      var table = new Table();
      table.Border = TableBorder.Double; // Doppelte Ränder
      table.BorderColor(Color.HotPink); // Pinker Rand

      // Spalten mit rosa Hintergrund, schwarzem Text und fett
      table.AddColumn(new TableColumn("[bold black on pink1]Status[/]"));
      table.AddColumn(new TableColumn("[bold black on pink1]Produkt[/]"));
      table.AddColumn(new TableColumn("[bold black on pink1]Bieter[/]"));
      table.AddColumn(new TableColumn("[bold black on pink1]Preis[/]"));
      table.AddColumn(new TableColumn("[bold black on pink1]Emotion[/]"));


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

        // Bieter mit verschiedenen Farben
        string bieterColor = o.OfferedBy switch
        {
          PersonType.Customer => "[cyan1]",
          PersonType.Vendor => "[blue]",
          _ => "[white]"
        };

        Emotion emotion = GetEmotion(o);

        string emotionColor = emotion switch
        {
          Emotion.Angry => "[red]",
          Emotion.Annoyed => "[orange1]",
          Emotion.Excited => "[yellow]",
          Emotion.Happy => "[green]",
          Emotion.Neutral => "[grey]",
          _ => "[white]"
        };

        string emotionIcon = emotion switch
        {
          Emotion.Angry => ">:[",
          Emotion.Annoyed => ">:/",
          Emotion.Excited => "🤩",
          Emotion.Happy => "😊",
          Emotion.Neutral => "-_-",
          _ => "❓"
        };
        string emotionOutput = $"{emotionColor}{emotionIcon} {emotion}[/]";


        table.AddRow(
          $"{statusColor}{o.Status}[/]",
          o.Product.Name,
          $"{bieterColor}{o.OfferedBy}[/]",
          $"{o.Price:0.00} €",
          emotionOutput
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
      table.BorderColor(Color.HotPink); // Pinker Rand

      // Spalten mit rosa Hintergrund, schwarzem Text und fett
      table.AddColumn(new TableColumn("[bold black on pink1]Name[/]"));
      table.AddColumn(new TableColumn("[bold black on pink1]Typ[/]"));
      table.AddColumn(new TableColumn("[bold black on pink1]Seltenheit[/]"));

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