import { RIInstitutionDepartment } from '@domain/ri-institution-department/ri-institution-department.entity';
import {
 Entity, Column, PrimaryColumn,
} from 'typeorm';
import { Institution } from '../institution/institution.entity';
import { Person } from '../person/person.entity';

@Entity({ schema: 'location' })
export class Country {
    @PrimaryColumn()
    CountryID: number;

    @Column({
        type: 'varchar',
    })
    Name: string;

    @Column({
        type: 'varchar',
    })
    Code: string;

    /**
     * Relations
     */

    institutions: Institution;

    RIInstitutionDepartments?: RIInstitutionDepartment;

    nationalityPersons: Person;

    birthPlacePersons: Person;

    /**
     * Trackers
     */
}
