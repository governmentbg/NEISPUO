import { Column, Entity, PrimaryColumn } from 'typeorm';

@Entity({ schema: 'logs', name: 'AuditModule' })
export class AuditModuleEntity {
  @PrimaryColumn()
  AuditModuleId: number;

  @Column()
  Name: string;
}
