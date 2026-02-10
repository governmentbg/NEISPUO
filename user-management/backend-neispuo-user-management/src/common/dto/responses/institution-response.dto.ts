import { DTO } from './dto.interface';

export class InstitutionResponseDTO implements DTO {
    sysUserID?: number;

    isAzureUser?: boolean;

    username?: string;

    institutionID?: number;

    institutionName?: string;

    townName?: string;

    municipalityName?: string;

    regionName?: string;

    baseSchoolTypeName?: string;

    detailedSchoolTypeName?: string;

    financialSchoolTypeName?: string;

    description?: string;

    principalId?: number;

    principalName?: string;

    principalEmail?: string;

    highestGrade?: number;

    lowestGrade?: number;

    phone?: string;

    areaName?: string;

    countryName?: string;

    postalCode?: string;

    street?: string;

    personID?: number;

    azureID?: string;
}
