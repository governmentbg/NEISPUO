namespace SB.Domain;

using System;

public class SysUser
{
    // EF constructor
    private SysUser()
    {
        this.Username = null!;
    }

    // only used properties should be mapped

    public int SysUserId { get; private set; }

    public int PersonId { get; private set; }

    public string Username { get; private set; }

    public DateTime? DeletedOn { get; private set; }
}
