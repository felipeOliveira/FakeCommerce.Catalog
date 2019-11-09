
using System.Collections.Generic;

namespace FakeCommerce.Catalog.Responses
{
    public class CategoryResponse
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<CategoryResponse> Categories { get; set; }
    }
}
