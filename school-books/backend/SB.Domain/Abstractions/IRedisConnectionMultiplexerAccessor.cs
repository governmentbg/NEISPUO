namespace SB.Domain;

using StackExchange.Redis;

public interface IRedisConnectionMultiplexerAccessor
{
    IConnectionMultiplexer ConnectionMultiplexer { get; }
}
