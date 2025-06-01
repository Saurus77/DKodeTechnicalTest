using Microsoft.AspNetCore.Mvc;
using MyApi.Repositories;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : Controller
    {
        private readonly ISupplierInfoRepository _repository;
        public SupplierController(ISupplierInfoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("suppliers/{supplierName}/supplierInfo")]
        public async Task<IActionResult> GetSupplierInfo(string supplierName)
        {
            var info = await _repository.GetSupplierInfoDtoAsync(supplierName);

            if (info == null) { return NotFound(); }

            return Ok(info);
        }
    }
}
