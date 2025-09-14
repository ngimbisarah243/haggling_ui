using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using haggling_interfaces;
using haggling_ui.Views;

namespace haggling_ui
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var offerChannel = Channel.CreateUnbounded<IOffer>();
            var ui = new SpectreConsoleUI(offerChannel);
            var uiTask = ui.RunAsync();
            
            await RunSuccessfulDeal(offerChannel);
            
            await RunFailedDeal(offerChannel);
            
            offerChannel.Writer.Complete();
            
            await uiTask;
        }

        // -------------------------------
        // Szenario 1: Erfolgreicher Deal
        // -------------------------------
        private static async Task RunSuccessfulDeal(Channel<IOffer> channel)
        {
            IProduct sword = new TestProduct("Stahlschwert", ProductType.Tools, new Percentage(60));
            IVendor vendor = new TestVendor("Schmied", 50, new Percentage(70), new[] { sword });
            ICustomer customer = new TestCustomer("Ritter", 25, new Percentage(60));

            var offer = vendor.GetStartingOffer(sword, customer);
            await channel.Writer.WriteAsync(offer);
            await Task.Delay(1000);
            
          
            offer = customer.RespondToOffer(offer, vendor);
            await channel.Writer.WriteAsync(offer);
            await Task.Delay(1000);

            offer = vendor.RespondToOffer(offer, customer);
            await channel.Writer.WriteAsync(offer);
            await Task.Delay(1000);
            
            offer = customer.RespondToOffer(offer, vendor);
            await channel.Writer.WriteAsync(offer);
            await Task.Delay(1000);
        }

        // ---------------------------------
        // Szenario 2: Gescheiterte Verhandlung
        // ---------------------------------
        private static async Task RunFailedDeal(Channel<IOffer> channel)
        {
            IProduct vase = new TestProduct("Porzellanvase", ProductType.Clothing, new Percentage(80));
            IVendor vendor = new TestVendor("Händler", 45, new Percentage(50), new[] { vase });
            ICustomer customer = new TestCustomer("Bauer", 30, new Percentage(40));

            var offer = vendor.GetStartingOffer(vase, customer);
            await channel.Writer.WriteAsync(offer);
            await Task.Delay(1000);
            
            offer = customer.RespondToOffer(new TestOffer
            {
                Product = vase,
                Price = 5m,
                Status = OfferStatus.Ongoing,
                OfferedBy = PersonType.Customer
            }, vendor);
            await channel.Writer.WriteAsync(offer);
            await Task.Delay(1000);

            offer = new TestOffer
            {
                Product = vase,
                Price = 5m,
                Status = OfferStatus.Stopped,
                OfferedBy = PersonType.Vendor
            };
            await channel.Writer.WriteAsync(offer);
            await Task.Delay(1000);
        }
    }
}
