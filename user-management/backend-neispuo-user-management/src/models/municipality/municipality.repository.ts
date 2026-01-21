import { MunicipalityResponseDTO } from 'src/common/dto/responses/municipality-response.dto';
import { MunicipalityMapper } from 'src/common/mappers/municipality.mapper';
import { SysUserEntity } from 'src/common/entities/sys-user.entity';
import { EntityRepository, Repository } from 'typeorm';
import { RoleEnum } from 'src/common/constants/enum/role.enum';

@EntityRepository(SysUserEntity)
export class MunicipalityRepository extends Repository<SysUserEntity> {
    async getAllMunicipality() {
        const result = await this.createQueryBuilder('user')
            .select('user.sysUserID as sysUserID')
            .addSelect('user.isAzureUser as isAzureUser')
            .addSelect('user.username as username')
            .addSelect('mun.name as name')
            .leftJoin('user.sysUserSysRoles', 'susr')
            .leftJoin('susr.municipality', 'mun')
            .leftJoin('susr.role', 'rol')
            .where(`rol.sysRoleId = ${RoleEnum.MUNICIPALITY}`)
            .andWhere('user.DeletedOn IS NULL')
            .take(100)
            .getRawMany();
        const transformedResult: MunicipalityResponseDTO[] = MunicipalityMapper.transform(result);
        return transformedResult;
    }
}
