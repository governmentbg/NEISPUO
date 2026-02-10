import { IsNotEmpty, IsNumber } from 'class-validator';

export class GetOrganizationInfoRequestDto {
    @IsNumber()
    @IsNotEmpty()
    schoolId: number;
}
