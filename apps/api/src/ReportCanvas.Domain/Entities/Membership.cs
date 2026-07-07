using ReportCanvas.Domain.Common;
using ReportCanvas.Domain.Enums;

namespace ReportCanvas.Domain.Entities;

public class Membership : BaseEntity
{
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;

    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public MembershipRole Role { get; set; } = MembershipRole.Member;
    public bool IsActive { get; set; } = true;
}
