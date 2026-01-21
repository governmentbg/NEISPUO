namespace SB.Domain;

public static class CacheConstants
{
    // The CacheEntry does contain a lot of data plus
    // a string key of 50chars is 100+ bytes;
    public const int CacheEntryAndKeySize = 500;

    // when approximating cache item size
    // * primitives(int, long, bool, ...) - use sizeof
    // * string - 2*(n+1), where n is the length of the string.
    // * objects - еach heap object costs as much as its primitive types, plus 8 bytes for object references
    public const int SmallObjectSize = CacheEntryAndKeySize + 32;
    public const int MediumObjectSize = CacheEntryAndKeySize + 128;
    public const int LargeObjectSize = CacheEntryAndKeySize + 1024;
}
