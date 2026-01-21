import { RUOResponseDTO } from 'src/common/dto/responses/ruo-response.dto';
import { RUOMapper } from 'src/common/mappers/ruo.mapper';
import { SysUserEntity } from 'src/common/entities/sys-user.entity';
import { EntityRepository, Repository } from 'typeorm';
import { RoleEnum } from 'src/common/constants/enum/role.enum';

@EntityRepository(SysUserEntity)
export class RUORepository extends Repository<SysUserEntity> {
    async getAllRUOs() {
        const result = await this.createQueryBuilder('user')
            .select('user.sysUserID as sysUserID')
            .addSelect('user.isAzureUser as isAzureUser')
            .addSelect('user.username as username')
            .addSelect('user.DeletedOn as deletedOn')
            .addSelect('person.firstName as firstName')
            .addSelect('person.middleName as middleName')
            .addSelect('person.lastName as lastName')
            .addSelect('rol.name as roleName')
            .addSelect('reg.name as regionName')
            .leftJoin('user.person', 'person')
            .leftJoin('user.sysUserSysRoles', 'susr')
            .leftJoin('susr.role', 'rol')
            .leftJoin('susr.region', 'reg')
            .where(`rol.sysRoleId = ${RoleEnum.RUO}`)
            .orWhere(`rol.sysRoleId = ${RoleEnum.RUO_EXPERT}`)
            .andWhere(`user.deletedOn IS NULL`)
            .take(100)
            .getRawMany();
        const transformedResult: RUOResponseDTO[] = RUOMapper.transform(result);
        return transformedResult;
    }
}
