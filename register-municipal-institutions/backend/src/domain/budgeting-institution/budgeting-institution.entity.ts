import {
 Column, Entity, PrimaryColumn,
} from 'typeorm';
import { Institution } from '../institution/institution.entity';

@Entity({ schema: 'noms', name: 'BudgetingInstitution' })
export class BudgetingInstitution {
    @PrimaryColumn()
    BudgetingInstitutionID: number;

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

    institutions: Institution;

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
