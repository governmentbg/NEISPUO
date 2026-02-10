import { Column, Entity, OneToMany, OneToOne, PrimaryGeneratedColumn } from 'typeorm';
import { DetailedSchoolTypeEntity } from './detailed-school-type.entity';
import { InstitutionEntity } from './institution.entity';

@Entity({ schema: 'noms', name: 'BaseSchoolType' })
export class BaseSchoolTypeEntity {
    @PrimaryGeneratedColumn({ name: 'baseSchoolTypeID' })
    baseSchoolTypeID: number;

    @Column({ name: 'name' })
    name: string;

    @Column({ name: 'description' })
    description: string;

    @Column({ name: 'isValid' })
    isValid: number;

    @OneToMany(() => InstitutionEntity, (institutionEntity) => institutionEntity.baseSchoolType)
    institution: InstitutionEntity[];

    @OneToOne((type) => DetailedSchoolTypeEntity, (detailedSchool) => detailedSchool.baseSchool)
    detailedSchool: DetailedSchoolTypeEntity;
}
