using PositionsService.EventBus;
using PositionsService.Services;

namespace PositionsService
{
    public class PositionsWorker : BackgroundService
    {
        private readonly ILogger<PositionsWorker> _logger;
        private readonly PositionService _positionService;
        private readonly RateChangeListener _rateChangeListener;

        public PositionsWorker(RateChangeListener rateChangeListener, PositionService positionService, ILogger<PositionsWorker> logger)
        {
            _rateChangeListener = rateChangeListener;
            _positionService = positionService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Positions Worker started.");

            try
            {
                _rateChangeListener.StartListening(_positionService);

                while (!stoppingToken.IsCancellationRequested)
                {
                    Console.WriteLine($"Worker running at: {DateTimeOffset.Now}");

                    await Task.Delay(10000, stoppingToken); // Simulate delay (10 seconds)
                }
            }
            catch (Exception ex)
            {
                // Log any unhandled exceptions
                _logger.LogError(ex, "An error occurred while executing the Positions Worker.");
            }
            finally
            {
                _logger.LogInformation("Positions Worker stopping.");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Safely close the rate change listener and RabbitMQ connection
                _rateChangeListener.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while stopping the RateChangeListener.");
            }

            await base.StopAsync(cancellationToken);
        }
    }
}