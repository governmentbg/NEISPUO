import { AuditModuleEnum } from '../constants/enum/audit-module.enum';
import { LogEntity } from '../entities/log.entity';
import { UserManagementErrorResponse } from '../dto/responses/user-management-error.response';
import { RequestContext } from 'nestjs-request-context';
import { EventDto } from '../dto/telelink/event-dto';

export class LogDtoFactory {
    static createFromMethodArguments(className: string, methodName: string, args: any[]) {
        const requestObject: any = RequestContext?.currentContext?.req;
        const userName = requestObject?._authObject?.selectedRole?.Username ?? null;
        const requestID = requestObject?.requestID ?? null;
        // eslint-disable-next-line prefer-const
        let logDTO = new LogEntity();
        logDTO.auditModuleId = AuditModuleEnum.USER_MANAGEMENT;
        logDTO.exception = '';
        logDTO.logEvent = JSON.stringify(requestObject?._authObject)?.substring(0, 500);
        logDTO.message = `[${requestID}][${userName}][${className}][${methodName}]${JSON.stringify(
            args,
        )}[${null}]`.substring(0, 500);
        logDTO.messageTemplate = `[:requestID][:username][:className][:methodName][:params][:message]`.substring(
            0,
            500,
        );
        return logDTO;
    }

    static createFromString(message: string) {
        // eslint-disable-next-line prefer-const
        let logDTO = new LogEntity();
        const requestObject: any = RequestContext?.currentContext?.req;
        const requestID = requestObject?.requestID ?? null;
        const username = requestObject?._authObject?.selectedRole?.Username ?? null;
        logDTO.auditModuleId = AuditModuleEnum.USER_MANAGEMENT;
        logDTO.exception = '';
        logDTO.logEvent = '';
        logDTO.message = `[${requestID}][${username}][${null}][${null}][${null}][${message}]`.substring(0, 500);
        logDTO.messageTemplate = `[:requestID][:username][:className][:methodName][:params][:message]`.substring(
            0,
            500,
        );
        return logDTO;
    }

    static createFromException(exception: UserManagementErrorResponse) {
        // eslint-disable-next-line prefer-const
        const requestObject: any = RequestContext?.currentContext?.req;
        const requestID = requestObject?.requestID ?? null;
        const stack = exception?.stack || '';
        const message = exception?.message || '';
        const username = requestObject?._authObject?.selectedRole?.Username ?? null;
        const logDTO = new LogEntity();
        logDTO.auditModuleId = AuditModuleEnum.USER_MANAGEMENT;
        logDTO.exception = stack?.substring(0, 500);
        logDTO.logEvent = ''.substring(0, 500);
        logDTO.message = `[${requestID}][${username}][${null}][${null}][${null}][${stack}]`.substring(0, 500);
        logDTO.messageTemplate = `[:requestID][:username][:className][:methodName][:params][:message]`.substring(
            0,
            500,
        );
        return logDTO;
    }

    static createFromAzureEventObject(eventDto: EventDto) {
        // eslint-disable-next-line prefer-const
        const requestObject: any = RequestContext?.currentContext?.req;
        const requestID = requestObject?.requestID ?? null;
        const stack = '';
        const logEvent = JSON?.stringify(eventDto)?.substring(0, 500);
        const username = requestObject?._authObject?.selectedRole?.Username ?? null;
        const logDTO = new LogEntity();
        logDTO.auditModuleId = AuditModuleEnum.USER_MANAGEMENT;
        logDTO.exception = stack?.substring(0, 500);
        logDTO.logEvent = logEvent;
        logDTO.message = `[${requestID}][${username}][${null}][${null}][${null}][${logEvent}]`.substring(0, 500);
        logDTO.messageTemplate = `[:requestID][:username][:className][:methodName][:params][:message]`.substring(
            0,
            500,
        );
        return logDTO;
    }
}
