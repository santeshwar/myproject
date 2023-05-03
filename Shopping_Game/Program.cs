namespace Shopping_Game
{
    class Program
    {
        public static IList<Product> Catalog = new List<Product>() {
                new Product() { SKU = "ipd", Name = "Super iPad", Price = 549.99 },
                new Product() { SKU = "mbp", Name = "MacBook Pro", Price = 1399.99 },
                new Product() { SKU = "atv", Name = "Apple TV", Price = 109.50 },
                new Product() { SKU = "vga", Name = "VGA Adapter", Price = 30.00 }
            };
        static void Main(string[] args)
        {
            PricingRules pricingRules = OpeningDayRule;
            Checkout co = new Checkout(pricingRules);
            co.Scan("atv");
            co.Scan("atv");
            co.Scan("atv");
            co.Scan("mbp");
            co.total();
        }

        delegate double PricingRules(List<Product> product);

        class Checkout
        {
            public static List<Product> cartItem;
            public PricingRules _pricingRules;
            public Checkout(PricingRules pricingRules)
            {
                _pricingRules = pricingRules;
            }
            public void Scan(string itm)
            {
                Product product = Catalog.Single(x => x.SKU == itm);
                cartItem.Add(product);
            }
            public void total()
            {
                double totalAmount = _pricingRules(cartItem);
                Console.WriteLine("Total expected: $" + totalAmount);
            }

        }
        public static double OpeningDayRule(List<Product> products)
        {
            var cart = products.GroupBy(x => x.SKU)
                               .Join(Catalog, crt => crt.Key, cat => cat.SKU, (crt, cat) =>
                               new
                               {
                                   Item = crt.Key,
                                   Count = crt.Count(),
                                   unitPrice = cat.Price
                               });
            double amount = 0;
            foreach (var itm in cart)
            {
                if (itm.Item == "atv")
                    amount += itm.unitPrice * ((itm.Count / 3) * 2 + (itm.Count % 3));
                else
                if (itm.Item == "ipd")
                {
                    double tmpPrice = itm.Count > 4 ? 499.99 : itm.unitPrice;
                    amount += tmpPrice * itm.Count;
                }
                else
                if (itm.Item == "mpb")
                    amount += itm.unitPrice * itm.Count;
                else
                if (itm.Item == "vga")
                {
                    var mbpItm = cart.SingleOrDefault(x => x.Item == "mbp");
                    if (mbpItm != null)
                        amount += itm.unitPrice * (itm.Count - mbpItm.Count);
                    else
                        amount += itm.unitPrice * itm.Count;
                }
            }
            return amount;
        }
    }

    public class Product
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }
}