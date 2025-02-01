using Microsoft.AspNetCore.Mvc;
using RatesService.Services;

namespace RatesService.Controllers
{
    [Route("api/[controller]")] // automatically the URL will be api/Rates,
                                // where rates comes automatically from the controlle name
    [ApiController]
    public class RatesController : Controller
    {
        private readonly RateFetcherService _rateFetcherService;

        public RatesController(RateFetcherService rateFetcherService)
        {
            _rateFetcherService = rateFetcherService;
        }

        [HttpGet("getlatestrates")]
        public async Task<IActionResult> GetRates()
        {
            var rates = await _rateFetcherService.GetRatesAsync();
            return Ok(rates);
        }
    }
}