namespace SB.ExtApi.IntegrationTests;

using System.Threading.Tasks;
using Xunit;

public class RestoreSnapshotFixture : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        if (await DatabaseUtils.IsAzureSqlEdgeAsync())
        {
            await DatabaseUtils.RestoreBackupAsync();
            await DatabaseUtils.CustomizeTestDataAsync();
            return;
        }

        await DatabaseUtils.RestoreSnapshotAsync();
        await DatabaseUtils.CustomizeTestDataAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
