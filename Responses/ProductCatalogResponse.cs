using StackExchange.Redis;

namespace FakeCommerce.Catalog.Responses
{
    public class ProductCatalogResponse
    {
        public string Name { get; set; }

        public string Image { get; set; }

        public string Price { get; set; }
    }
}
