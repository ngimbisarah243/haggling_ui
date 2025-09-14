using haggling_ui.Views;


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
            throw new NotImplementedException();
        }

        public void ShowProducts(IEnumerable<Product> products, Vendor vendor, Customer customer)
        {

        }
    }
}