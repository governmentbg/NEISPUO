import {
    Column,
    Entity,
    JoinColumn,
    ManyToOne,
    OneToMany,
    PrimaryGeneratedColumn,
} from 'typeorm';
import { Municipality } from '@domain/municipality/municipality.entity';
import { SysUser } from '@domain/sys-user/sys-user.entity';
import { RIFlexFieldValue } from '../ri-flex-field-value/ri-flex-field-value.entity';

@Entity({ schema: 'reginst_basic', name: 'RIFlexField' })
export class RIFlexField {
    @PrimaryGeneratedColumn({ name: 'RIFlexFieldID' })
    RIFlexFieldID: number;

    @Column({
        type: 'simple-json',
    })
    Data: object;

    @Column({
        type: 'tinyint',
        nullable: true,
    })
    Mandatory: boolean;

    /**
     * Relations
     */

    @ManyToOne(
        (type) => SysUser,
        (sysUser) => sysUser.RIFlexFields,
    )
    @JoinColumn({ name: 'SysUserID' })
    SysUser?: SysUser[];

    @ManyToOne(
        (type) => Municipality,
        (municipality) => municipality.RIFlexFields,
    )
    @JoinColumn({ name: 'MunicipalityID' })
    Municipality?: Municipality[];

    @Column({ type: 'bigint' })
    MunicipalityID: number;

    @OneToMany(
        (type) => RIFlexFieldValue,
        (riFlexFieldValue) => riFlexFieldValue.RIFlexField,
    )
    RIFlexFieldValues?: RIFlexFieldValue[];
}
