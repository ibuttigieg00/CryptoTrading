using PositionsService.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace PositionsService.Services
{
    public class PositionService
    {
        private readonly string _filePath;
        private readonly ILogger<PositionService> _logger;
        private List<Position> _positions;

        public PositionService(IConfiguration configuration, ILogger<PositionService> logger)
        {
            _filePath = configuration["FileSettings:PositionsFilePath"]!;
            _logger = logger;
            _positions = new List<Position>();

            LoadPositionsFromFile();
        }

        private void LoadPositionsFromFile()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    using (var reader = new StreamReader(_filePath))
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                    {
                        _positions = csv.GetRecords<Position>().ToList();
                        Console.WriteLine($"Loaded {_positions.Count} positions from {_filePath}");
                    }
                }
                else
                {
                    Console.WriteLine($"File not found: {_filePath}");
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Something went wrong when trying to load positions from file." ex);
            }
        }

        // Calculate profit/loss when a rate update is received
        public string CalculateProfitLoss(string instrumentId, decimal currentRate)
        {
            try
            {
                Console.OutputEncoding = Encoding.UTF8;

                foreach (var position in _positions)
                {
                    if (position.InstrumentId.Split("/")[0].Equals(instrumentId))
                    {
                        var priceDifference = currentRate - position.InitialRate;
                        var profitLoss = position.Quantity * priceDifference * (position.Side == "BUY" ? 1 : -1);

                        string euroFormat = string.Format("€{0:#,##0.################}", profitLoss);
                        return $"Calculated profit/loss for {instrumentId}: {euroFormat}";
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong when trying to calculate profits and losses." ex);
            }
        }
    }
}