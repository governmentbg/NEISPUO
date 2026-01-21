import { Column, Entity, JoinColumn, OneToMany, OneToOne, PrimaryGeneratedColumn } from 'typeorm';
import { InstitutionEntity } from './institution.entity';
import { MunicipalityEntity } from './municipality.entity';

@Entity({ schema: 'location', name: 'Town' })
export class TownEntity {
    @PrimaryGeneratedColumn({ name: 'townID' })
    townID: number;

    @Column({ name: 'name' })
    name: string;

    @Column({ name: 'code' })
    code: number;

    @Column({ name: 'type' })
    type: string;

    @Column({ name: 'category' })
    category: number;

    @Column({ name: 'longtitude' })
    longtitude: number;

    @Column({ name: 'latitude' })
    latitude: number;

    @Column({ name: 'municipalityID' })
    municipalityID: number;

    @Column({ name: 'description' })
    description: string;

    @OneToMany(() => InstitutionEntity, (institution) => institution.town)
    institution: InstitutionEntity[];

    @OneToOne(() => MunicipalityEntity, (municipality) => municipality.municipalityID)
    @JoinColumn({ name: 'municipalityID', referencedColumnName: 'municipalityID' })
    municipality: MunicipalityEntity;
}
