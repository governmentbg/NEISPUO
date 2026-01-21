import {
 Column, Entity, JoinColumn, ManyToOne, PrimaryGeneratedColumn,
} from 'typeorm';
import { RIProcedure } from '../ri-procedure/ri-procedure.entity';

@Entity({ schema: 'reginst_basic', name: 'RIDocument' })
export class RIDocument {
    @PrimaryGeneratedColumn({ name: 'RIDocumentID' })
    RIDocumentID: number;

    @Column({ type: 'nvarchar', length: 255 })
    DocumentNo: string;

    @Column({ type: 'date' })
    DocumentDate: Date | string;

    @Column({ type: 'nvarchar', length: 2048 })
    DocumentNotes: string;

    @Column({ type: 'int' })
    DocumentFile: number;

    @Column({ type: 'nvarchar', length: 255 })
    DocumentPublishedNoYear: string;

    @Column({ type: 'nvarchar', length: 50 })
    StateNewspaperData: string;

    /**
     * Relations
     */

    @ManyToOne(
        (type) => RIProcedure,
        (riProcedure) => riProcedure.RIDocument,
        { nullable: false },
    )
    @JoinColumn({ name: 'RIprocedureID' })
    RIProcedure: RIProcedure;

    // TODO: Add SysUser as in db
}
