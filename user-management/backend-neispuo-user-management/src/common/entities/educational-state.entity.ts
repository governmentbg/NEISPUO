import { Column, Entity, JoinColumn, ManyToOne, PrimaryGeneratedColumn } from 'typeorm';
import { InstitutionEntity } from './institution.entity';
import { PersonEntity } from './person.entity';
import { PositionEntity } from './position.entity';

@Entity({ schema: 'core', name: 'EducationalState' })
export class EducationalStateEntity {
    @PrimaryGeneratedColumn({ name: 'educationalStateID' })
    educationalStateID: number;

    @Column({ name: 'personID' })
    personID: number;

    @Column({ name: 'institutionID' })
    institutionID: number;

    @Column({ name: 'positionID' })
    positionID: number;

    @ManyToOne((type) => PersonEntity, (personEntity) => personEntity.educationalStates)
    @JoinColumn({ name: 'personID', referencedColumnName: 'personID' })
    person: PersonEntity;

    @ManyToOne((type) => InstitutionEntity, (institution) => institution.educationalStates)
    @JoinColumn({ name: 'institutionID', referencedColumnName: 'institutionID' })
    institution: InstitutionEntity;

    @ManyToOne((type) => PositionEntity, (institution) => institution.educationalStates)
    @JoinColumn({ name: 'positionID', referencedColumnName: 'positionID' })
    position: PositionEntity;
}
