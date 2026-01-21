import {
    Column,
    Entity,
    OneToMany,
    PrimaryGeneratedColumn,
} from 'typeorm';
import { RIFlexField } from '@domain/ri-flex-field/ri-flex-field.entity';
import { RIPremInstitution } from '@domain/ri-prem-institution/ri-prem-institution.entity';
import { RIInstitution } from '@domain/ri-institution/ri-institution.entity';
import { Exclude } from 'class-transformer';
import { RIInstitutionDepartment } from '@domain/ri-institution-department/ri-institution-department.entity';
import { RICPLRArea } from '@domain/ri-cplr-area/ri-cplr-area.entity';
import { SysUserSysRole } from '../sysuser-sysrole/sysuser-sysrole.entity';
import { RIProcedure } from '../ri-procedure/ri-procedure.entity';
import { Person } from '../person/person.entity';
import { Institution } from '../institution/institution.entity';

@Entity({ schema: 'core', name: 'SysUser' })
export class SysUser {
    @PrimaryGeneratedColumn({ name: 'SysUserID' })
    SysUserID: number;

    @Column({
        type: 'varchar',
        unique: true,
    })
    Username: string;

    @Column({
        type: 'varchar',
        nullable: true,
    })
    @Exclude() // deletes password field at middleware level if it is present on either incoming and outgoing DTOs
    Password: string;

    /**
     * Relations
     */

    @OneToMany(
        (type) => SysUserSysRole,
        (sysUserSysRole) => sysUserSysRole.SysUser,
    )
    SysUserSysRole: SysUserSysRole[];

    institutions: Institution;

    RIInstitutions: RIInstitution;

    RIInstitutionDepartments: RIInstitutionDepartment;

    persons: Person;

    riProcedures: RIProcedure;

    RIPremInstitution: RIPremInstitution;

    RIFlexFields: RIFlexField;

    RICPLRAreas?: RICPLRArea;

    /**
     * Trackers
     */

    // @CreateDateColumn()
    // @Exclude({toPlainOnly: true})
    // createdAt?: Date | string;
    //
    // @UpdateDateColumn()
    // @Exclude({toPlainOnly: true})
    // updatedAt?: Date | string;
}
