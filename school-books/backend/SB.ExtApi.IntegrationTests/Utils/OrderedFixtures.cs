namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

// XUnit will create and InitializeAsync all fixtures concurrently,
// but we need the CreateSnapshotFixture(The CustomizeTestDataAsync method) to be
// executed before the other fixtures(DataFixture_xxx).
// This class achieves that.
public class OrderedFixtures<T> : IAsyncLifetime
    where T : ITuple
{
    private Type[] fixtureTypes;
    private object[] fixtureInstances;
    private T? values;

    public OrderedFixtures()
    {
        this.fixtureTypes = typeof(T).GetGenericArguments().ToArray();
        this.fixtureInstances = new object[this.fixtureTypes.Length];
        for (int i = 0; i < this.fixtureTypes.Length; i++)
        {
            this.fixtureInstances[i] =
                Activator.CreateInstance(this.fixtureTypes[i])
                ?? throw new InvalidOperationException($"Failed to create instance of {this.fixtureTypes[i].FullName}");
        }
    }

    public async Task InitializeAsync()
    {
        for (int i = 0; i < this.fixtureInstances.Length; i++)
        {
            if (this.fixtureInstances[i] is IAsyncLifetime instance)
            {
                await instance.InitializeAsync();
            }
        }

        this.values =  (T)(typeof(Tuple).GetMethods()
            .Single(method => method.Name == "Create" && method.GetParameters().Length == this.fixtureTypes.Length)
            .MakeGenericMethod(this.fixtureTypes)
            .Invoke(null, this.fixtureInstances)
            ?? throw new InvalidOperationException($"Failed to create instance of {typeof(T).FullName}"));
    }

    public async Task DisposeAsync()
    {
        for (int i = this.fixtureInstances.Length - 1; i >= 0; i--)
        {
            if (this.fixtureInstances[i] is IAsyncLifetime instance)
            {
                await instance.DisposeAsync();
            }
            else if (this.fixtureInstances[i] is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else if (this.fixtureInstances[i] is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    public T Values => this.values ?? throw new InvalidOperationException("InitializeAsync has not been called yet");
}
