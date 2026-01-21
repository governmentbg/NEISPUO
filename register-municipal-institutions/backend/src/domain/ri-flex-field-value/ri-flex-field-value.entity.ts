import {
    Column,
    Entity,
    JoinColumn,
    ManyToOne,
    PrimaryGeneratedColumn,
} from 'typeorm';
import { RIInstitution } from '@domain/ri-institution/ri-institution.entity';
import { RIFlexField } from '../ri-flex-field/ri-flex-field.entity';

@Entity({ schema: 'reginst_basic', name: 'RIFlexFieldValue' })
export class RIFlexFieldValue {
    @PrimaryGeneratedColumn({ name: 'RIFlexFieldValueID' })
    RIFlexFieldValueID: number;

    @Column({
        type: 'simple-json',
    })
    Value: object;

    /**
     * Relations
     */

    @ManyToOne(
        (type) => RIInstitution,
        (riInstitution) => riInstitution.RIFlexFieldValues,
        { orphanedRowAction: 'delete' },
    )
    @JoinColumn({ name: 'RIInstitutionID' })
    RIInstitution: RIInstitution;

    @ManyToOne(
        (type) => RIFlexField,
        (riFlexField) => riFlexField.RIFlexFieldValues,
    )
    @JoinColumn({ name: 'RIFlexFieldID' })
    RIFlexField: RIFlexField;
}
