import {
 Column, Entity, PrimaryColumn,
} from 'typeorm';
import { RIProcedure } from '../ri-procedure/ri-procedure.entity';

@Entity({ schema: 'reginst_nom', name: 'StatusType' })
export class StatusType {
    @PrimaryColumn()
    StatusTypeID: number;

    @Column({
        type: 'varchar',
    })
    Name: string;

    @Column({
        type: 'varchar',
        nullable: true,
    })
    Description: string;

    @Column({
        type: 'tinyint',
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
