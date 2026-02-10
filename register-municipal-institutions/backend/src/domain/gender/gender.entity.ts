import {
 Column, Entity, PrimaryColumn,
} from 'typeorm';
import { Person } from '../person/person.entity';

@Entity({ schema: 'noms' })
export class Gender {
    @PrimaryColumn()
    GenderID: number;

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

    persons: Person;

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
