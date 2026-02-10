import {
 Column, Entity, OneToMany, PrimaryColumn,
} from 'typeorm';
import { RIProcedure } from '../ri-procedure/ri-procedure.entity';

@Entity({ schema: 'reginst_nom', name: 'ProcedureType' })
export class ProcedureType {
    @PrimaryColumn()
    ProcedureTypeID: number;

    @Column()
    Name: string;

    @Column()
    Description: string;

    @Column()
    IsValid: boolean;

    /**
     * Relations
     */
     @OneToMany(
        (type) => RIProcedure,
        (riProcedure) => riProcedure.ProcedureType,
    )
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
