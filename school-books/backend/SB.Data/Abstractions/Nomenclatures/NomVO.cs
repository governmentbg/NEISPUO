namespace SB.Data;

public record NomVO
{
    public NomVO(int id, string name)
    {
        this.Id = id;
        this.Name = name;
    }

    public NomVO(int id, string name, string? badge)
    {
        this.Id = id;
        this.Name = name;
        this.Badge = badge;
    }

    public int Id { get; init; }

    public string Name { get; init; }

    public string? Badge { get; init; }
}
