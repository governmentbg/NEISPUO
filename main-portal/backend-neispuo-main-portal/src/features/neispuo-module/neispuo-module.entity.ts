import {
  Entity,
  Column,
  ManyToMany,
  JoinTable,
  PrimaryGeneratedColumn,
  ManyToOne,
} from 'typeorm';
import { NeispuoCategory } from '../neispuo-category/neispuo-category.entity';
import { SysRole } from '../../entities/sys-role.entity';

@Entity({ schema: 'portal', name: 'Module' })
export class NeispuoModule {
  @PrimaryGeneratedColumn({ name: 'ModuleID' })
  id: number;

  @Column({ name: 'Name' })
  name: string;

  @Column({ name: 'Description' })
  description: string;

  @Column({ name: 'Link' })
  link: string;

  @ManyToOne(
    neispuoCategory => NeispuoCategory,
    nc => nc.neispuoModules,
  )
  category: NeispuoCategory;

  @ManyToMany(
    () => SysRole,
    role => role.modules,
  )
  @JoinTable({
    name: 'ModuleSysRole',
    joinColumn: { name: 'ModuleID' },
    inverseJoinColumn: { name: 'SysRoleID' },
  })
  roles: SysRole[];
}
