import { Column, Entity, OneToMany, PrimaryGeneratedColumn } from 'typeorm';
import { SysUserSysRoleEntity } from './sys-user-sys-role.entity';

@Entity({ schema: 'noms', name: 'BudgetingInstitution' })
export class BudgetingInstitutionEntity {
    @PrimaryGeneratedColumn({ name: 'budgetingInstitutionID' })
    budgetingInstitutionID: number;

    @Column({ name: 'name' })
    name: string;

    @Column({ name: 'description' })
    description: string;

    @Column({ name: 'isValid' })
    isValid: number;

    @OneToMany(() => SysUserSysRoleEntity, (sysUserSysRoles) => sysUserSysRoles.budgetingInstitution)
    sysUserSysRoles: SysUserSysRoleEntity[];
}
