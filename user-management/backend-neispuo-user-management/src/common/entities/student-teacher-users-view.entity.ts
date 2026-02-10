import { Column, ViewEntity } from 'typeorm';

@ViewEntity({ schema: 'azure_temp', name: 'StudentTeacherUsers' })
export class StudentTeacherUsersEntity {
    @Column()
    personID: number;

    @Column()
    firstName: string;

    @Column()
    threeNames: string;

    @Column()
    middleName: string;

    @Column()
    lastName: string;

    @Column()
    schoolBooksCodesID: string;

    @Column()
    institutionID: number;

    @Column()
    institutionName: string;

    @Column()
    townID: number;

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
    positionName: string;

    @Column()
    personalID: string;

    @Column()
    publicEduNumber: number;

    @Column()
    sysRoleID: number;

    @Column()
    azureID: string;

    @Column()
    hasAzureID: boolean;
}
