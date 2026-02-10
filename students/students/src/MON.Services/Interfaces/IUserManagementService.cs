using MON.Models.UserManagement;
using System.Threading;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface IUserManagementService
    {
        Task CreateAsync(StudentRequestDto studentDto, CancellationToken cancellationToken = default);
        Task UpdateAsync(StudentRequestDto studentDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(StudentDeleteDisableRequestDto studentDto, CancellationToken cancellationToken = default);
        Task EnrollmentSchoolCreateAsync(EnrollmentStudentToSchoolDeleteRequestDto studentDto, CancellationToken cancellationToken = default);
        Task EnrollmentSchoolDeleteAsync(EnrollmentStudentToSchoolDeleteRequestDto studentDto, CancellationToken cancellationToken = default);
        Task EnrollmentStudentToClassCreateAsync(EnrollmentStudentToClassCreateRequestDto studentDto, CancellationToken cancellationToken = default);
        Task<bool> EnrollmentStudentToClassDeleteAsync(EnrollmentStudentToClassDeleteRequestDto studentDto, CancellationToken cancellationToken = default);
    }
}
