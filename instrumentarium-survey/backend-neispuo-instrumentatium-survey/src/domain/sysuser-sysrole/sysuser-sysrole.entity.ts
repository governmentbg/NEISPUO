import { ApiModelProperty } from '@nestjs/swagger';
import { Entity, JoinColumn, ManyToOne, PrimaryColumn } from 'typeorm';
import { SysRole } from '../sys-role/sys-role.entity';
import { SysUser } from '../sys-user/sys-user.entity';

@Entity({ schema: 'core', name: 'SysUserSysRole' })
export class SysUserSysRole {
    @PrimaryColumn()
    SysUserID: number; // This is only for typeorm not to complain, table has no primary key

    /**
     * Relations
     */
    @ApiModelProperty()
    @ManyToOne(
        type => SysUser,
        sysUser => sysUser.SysUserSysRole
    )
    @JoinColumn({ name: 'SysUserID' })
    SysUser: SysUser;

    @ApiModelProperty()
    @ManyToOne(
        type => SysRole,
        sysUser => sysUser.SysUserSysRole
    )
    @JoinColumn({ name: 'SysRoleID' })
    SysRole: SysRole;
}
