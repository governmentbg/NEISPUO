import { AuditModuleEnum } from 'src/common/constants/enum/audit-module.enum';
import { LogLevelEnum } from 'src/common/constants/enum/log-level.enum';
import { Column, Entity, PrimaryGeneratedColumn } from 'typeorm';

@Entity({ schema: 'logs', name: 'Log' })
export class LogEntity {
    @PrimaryGeneratedColumn({ name: 'ID' })
    id?: number;

    @Column()
    auditModuleId?: AuditModuleEnum;

    @Column()
    message?: string;

    @Column()
    messageTemplate?: string;

    @Column()
    level?: LogLevelEnum;

    @Column()
    timeStamp?: Date;

    @Column()
    exception?: string;

    @Column()
    logEvent?: string;
}
