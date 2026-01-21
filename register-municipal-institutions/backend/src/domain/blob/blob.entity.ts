import {
 Column, Entity, JoinColumn, OneToOne, PrimaryGeneratedColumn,
} from 'typeorm';
import { BlobContent } from './blobcontent.entity';

@Entity('Blobs', { schema: 'blobs' })
export class BlobEntity {
    @PrimaryGeneratedColumn({ name: 'BlobId' })
    BlobId: number;

    @Column({ type: 'nvarchar', length: 500 })
    FileName: string;

    @Column({ type: 'datetime2' })
    CreateDate: Date;

    @Column({ type: 'timestamp' })
    Version: Date;

    @OneToOne(
        (type) => BlobContent,
        (blobContent) => blobContent.Blob,
    )
    @JoinColumn([{ name: 'BlobContentId', referencedColumnName: 'BlobContentId' }])
    BlobContent: BlobContent;
}
