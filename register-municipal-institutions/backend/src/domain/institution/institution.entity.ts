import {
    Column,
    Entity,
    JoinColumn,
    ManyToOne,
    PrimaryColumn,
} from 'typeorm';
import { RIInstitution } from '@domain/ri-institution/ri-institution.entity';
import { RIProcedure } from '@domain/ri-procedure/ri-procedure.entity';
import { Country } from '../country/country.entity';
import { LocalArea } from '../local-area/local-area.entity';
import { FinancialSchoolType } from '../financial-school-type/financial-school-type.entity';
import { DetailedSchoolType } from '../detailed-school-type/detailed-school-type.entity';
import { BudgetingInstitution } from '../budgeting-institution/budgeting-institution.entity';
import { SysUser } from '../sys-user/sys-user.entity';
import { Town } from '../town/town.entity';
import { BaseSchoolType } from '../base-school-type/base-school-type.entity';

@Entity({ schema: 'core', name: 'Institution' })
export class Institution {
    @PrimaryColumn({ name: 'InstitutionID' })
    InstitutionID: number;

    @Column()
    Name: string;

    @Column()
    Abbreviation: string;

    @Column()
    Bulstat: string;

    RIInstitution?: RIInstitution;

    RIPRocedure?: RIProcedure;

    /**
     * Relations
     */

    @ManyToOne(
        (type) => Country,
        (country) => country.institutions,
    )
    @JoinColumn({ name: 'CountryID' })
    Country?: Country;

    @ManyToOne(
        (type) => LocalArea,
        (localArea) => localArea.institutions,
    )
    @JoinColumn({ name: 'LocalAreaID', referencedColumnName: 'LocalAreaID' })
    LocalArea?: LocalArea;

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
        (type) => SysUser,
        (sysUser) => sysUser.institutions,
    )
    @JoinColumn({ name: 'SysUserID' })
    SysUser?: SysUser;

    @ManyToOne(
        (type) => Town,
        (town) => town.institutions,
    )
    @JoinColumn({ name: 'TownID' })
    Town?: Town;

    @ManyToOne(
        (type) => BaseSchoolType,
        (baseSchoolType) => baseSchoolType.institutions,
    )
    @JoinColumn({ name: 'BaseSchoolTypeID' })
    BaseSchoolType: BaseSchoolType;

    /** There is no FK in DB at this moment */
    // @ManyToOne(
    //     type => RIInstitution,
    //     riInstitution => riInstitution.institution
    // )
    // @JoinColumn({ name: 'RIinstitutionID' })
    // riInstitution: RIInstitution;

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
