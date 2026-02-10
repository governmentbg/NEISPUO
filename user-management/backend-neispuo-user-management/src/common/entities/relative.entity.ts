import { Column, Entity, OneToOne, PrimaryColumn } from 'typeorm';
import { PersonEntity } from './person.entity';

@Entity({ schema: 'family', name: 'Relative' })
export class RelativeEntity {
    @PrimaryColumn({ name: 'relativeID' })
    relativeID: number;

    @Column({ name: 'Notes' })
    notes: string;

    @Column({ name: 'PersonID' })
    personID: number;

    @Column({ name: 'WorkStatusID' })
    workStatusID: number;

    @Column({ name: 'Description' })
    description: string;

    @Column({ name: 'Email' })
    email: string;

    @Column({ name: 'PhoneNumber' })
    phoneNumber: string;

    @Column({ name: 'EducationTypeId' })
    educationTypeId: number;

    @OneToOne((type) => PersonEntity)
    person: PersonEntity;
}
