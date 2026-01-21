import { Column, PrimaryGeneratedColumn, ViewEntity } from 'typeorm';

@ViewEntity({ schema: 'azure_temp', name: 'ExternalUserView' })
export class ExternalUserViewEntity {
    @PrimaryGeneratedColumn()
    sysUserID: number;

    @Column()
    username: string;

    @Column()
    isAzureUser: number;

    @Column()
    firstName: string;

    @Column()
    middleName: string;

    @Column()
    lastName: string;

    @Column()
    threeNames: string;

    @Column()
    financialSchoolTypeName: string;

    @Column()
    financialSchoolTypeID: number;

    @Column()
    baseSchoolTypeName: string;

    @Column()
    baseSchoolTypeID: number;

    @Column()
    detailedSchoolTypeName: string;

    @Column()
    detailedSchoolTypeID: number;

    @Column()
    townName: string;

    @Column()
    municipalityID: number;

    @Column()
    municipalityName: string;

    @Column()
    regionID: number;

    @Column()
    regionName: string;

    @Column()
    institutionID: number;

    @Column()
    institutionName: string;

    @Column()
    budgetingInstitutionID: number;

    @Column()
    budgetingInstitutionName: string;

    @Column()
    SysRoleID: number;

    @Column()
    RoleName: string;
}
