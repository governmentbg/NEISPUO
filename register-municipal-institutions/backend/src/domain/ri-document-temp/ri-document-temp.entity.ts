import { Column, Entity, PrimaryGeneratedColumn } from 'typeorm';

@Entity({ schema: 'reginst_basic', name: 'RIDocumentTemp' })
export class RIDocumentTemp {
    @PrimaryGeneratedColumn({ name: 'RIDocumentTempID' })
    RIDocumentTempID: number;

    @Column({ type: 'int' })
    BlobId: number;

    @Column({ type: 'int' })
    MunicipalityID: number;
}
