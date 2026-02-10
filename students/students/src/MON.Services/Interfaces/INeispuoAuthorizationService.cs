namespace MON.Services.Interfaces
{
    using MON.Models;
    using MON.Services.Security.Permissions;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface INeispuoAuthorizationService
    {
        /// <summary>
        /// Проверка дали потребител притежава определено право. За сега се наследяват само от ролите им.
        /// </summary>
        /// <param name="permission">Име на право.<see cref="DefaultPermissions"/></param>
        /// <returns></returns>
        /// 
        Task<bool> AuthorizeUser(string permission);

        /// <summary>
        /// Проверка дали потребител притежава определени права. За сега се наследяват само от ролите им.
        /// </summary>
        /// <returns></returns>
        Task<bool> AuthorizeUser(string[] permissions, bool hasSomePermission = false);

        /// <summary>
        /// Проверка за притежание на определено право що се отнася до определен контекст.
        /// пр. проверка за право за създаване на документ за отписване на ученик, който е записван в нашата институция. 
        /// </summary>
        /// <param name="permission"></param>
        /// <param name="contextId"></param>
        /// <returns></returns>
        abstract Task<bool> AuthorizeUser(DemandPermissionModel model);

        Task<HashSet<string>> GetUserPermissionsForInstitutionForLoggedUser();

        /// <summary>
        /// Проверка дали потребител притежава определено право над даден ученик, заредени чрез притежаваните <see cref="StudentPermissionLevelGroups"/>
        /// </summary>
        /// <returns></returns>
        /// 
        Task<bool> HasPermissionForStudent(int personId, string permission);

        /// <summary>
        /// Проверка дали потребител притежава определени права над даден ученик, заредени чрез притежаваните <see cref="StudentPermissionLevelGroups"/>
        /// </summary>
        /// <returns></returns>
        Task<bool> HasPermissionsForStudent(int personId, string[] permissions);

        /// <summary>
        /// Проверка дали потребител притежава определено право над дадена институция, заредени чрез притежаваните <see cref="InstitutionPermissionLevelGroups"/>
        /// </summary>
        /// <returns></returns>
        /// 
        Task<bool> HasPermissionForInstitution(int institutionId, string permission);

        /// <summary>
        /// Проверка дали потребител притежава определени права над дадена институция, заредени чрез притежаваните <see cref="InstitutionPermissionLevelGroups"/>
        /// </summary>
        /// <returns></returns>
        Task<bool> HasPermissionForInstitution(int institutionId, string[] permissions);

        /// <summary>
        /// Проверка дали потребител притежава определено право над даден клас, заредени чрез притежаваните <see cref="ClassPermissionLevelGroup"/>
        /// </summary>
        /// <returns></returns>
        /// 
        Task<bool> HasPermissionForClass(int classGroupId, string permission);

        /// <summary>
        /// Проверка дали потребител притежава определени права над даден клас, заредени чрез притежаваните <see cref="ClassPermissionLevelGroup"/>
        /// </summary>
        /// <returns></returns>
        Task<bool> HasPermissionForClass(int classGroupId, string[] permissions);


        /// <summary>
        /// Връща всичките права на логнатия потребител.
        /// </summary>
        /// <returns></returns>
        Task<HashSet<string>> GetUserPermissions();

        /// <summary>
        /// Връща всички права върху ученик, определени от връзката между логнатия потребител и ученика (owner, reader и др.)
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        Task<HashSet<string>> GetUserPermissionsForStudent(int personId);

        /// <summary>
        /// Връща всички права върху институция, определени от връзката между логнатия потребител и институцията (owner, reader и др.)
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        Task<HashSet<string>> GetUserPermissionsForInstitution(int institutionId);

        /// <summary>
        /// Връща всички права върху група/паралелка, определени от връзката между логнатия потребител и групата/паралелката (owner, reader и др.)
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        Task<HashSet<string>> GetUserPermissionsForClass(int classId);

        /// <summary>
        /// Връща всияки права от <seealso cref="DefaultPermissions"/> като списък от <see cref="Permission"/>
        /// </summary>
        /// <returns></returns>
        Task<List<Permission>> GetAllPermissons();
    }
}
