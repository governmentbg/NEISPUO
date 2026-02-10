namespace SB.Blobs;

public class DataOptions
{
#pragma warning disable CA1024 // Use properties where appropriate
    public string GetConnectionString()
#pragma warning restore CA1024 // Use properties where appropriate
    {
        return this.ConnectionString ??
            $"Server={this.DbIP},{this.DbPort};User Id={this.DbUser};Password={this.DbPass};Initial Catalog={this.DbName};Encrypt=False";
    }

#pragma warning disable CA1721 // Property names should not match get methods
    public string ConnectionString { get; set; }
#pragma warning restore CA1721 // Property names should not match get methods

    public string DbIP { get; set; }

    public string DbPort { get; set; }

    public string DbUser { get; set; }

    public string DbPass { get; set; }

    public string DbName { get; set; }
}
