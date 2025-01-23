namespace Storify.Core.Models;

public class StorifyPagination
{
    public int Index { get; set; }
    
    public int MaxIndex { get; set; }
    
    public int Size { get; set; }
    
    public int Skip => (Index - 1) * Size;

    public int TotalRecord { get; set; } = 0;
    
    public bool HasNext => Index < MaxIndex;
    
    public bool HasPrevious => Index > 1;
}