using ReportCanvas.Domain.Common;

namespace ReportCanvas.Domain.Entities;

public class ReportPage : BaseEntity
{
    public string Name { get; set; } = "Page 1";
    public int PageNumber { get; set; }

    /// <summary>Page dimensions in pixels (e.g. 1200x900 for landscape)</summary>
    public int Width { get; set; } = 1200;
    public int Height { get; set; } = 900;

    public string BackgroundColor { get; set; } = "#FFFFFF";

    public Guid ReportId { get; set; }
    public Report Report { get; set; } = null!;

    public ICollection<Widget> Widgets { get; set; } = [];
}
