import { Column, Entity, JoinColumn, ManyToOne, OneToMany, PrimaryGeneratedColumn } from 'typeorm';
import { BaseSchoolTypeEntity } from './base-school-type.entity';
import { ClassBookEntity } from './class-book.entity';
import { CountryEntity } from './country.entity';
import { DetailedSchoolTypeEntity } from './detailed-school-type.entity';
import { EducationalStateEntity } from './educational-state.entity';
import { FinancialSchoolTypeEntity } from './financial-school-type.entity';
import { SysUserSysRoleEntity } from './sys-user-sys-role.entity';
import { TownEntity } from './town.entity';

@Entity({ schema: 'core', name: 'Institution' })
export class InstitutionEntity {
    @PrimaryGeneratedColumn({ name: 'InstitutionID' })
    institutionID: number;

    @Column({ name: 'name' })
    name: string;

    @Column({ name: 'abbreviation' })
    abbreviation: string;

    @Column({ name: 'bulstat' })
    bulstat: string;

    @Column({ name: 'countryID' })
    countryID: number;

    @Column({ name: 'localAreaID' })
    localAreaID: number;

    @Column({ name: 'financialSchoolTypeID' })
    financialSchoolTypeID: number;

    @Column({ name: 'detailedSchoolTypeID' })
    detailedSchoolTypeID: number;

    @Column({ name: 'budgetingSchoolTypeID' })
    budgetingSchoolTypeID: number;

    @Column({ name: 'sysUserID' })
    sysUserID: number;

    @Column({ name: 'townID' })
    townID: number;

    @Column({ name: 'baseSchoolTypeID' })
    baseSchoolTypeID: number;

    @OneToMany((type) => EducationalStateEntity, (educationalStates) => educationalStates.institutionID)
    educationalStates: EducationalStateEntity[];

    @OneToMany((type) => SysUserSysRoleEntity, (sysUserSysRoles) => sysUserSysRoles.institutionID)
    sysUserSysRoles: SysUserSysRoleEntity[];

    @ManyToOne((type) => FinancialSchoolTypeEntity, (financialSchoolType) => financialSchoolType.institution)
    @JoinColumn({ name: 'financialSchoolTypeID', referencedColumnName: 'financialSchoolTypeID' })
    financialSchoolType: FinancialSchoolTypeEntity;

    @ManyToOne((type) => BaseSchoolTypeEntity, (baseSchoolType) => baseSchoolType.institution)
    @JoinColumn({ name: 'baseSchoolTypeID', referencedColumnName: 'baseSchoolTypeID' })
    baseSchoolType: BaseSchoolTypeEntity;

    @ManyToOne((type) => DetailedSchoolTypeEntity, (detailedSchoolType) => detailedSchoolType.institution)
    @JoinColumn({ name: 'detailedSchoolTypeID', referencedColumnName: 'detailedSchoolTypeID' })
    detailedSchoolType: DetailedSchoolTypeEntity;

    @ManyToOne((type) => CountryEntity, (country) => country.institution)
    @JoinColumn({ name: 'countryID', referencedColumnName: 'countryID' })
    country: CountryEntity;

    @ManyToOne((type) => TownEntity, (town) => town.institution)
    @JoinColumn({ name: 'townID', referencedColumnName: 'townID' })
    town: TownEntity;

    @OneToMany((type) => ClassBookEntity, (classBook) => classBook.institution)
    @JoinColumn({ name: 'institutionID', referencedColumnName: 'InstId' })
    classBooks: ClassBookEntity[];
}
