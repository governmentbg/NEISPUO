import { ForbiddenException, Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { SysRoleEnum } from 'src/shared/enums/role.enum';
import { AuthObject } from 'src/shared/interfaces/authed-request.interface';
import { Repository } from 'typeorm';
import { SysUser } from '../sys-user/sys-user.entity';
import { Report } from './report.entity';

@Injectable()
export class ReportService extends TypeOrmCrudService<Report> {
  constructor(
    @InjectRepository(Report)
    public readonly reportsRepository: Repository<Report>,
  ) {
    super(reportsRepository);
  }

  async createReport(report: Report, authObject: AuthObject) {
    report.CreatedBy = {
      SysUserID: authObject.selectedRole.SysUserID,
    } as SysUser;

    if (authObject.isRuo) {
      report.RegionID = authObject.selectedRole.RegionID;
    }

    if (authObject.isMunicipality) {
      report.MunicipalityID = authObject.selectedRole.MunicipalityID;
    }
  }

  async validateShareWith(report: Report, authObject: AuthObject) {
    if (!authObject.isMon && !authObject.isRuo && !authObject.isMunicipality) {
      throw new ForbiddenException(
        `Report sharing is not allowed for a user with ${authObject.selectedRole.SysRoleID} role`,
        'ROLE_NOT_ALLOWED_TO_SHARE',
      );
    }

    if (authObject.isRuo) {
      const allowedRolesToShare = [
        SysRoleEnum.RUO,
        SysRoleEnum.RUO_EXPERT,
        SysRoleEnum.MUNICIPALITY,
        SysRoleEnum.INSTITUTION,
      ];
      const containsAll = report.SharedWith.every((element) => {
        return allowedRolesToShare.includes(element);
      });

      if (!containsAll) {
        throw new ForbiddenException(
          `A user with the ${
            SysRoleEnum.RUO
          } role can only share with roles ${allowedRolesToShare.join(', ')}`,
          'ROLES_NOT_ALLOWED_TO_SHARE_WITH',
        );
      }
    }

    if (authObject.isMunicipality) {
      const allowedRolesToShare = [
        SysRoleEnum.MUNICIPALITY,
        SysRoleEnum.INSTITUTION,
      ];
      const containsAll = report.SharedWith.every((element) => {
        return allowedRolesToShare.includes(element);
      });

      if (!containsAll) {
        throw new ForbiddenException(
          `A user with the ${
            SysRoleEnum.MUNICIPALITY
          } role can only share with roles ${allowedRolesToShare.join(', ')}`,
          'ROLES_NOT_ALLOWED_TO_SHARE_WITH',
        );
      }
    }
  }
}
