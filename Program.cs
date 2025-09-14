using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using haggling_interfaces;
using haggling_ui.Views;
using Mocking;

namespace haggling_ui
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Channel für Angebote erstellen
            var offerChannel = Channel.CreateUnbounded<IOffer>();
            
            // MockEventGenerator starten für kontinuierliche Events
            var generator = new MockEventGenerator(offerChannel);
            var generatorTask = generator.Start();
            
            // UI starten (beide Tasks parallel laufen lassen)
            var ui = new SpectreConsoleUI(offerChannel);
            var uiTask = ui.RunAsync();
            
            // Warten bis der Generator fertig ist und dann die UI beenden lassen
            await generatorTask;
            await uiTask;
        }
    }
}
