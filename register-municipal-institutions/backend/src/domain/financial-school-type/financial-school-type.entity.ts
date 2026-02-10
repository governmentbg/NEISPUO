import {
 Column, Entity, PrimaryColumn,
} from 'typeorm';
import { Institution } from '../institution/institution.entity';

@Entity({ schema: 'noms', name: 'FinancialSchoolType' })
export class FinancialSchoolType {
    @PrimaryColumn()
    FinancialSchoolTypeID: number;

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
        nullable: true,
    })
    IsValid: boolean;

    /**
     * Relations
     */

    institutions: Institution[];

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
