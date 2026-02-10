import {
    Entity,
    Column,
    PrimaryGeneratedColumn,
    CreateDateColumn,
    UpdateDateColumn,
} from 'typeorm';
import { SettingTypeEnum, SettingValue } from './enums/setting-type.enum';

@Entity()
export class Setting<T extends SettingTypeEnum = any> {
    @PrimaryGeneratedColumn('uuid')
    id?: string;

    @Column({ unique: true })
    type: SettingTypeEnum;

    @Column('simple-json')
    value: SettingValue[T];

    @CreateDateColumn()
    createdAt?: Date;

    @UpdateDateColumn()
    updatedAt?: Date;
}
