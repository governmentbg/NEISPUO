using Microsoft.EntityFrameworkCore;
using RegStamps.Data.Entities;

namespace RegStamps.Services.Validation
{
    public class ValidationService : IValidationService
    {
        private readonly DataStampsContext context;

        public ValidationService(DataStampsContext context)
            => this.context = context;
        
        public async Task<(bool isValid, string message)> ValidateActiveRequestExistAsync(int schoolId, int stampId)
        {
            bool isValid = true;
            string message = string.Empty;

            bool isStampRegistered = await this.context
                                            .TbStamps
                                            .AnyAsync(x => x.SchoolId == schoolId
                                                        && x.StampId == stampId
                                                        && x.StampStatus == 2);
            if (!isStampRegistered)
            {
                isValid = false;
                message = $"Печат с УИН {stampId} не е регистриран. Не можете да правите промени докато печата не бъде одобрен от МОН";
                return (isValid, message);
            }

            bool isExistActiveKeeper = await this.context
                                            .RefRequestStamps
                                            .AnyAsync(x => x.SchoolId == schoolId 
                                                        && x.StampId == stampId 
                                                        && x.IsActive);
            if (!isExistActiveKeeper)
            {
                isValid = false;
                message = $"Печат с УИН {stampId} няма активен пазител. Не можете да правите промени докато печата не бъде одобрен от МОН";
                return (isValid, message);
            }

            bool isExistActiveRequest = await this.context
                                            .RefRequestStamps
                                            .AnyAsync(x => x.SchoolId == schoolId
                                                        && x.StampId == stampId
                                                        && !x.IsActive
                                                        && x.TbRequest.RequestType > 0 
                                                        && x.TbRequest.RequestStatus < 3);

            if (isExistActiveRequest)
            {
                int currentRequestId = await this.context
                                            .RefRequestStamps
                                            .Where(x => x.SchoolId == schoolId
                                                    && x.StampId == stampId
                                                    && !x.IsActive
                                                    && x.TbRequest.RequestType > 0
                                                    && x.TbRequest.RequestStatus < 3)
                                            .Select(x => x.RequestId)
                                            .MaxAsync();

                int currentRequestStatusId = await this.context
                                                .TbRequests
                                                .Where(x => x.SchoolId == schoolId
                                                        && x.RequestId == currentRequestId)
                                                .Select(x => x.RequestStatus)
                                                .FirstOrDefaultAsync();

                if (currentRequestStatusId == 0)
                {
                    isValid = false;
                    message = $"Печат с УИН {stampId} има създадена заявка за промяна. Текущата заявка {currentRequestId} трябва да бъде обработена или изтрита от Вас.";
                }
                else if (currentRequestStatusId == 1)
                {
                    isValid = false;
                    message = $"Печат с УИН {stampId} има създадена заявка за промяна. Текущата заявка {currentRequestId} трябва да бъде одобрена от МОН.";
                }
                else if (currentRequestStatusId == 2)
                {
                    isValid = false;
                    message = $"Печат с УИН {stampId} има върната заявка за промяна. Заявка {currentRequestId} трябва да бъде коригирана или изтрита от Вас.";
                }
            }

            return (isValid, message);
        }
    }
}
