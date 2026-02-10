import { Entity, Column, PrimaryColumn } from 'typeorm';

@Entity({ schema: 'core', name: 'SysRole' })
export class SysRole {
    @PrimaryColumn({ name: 'SysRoleID' })
    id: number;

    @Column({ name: 'Name' })
    name: string;

    modules: [];

}