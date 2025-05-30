using Microsoft.AspNetCore.Mvc;
using MyApi.Helpers;
using System.Net.Http;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : Controller
    {
        private readonly DataBaseHelper _dataBaseHelper;
        private readonly IHttpClientFactory _httpClientFactory;

        public ImportController(DataBaseHelper dataBaseHelper, IHttpClientFactory httpClientFactory)
        {
            _dataBaseHelper = dataBaseHelper;
            _httpClientFactory = httpClientFactory;
        }

    //    [HttpPost("import")]
   //     public async Task<IActionResult> ImportCsvData()
   //     {

  //      }
        
    }
}
