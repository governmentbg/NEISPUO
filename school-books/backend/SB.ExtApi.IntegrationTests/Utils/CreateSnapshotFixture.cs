namespace SB.ExtApi.IntegrationTests;

using System.Threading.Tasks;
using Xunit;

public class CreateSnapshotFixture : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        if (await DatabaseUtils.IsAzureSqlEdgeAsync())
        {
            await DatabaseUtils.TryDeleteBackupAsync();
            await DatabaseUtils.CreateBackupAsync();
            await DatabaseUtils.CustomizeTestDataAsync();
            return;
        }

        await DatabaseUtils.TryDeleteSnapshotAsync();
        await DatabaseUtils.CreateSnapshotAsync();
        await DatabaseUtils.CustomizeTestDataAsync();
    }

    public async Task DisposeAsync()
    {
        if (await DatabaseUtils.IsAzureSqlEdgeAsync())
        {
            await DatabaseUtils.RestoreBackupAsync();
            await DatabaseUtils.TryDeleteBackupAsync();
            return;
        }

        await DatabaseUtils.RestoreSnapshotAsync();
        await DatabaseUtils.TryDeleteSnapshotAsync();
    }
}
