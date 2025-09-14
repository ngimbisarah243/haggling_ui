using haggling_ui.Views;

namespace haggling_ui
{
    public interface IUi
    {
        void ShowProducts(IEnumerable<Product> products, Vendor vendor, Consumer consumer);
        void ShowOffer(Offer offer, Vendor vendor, Consumer consumer);
    }

    public class HagglingUI : IUi
    {
        public void ShowProducts(IEnumerable<Product> products, Vendor vendor, Consumer consumer)
        {

        }
    }
}