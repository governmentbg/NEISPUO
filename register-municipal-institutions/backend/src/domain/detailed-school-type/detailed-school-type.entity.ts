import {
 Column, Entity, JoinColumn, ManyToOne, PrimaryColumn,
} from 'typeorm';
import { Institution } from '../institution/institution.entity';
import { BaseSchoolType } from '../base-school-type/base-school-type.entity';

@Entity({ schema: 'noms', name: 'DetailedSchoolType' })
export class DetailedSchoolType {
    @PrimaryColumn()
    DetailedSchoolTypeID: number;

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

    institutions: Institution;

    @ManyToOne(
        (type) => BaseSchoolType,
        (baseSchoolType) => baseSchoolType.detailedSchoolTypes,
    )
    @JoinColumn({ name: 'BaseSchoolTypeID' })
    BaseSchoolType?: BaseSchoolType;

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
