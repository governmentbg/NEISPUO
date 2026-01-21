import { DTO } from './dto.interface';

export class ClassesResponseDTO implements DTO {
    institutionID?: number;

    institutionAzureID?: string;

    curriculumID?: number;

    className?: string;

    subjectName?: string;

    generatedClassTitle?: string;

    azureID?: string;

    startDate?: string;

    endDate?: string;

    subjectTypeName?: string;
}
