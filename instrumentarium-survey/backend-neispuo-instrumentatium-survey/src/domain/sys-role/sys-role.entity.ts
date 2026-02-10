import { ApiModelProperty } from '@nestjs/swagger';
import { Column, Entity, OneToMany, PrimaryColumn } from 'typeorm';
import { SysUserSysRole } from '../sysuser-sysrole/sysuser-sysrole.entity';

@Entity({ schema: 'core', name: 'SysRole' })
export class SysRole {
    @PrimaryColumn()
    SysRoleID: number;

    @ApiModelProperty()
    @Column({
        type: 'varchar',
        width: 255
    })
    Name: string;

    @ApiModelProperty()
    @Column({
        type: 'varchar'
    })
    Description: string;

    /**
     * Relations
     */
     @ApiModelProperty()
    @OneToMany(
        type => SysUserSysRole,
        sysUserSysRole => sysUserSysRole.SysRole
    )
    SysUserSysRole: SysUserSysRole[];

    /**
     * Trackers
     */

    // @CreateDateColumn()
    // @Exclude({toPlainOnly: true})
    // createdAt?: Date | string;
    //
    // @UpdateDateColumn()
    // @Exclude({toPlainOnly: true})
    // updatedAt?: Date | string;
}
