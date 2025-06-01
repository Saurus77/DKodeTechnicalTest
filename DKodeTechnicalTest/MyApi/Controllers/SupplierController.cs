using Microsoft.AspNetCore.Mvc;
using MyApi.Repositories;

namespace MyApi.Controllers
{
    // Mark class as API controller and set base route
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : Controller
    {
        // Pvt readonly field for repo dependency
        private readonly ISupplierInfoRepository _repository;

        // Constructor with suuplier interface DI
        public SupplierController(ISupplierInfoRepository repository)
        {
            // Assign repo to pvt field
            _repository = repository;
        }

        // Define http get endpoint for supplier info
        [HttpGet("suppliers/{supplierName}/supplierInfo")]

        // Mark supplier info method as async
        public async Task<IActionResult> GetSupplierInfo(string supplierName)
        {
            // Call supplier info method from supplier info repo to retrieve data from db
            var info = await _repository.GetSupplierInfoDtoAsync(supplierName);

            // Return null 404 if nothing found
            if (info == null) { return NotFound(); }

            // Return 200 ok if info found
            return Ok(info);
        }
    }
}
