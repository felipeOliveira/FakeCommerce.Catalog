using System.Collections.Generic;
using System.Threading.Tasks;
using FakeCommerce.Catalog.Core.Repositories;
using FakeCommerce.Catalog.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace FakeCommerce.Catalog.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Produces("application/json")]
    public class CategoriesController : ControllerBase
    {

        private readonly ILogger<CategoriesController> _logger;
        private readonly ICatalogRepository _repository;

        public CategoriesController(ILogger<CategoriesController> logger, ICatalogRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        [Route("")]
        [OpenApiOperation("GetCategory", description:"List all categories and their sub categories")]
        [ProducesResponseType(200,Type = typeof(CategoryResponse))]
        [ProducesResponseType(500)]
        public Task<IEnumerable<CategoryResponse>> GetCategories()
        {
            _logger.LogInformation("retrive categories");
            return _repository.GetCategoriesAsync();
        }

        [HttpGet]
        [OpenApiOperation("GetProductsByCategory", description:"List all products of specific category")]
        [Route("{category}/products")]
        [ProducesResponseType(typeof(ProductCatalogResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IEnumerable<ProductCatalogResponse>> GetProductsByCategory(string category)
        {
            _logger.LogInformation("List products of Category {CATEGORY}", category);
            return await _repository.GetProductsByCategoryAsync(category);
        }

        [HttpGet]
        [OpenApiOperation("GetProductsbyCategory", description: "List all products of specific category and sub category")]
        [Route("{category}/{subcategory}/products")]
        [ProducesResponseType(typeof(ProductCatalogResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IEnumerable<ProductCatalogResponse>> GetProductsByCategory(string category, string subcategory)
        {
            _logger.LogInformation("List products of Category {CATEGORY}/{SUBCATEGORY}", category, subcategory);
            return await _repository.GetProductsByCategoryAsync(category, subcategory);
        }

        [HttpGet]
        [OpenApiOperation("GetProductsbyCategory", description: "List all products of specific")]
        [Route("{category}/{subcategory}/{grandCategory}/products")]
        [ProducesResponseType(typeof(ProductCatalogResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IEnumerable<ProductCatalogResponse>> GetProductsByCategory(string category, string subcategory, string grandCategory)
        {
            _logger.LogInformation("List products of Category {CATEGORY}/{SUBCATEGORY}/{GRANDCATEGORY}", category, subcategory, grandCategory);
            return await _repository.GetProductsByCategoryAsync(category, subcategory, grandCategory);
        }
    }
}
