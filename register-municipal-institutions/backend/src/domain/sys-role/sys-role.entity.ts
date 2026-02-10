import {
 Column, Entity, OneToMany, PrimaryColumn,
} from 'typeorm';
import { SysUserSysRole } from '../sysuser-sysrole/sysuser-sysrole.entity';

@Entity({ schema: 'core', name: 'SysRole' })
export class SysRole {
    @PrimaryColumn()
    SysRoleID: number;

    @Column({
        type: 'varchar',
        width: 255,
    })
    Name: string;

    @Column({
        type: 'varchar',
    })
    Description: string;

    /**
     * Relations
     */
    @OneToMany(
        (type) => SysUserSysRole,
        (sysUserSysRole) => sysUserSysRole.SysRole,
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
