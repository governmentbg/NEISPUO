import {
 Entity, JoinColumn, ManyToOne, PrimaryColumn,
} from 'typeorm';
import { BudgetingInstitution } from '../budgeting-institution/budgeting-institution.entity';
import { Institution } from '../institution/institution.entity';
import { Municipality } from '../municipality/municipality.entity';
import { Region } from '../region/region.entity';
import { SysRole } from '../sys-role/sys-role.entity';
import { SysUser } from '../sys-user/sys-user.entity';

@Entity({ schema: 'core', name: 'SysUserSysRole' })
export class SysUserSysRole {
    @PrimaryColumn()
    SysUserID: number; // This is only for typeorm not to complain, table has no primary key

    /**
     * Relations
     */
    @ManyToOne(
        (type) => SysUser,
        (sysUser) => sysUser.SysUserSysRole,
    )
    @JoinColumn({ name: 'SysUserID' })
    SysUser: SysUser;

    @ManyToOne(
        (type) => SysRole,
        (sysUser) => sysUser.SysUserSysRole,
    )
    @JoinColumn({ name: 'SysRoleID' })
    SysRole: SysRole;

    @ManyToOne((type) => Institution)
    @JoinColumn({ name: 'InstitutionID' })
    Institution: Institution;

    @ManyToOne((type) => BudgetingInstitution)
    @JoinColumn({ name: 'BudgetingInstitutionID' })
    BudgetingInstitution: BudgetingInstitution;

    @ManyToOne((type) => Municipality)
    @JoinColumn({ name: 'MunicipalityID' })
    Municipality: Municipality;

    @ManyToOne((type) => Region)
    @JoinColumn({ name: 'RegionID' })
    Region: Region;
}
