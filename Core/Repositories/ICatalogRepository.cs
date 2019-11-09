using System.Collections.Generic;
using System.Threading.Tasks;
using FakeCommerce.Catalog.Responses;

namespace FakeCommerce.Catalog.Core.Repositories
{
    public interface ICatalogRepository
    {
        Task<IEnumerable<CategoryResponse>> GetCategoriesAsync();

        Task<IEnumerable<ProductCatalogResponse>> GetProductsByCategoryAsync(string categoryName, string subCategory = null, string grandCategoryName = null);
        
        Task<ProductDetailsResponse> GetProductDetailsAsync(long productId);
    }
}
