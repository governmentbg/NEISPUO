import {
 Column, Entity, OneToOne, PrimaryGeneratedColumn,
} from 'typeorm';
import { BlobEntity } from './blob.entity';

@Entity('BlobContents', { schema: 'blobs' })
export class BlobContent {
    @PrimaryGeneratedColumn({ name: 'BlobContentId' })
    BlobContentId: number;

    @Column('nvarchar', { nullable: true, length: 64 })
    Hash: string | null;

    @Column('bigint', { nullable: true })
    Size: string | null;

    @Column('varbinary', { nullable: true })
    Content: Buffer | null;

    @Column('datetime2')
    CreateDate: Date;

    @Column('datetime2')
    ModifyDate: Date;

    @Column('timestamp')
    Version: Date;

    @OneToOne(
        (type) => BlobEntity,
        (blob) => blob.BlobContent,
    )
    Blob: BlobEntity;
}
