import { Column, Entity, OneToMany, PrimaryGeneratedColumn } from 'typeorm';
import { SchemaRoleAccess } from '../schema-role-access/schema-role-access.entity';

@Entity({ schema: 'core', name: 'SysRole' })
export class SysRole {
  @PrimaryGeneratedColumn({ name: 'SysRoleID' })
  SysRoleID: number;

  @Column({
    type: 'varchar',
    unique: true,
  })
  Name: string;

  @OneToMany((type) => SchemaRoleAccess, (sra) => sra.AllowedSysRole)
  SchemaRoleAccesses: SchemaRoleAccess[];
}
