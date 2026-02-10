import { SIEMLogEventType } from 'src/models/siem-logger/siem-log-event-type.enum';
import { UserActionLogMapType } from './user-action-log-map.type';

export type UserActionLogType = UserActionLogMapType[SIEMLogEventType];
