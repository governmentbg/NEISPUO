import { IsIn, IsNumber, IsOptional, IsString } from 'class-validator';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';

export class UserFindManyRequestDto {
    @IsOptional()
    @IsNumber()
    personID?: number;

    @IsOptional()
    @IsString()
    email?: string;

    @IsIn([UserRoleType.STUDENT, UserRoleType.PARENT])
    userRoleType: UserRoleType.STUDENT | UserRoleType.PARENT;
}
