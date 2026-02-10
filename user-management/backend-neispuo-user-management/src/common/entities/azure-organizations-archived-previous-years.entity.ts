import { Column, Entity, PrimaryColumn } from 'typeorm';

@Entity({ schema: 'azure_temp', name: 'OrganizationsArchivedPreviousYears' })
export class OrganizationsArchivedPreviousYearsEntity {
    @PrimaryColumn({ name: 'rowID' })
    rowID: number;

    @Column({ name: 'organizationID' })
    organizationID: string;

    @Column({ name: 'workflowType' })
    workflowType: string;

    @Column({ name: 'name' })
    name: string;

    @Column({ name: 'description' })
    description: string;

    @Column({ name: 'principalId' })
    principalId: string;

    @Column({ name: 'principalName' })
    principalName: string;

    @Column({ name: 'principalEmail' })
    principalEmail: string;

    @Column({ name: 'highestGrade' })
    highestGrade: number;

    @Column({ name: 'lowestGrade' })
    lowestGrade: number;

    @Column({ name: 'phone' })
    phone: string;

    @Column({ name: 'city' })
    city: string;

    @Column({ name: 'area' })
    area: string;

    @Column({ name: 'country' })
    country: string;

    @Column({ name: 'postalCode' })
    postalCode: string;

    @Column({ name: 'street' })
    street: string;

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

    @Column({ name: 'status' })
    status: string;

    @Column({ name: 'username' })
    username: string;

    @Column({ name: 'password' })
    password: string;

    @Column({ name: 'personID' })
    personID: number;

    @Column({ name: 'isForArchivation' })
    isForArchivation: number;

    @Column({ name: 'azureID' })
    azureID: string;

    @Column({ name: 'inProgressResultCount' })
    inProgressResultCount: number;
}
