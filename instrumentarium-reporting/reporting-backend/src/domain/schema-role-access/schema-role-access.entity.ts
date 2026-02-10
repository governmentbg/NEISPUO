import {
  Column,
  Entity,
  JoinColumn,
  ManyToOne,
  PrimaryGeneratedColumn,
} from 'typeorm';
import { SysRole } from '../sys-role/sys-role.entity';

@Entity({ schema: 'reporting', name: 'SchemaRoleAccess' })
export class SchemaRoleAccess {
  @PrimaryGeneratedColumn({ name: 'SchemaRoleAccessID' })
  SchemaRoleAccessID: number;

  @Column({
    type: 'varchar',
    unique: true,
  })
  SchemaName: string;

  @ManyToOne((type) => SysRole, (sysRole) => sysRole.SchemaRoleAccesses)
  @JoinColumn({ name: 'AllowedSysRole', referencedColumnName: 'SysRoleID' })
  AllowedSysRole?: SysRole;
}
