using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeCommerce.Catalog.Core.Repositories;
using FakeCommerce.Catalog.Responses;
using StackExchange.Redis;

namespace FakeCommerce.Catalog.Infrastructure.Data
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly CatalogDataSource _dataSource;

        public CatalogRepository(CatalogDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IEnumerable<CategoryResponse>> GetCategoriesAsync()
        {
            var categoriesKeys = await _dataSource.Database.SetMembersAsync("cat");
            if (!categoriesKeys.Any())
            {
                return null;
            }

            List<CategoryResponse> categories = new List<CategoryResponse>(categoriesKeys.Length);

            for (int i = 0; i < categoriesKeys.Length; i++)
            {
                var category = new CategoryResponse()
                {
                    Id = categoriesKeys[i],
                    Name = await _dataSource.Database.StringGetAsync($"cat:{categoriesKeys[i]}")
                };

                categories.Add(category);

                var subcategoriesKey = await _dataSource.Database.SetMembersAsync($"cat:{category.Id}:subcat");
                if (subcategoriesKey.Any())
                {
                    List<CategoryResponse> subCategories = new List<CategoryResponse>(subcategoriesKey.Length);
                    category.Categories = subCategories;

                    for (int j = 0; j < subcategoriesKey.Length; j++)
                    {
                        var subCategory = new CategoryResponse()
                        {
                            Id = subcategoriesKey[j],
                            Name = await _dataSource.Database.StringGetAsync($"cat:{subcategoriesKey[j]}")
                        };

                        subCategories.Add(subCategory);

                        var grandChildCategoriesKey = await _dataSource.Database.SetMembersAsync($"cat:{category.Id}:subcat:{subCategory.Id}");

                        if (grandChildCategoriesKey.Any())
                        {
                            var grandChildCategories = new List<CategoryResponse>();
                            subCategory.Categories = grandChildCategories;

                            for (int k = 0; k < grandChildCategoriesKey.Length; k++)
                            {
                                var grandChildCategory = new CategoryResponse()
                                {
                                    Id = grandChildCategoriesKey[k],
                                    Name = await _dataSource.Database.StringGetAsync($"cat:{grandChildCategoriesKey[k]}")
                                };
                                grandChildCategories.Add(grandChildCategory);
                            }
                        }
                    }
                }
            }
            return categories;
        }

        public async Task<ProductDetailsResponse> GetProductDetailsAsync(long productId)
        {
            var productInfo = await _dataSource.Database.HashGetAllAsync($"prod:{productId}");
            if (productInfo is null) return null;

            var product = new ProductDetailsResponse();
            foreach (var info in productInfo)
            {
                if ("Name".Equals(info.Name, StringComparison.OrdinalIgnoreCase))
                    product.Name = info.Value;
                else if ("Description".Equals(info.Name, StringComparison.OrdinalIgnoreCase))
                    product.Description = info.Value;
                else if ("Price".Equals(info.Name, StringComparison.OrdinalIgnoreCase))
                    if (double.TryParse(info.Value, out var price))
                        product.Price = price;
            }

            var imagesTask = _dataSource.Database.SetMembersAsync($"prod:{productId}:images");
            var colorsTask = _dataSource.Database.SetMembersAsync($"prod:{productId}:colors");
            var sizesTask = _dataSource.Database.SetMembersAsync($"prod:{productId}:sizes");
            var featuresTask = _dataSource.Database.HashGetAllAsync($"prod:{productId}:features");

            Task.WaitAll(imagesTask, colorsTask, sizesTask, featuresTask);

            if (imagesTask.Result != null)
                product.Images = Array.ConvertAll(imagesTask.Result, img => img.ToString());

            if (colorsTask.Result != null)
                product.Colors = Array.ConvertAll(colorsTask.Result, color => color.ToString());

            if (sizesTask.Result != null)
                product.Sizes = Array.ConvertAll(sizesTask.Result, size => size.ToString());

            foreach (var feature in featuresTask.Result)
            {
                product.Features.Add(feature.Name, feature.Value);
            }

            return product;
        }

        public async Task<IEnumerable<ProductCatalogResponse>> GetProductsByCategoryAsync(string categoryName, string subCategory = null, string grandCategoryName = null)
        {
            RedisValue[] productKeys = new RedisValue[0];
            if (!string.IsNullOrEmpty(subCategory) && !string.IsNullOrEmpty(grandCategoryName))
            {
                productKeys = await _dataSource.Database.SetMembersAsync($"cat:{categoryName}:subcat:{subCategory}:subcat:{grandCategoryName}:prods");
            }
            else if (!string.IsNullOrEmpty(subCategory))
            {
                productKeys = await _dataSource.Database.SetMembersAsync($"cat:{categoryName}:subcat:{subCategory}:prods");
            }
            else
            {
                productKeys = await _dataSource.Database.SetMembersAsync($"cat:{categoryName}:prods");
            }

            List<ProductCatalogResponse> products = new List<ProductCatalogResponse>(productKeys.Length);

            for (int i = 0; i < productKeys.Length; i++)
            {
                var productInfo = await _dataSource.Database.HashGetAsync($"prod:{productKeys[i]}", new RedisValue[] { "Name", "Price" });

                if (productInfo is null) continue;

                products.Add(new ProductCatalogResponse()
                {
                    Name = productInfo[0],
                    Price = productInfo[1]
                });
            }

            return products;
        }
    }
}
