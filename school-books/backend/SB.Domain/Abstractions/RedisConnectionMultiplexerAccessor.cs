namespace SB.Domain;

using System;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

// Enables injection of Redis dependent services
// without actually trying to connect to a redis database
// until a connection is required
class RedisConnectionMultiplexerAccessor : IRedisConnectionMultiplexerAccessor, IDisposable
{
    private readonly object syncRoot = new object();

    private bool disposed;
    private IOptions<DomainOptions> options;
    private IConnectionMultiplexer? connectionMultiplexer;

    public RedisConnectionMultiplexerAccessor(IOptions<DomainOptions> options)
    {
        this.options = options;
    }

    public IConnectionMultiplexer ConnectionMultiplexer
    {
        get
        {
            if (this.connectionMultiplexer == null)
            {
                lock(this.syncRoot)
                {
                    if (this.connectionMultiplexer == null)
                    {
                        this.connectionMultiplexer =
                            StackExchange.Redis.ConnectionMultiplexer.Connect(
                                this.options.Value.RedisConnectionString
                                    ?? throw new Exception(
                                        $"RedisConnectionString is not configured in {nameof(DomainOptions)}"));
                    }
                }
            }

            return this.connectionMultiplexer;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing && this.connectionMultiplexer != null)
            {
                this.connectionMultiplexer.Dispose();
            }

            this.connectionMultiplexer = null;
            this.disposed = true;
        }
    }
}
