using ReportCanvas.Domain.Common;
using ReportCanvas.Domain.Enums;

namespace ReportCanvas.Domain.Entities;

public class Widget : BaseEntity
{
    public WidgetType Type { get; set; }

    // Position & size on the canvas (pixels)
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; } = 300;
    public int Height { get; set; } = 200;
    public int ZIndex { get; set; }

    /// <summary>Widget-specific config (title, labels, axis config, etc.) as JSON</summary>
    public string ConfigJson { get; set; } = "{}";

    /// <summary>Visual styling (colors, fonts, borders, padding) as JSON</summary>
    public string StyleJson { get; set; } = "{}";

    /// <summary>
    /// Data binding definition as JSON:
    /// { "datasetId": "...", "groupBy": "month", "metric": "revenue", "aggregation": "sum" }
    /// Null for editorial widgets (TextBlock, Image, etc.)
    /// </summary>
    public string? DataBindingJson { get; set; }

    public Guid ReportPageId { get; set; }
    public ReportPage ReportPage { get; set; } = null!;
}
