import {
 Entity, PrimaryColumn,
} from 'typeorm';
import { Person } from '../person/person.entity';
import { Institution } from '../institution/institution.entity';
import { Position } from '../position/position.entity';

@Entity({ schema: 'core' })
export class EducationalState {
    @PrimaryColumn()
    EducationalStateID: number;

    /**
     * Relations
     */

    person: Person;

    institution: Institution;

    position: Position;

    /**
     * Trackers
     */
}
