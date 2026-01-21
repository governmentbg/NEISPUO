import { MonResponseDTO } from 'src/common/dto/responses/mon-response.dto';
import { MonMapper } from 'src/common/mappers/mon.mapper';
import { SysUserEntity } from 'src/common/entities/sys-user.entity';
import { EntityRepository, Repository } from 'typeorm';
import { RoleEnum } from 'src/common/constants/enum/role.enum';

@EntityRepository(SysUserEntity)
export class MonRepository extends Repository<SysUserEntity> {
    async getAllMons() {
        const result = await this.createQueryBuilder('user')
            .select('user.sysUserID as sysUserID')
            .addSelect('user.isAzureUser as isAzureUser')
            .addSelect('user.username as username')
            .addSelect('user.DeletedOn as deletedOn')
            .addSelect('person.firstName as firstName')
            .addSelect('person.middleName as middleName')
            .addSelect('person.lastName as lastName')
            .addSelect('rol.name as roleName')
            .leftJoin('user.sysUserSysRoles', 'susr')
            .leftJoin('user.person', 'person')
            .leftJoin('susr.role', 'rol')
            .where(`rol.sysRoleId = ${RoleEnum.MON_ADMIN}`)
            .orWhere(`rol.sysRoleId = ${RoleEnum.MON_EXPERT}`)
            .orWhere(`rol.sysRoleId = ${RoleEnum.MON_OBGUM}`)
            .orWhere(`rol.sysRoleId = ${RoleEnum.MON_OBGUM_FINANCES}`)
            .orWhere(`rol.sysRoleId = ${RoleEnum.MON_CHRAO}`)
            .orWhere(`rol.sysRoleId = ${RoleEnum.MON_USER_ADMIN}`)
            .andWhere('user.deletedOn is NULL')
            .take(100)
            .getRawMany();
        const transformedResult: MonResponseDTO[] = MonMapper.transform(result);
        return transformedResult;
    }
}
