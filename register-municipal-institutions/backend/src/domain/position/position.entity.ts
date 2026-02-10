import {
 Column, Entity, PrimaryColumn,
} from 'typeorm';
import { EducationalState } from '../educational-state/educational-state.entity';

@Entity({ schema: 'core' })
export class Position {
    @PrimaryColumn()
    PositionID: number;

    @Column({
        type: 'varchar',
    })
    Name: string;

    @Column({
        type: 'varchar',
    })
    Description: string;

    /**
     * Relations
     */

    educationalStates?: EducationalState[];

    /**
     * Trackers
     */

    @Column({
        type: 'datetime2',
    })
    ValidFrom: Date;

    @Column({
        type: 'datetime2',
    })
    ValidTo: Date;
}
