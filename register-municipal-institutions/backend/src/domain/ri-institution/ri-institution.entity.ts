import {
    Column,
    Entity,
    JoinColumn,
    ManyToOne,
    OneToMany,
    PrimaryGeneratedColumn,
} from 'typeorm';
import { RIFlexFieldValue } from '@domain/ri-flex-field-value/ri-flex-field-value.entity';
import { FinancialSchoolType } from '@domain/financial-school-type/financial-school-type.entity';
import { DetailedSchoolType } from '@domain/detailed-school-type/detailed-school-type.entity';
import { BudgetingInstitution } from '@domain/budgeting-institution/budgeting-institution.entity';
import { BaseSchoolType } from '@domain/base-school-type/base-school-type.entity';
import { Town } from '@domain/town/town.entity';
import { LocalArea } from '@domain/local-area/local-area.entity';
import { SysUser } from '../sys-user/sys-user.entity';
import { RIProcedure } from '../ri-procedure/ri-procedure.entity';
import { Country } from '../country/country.entity';

@Entity({ schema: 'reginst_basic', name: 'RIInstitution' })
export class RIInstitution {
    @PrimaryGeneratedColumn({ name: 'RIinstitutionID' })
    RIInstitutionID: number;

    @Column()
    Name: string;

    @Column()
    Abbreviation: string;

    @Column()
    Bulstat: string;

    // @ManyToOne(
    //     type => Institution,
    //     institution => institution.RIInstitution,
    //     { eager: true }
    // )
    // @JoinColumn({ name: 'InstitutionID' })
    // Institution?: Institution;

    @Column({ name: 'InstitutionID' })
    InstitutionID: number;

    @ManyToOne(
        (type) => FinancialSchoolType,
        (financialSchoolType) => financialSchoolType.institutions,
    )
    @JoinColumn({ name: 'FinancialSchoolTypeID' })
    FinancialSchoolType: FinancialSchoolType;

    @ManyToOne(
        (type) => DetailedSchoolType,
        (detailedSchoolType) => detailedSchoolType.institutions,
    )
    @JoinColumn({ name: 'DetailedSchoolTypeID' })
    DetailedSchoolType: DetailedSchoolType;

    @ManyToOne(
        (type) => BudgetingInstitution,
        (budgetingInstitution) => budgetingInstitution.institutions,
    )
    @JoinColumn({ name: 'BudgetingSchoolTypeID' })
    BudgetingInstitution: BudgetingInstitution;

    @ManyToOne(
        (type) => BaseSchoolType,
        (baseSchoolType) => baseSchoolType.riInstitutions,
        { eager: true },
    )
    @JoinColumn({ name: 'BaseSchoolTypeID' })
    BaseSchoolType: BaseSchoolType;

    @ManyToOne(
        (type) => Country,
        (country) => country.institutions,
    )
    @JoinColumn({ name: 'TRCountryID' })
    Country?: Country;

    @ManyToOne(
        (type) => LocalArea,
        (localArea) => localArea.institutions,
    )
    @JoinColumn({ name: 'TRLocalAreaID' })
    LocalArea?: LocalArea;

    @ManyToOne(
        (type) => Town,
        (town) => town.riInstitutions,
    )
    @JoinColumn({ name: 'TRTownID' })
    Town?: Town;

    @Column()
    TRAddress: string;

    @Column()
    TRPostCode: number;

    @Column()
    ReligInstDetails: string;

    @Column()
    HeadFirstName: string;

    @Column()
    HeadMiddleName: string;

    @Column()
    HeadLastName: string;

    @Column()
    IsNational: boolean;

    @Column()
    PersonnelCount: number;

    @Column()
    AuthProgram: string;

    @Column()
    IsDataDue: boolean;

    @ManyToOne(
        (type) => SysUser,
        (sysUser) => sysUser.RIInstitutions,
    )
    @JoinColumn({ name: 'SysUserID' })
    SysUser?: SysUser;

    /**
     * Relations
     */

    @ManyToOne(
        (type) => RIProcedure,
        (riProcedure) => riProcedure.RIInstitutions,
        { eager: true },
    )
    @JoinColumn({ name: 'RIprocedureID' })
    RIProcedure: RIProcedure;

    @OneToMany(
        (type) => RIFlexFieldValue,
        (riFlexFieldValue) => riFlexFieldValue.RIInstitution,
    )
    RIFlexFieldValues?: RIFlexFieldValue[];

    /** There is no FK in DB at this moment */
    // @OneToMany(
    //     type => Institution,
    //     institution => institution.riInstitution
    // )
    // institution: Institution[];

    /**
     * Trackers
     */

    @Column({
        type: 'datetime2',
    })
    ValidFrom: Date;

    @Column({
        type: 'datetime2',
    })
    ValidTo: Date;
}
