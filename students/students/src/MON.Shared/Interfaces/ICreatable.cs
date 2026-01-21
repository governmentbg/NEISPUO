namespace MON.Shared.Interfaces
{
    using System;

    public interface ICreatable
    {
        public int CreatedBySysUserId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
