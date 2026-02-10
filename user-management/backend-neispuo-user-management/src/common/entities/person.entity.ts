import { Column, Entity, OneToMany, OneToOne, PrimaryGeneratedColumn } from 'typeorm';
import { EducationalStateEntity } from './educational-state.entity';
import { PersonnelSchoolBookAccess } from './personnel-school-book-access.entity';
import { RelativeChildEntity } from './relative-child.entity';
import { SysUserEntity } from './sys-user.entity';

@Entity({ schema: 'core', name: 'Person' })
export class PersonEntity {
    @PrimaryGeneratedColumn({ name: 'personID' })
    personID: number;

    @Column({ name: 'firstName' })
    firstName: string;

    @Column({ name: 'middleName' })
    middleName: string;

    @Column({ name: 'lastName' })
    lastName: string;

    @Column({ name: 'permanentAddress' })
    permanentAddress: string;

    @Column({ name: 'permanentTownID' })
    permanentTownID: number;

    @Column({ name: 'currentAddress' })
    currentAddress: string;

    @Column({ name: 'publicEduNumber' })
    publicEduNumber: string;

    @Column({ name: 'personalIDType' })
    personalIDType: number;

    @Column({ name: 'nationalityID' })
    nationalityID: number;

    @Column({ name: 'personalID' })
    personalID: string;

    @Column({ name: 'azureID' })
    azureID: string;

    @Column({ name: 'birthDate' })
    birthDate: Date;

    @Column({ name: 'birthPlaceTownID' })
    birthPlaceTownID: number;

    @Column({ name: 'birthPlaceCountry' })
    birthPlaceCountry: number;

    @Column({ name: 'gender' })
    gender: number;

    @OneToMany((type) => EducationalStateEntity, (educationalStates) => educationalStates.person)
    educationalStates: EducationalStateEntity[];

    @OneToMany((type) => PersonnelSchoolBookAccess, (personnelSchoolBookAccess) => personnelSchoolBookAccess.person)
    personnelSchoolBookAccess: PersonnelSchoolBookAccess[];

    @OneToOne((type) => SysUserEntity, (sysUserEntity) => sysUserEntity.person)
    user: SysUserEntity;

    @OneToOne((type) => RelativeChildEntity, (RelativeChildEntity) => RelativeChildEntity.person)
    relativeChild: RelativeChildEntity;
}
