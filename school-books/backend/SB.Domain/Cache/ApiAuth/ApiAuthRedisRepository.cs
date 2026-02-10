namespace SB.Domain;

using System;
using System.Buffers.Binary;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;

class ApiAuthRedisRepository : IApiAuthRedisRepository
{
    private enum CacheEntryType : byte
    {
        PersonIsClassBookTeacher = 1,
        PersonIsClassBookLeadTeacher = 2,
        PersonIsClassBookLeadTeacherOrCurriculumTeacher = 3,
        PersonIsClassBookLeadTeacherOrLessonTeacher = 4,
        PersonIsInSupportTeam = 5,
        InstitutionBelongsToRegion = 6,
        ClassBookBelongsToInstitution = 7,
        PersonIsReplTeacherForDate = 8,
        PersonIsReplTeacherForDateAndCurriculum = 9,
        HisMedicalNoticeBelongsToRegion = 10,
        StudentBelongsToClassBook = 11,
        ParentOrStudentBelongsToInstitution = 12
    }

    private const string ApiAuthCacheKeyPrefix = "AA";

    private IRedisConnectionMultiplexerAccessor connectionMultiplexerAccessor;

    public ApiAuthRedisRepository(IRedisConnectionMultiplexerAccessor connectionMultiplexerAccessor)
    {
        this.connectionMultiplexerAccessor = connectionMultiplexerAccessor;
    }

    public async Task<bool?> GetPersonIsClassBookTeacherAsync(string jti, int personId, int schoolYear, int instId, int classBookId, CancellationToken _)
    {
        IDatabase db = this.GetDatabase();

        return (bool?) await db.HashGetAsync($"{ApiAuthCacheKeyPrefix}:{jti}", this.GetKey(CacheEntryType.PersonIsClassBookTeacher, new[] { personId, schoolYear, instId, classBookId }));
    }

    public async Task CachePersonIsClassBookTeacherAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, bool result, CancellationToken _)
    {
        IDatabase db = this.GetDatabase();

        await db.HashSetAsync($"{ApiAuthCacheKeyPrefix}:{jti}", this.GetKey(CacheEntryType.PersonIsClassBookTeacher, new[] { personId, schoolYear, instId, classBookId }), result);

        await db.KeyExpireAsync($"{ApiAuthCacheKeyPrefix}:{jti}", exp);
    }

    public async Task<bool?> GetPersonIsClassBookLeadTeacherAsync(string jti, int personId, int schoolYear, int instId, int classBookId, CancellationToken _)
    {
        IDatabase db = this.GetDatabase();

        return (bool?)await db.HashGetAsync($"{ApiAuthCacheKeyPrefix}:{jti}", this.GetKey(CacheEntryType.PersonIsClassBookLeadTeacher, new[] { personId, schoolYear, instId, classBookId }));
    }

    public async Task CachePersonIsClassBookLeadTeacherAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, bool result, CancellationToken _)
    {
        IDatabase db = this.GetDatabase();

        await db.HashSetAsync($"{ApiAuthCacheKeyPrefix}:{jti}", this.GetKey(CacheEntryType.PersonIsClassBookLeadTeacher, new[] { personId, schoolYear, instId, classBookId }), result);

        await db.KeyExpireAsync($"{ApiAuthCacheKeyPrefix}:{jti}", exp);
    }

    public async Task<bool?> GetPersonIsClassBookLeadTeacherOrCurriculumTeacherAsync(string jti, int personId, int schoolYear, int instId, int classBookId, int curriculumId, CancellationToken _)
    {
        IDatabase db = this.GetDatabase();

        return (bool?) await db.HashGetAsync($"{ApiAuthCacheKeyPrefix}:{jti}", this.GetKey(CacheEntryType.PersonIsClassBookLeadTeacherOrCurriculumTeacher, new[] { personId, schoolYear, instId, classBookId, curriculumId }));
    }

    public async Task CachePersonIsClassBookLeadTeacherOrCurriculumTeacherAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, int curriculumId, bool result, CancellationToken _)
    {
        IDatabase db = this.GetDatabase();

        await db.HashSetAsync($"{ApiAuthCacheKeyPrefix}:{jti}", this.GetKey(CacheEntryType.PersonIsClassBookLeadTeacherOrCurriculumTeacher, new[] { personId, schoolYear, instId, classBookId, curriculumId }), result);

        await db.KeyExpireAsync($"{ApiAuthCacheKeyPrefix}:{jti}", exp);
    }

    public async Task<bool?> GetPersonIsClassBookLeadTeacherOrLessonTeacherAsync(string jti, int personId, int schoolYear, int instId, int classBookId, int scheduleLessonId, CancellationToken _)
    {
        IDatabase db = this.GetDatabase();

        return (bool?) await db.HashGetAsync($"{ApiAuthCacheKeyPrefix}:{jti}", this.GetKey(CacheEntryType.PersonIsClassBookLeadTeacherOrLessonTeacher, new[] { personId, schoolYear, instId, classBookId, scheduleLessonId }));
    }

    public async Task CachePersonIsClassBookLeadTeacherOrLessonTeacherAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, int scheduleLessonId, bool result, CancellationToken _)
    {
        IDatabase db = this.GetDatabase();

        await db.HashSetAsync($"{ApiAuthCacheKeyPrefix}:{jti}", this.GetKey(CacheEntryType.PersonIsClassBookLeadTeacherOrLessonTeacher, new[] { personId, schoolYear, instId, classBookId, scheduleLessonId }), result);

        await db.KeyExpireAsync($"{ApiAuthCacheKeyPrefix}:{jti}", exp);
    }

    public async Task<bool?> GetPersonIsInSupportTeamAsync(string jti, int personId, int schoolYear, int instId, int classBookId, int supportId, CancellationToken _)
    {
        IDatabase db = this.GetDatabase();

        return (bool?)await db.HashGetAsync($"{ApiAuthCacheKeyPrefix}:{jti}", this.GetKey(CacheEntryType.PersonIsInSupportTeam, new[] { personId, schoolYear, instId, classBookId, supportId }));
    }

    public async Task CachePersonIsInSupportTeamAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, int supportId, bool result, CancellationToken _)
    {
        IDatabase db = this.GetDatabase();

        await db.HashSetAsync($"{ApiAuthCacheKeyPrefix}:{jti}", this.GetKey(CacheEntryType.PersonIsInSupportTeam, new[] { personId, schoolYear, instId, classBookId, supportId }), result);

        await db.KeyExpireAsync($"{ApiAuthCacheKeyPrefix}:{jti}", exp);
    }

    public async Task<bool?> GetPersonIsReplTeacherForDateAsync(string jti, int personId, int schoolYear, int instId, int classBookId, DateTime date, CancellationToken _)
    {
        IDatabase db = this.GetDatabase();

        return (bool?)await db.HashGetAsync($"{ApiAuthCacheKeyPrefix}:{jti}", this.GetKey(CacheEntryType.PersonIsReplTeacherForDate, new[] { personId, schoolYear, instId, classBookId }, new[] { date.Ticks }));
    }

    public async Task<bool?> GetPersonIsReplTeacherForDateAndCurriculumAsync(string jti, int personId, int schoolYear, int instId, int classBookId, int curriculumId, DateTime date, CancellationToken _)
    {
        IDatabase db = this.GetDatabase();

        return (bool?)await db.HashGetAsync($"{ApiAuthCacheKeyPrefix}:{jti}", this.GetKey(CacheEntryType.PersonIsReplTeacherForDateAndCurriculum, new[] { personId, schoolYear, instId, classBookId, curriculumId }, new[] { date.Ticks }));
    }

    public async Task CachePersonIsReplTeacherForDateAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, DateTime date, bool result, CancellationToken _)
    {
        IDatabase db = this.GetDatabase();

        await db.HashSetAsync($"{ApiAuthCacheKeyPrefix}:{jti}", this.GetKey(CacheEntryType.PersonIsReplTeacherForDate, new[] { personId, schoolYear, instId, classBookId }, new[] { date.Ticks }), result);

        await db.KeyExpireAsync($"{ApiAuthCacheKeyPrefix}:{jti}", exp);
    }

    public async Task CachePersonIsReplTeacherForDateAndCurriculumAsync(string jti, DateTime exp, int personId, int schoolYear, int instId, int classBookId, int curriculumId, DateTime date, bool result, CancellationToken _)
    {
        IDatabase db = this.GetDatabase();

        await db.HashSetAsync($"{ApiAuthCacheKeyPrefix}:{jti}", this.GetKey(CacheEntryType.PersonIsReplTeacherForDateAndCurriculum, new[] { personId, schoolYear, instId, classBookId, curriculumId }, new[] { date.Ticks }), result);

        await db.KeyExpireAsync($"{ApiAuthCacheKeyPrefix}:{jti}", exp);
    }

    public async Task<bool?> GetInstitutionBelongsToRegionAsync(int instId, int regionId, CancellationToken _)
    {
        IDatabase db = this.GetDatabase().WithKeyPrefix(ApiAuthCacheKeyPrefix);

        return (bool?)await db.StringGetAsync(this.GetKey(CacheEntryType.InstitutionBelongsToRegion, new[] { instId, regionId }));
    }

    public async Task CacheInstitutionBelongsToRegionAsync(TimeSpan exp, int instId, int regionId, bool result, CancellationToken _)
    {
        IDatabase db = this.GetDatabase().WithKeyPrefix(ApiAuthCacheKeyPrefix);

        await db.StringSetAsync(this.GetKey(CacheEntryType.InstitutionBelongsToRegion, new[] { instId, regionId }), result, exp);
    }

    public async Task<bool?> GetClassBookBelongsToInstitutionAsync(int schoolYear, int instId, int classBookId, CancellationToken _)
    {
        IDatabase db = this.GetDatabase().WithKeyPrefix(ApiAuthCacheKeyPrefix);

        return (bool?)await db.StringGetAsync(this.GetKey(CacheEntryType.ClassBookBelongsToInstitution, new[] { schoolYear, instId, classBookId }));
    }

    public async Task CacheClassBookBelongsToInstitutionAsync(TimeSpan exp, int schoolYear, int instId, int classBookId, bool result, CancellationToken _)
    {
        IDatabase db = this.GetDatabase().WithKeyPrefix(ApiAuthCacheKeyPrefix);

        await db.StringSetAsync(this.GetKey(CacheEntryType.ClassBookBelongsToInstitution, new[] { schoolYear, instId, classBookId }), result, exp);
    }

    public async Task<bool?> GetHisMedicalNoticeBelongsToRegionAsync(int hisMedicalNoticeId, int regionId, CancellationToken _)
    {
        IDatabase db = this.GetDatabase().WithKeyPrefix(ApiAuthCacheKeyPrefix);

        return (bool?)await db.StringGetAsync(this.GetKey(CacheEntryType.HisMedicalNoticeBelongsToRegion, new[] { hisMedicalNoticeId, regionId }));
    }

    public async Task CacheHisMedicalNoticeBelongsToRegionAsync(TimeSpan exp, int hisMedicalNoticeId, int regionId, bool result, CancellationToken _)
    {
        IDatabase db = this.GetDatabase().WithKeyPrefix(ApiAuthCacheKeyPrefix);

        await db.StringSetAsync(this.GetKey(CacheEntryType.HisMedicalNoticeBelongsToRegion, new[] { hisMedicalNoticeId, regionId }), result, exp);
    }

    public async Task<bool?> GetStudentBelongsToClassBookAsync(int schoolYear, int instId, int classBookId, int personId, CancellationToken _)
    {
        IDatabase db = this.GetDatabase().WithKeyPrefix(ApiAuthCacheKeyPrefix);

        return (bool?)await db.StringGetAsync(this.GetKey(CacheEntryType.StudentBelongsToClassBook, new[] { schoolYear, instId, classBookId, personId }));
    }

    public async Task CacheStudentBelongsToClassBookAsync(TimeSpan exp, int schoolYear, int instId, int classBookId, int personId, bool result, CancellationToken _)
    {
        IDatabase db = this.GetDatabase().WithKeyPrefix(ApiAuthCacheKeyPrefix);

        await db.StringSetAsync(this.GetKey(CacheEntryType.StudentBelongsToClassBook, new[] { schoolYear, instId, classBookId, personId }), result, exp);
    }

    public async Task<bool?> GetStudentsBelongsToInstitutionAsync(int schoolYear, int instId, int[] studentsPersonId, CancellationToken _)
    {
        IDatabase db = this.GetDatabase().WithKeyPrefix(ApiAuthCacheKeyPrefix);

        //TODO: To change this method after update dotnet version to 9.0
        //var keyArgs = [schoolYear, instId, ...studentsPersonId];
        var keyArgs = this.CreateKeyArgs(new[] { schoolYear, instId }, studentsPersonId);

        return (bool?)await db.StringGetAsync(this.GetKey(CacheEntryType.ParentOrStudentBelongsToInstitution, keyArgs));
    }

    public async Task CacheStudentsBelongsToInstitutionAsync(TimeSpan exp, int schoolYear, int instId,
        int[] studentsPersonId, bool result, CancellationToken ct)
    {
        IDatabase db = this.GetDatabase().WithKeyPrefix(ApiAuthCacheKeyPrefix);

        //TODO: To change this method after update dotnet version to 9.0
        //var keyArgs = [schoolYear, instId, ...studentsPersonId];
        var keyArgs = this.CreateKeyArgs(new[] { schoolYear, instId }, studentsPersonId);

        await db.StringSetAsync(this.GetKey(CacheEntryType.ParentOrStudentBelongsToInstitution, keyArgs), result, exp);
    }

    private IDatabase GetDatabase()
    {
        return this.connectionMultiplexerAccessor.ConnectionMultiplexer.GetDatabase();
    }

    private byte[] GetKey(CacheEntryType type, int[] args, long[]? longArgs = null)
    {
        longArgs ??= Array.Empty<long>();

        var result = new byte[1 + (args.Length * sizeof(int)) + (longArgs.Length * sizeof(long))];

        result[0] = (byte)type;

        Span<byte> resultSpan = result;

        for (int i = 0; i < args.Length; i++)
        {
            BinaryPrimitives.WriteInt32BigEndian(
                resultSpan.Slice(
                    start: 1 + (i * sizeof(int)),
                    length: sizeof(int)),
                args[i]);
        }

        for (int i = 0; i < longArgs.Length; i++)
        {
            BinaryPrimitives.WriteInt64BigEndian(
                resultSpan.Slice(
                    start: 1 + (args.Length * sizeof(int)) + (i * sizeof(long)),
                    length: sizeof(long)),
                longArgs[i]);
        }

        return result;
    }

    private int[] CreateKeyArgs(int[] initialArgs, int[] additionalArgs)
    {
        var keyArgs = new int[initialArgs.Length + additionalArgs.Length];
        var keyArgsSpan = keyArgs.AsSpan();
        initialArgs.AsSpan().CopyTo(keyArgsSpan);
        additionalArgs.AsSpan().CopyTo(keyArgsSpan.Slice(initialArgs.Length));
        return keyArgs;
    }
}
