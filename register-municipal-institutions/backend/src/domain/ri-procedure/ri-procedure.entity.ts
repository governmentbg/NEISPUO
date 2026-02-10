/* eslint-disable @typescript-eslint/no-shadow */
import {
    Column,
    Entity,
    JoinColumn,
    ManyToOne,
    OneToMany,
    OneToOne,
    PrimaryGeneratedColumn,
} from 'typeorm';
import { Type } from 'class-transformer';
import { RIPremInstitution } from '@domain/ri-prem-institution/ri-prem-institution.entity';
import { RIInstitutionDepartment } from '@domain/ri-institution-department/ri-institution-department.entity';
import { RICPLRArea } from '@domain/ri-cplr-area/ri-cplr-area.entity';
import { RIInstitution } from '../ri-institution/ri-institution.entity';
import { ProcedureType } from '../procedure-type/procedure-type.entity';
import { TransformType } from '../transform-type/transform-type.entity';
import { StatusType } from '../status-type/status-type.entity';
import { SysUser } from '../sys-user/sys-user.entity';
import { RIDocument } from '../ri-document/ri-document.entity';

@Entity({ schema: 'reginst_basic', name: 'RIProcedure' })
export class RIProcedure {
    @PrimaryGeneratedColumn({ name: 'RIprocedureID' })
    RIProcedureID: number;

    @Column()
    InstitutionID: number;

    @Column()
    TransformDetails: string;

    @Column()
    YearDue: number;

    @Column()
    IsActive: boolean;

    @Type(() => Date)
    @Column('datetime')
    ProcedureDate: Date;

    /**
     * Relations
     */
    @OneToMany(
        (type) => RIInstitution,
        (RIInstitution) => RIInstitution.RIProcedure,
    )
    RIInstitutions?: RIInstitution[];

    @OneToMany(
        (type) => RIInstitutionDepartment,
        (RIInstitutionDepartments) => RIInstitutionDepartments.RIProcedure,
        { eager: true, cascade: true },
    )
    RIInstitutionDepartments: RIInstitutionDepartment[];

    @OneToOne(
        (type) => RIPremInstitution,
        (RIPremInstitution) => RIPremInstitution.RIProcedure,
    )
    RIPremInstitution?: RIPremInstitution;

    @OneToOne(
        (type) => RICPLRArea,
        (RICPLRArea) => RICPLRArea.RIProcedure,
        { eager: true, cascade: true },
    )
    RICPLRArea?: RICPLRArea;

    @OneToOne(
        (type) => RIDocument,
        (RIDocument) => RIDocument.RIProcedure,
    )
    RIDocument?: RIDocument;

    @ManyToOne(
        (type) => ProcedureType,
        (procedureType) => procedureType.riProcedures,
        { eager: true },
    )
    @JoinColumn({ name: 'ProcedureTypeID' })
    ProcedureType?: ProcedureType;

    @ManyToOne(
        (type) => TransformType,
        (transformType) => transformType.riProcedures,
    )
    @JoinColumn({ name: 'TransformTypeID' })
    TransformType?: TransformType;

    @ManyToOne(
        (type) => StatusType,
        (statusType) => statusType.riProcedures,
    )
    @JoinColumn({ name: 'StatusTypeID' })
    StatusType?: StatusType;

    // @ManyToOne(
    //     type => Institution,
    //     Institution => Institution.RIPRocedure
    // )
    // @JoinColumn({ name: 'InstitutionID' })
    // Institution: Institution;

    @ManyToOne(
        (type) => SysUser,
        (sysUser) => sysUser.riProcedures,
    )
    @JoinColumn({ name: 'SysUserID' })
    SysUser?: SysUser;

    /**
     * Trackers
     */

    @Column({
        type: 'datetime2',
    })
    ValidFrom?: Date;

    @Column({
        type: 'datetime2',
    })
    ValidTo?: Date;

    @Column()
    Ord: Number;
}
