import { AuditEntity } from 'src/common/entities/audit.entity';
import { EventAttributesMap } from 'src/common/types/siem/event-attributes-map.type';
import { SIEMLogEventType } from 'src/models/siem-logger/siem-log-event-type.enum';
import { AuthedRequest } from '../../dto/authed-request.interface';
import { MessageLevel } from 'src/common/constants/enum/siem/logger-level.enum';

export type UserActionLogMapType = {
    [K in SIEMLogEventType]: {
        messageLevel: MessageLevel;
        event: K;
        attributes?: Partial<EventAttributesMap[K]>;
        auditEntity?: Partial<AuditEntity>;
        request?: AuthedRequest;
    };
};
