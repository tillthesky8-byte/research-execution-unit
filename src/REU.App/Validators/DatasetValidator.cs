using Contracts.Enums;
using Contracts.Definitions;
namespace App.Validators;

public static class DatasetValidator
{
    public static bool ValidateDataset(DateTime? startDate, DateTime? endDate, InstrumentDefinition?[]? instrument, Timeframe? timeframe)
    {
        if (instrument == null || instrument.Length == 0)
            throw new ArgumentException("At least one instrument must be specified. Use --instrument or -i option to specify the instrument(s) for the simulation.");

        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            throw new ArgumentException("Start date cannot be later than end date.");

        if (instrument != null && instrument.Length > 0)
        {
            if (timeframe == null)
                throw new ArgumentException("Timeframe must be specified when instrument is provided.");

        }
        return true;
    }
}