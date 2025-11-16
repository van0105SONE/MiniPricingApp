using CsvHelper;
using MiniPricingApp.Modules.Qoutes.Domain.Interfaces;
using MiniPricingApp.Shares.Exceptions;
using System.Globalization;

namespace MiniPricingApp.Modules.Qoutes.Domain.Validators
{
    /*
     * QouteCsvValidator
     * ------------------
     * Validates the structure and data format of an uploaded CSV file before it is
     * processed. This ensures that the file contains the required columns and that
     * each row is in the correct format.
     *
     * Validation steps:
     * 1. Reads the CSV header and checks whether all required columns exist
     *    (defined in ExpectedHeaders).
     * 2. Iterates through each row:
     *      - Ensures "Weight" is a valid decimal value.
     *
     * If any validation fails, a CsvFormatException is thrown with a clear message.
     * This prevents invalid CSV files from corrupting the system or causing incorrect data.
     */
    public class QouteCsvValidator : IQouteCsvValidator
    {

        private static readonly string[] ExpectedHeaders = new[]
    {
                "Weight",
                "AreaCode"
          };
        public async Task Validate(string FilePath)
        {
            try
            {
                using var reader = new StreamReader(FilePath);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var rowNumber = 1;

                if (!csv.Read() || !csv.ReadHeader())
                    throw new CsvFormatException("CSV header row is missing or unreadable.");

                var header = csv.HeaderRecord;

                // Validate header names
                foreach (var expected in ExpectedHeaders)
                {
                    if (!header.Contains(expected))
                        throw new CsvFormatException($"Missing required column: '{expected}'.");
                }


                while (await csv.ReadAsync())
                {
                    rowNumber++;


                    if (!decimal.TryParse(csv.GetField("Weight"), out var weight))
                        throw new CsvFormatException($"Invalid format at row {rowNumber}: Weight must be decimal.");


                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
