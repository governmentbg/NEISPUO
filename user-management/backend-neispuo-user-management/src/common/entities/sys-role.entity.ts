import { Column, Entity, OneToMany, OneToOne, PrimaryGeneratedColumn } from 'typeorm';
import { PositionEntity } from './position.entity';
import { SysUserSysRoleEntity } from './sys-user-sys-role.entity';

@Entity({ schema: 'core', name: 'sysRole' })
export class SysRoleEntity {
    @PrimaryGeneratedColumn({ name: 'sysRoleID' })
    sysRoleID: number;

    @Column({ name: 'name' })
    name: string;

    @Column({ name: 'description' })
    description: string;

    @OneToOne((type) => PositionEntity, (positionEntity) => positionEntity.role)
    position: PositionEntity;

    @OneToMany((type) => SysUserSysRoleEntity, (sysUserSysRoles) => sysUserSysRoles.role)
    sysUserSysRoles: SysUserSysRoleEntity[];
}
