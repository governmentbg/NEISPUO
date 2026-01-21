namespace MON.Shared.Interfaces
{
    public interface IStatus
    {
        int Status { get; set; }
        bool IsDraft { get; }
    }
}
