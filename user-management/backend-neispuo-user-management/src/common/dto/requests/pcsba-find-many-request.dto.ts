import { IsIn, IsNumber } from 'class-validator';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';

export class ParentChildSchoolBookAccessesFindManyRequestDto {
    @IsNumber()
    personID?: number;

    @IsIn([UserRoleType.STUDENT, UserRoleType.PARENT])
    userRoleType: UserRoleType.STUDENT | UserRoleType.PARENT;
}
