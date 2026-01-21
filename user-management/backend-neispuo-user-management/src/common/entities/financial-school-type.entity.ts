import { Column, Entity, OneToMany, PrimaryGeneratedColumn } from 'typeorm';
import { InstitutionEntity } from './institution.entity';

@Entity({ schema: 'noms', name: 'FinancialSchoolType' })
export class FinancialSchoolTypeEntity {
    @PrimaryGeneratedColumn({ name: 'financialSchoolTypeID' })
    financialSchoolTypeID: number;

    @Column({ name: 'name' })
    name: string;

    @Column({ name: 'description' })
    description: string;

    @Column({ name: 'isValid' })
    isValid: number;

    @OneToMany(() => InstitutionEntity, (institutionEntity) => institutionEntity.financialSchoolType)
    institution: InstitutionEntity[];
}
