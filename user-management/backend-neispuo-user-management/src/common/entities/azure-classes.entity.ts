import { Column, PrimaryColumn, ViewEntity } from 'typeorm';

@ViewEntity({ schema: 'azure_temp', name: 'AzureClassesView' })
export class AzureClassesEntity {
    @PrimaryColumn({ name: 'rowID' })
    rowID: number;

    @Column({ name: 'classID' })
    classID: string;

    @Column({ name: 'workflowType' })
    workflowType: string;

    @Column({ name: 'title' })
    title: string;

    @Column({ name: 'classCode' })
    classCode: string;

    @Column({ name: 'orgID' })
    orgID: string;

    @Column({ name: 'termID' })
    termID: number;

    @Column({ name: 'termName' })
    termName: string;

    @Column({ name: 'termStartDate' })
    termStartDate: Date;

    @Column({ name: 'termEndDate' })
    termEndDate: Date;

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

    @Column({ name: 'azureID' })
    azureID: string;

    @Column({ name: 'inProgressResultCount' })
    inProgressResultCount: number;

    @Column({ name: 'isForArchivation' })
    isForArchivation: number;
}
