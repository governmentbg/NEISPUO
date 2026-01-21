import { LogEntity } from '../entities/log.entity';

export class LogMapper {
    static transform(logObjects: any[]) {
        const result: LogEntity[] = [];
        for (const logObject of logObjects) {
            const elementToBeInserted: LogEntity = {
                id: logObject.id,
                auditModuleId: logObject.auditModuleId,
                message: logObject.message,
                messageTemplate: logObject.messageTemplate,
                level: logObject.level,
                timeStamp: logObject.timeStamp,
                exception: logObject.exception,
                logEvent: logObject.logEvent,
            };
            result.push(elementToBeInserted);
        }
        return result;
    }
}
