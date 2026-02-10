import { Column, Entity, OneToMany, PrimaryGeneratedColumn } from 'typeorm';
import { Report } from '../report/report.entity';

@Entity({ schema: 'core', name: 'SysUser' })
export class SysUser {
  @PrimaryGeneratedColumn({ name: 'SysUserID' })
  SysUserID: number;

  @Column({
    type: 'varchar',
    unique: true,
  })
  Username: string;

  @OneToMany((type) => Report, (report) => report.CreatedBy)
  Reports: Report[];
}
