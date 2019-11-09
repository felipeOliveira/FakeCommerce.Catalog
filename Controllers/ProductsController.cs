
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
    public class ProductsController : ControllerBase
    {
        private readonly ICatalogRepository _catalogRepository;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ICatalogRepository catalogRepository, ILogger<ProductsController> logger)
        {
            _catalogRepository = catalogRepository;
            _logger = logger;
        }


        [HttpGet]
        [Route("{productId}")]
        [OpenApiOperation("GetProductDetails", "Get a product detail")]
        [ProducesResponseType(typeof(ProductDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetProductDetails(long productId)
        {
            _logger.LogInformation("Recovery details of  product {PRODUCT}", productId);
            var product = await _catalogRepository.GetProductDetailsAsync(productId);

            if (product != null)
                return Ok(product);

            return NotFound();
        }

    }
}
