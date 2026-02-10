import {
 Column, Entity, JoinColumn, ManyToOne, PrimaryGeneratedColumn,
} from 'typeorm';
import { AuditModuleEntity } from './audit-module.entity';

@Entity({ schema: 'logs', name: 'Audit' })
export class AuditEntity {
    @PrimaryGeneratedColumn()
    AuditId: number;

    @Column()
    AuditCorrelationId: string;

    @Column()
    SysUserId: number;

    @Column()
    SysRoleId: number;

    @Column()
    Username: string;

    @Column()
    FirstName: string;

    @Column()
    MiddleName: string;

    @Column()
    LastName: string;

    @Column()
    LoginSessionId: string;

    @Column()
    RemoteIpAddress: string;

    @Column()
    UserAgent: string;

    @Column()
    DateUtc: Date;

    @Column()
    SchoolYear: number;

    @Column()
    InstId: number;

    @Column()
    PersonId: number;

    @Column()
    ObjectName: string;

    @Column()
    ObjectId: number;

    @Column()
    Action: string;

    @Column('simple-json')
    Data: {};

    @Column()
    AuditModuleId: number;

    @ManyToOne((type) => AuditModuleEntity)
    @JoinColumn({ name: 'AuditModuleId' })
    AuditModule: AuditModuleEntity;
}
