import {
  Entity,
  Column,
  PrimaryGeneratedColumn,
  ManyToOne,
  JoinColumn,
} from 'typeorm';
import { SysUser } from '../sys-user/sys-user.entity';

@Entity({ schema: 'reporting', name: 'Report' })
export class Report {
  @PrimaryGeneratedColumn({ name: 'ReportID' })
  ReportID: number;

  @Column({
    type: 'nvarchar',
    length: 512,
  })
  Name: string;

  @Column({
    type: 'nvarchar',
    length: 255,
  })
  DatabaseView: string;

  @Column({
    type: 'nvarchar',
    length: 5012,
  })
  Description?: string;

  @Column({ type: 'simple-json' })
  SharedWith?: any;

  @Column({ default: null })
  RegionID?: number;

  @Column({ default: null })
  MunicipalityID?: number;

  @Column({ type: 'simple-json' })
  Query: any;

  @Column({
    type: 'nvarchar',
    length: 255,
  })
  Visualization: 'pie_chart' | 'pivot_table' | 'table';

  @ManyToOne((type) => SysUser, (sysUser) => sysUser.Reports)
  @JoinColumn({ name: 'CreatedBy', referencedColumnName: 'SysUserID' })
  CreatedBy?: SysUser;
}
