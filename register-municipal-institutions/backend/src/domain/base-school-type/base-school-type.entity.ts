import {
 Column, Entity, PrimaryColumn,
} from 'typeorm';
import { RIInstitution } from '@domain/ri-institution/ri-institution.entity';
import { Institution } from '../institution/institution.entity';
import { DetailedSchoolType } from '../detailed-school-type/detailed-school-type.entity';
import { CPLRAreaType } from '../cplr-area-type/cplr-area-type.entity';

@Entity({ schema: 'noms', name: 'BaseSchoolType' })
export class BaseSchoolType {
    @PrimaryColumn()
    BaseSchoolTypeID: number;

    @Column({
        type: 'varchar',
    })
    Name: string;

    @Column({
        type: 'varchar',
        nullable: true,
    })
    Description: string;

    @Column({
        type: 'tinyint',
    })
    IsValid: boolean;

    /**
     * Relations
     */

    institutions: Institution[];

    riInstitutions: RIInstitution;

    detailedSchoolTypes: DetailedSchoolType;

    cplrAreaTypes: CPLRAreaType[];

    /**
     * Trackers
     */

    @Column({
        type: 'datetime2',
    })
    ValidFrom: Date;

    @Column({
        type: 'datetime2',
    })
    ValidTo: Date;
}
