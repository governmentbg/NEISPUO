import { RICPLRArea } from '@domain/ri-cplr-area/ri-cplr-area.entity';
import {
 Column, Entity, OneToMany, PrimaryColumn,
} from 'typeorm';

@Entity({ schema: 'reginst_nom', name: 'CPLRAreaType' })
export class CPLRAreaType {
    @PrimaryColumn()
    CPLRAreaTypeID: number;

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
        nullable: true,
    })
    IsValid: boolean;

    /**
     * Relations
     */

    @OneToMany(
        () => RICPLRArea,
        (RICPLRArea) => RICPLRArea.CPLRAreaType,
    )
    RICPLRAreas?: RICPLRArea[];

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
