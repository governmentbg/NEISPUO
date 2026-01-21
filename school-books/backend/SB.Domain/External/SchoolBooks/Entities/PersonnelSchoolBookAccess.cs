namespace SB.Domain;

// this table is in the school_books schema but is
// maintained and modified by DSS (user management module)
public class PersonnelSchoolBookAccess
{
    public int RowId { get; private set; }

    public int SchoolYear { get; private set; }

    public int ClassBookId { get; private set; }

    public int PersonId { get; private set; }

    public bool HasAdminAccess { get; private set; }
}
