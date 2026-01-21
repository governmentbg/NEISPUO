import {
 Entity, Column, PrimaryColumn,
} from 'typeorm';
import { RIInstitutionDepartment } from '@domain/ri-institution-department/ri-institution-department.entity';
import { Town } from '../town/town.entity';
import { Institution } from '../institution/institution.entity';

@Entity({ schema: 'location', name: 'LocalArea' })
export class LocalArea {
    @PrimaryColumn()
    LocalAreaID: number;

    @Column({
        type: 'varchar',
    })
    Name: string;

    // @Column({
    //     type: 'varchar',
    //     nullable: true
    // })
    // Description: string;

    @Column()
    TownCode: number;

    // @Column()
    // Longtitude: string;

    // @Column()
    // Latitude: string;

    /**
     * Relations
     */

    // @ManyToOne(
    //     type => Town,
    //     town => town.localAreas
    // )
    // @JoinColumn({ name: 'TownID' })
    Town: Town;

    institutions: Institution;

    RIInstitutionDepartments?: RIInstitutionDepartment;

    /**
     * Trackers
     */
}
