import { SysUser } from '../sys-user/sys-user.entity';
import { ApiModelProperty } from '@nestjs/swagger';
import {
    Entity,
    Column,
    PrimaryGeneratedColumn,
    OneToMany,
} from 'typeorm';

@Entity({ schema: 'core', name: 'Person' })
export class Person {
    @PrimaryGeneratedColumn({ name: 'PersonID' })
    id: number;

    @ApiModelProperty()
    @Column({
        type: 'varchar'
    })
    firstName: string;

    @ApiModelProperty()
    @Column({
        type: 'varchar'
    })
    lastName: string;

    @ApiModelProperty()
    @Column({
        type: 'varchar'
    })
    middleName: string;

    @ApiModelProperty()
    @OneToMany(
        () => SysUser,
        SysUser => SysUser.person
    )
    sysUser: SysUser[];
}
