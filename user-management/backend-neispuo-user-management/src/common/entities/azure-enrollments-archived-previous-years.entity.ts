import { Column, Entity, PrimaryColumn } from 'typeorm';

@Entity({
    schema: 'azure_temp',
    name: 'EnrollmentsArchivedPreviousYears',
})
export class EnrollmentsArchivedPreviousYearsEntity {
    @PrimaryColumn({ name: 'rowID' })
    rowID: number;

    @Column({ name: 'workflowType' })
    workflowType: string;

    @Column({ name: 'userAzureID' })
    userAzureID: string;

    @Column({ name: 'classAzureID' })
    classAzureID: string;

    @Column({ name: 'organizationAzureID' })
    organizationAzureID: string;

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

    @Column({ name: 'inProgressResultCount' })
    inProgressResultCount: number;

    @Column({ name: 'userPersonID' })
    userPersonID: number;

    @Column({ name: 'organizationPersonID' })
    organizationPersonID: number;

    @Column({ name: 'curriculumID' })
    curriculumID: number;

    @Column({ name: 'userRole' })
    userRole: string;

    @Column({ name: 'isForArchivation' })
    isForArchivation: number;
}
