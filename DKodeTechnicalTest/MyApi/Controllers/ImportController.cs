using Microsoft.AspNetCore.Mvc;
using MyApi.Services;

namespace MyApi.Controllers
{
    // Mark class as API controller and set base route
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : Controller
    {
        // Pvt readonly field for csv data service dependency
        private readonly CsvDataService _csvDataService;
        
        // Constructor with DI of csv data service
        public ImportController(CsvDataService csvDataService)
        {
            // Assign injected service to pvt field
            _csvDataService = csvDataService;
        }

        // Define http post endpoint for csv import
        [HttpPost("import")]

        // Mark import method as asnyc
        public async Task<IActionResult> ImportCsvData()
        {
            // Define vars holding files urls - hardcoded for ease - can be configurable
            const string productsCsvUrl = "https://rekturacjazadanie.blob.core.windows.net/zadanie/Products.csv";
            const string inventoryCsvUrl = "https://rekturacjazadanie.blob.core.windows.net/zadanie/Inventory.csv";
            const string pricesCsvUrl = "https://rekturacjazadanie.blob.core.windows.net/zadanie/Prices.csv";

            try
            {
                // Process cvs files using data serivce
                await _csvDataService.ProcessProductsCsvAsync(productsCsvUrl);
                await _csvDataService.ProcessInventoryCsvAsync(inventoryCsvUrl);
                await _csvDataService.ProcessPricesCsvAsync(pricesCsvUrl);

                // If all processing complete without exepctions, return ok 200
                return Ok(new { message = "CSV import successful" });
            }
            catch (System.Exception ex)
            {
                // If exceptions caught, return 500 internal server error
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
