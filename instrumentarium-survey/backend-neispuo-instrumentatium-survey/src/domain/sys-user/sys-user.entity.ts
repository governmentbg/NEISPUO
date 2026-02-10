import {
    Column,
    Entity,
    JoinColumn,
    ManyToOne,
    OneToMany,
    PrimaryGeneratedColumn
} from 'typeorm';
import { Exclude } from 'class-transformer';
import { SysUserSysRole } from '../sysuser-sysrole/sysuser-sysrole.entity';
import { Campaign } from '../campaigns/campaign.entity';
import { ApiModelProperty } from '@nestjs/swagger';
import { Person } from '../person/person.entity';

@Entity({ schema: 'core', name: 'SysUser' })
export class SysUser {
    @PrimaryGeneratedColumn({ name: 'SysUserID' })
    SysUserID: number;

    @ApiModelProperty()
    @Column({
        type: 'varchar',
        unique: true
    })
    Username: string;

    @ApiModelProperty()
    @Column({
        type: 'varchar',
        nullable: true
    })
    @Exclude() // deletes password field at middleware level if it is present on either incoming and outgoing DTOs
    Password: string;

    /**
     * Relations
     */
    @ApiModelProperty()
    @OneToMany(
        type => SysUserSysRole,
        sysUserSysRole => sysUserSysRole.SysUser
    )
    SysUserSysRole: SysUserSysRole[];

    @ApiModelProperty()
    @OneToMany(
        () => Campaign,
        Campaign => Campaign.createdBy
    )
    campaignCreatedBy: Campaign[];

    @ApiModelProperty()
    @ManyToOne(
        type => Person,
        Person => Person.sysUser
    )
    @JoinColumn({ name: 'PersonID', referencedColumnName: 'id' })
    person: Person;
}