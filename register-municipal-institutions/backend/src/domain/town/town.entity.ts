import {
 Entity, Column, ManyToOne, PrimaryColumn, JoinColumn, OneToMany,
} from 'typeorm';
import { RIInstitution } from '@domain/ri-institution/ri-institution.entity';
import { RIInstitutionDepartment } from '@domain/ri-institution-department/ri-institution-department.entity';
import { TownTypeEnum } from './town-type.enum';
import { TownCategoryEnum } from './town-category.enum';
import { Municipality } from '../municipality/municipality.entity';
import { LocalArea } from '../local-area/local-area.entity';
import { Institution } from '../institution/institution.entity';
import { Person } from '../person/person.entity';

@Entity({ schema: 'location', name: 'Town' })
export class Town {
    @PrimaryColumn({ name: 'TownID' })
    TownID: number;

    @Column({
        type: 'varchar',
    })
    Name: string;

    @Column({
        type: 'int',
    })
    Code: number;

    @Column({
        type: 'varchar',
    })
    Type: TownTypeEnum;

    @Column({
        type: 'int',
    })
    Category: TownCategoryEnum;

    @Column({
        type: 'decimal',
        scale: 2,
        nullable: true,
    })
    Longtitude: string;

    @Column({
        type: 'decimal',
        scale: 2,
        nullable: true,
    })
    Latitude: number;

    @Column({
        type: 'varchar',
        nullable: true,
    })
    Description: number;

    /**
     * Relations
     */

    @OneToMany(
        (type) => LocalArea,
        (localAreas) => localAreas.Town,
    )
    @JoinColumn({ name: 'LocalAreaID' })
    LocalAreas: LocalArea;

    @ManyToOne(
        (type) => Municipality,
        // eslint-disable-next-line @typescript-eslint/no-shadow
        (Municipality) => Municipality.Towns,
    )
    @JoinColumn({ name: 'MunicipalityID' })
    Municipality: Municipality;

    institutions: Institution[];

    RIInstitutionDepartments?: RIInstitutionDepartment;

    @OneToMany(
        (type) => RIInstitution,
        (RIInstitutions) => RIInstitutions.Town,
    )
    @JoinColumn({ name: 'TRTownID' })
    riInstitutions: RIInstitution[];

    permanentTownPersons: Person;

    currentTownPersons: Person;

    birthPlaceTownPersons: Person;

    /**
     * Trackers
     */
}
