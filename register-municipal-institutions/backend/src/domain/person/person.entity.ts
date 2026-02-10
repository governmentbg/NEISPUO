import {
 Column, Entity, ManyToOne, PrimaryColumn,
} from 'typeorm';
import { EducationalState } from '../educational-state/educational-state.entity';
import { Town } from '../town/town.entity';
import { PersonalIDType } from '../personal-id-type/personal-id-type.entity';
import { Country } from '../country/country.entity';
import { Gender } from '../gender/gender.entity';
import { SysUser } from '../sys-user/sys-user.entity';

@Entity({ schema: 'core' })
export class Person {
    @PrimaryColumn()
    PersonID: number;

    @Column({
        type: 'varchar',
    })
    FirstName: string;

    @Column({
        type: 'varchar',
    })
    MiddleName: string;

    @Column({
        type: 'varchar',
    })
    LastName: string;

    @Column({
        type: 'varchar',
    })
    PermanentAddress: string;

    @Column({
        type: 'varchar',
    })
    CurrentAddress: string;

    @Column({
        type: 'bigint',
        unique: true,
    })
    PublicEduNumber: number;

    @Column({
        type: 'varchar',
    })
    PersonalID: string;

    @Column({
        type: 'date',
    })
    BirthDate: Date;

    /**
     * Relations
     */

    @ManyToOne(
        (type) => EducationalState,
        (educationalStates) => educationalStates.person,
    )
    educationalStates?: EducationalState[];

    @ManyToOne(
        (type) => Town,
        (permanentTown) => permanentTown.permanentTownPersons,
    )
    permanentTown?: Town[];

    @ManyToOne(
        (type) => Town,
        (currentTown) => currentTown.currentTownPersons,
    )
    currentTown?: Town[];

    @ManyToOne(
        (type) => PersonalIDType,
        (personalIDType) => personalIDType.persons,
    )
    personalIDType?: PersonalIDType[];

    @ManyToOne(
        (type) => Country,
        (nationality) => nationality.nationalityPersons,
    )
    nationality?: Country[];

    @ManyToOne(
        (type) => Town,
        (birthPlaceTown) => birthPlaceTown.birthPlaceTownPersons,
    )
    birthPlaceTown?: Town[];

    @ManyToOne(
        (type) => Country,
        (birthPlaceCountry) => birthPlaceCountry.birthPlacePersons,
    )
    birthPlaceCountry?: Country[];

    @ManyToOne(
        (type) => Gender,
        (gender) => gender.persons,
    )
    gender?: Gender[];

    @ManyToOne(
        (type) => SysUser,
        (sysUser) => sysUser.persons,
    )
    sysUser?: SysUser[];

    /**
     * Trackers
     */
}
