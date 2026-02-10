import { Column, PrimaryColumn, ViewEntity } from 'typeorm';

@ViewEntity({ schema: 'azure_temp', name: 'InstitutionsTable' })
export class InstitutionsTableEntity {
    @PrimaryColumn({ name: 'institutionID' })
    institutionID: number;

    @Column()
    sysUserID: number;

    @Column()
    sysRoleID: number;

    @Column()
    username: string;

    @Column()
    isAzureUser: boolean;

    @Column()
    financialSchoolTypeName: string;

    @Column()
    baseSchoolTypeName: string;

    @Column()
    detailedSchoolTypeName: string;

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
    institutionName: string;
}
