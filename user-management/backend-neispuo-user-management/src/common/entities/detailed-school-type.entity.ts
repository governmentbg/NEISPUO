import { Column, Entity, JoinColumn, OneToMany, OneToOne, PrimaryGeneratedColumn } from 'typeorm';
import { BaseSchoolTypeEntity } from './base-school-type.entity';
import { InstitutionEntity } from './institution.entity';

@Entity({ schema: 'noms', name: 'DetailedSchoolType' })
export class DetailedSchoolTypeEntity {
    @PrimaryGeneratedColumn({ name: 'detailedSchoolTypeID' })
    detailedSchoolTypeID: number;

    @Column({ name: 'baseSchoolTypeID' })
    baseSchoolTypeID: number;

    @Column({ name: 'name' })
    name: string;

    @Column({ name: 'description' })
    description: string;

    @Column({ name: 'isValid' })
    isValid: number;

    @OneToMany(() => InstitutionEntity, (institutionEntity) => institutionEntity.detailedSchoolType)
    institution: InstitutionEntity[];

    @OneToOne(() => BaseSchoolTypeEntity, (baseSchool) => baseSchool.baseSchoolTypeID)
    @JoinColumn({ name: 'baseSchoolTypeID', referencedColumnName: 'baseSchoolTypeID' })
    baseSchool: BaseSchoolTypeEntity;
}
