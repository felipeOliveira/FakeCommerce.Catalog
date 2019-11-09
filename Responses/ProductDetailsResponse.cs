using System;
using System.Collections.Generic;
using System.Linq;

namespace FakeCommerce.Catalog.Responses
{
    public class ProductDetailsResponse
    {
        public ProductDetailsResponse()
        {
            Colors = new string[0];
            Images = new string[0];
            Sizes = new string[0];
            Features = new Dictionary<string, string>();

        }
        public string Name { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public IEnumerable<string> Colors { get; set; }

        public IEnumerable<string> Sizes { get; set; }

        public IEnumerable<string> Images { get; set; }

        public IDictionary<string,string> Features { get; set; }

    }
}
