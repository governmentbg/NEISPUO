namespace SB.Domain;

public class SkillsCheckExamDutyProtocolSupervisor
{
    // EF constructor
    private SkillsCheckExamDutyProtocolSupervisor()
    {
        this.SkillsCheckExamDutyProtocol = null!;
    }

    public SkillsCheckExamDutyProtocolSupervisor(
        SkillsCheckExamDutyProtocol skillsCheckExamDutyProtocol, int personId)
    {
        this.SkillsCheckExamDutyProtocol = skillsCheckExamDutyProtocol;
        this.PersonId = personId;
    }

    public int SkillsCheckExamDutyProtocolId { get; private set; }

    public int SchoolYear { get; private set; }

    public int PersonId { get; private set; }

    // relations
    public SkillsCheckExamDutyProtocol SkillsCheckExamDutyProtocol { get; private set; }
}
