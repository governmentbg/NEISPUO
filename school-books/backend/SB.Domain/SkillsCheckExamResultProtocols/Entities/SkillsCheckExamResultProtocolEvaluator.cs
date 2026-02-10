namespace SB.Domain;
public class SkillsCheckExamResultProtocolEvaluator
{
    // EF constructor
    private SkillsCheckExamResultProtocolEvaluator()
    {
        this.SkillsCheckExamResultProtocol = null!;
        this.Name = null!;
    }

    internal SkillsCheckExamResultProtocolEvaluator(
        SkillsCheckExamResultProtocol skillsCheckExamResultProtocol,
        string name)
    {
        this.SkillsCheckExamResultProtocol = skillsCheckExamResultProtocol;
        this.Name = name;
    }

    public int SchoolYear { get; private set; }
    public int SkillsCheckExamResultProtocolId { get; private set; }
    public int SkillsCheckExamResultProtocolEvaluatorId { get; private set; }
    public string Name { get; private set; }

    // relations
    public SkillsCheckExamResultProtocol SkillsCheckExamResultProtocol { get; private set; }

    public void UpdateData(
        string name)
    {
        this.Name = name;
    }
}
