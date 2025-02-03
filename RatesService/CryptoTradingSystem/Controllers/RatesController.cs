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
            var rates = await _rateFetcherService.GetRatesAsync();
            return Ok(rates);
        }
    }
}