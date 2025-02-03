using Microsoft.AspNetCore.Mvc;
using RatesService.EventBus;
using RatesService.Services;

namespace RatesService.Controllers
{
    [Route("api/[controller]")] // automatically the URL will be api/Rates,
                                // where rates comes automatically from the controller name
    [ApiController]
    public class RatesController : Controller
    {
        private readonly RateFetcherService _rateFetcherService;
        private readonly IEventBus _eventBus;

        public RatesController(RateFetcherService rateFetcherService, IEventBus eventBus)
        {
            _rateFetcherService = rateFetcherService;
            _eventBus = eventBus;
        }

        [HttpGet("getlatestrates")]
        public async Task<IActionResult> GetRates()
        {
            try
            {
                var ratesResponse = await _rateFetcherService.GetRatesAsync();
                
                if (ratesResponse.IsSuccessStatusCode)
                {
                    var content = await ratesResponse.Content.ReadAsStringAsync();
                    return Content(content, "text/plain");
                }

                return StatusCode((int)ratesResponse.StatusCode, await ratesResponse.Content.ReadAsStringAsync());
            }
            catch (Exception ex) 
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}