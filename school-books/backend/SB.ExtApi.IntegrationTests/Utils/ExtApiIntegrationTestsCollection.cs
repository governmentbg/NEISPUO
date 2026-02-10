namespace SB.ExtApi.IntegrationTests;

using System;
using Xunit;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix

[CollectionDefinition(nameof(ExtApiIntegrationTestsCollection))]
public class ExtApiIntegrationTestsCollection
    : ICollectionFixture<
        OrderedFixtures<
            // TODO dotnet8 Use new type aliases for the Tuple (here and in the tests)
            // https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-12#alias-any-type
            Tuple<
                CreateSnapshotFixture,
                DataFixture_V_XII,
                DataFixture_V_XII_TA,
                DataFixture_PG,
                // Using a ICollectionFixture to share the same appFactory
                // between all tests in this collection, which is twice as fast as
                // creating a new appFactory for each test, but the tests are not
                // completely isolated from each other.
                ExtApiWebApplicationFactory
            >>>
{
}
