namespace MON.Shared.Interfaces
{
    using System;

    public interface IUpdatable
    {
        public int? ModifiedBySysUserId { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
