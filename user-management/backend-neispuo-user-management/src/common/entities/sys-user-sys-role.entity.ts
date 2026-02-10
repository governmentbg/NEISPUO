import { Column, Entity, JoinColumn, ManyToOne, PrimaryColumn } from 'typeorm';
import { BudgetingInstitutionEntity } from './budgeting-institution.entity';
import { InstitutionEntity } from './institution.entity';
import { MunicipalityEntity } from './municipality.entity';
import { RegionEntity } from './region.entity';
import { SysRoleEntity } from './sys-role.entity';
import { SysUserEntity } from './sys-user.entity';

@Entity({ schema: 'core', name: 'SysUserSysRole' })
export class SysUserSysRoleEntity {
    @PrimaryColumn({ name: 'sysRoleID' })
    sysRoleID: number;

    @PrimaryColumn({ name: 'sysUserID' })
    sysUserID: number;

    @Column({ name: 'institutionID' })
    institutionID: number;

    @Column({ name: 'budgetingInstitutionID' })
    budgetingInstitutionID: number;

    @Column({ name: 'municipalityID' })
    municipalityID: number;

    @Column({ name: 'regionID' })
    regionID: number;

    @ManyToOne((type) => SysRoleEntity, (roleEntity) => roleEntity.sysUserSysRoles)
    @JoinColumn({ name: 'sysRoleID', referencedColumnName: 'sysRoleID' })
    role: SysRoleEntity;

    @ManyToOne((type) => SysUserEntity, (userEntity) => userEntity.sysUserSysRoles)
    @JoinColumn({ name: 'sysUserID', referencedColumnName: 'sysUserID' })
    user: SysUserEntity;

    @ManyToOne((type) => InstitutionEntity, (institutionEntity) => institutionEntity.sysUserSysRoles)
    @JoinColumn({ name: 'institutionID', referencedColumnName: 'institutionID' })
    institution: InstitutionEntity;

    @ManyToOne(() => MunicipalityEntity, (municipality) => municipality.sysUserSysRoles)
    @JoinColumn({ name: 'municipalityID', referencedColumnName: 'municipalityID' })
    municipality: MunicipalityEntity;

    @ManyToOne(() => RegionEntity, (region) => region.sysUserSysRoles)
    @JoinColumn({ name: 'regionID', referencedColumnName: 'regionID' })
    region: RegionEntity;

    @ManyToOne(() => BudgetingInstitutionEntity, (budgetingInstitution) => budgetingInstitution.sysUserSysRoles)
    @JoinColumn({ name: 'budgetingInstitutionID', referencedColumnName: 'budgetingInstitutionID' })
    budgetingInstitution: BudgetingInstitutionEntity;
}
