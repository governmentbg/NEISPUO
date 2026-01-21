import { AzureClassesResponseDTO } from '../dto/responses/azure-classes-response.dto';
import { ClassesResponseDTO } from '../dto/responses/classes-response.dto';

export class AzureClassesResponseFactory {
    static createFromClassesResponseDTO(response: ClassesResponseDTO) {
        const { institutionID, curriculumID, generatedClassTitle, azureID, startDate, endDate } = response;
        const currentMonth = new Date().getMonth();
        const currentYear = new Date().getFullYear();
        const isSecondSemester = currentMonth < 6 && currentMonth >= 0 ? true : false;
        // we dont want to be 2024 and the date to be 2024-09-15 so we substract one
        const defaultStartYear = isSecondSemester ? currentYear - 1 : currentYear;
        // when its 2024 we want it to be 2024. when its 2023 we also want to make the date 2023 just in case
        const defaultEndYear = isSecondSemester ? currentYear : currentYear + 1;
        const dto: AzureClassesResponseDTO = {
            title: generatedClassTitle?.substring(0, 230),
            orgID: institutionID.toString(),
            termStartDate: startDate || `${defaultStartYear}-09-15`,
            termEndDate: endDate || `${defaultEndYear}-06-30`,
            classID: `${curriculumID}`,
            azureID,
        };
        return dto;
    }
}
