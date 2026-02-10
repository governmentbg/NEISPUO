import { Column, Entity, OneToMany, PrimaryGeneratedColumn } from 'typeorm';
import { InstitutionEntity } from './institution.entity';

@Entity({ schema: 'location', name: 'Country' })
export class CountryEntity {
    @PrimaryGeneratedColumn({ name: 'countryID' })
    countryID: number;

    @Column({ name: 'name' })
    name: string;

    @Column({ name: 'code' })
    code: string;

    @OneToMany(() => InstitutionEntity, (institution) => institution.country)
    institution: InstitutionEntity[];
}
