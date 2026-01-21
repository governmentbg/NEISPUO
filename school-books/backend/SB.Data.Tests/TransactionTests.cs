namespace SB.Data.Tests;

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Xunit;

public class TransactionTests
{
    [Fact]
    public async Task When_disposed_should_rollback_the_underlying_transaction()
    {
        var dbContextTransactionMock = new Mock<IDbContextTransaction>();

        await using (var rootTran = new Transaction(dbContextTransactionMock.Object, false))
        {
        }

        dbContextTransactionMock.Verify(dbcTran => dbcTran.RollbackAsync(default), Times.Once(), "Root transactions should rollback on disposal if uncommited");
        dbContextTransactionMock.Verify(dbcTran => dbcTran.CommitAsync(default), Times.Never());
        dbContextTransactionMock.Verify(dbcTran => dbcTran.DisposeAsync(), Times.Once(), "Root transactions should dispose of the undelying dbc transaction");
    }

    [Fact]
    public async Task When_async_disposed_should_rollback_the_underlying_transaction()
    {
        var dbContextTransactionMock = new Mock<IDbContextTransaction>();

        await using (var rootTran = new Transaction(dbContextTransactionMock.Object, false))
        {
        }

        dbContextTransactionMock.Verify(dbcTran => dbcTran.RollbackAsync(default), Times.Once(), "Root transactions should rollback on disposal if uncommited");
        dbContextTransactionMock.Verify(dbcTran => dbcTran.CommitAsync(default), Times.Never());
        dbContextTransactionMock.Verify(dbcTran => dbcTran.DisposeAsync(), Times.Once(), "Root transactions should dispose of the undelying dbc transaction");
    }

    [Fact]
    public async Task Should_call_commit_on_the_underlying_transaction()
    {
        var dbContextTransactionMock = new Mock<IDbContextTransaction>();

        await using (var rootTran = new Transaction(dbContextTransactionMock.Object, false))
        {
            await rootTran.CommitAsync(default);
        }

        dbContextTransactionMock.Verify(dbcTran => dbcTran.RollbackAsync(default), Times.Never());
        dbContextTransactionMock.Verify(dbcTran => dbcTran.CommitAsync(default), Times.Once(), "Root transactions should proxy the commit");
        dbContextTransactionMock.Verify(dbcTran => dbcTran.DisposeAsync(), Times.Once(), "Root transactions should dispose of the undelying dbc transaction");
    }

    [Fact]
    public async Task Child_transaction_when_uncommited_and_disposed_should_rollback_the_underlying_transaction()
    {
        var dbContextTransactionMock = new Mock<IDbContextTransaction>();

        await using (var rootTran = new Transaction(dbContextTransactionMock.Object, true))
        {
        }

        dbContextTransactionMock.Verify(dbcTran => dbcTran.RollbackAsync(default), Times.Once(), "Child transactions should rollback on disposal if uncommited");
        dbContextTransactionMock.Verify(dbcTran => dbcTran.CommitAsync(default), Times.Never());
        dbContextTransactionMock.Verify(dbcTran => dbcTran.DisposeAsync(), Times.Never(), "Child transactions should NOT dispose of the undelying dbc transaction");
    }

    [Fact]
    public async Task Child_transaction_should_not_call_commit_on_the_underlying_transaction()
    {
        var dbContextTransactionMock = new Mock<IDbContextTransaction>();

        await using (var rootTran = new Transaction(dbContextTransactionMock.Object, true))
        {
            await rootTran.CommitAsync(default);
        }

        dbContextTransactionMock.Verify(dbcTran => dbcTran.RollbackAsync(default), Times.Never(), "Child transaction should not rollback commited transactions on disposal");
        dbContextTransactionMock.Verify(dbcTran => dbcTran.CommitAsync(default), Times.Never(), "Child transactions should NOT proxy the commit");
        dbContextTransactionMock.Verify(dbcTran => dbcTran.DisposeAsync(), Times.Never(), "Child transactions should NOT dispose of the undelying dbc transaction");
    }
}
