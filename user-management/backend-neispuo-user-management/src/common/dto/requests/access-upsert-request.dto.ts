import { IsEnum, IsNumber } from 'class-validator';
import { ParentHasAccessEnum } from 'src/common/constants/enum/parent-has-access.enum';
import { DTO } from '../responses/dto.interface';

export class AccessUpsertRequestDTO implements DTO {
    @IsNumber()
    parentID: number;

    @IsNumber()
    childID: number;

    @IsEnum(ParentHasAccessEnum)
    hasAccess: ParentHasAccessEnum;
}
