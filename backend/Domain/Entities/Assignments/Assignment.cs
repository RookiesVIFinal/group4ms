using Domain.Base;
using Domain.Entities.Assets;
using Domain.Entities.Users;
using Domain.Entities.RequestsForReturning;
using Domain.Shared.Enums;

namespace Domain.Entities.Assignments;

public class Assignment : AuditableEntity<Guid>
{
    public Guid AssetId { get; set; }

    public Asset Asset { get; set;} = null!;

    public Guid AssignedBy { get; set; }

    public User Assignee { get; set; } = null!;

    public Guid AssignedTo { get; set; }

    public User Assigner { get; set; } = null!;

    public DateTime AssignedDate { get; set; }

    public AssignmentState State { get; set; }

    public string? Note { get; set; }

    public ICollection<RequestForReturning> RequestsForReturning { get; set; } = null!;
}