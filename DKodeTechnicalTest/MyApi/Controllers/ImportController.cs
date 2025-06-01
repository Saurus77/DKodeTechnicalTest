using Microsoft.AspNetCore.Mvc;
using MyApi.Services;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : Controller
    {
        private readonly CsvDataService _csvDataService;
        
        public ImportController(CsvDataService csvDataService)
        {
            _csvDataService = csvDataService;
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportCsvData()
        {
            string productsCsvUrl = "https://rekturacjazadanie.blob.core.windows.net/zadanie/Products.csv";
            string inventoryCsvUrl = "https://rekturacjazadanie.blob.core.windows.net/zadanie/Inventory.csv";
            string pricesCsvUrl = "https://rekturacjazadanie.blob.core.windows.net/zadanie/Prices.csv";

            try
            {
                await _csvDataService.ProcessProductsCsvAsync(productsCsvUrl);
                await _csvDataService.ProcessInventoryCsvAsync(inventoryCsvUrl);
                await _csvDataService.ProcessPricesCsvAsync(pricesCsvUrl);

                return Ok(new { message = "CSV import successful" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
