import {
 Column, Entity, PrimaryColumn,
} from 'typeorm';
import { RIProcedure } from '../ri-procedure/ri-procedure.entity';

@Entity({ schema: 'reginst_nom', name: 'TransformType' })
export class TransformType {
    @PrimaryColumn()
    TransformTypeID: number;

    @Column({
        type: 'varchar',
    })
    Name: string;

    @Column({
        type: 'varchar',
    })
    PublicName: string;

    @Column({
        type: 'tinyint',
        nullable: true,
    })
    IsNEISPUOActive: boolean;

    @Column({
        type: 'tinyint',
        nullable: true,
    })
    IsNEISPUOData: boolean;

    @Column({
        type: 'tinyint',
        nullable: true,
    })
    IsNEISPUOAccessDenied: boolean;

    @Column({
        type: 'tinyint',
        nullable: true,
    })
    IsValid: boolean;

    /**
     * Relations
     */

    riProcedures: RIProcedure;

    /**
     * Trackers
     */

    @Column({
        type: 'datetime',
    })
    ValidFrom: Date;

    @Column({
        type: 'datetime',
    })
    ValidTo: Date;
}
