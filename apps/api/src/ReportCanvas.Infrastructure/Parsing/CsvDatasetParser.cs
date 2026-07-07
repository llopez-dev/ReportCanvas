using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using ReportCanvas.Application.Common.Interfaces;

namespace ReportCanvas.Infrastructure.Parsing;

public class CsvDatasetParser : IDatasetParser
{
    public bool CanHandle(string fileExtension) =>
        fileExtension.Equals(".csv", StringComparison.OrdinalIgnoreCase);

    public async Task<ParsedDatasetResult> ParseAsync(
        Stream fileStream,
        int previewRowCount = 20,
        CancellationToken ct = default)
    {
        using var reader = new StreamReader(fileStream, leaveOpen: true);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null
        };

        using var csv = new CsvReader(reader, config);

        await csv.ReadAsync();
        csv.ReadHeader();

        var headers = csv.HeaderRecord?.ToList() ?? [];
        var previewRows = new List<IReadOnlyList<string>>();
        int totalRows = 0;

        while (await csv.ReadAsync())
        {
            ct.ThrowIfCancellationRequested();
            totalRows++;

            if (totalRows <= previewRowCount)
            {
                var row = headers.Select(h => csv.GetField(h) ?? string.Empty).ToList();
                previewRows.Add(row);
            }
        }

        var inferredTypes = InferColumnTypes(headers, previewRows);

        return new ParsedDatasetResult(headers, inferredTypes, previewRows, totalRows);
    }

    private static IReadOnlyList<string> InferColumnTypes(
        IReadOnlyList<string> headers,
        IReadOnlyList<IReadOnlyList<string>> rows)
    {
        var types = new List<string>();

        for (int col = 0; col < headers.Count; col++)
        {
            var samples = rows.Select(r => col < r.Count ? r[col] : string.Empty)
                              .Where(v => !string.IsNullOrWhiteSpace(v))
                              .Take(10)
                              .ToList();

            if (samples.All(v => double.TryParse(v, NumberStyles.Any, CultureInfo.InvariantCulture, out _)))
                types.Add("number");
            else if (samples.All(v => DateTime.TryParse(v, out _)))
                types.Add("date");
            else if (samples.All(v => bool.TryParse(v, out _)))
                types.Add("boolean");
            else
                types.Add("string");
        }

        return types;
    }
}
