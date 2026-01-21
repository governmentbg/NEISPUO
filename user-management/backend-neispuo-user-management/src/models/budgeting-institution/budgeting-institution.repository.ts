import { BudgetingInstitutionResponseDTO } from 'src/common/dto/responses/budgeting-institution-response.dto';
import { BudgetingInstitutionMapper } from 'src/common/mappers/budgeting-institutions.mapper';
import { SysUserEntity } from 'src/common/entities/sys-user.entity';
import { EntityRepository, Repository } from 'typeorm';
import { RoleEnum } from 'src/common/constants/enum/role.enum';

@EntityRepository(SysUserEntity)
export class BudgetingInstitutionsRepository extends Repository<SysUserEntity> {
    async getAllBudgetingInstitutions() {
        const result = await this.createQueryBuilder('user')
            .select('user.sysUserID as sysUserID')
            .addSelect('user.isAzureUser as isAzureUser')
            .addSelect('user.username as username')
            .addSelect('bi.name as name')
            .leftJoin('user.sysUserSysRoles', 'susr')
            .leftJoin('susr.budgetingInstitution', 'bi')
            .leftJoin('susr.role', 'rol')
            .where(`rol.sysRoleId = ${RoleEnum.BUDGETING_INSTITUTION}`)
            .andWhere(`user.deletedOn IS NULL`)
            .getRawMany();
        const transformedResult: BudgetingInstitutionResponseDTO[] = BudgetingInstitutionMapper.transform(result);
        return transformedResult;
    }
}
