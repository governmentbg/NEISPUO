import { Column, Entity, PrimaryColumn } from 'typeorm';

@Entity({ schema: 'azure_temp', name: 'UsersArchivedPreviousYears' })
export class UsersArchivedPreviousYearsEntity {
    @PrimaryColumn({ name: 'rowID' })
    rowID: number;

    @Column({ name: 'userID' })
    userID: string;

    @Column({ name: 'workflowType' })
    workflowType: string;

    @Column({ name: 'identifier' })
    identifier: string;

    @Column({ name: 'firstName' })
    firstName: string;

    @Column({ name: 'middleName' })
    middleName: string;

    @Column({ name: 'surname' })
    surname: string;

    @Column({ name: 'password' })
    password: string;

    @Column({ name: 'email' })
    email: string;

    @Column({ name: 'phone' })
    phone: string;

    @Column({ name: 'grade' })
    grade: string;

    @Column({ name: 'schoolId' })
    schoolId: string;

    @Column({ name: 'birthDate' })
    birthDate: string;

    @Column({ name: 'userRole' })
    userRole: string;

    @Column({ name: 'accountEnabled' })
    accountEnabled: number;

    @Column({ name: 'inProcessing' })
    inProcessing: number;

    @Column({ name: 'errorMessage' })
    errorMessage: string;

    @Column({ name: 'createdOn' })
    createdOn: Date;

    @Column({ name: 'updatedOn' })
    updatedOn: Date;

    @Column({ name: 'guid' })
    guid: string;

    @Column({ name: 'retryAttempts' })
    retryAttempts: number;

    @Column({ name: 'username' })
    username: string;

    @Column({ name: 'status' })
    status: string;

    @Column({ name: 'personID' })
    personID: number;

    @Column({ name: 'deletionType', nullable: true })
    deletionType: number;

    @Column({ name: 'additionalRole', nullable: true })
    additionalRole: number;

    @Column({ name: 'hasNeispuoAccess', nullable: true })
    hasNeispuoAccess: number;

    @Column({ name: 'assignedAccountantSchools', nullable: true })
    assignedAccountantSchools: string;

    @Column({ name: 'azureID', nullable: true })
    azureID: string;

    @Column({ name: 'inProgressResultCount', nullable: true })
    inProgressResultCount: number;

    @Column({ name: 'isForArchivation' })
    isForArchivation: number;

    @Column({ name: 'sisAccessSecondaryRole', nullable: true })
    sisAccessSecondaryRole: number;

    @Column({ name: 'createdBy', nullable: true })
    createdBy: string;
}
