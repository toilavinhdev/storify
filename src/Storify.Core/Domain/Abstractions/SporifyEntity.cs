using Storify.Core.Extensions;

namespace Storify.Core.Domain.Abstractions;

public abstract class SporifyEntity
{
    public Guid Id { get; set; }
    
    public long SubId { get; set; }
    
    public DateTimeOffset? CreatedDate { get; set; }
    
    public DateTimeOffset? ModifiedDate { get; set; }
    
    public Guid? CreatedBy { get; set; }
    
    public Guid? ModifiedBy { get; set; }

    public void MarkCreated(Guid? createdBy = null)
    {
        CreatedDate = DateTimeExtensions.Now;
        CreatedBy = createdBy;
    }
    
    public void MarkModified(Guid? modifiedBy = null)
    {
        ModifiedDate = DateTimeExtensions.Now;
        CreatedBy = modifiedBy;
    }
}