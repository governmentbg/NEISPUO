namespace SB.Domain;

public class SupportDifficulty
{
    // EF constructor
    private SupportDifficulty()
    {
        this.Support = null!;
    }

    internal SupportDifficulty(
        Support support,
        int supportDifficultyTypeId)
    {
        this.Support = support;
        this.SupportDifficultyTypeId = supportDifficultyTypeId;
    }

    public int SchoolYear { get; private set; }
    public int SupportId { get; private set; }
    public int SupportDifficultyTypeId { get; private set; }

    // relations
    public Support Support { get; private set; }
}
