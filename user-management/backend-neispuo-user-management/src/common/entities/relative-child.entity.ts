import { Column, Entity, OneToOne, PrimaryColumn } from 'typeorm';
import { PersonEntity } from './person.entity';

@Entity({ schema: 'core', name: 'RelativeChild' })
export class RelativeChildEntity {
    @Column({ name: 'relativeID' })
    relativeID: number;

    @PrimaryColumn({ name: 'childID' })
    childID: number;

    @Column({ name: 'relativeTypeID' })
    relativeTypeID: number;

    @OneToOne((type) => PersonEntity, (person) => person.relativeChild)
    person: PersonEntity;
}
