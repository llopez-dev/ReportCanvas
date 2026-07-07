using System.Globalization;
using ClosedXML.Excel;
using ReportCanvas.Application.Common.Interfaces;

namespace ReportCanvas.Infrastructure.Parsing;

public class ExcelDatasetParser : IDatasetParser
{
    public bool CanHandle(string fileExtension) =>
        fileExtension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase) ||
        fileExtension.Equals(".xls", StringComparison.OrdinalIgnoreCase);

    public Task<ParsedDatasetResult> ParseAsync(
        Stream fileStream,
        int previewRowCount = 20,
        CancellationToken ct = default)
    {
        using var workbook = new XLWorkbook(fileStream);
        var sheet = workbook.Worksheets.First();

        var usedRange = sheet.RangeUsed();
        if (usedRange is null)
            return Task.FromResult(new ParsedDatasetResult([], [], [], 0));

        var rows = usedRange.RowsUsed().ToList();
        if (rows.Count == 0)
            return Task.FromResult(new ParsedDatasetResult([], [], [], 0));

        // First row = headers
        var headers = rows[0].Cells().Select(c => c.GetValue<string>()).ToList();
        var dataRows = rows.Skip(1).ToList();
        int totalRows = dataRows.Count;

        var previewRows = dataRows
            .Take(previewRowCount)
            .Select(r => (IReadOnlyList<string>)r.Cells(1, headers.Count)
                .Select(c => c.GetValue<string>())
                .ToList())
            .ToList();

        var inferredTypes = InferColumnTypes(headers.Count, previewRows);

        return Task.FromResult(new ParsedDatasetResult(headers, inferredTypes, previewRows, totalRows));
    }

    private static IReadOnlyList<string> InferColumnTypes(int columnCount, IReadOnlyList<IReadOnlyList<string>> rows)
    {
        var types = new List<string>();

        for (int col = 0; col < columnCount; col++)
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
