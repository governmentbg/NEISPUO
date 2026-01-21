import { Inject, Injectable } from '@nestjs/common';
import { MessageLevel } from 'src/common/constants/enum/siem/logger-level.enum';
import { AuthedRequest } from 'src/common/dto/authed-request.interface';
import { UserActionLogType } from 'src/common/types/siem/user-action-log.type';
import { UDPTransport } from 'udp-transport-winston';
import { SIEMLogEventType } from './siem-log-event-type.enum';
import { SIEMLoggerOptions } from './siem-logger-options.interface';
import winston = require('winston');

@Injectable()
export class SIEMLoggerService {
    private logger: winston.Logger;

    private isLoggingEnabled: boolean;

    constructor(@Inject('SIEM_LOGGER_OPTIONS') private readonly options: SIEMLoggerOptions) {
        this.isLoggingEnabled = this.options?.enabled === 'true' || false;
        if (this.isLoggingEnabled) {
            this.logger = this.createLogger();
        }
    }

    send(dto: ReturnType<typeof SIEMLoggerService.prototype.buildSIEMLogObject>) {
        if (this.isLoggingEnabled)
            switch (dto.metadata.level) {
                case MessageLevel.INFO:
                    this.logger.info(dto);
                    break;
                case MessageLevel.WARN:
                    this.logger.warn(dto);
                    break;
                case MessageLevel.ERROR:
                    this.logger.error(dto);
                    break;
            }
    }

    buildSIEMLogObject(dto: UserActionLogType) {
        const timestamp = new Date().toISOString();
        const sourceIp = this.getClientIp(dto?.request);
        const description = this.buildDescription(dto);
        const event = this.buildEvent(dto);
        const user = dto?.request?._authObject;
        const metadata = {
            level: dto?.messageLevel,
            event: event,
        };

        const response = {
            appid: this.options?.appId,
            timestamp,
            source_ip: sourceIp,
            event_type: dto?.event,
            description,
            user: user,
            attributes: dto.attributes,
            audit: dto.auditEntity,
            metadata,
        };
        if (dto.event === SIEMLogEventType.PARENT_CREATED) delete response.user;

        return response;
    }

    private buildEvent(dto: UserActionLogType) {
        let result: string;
        if (dto.event === SIEMLogEventType.PARENT_CREATED) {
            result = `${dto.event}:${dto.attributes.email}`;
        } else {
            const username = dto?.request?._authObject?.selectedRole?.Username;
            result = `${dto.event}:${username ? username : 'System'}`;
        }
        return result;
    }

    private getClientIp(request: AuthedRequest): string {
        const forwarded = request?.headers['x-forwarded-for'];
        const ip = typeof forwarded === 'string' ? forwarded.split(',')[0]?.trim() : request?.socket?.remoteAddress;
        return ip || null;
    }

    private createLogger() {
        return winston.createLogger({
            format: this.customFormat(),
            transports: [
                new UDPTransport({
                    host: this.options?.host,
                    port: this.options?.port,
                }),
            ],
        });
    }

    private customFormat() {
        return winston.format.printf(({ message }) => {
            if (typeof message === 'object') {
                return JSON.stringify(message);
            }
            return message;
        });
    }

    private buildDescription(dto: UserActionLogType) {
        let description: string;
        switch (dto?.event) {
            case SIEMLogEventType.PRIVILEGE_PERMISSIONS_CHANGED:
                description = dto?.request
                    ? `${dto?.auditEntity?.Username}(SysUserID: ${dto?.auditEntity?.SysUserId}, PersonID: ${dto?.auditEntity?.PersonId}) performed a/an ${dto?.auditEntity?.Action} action for ${dto?.auditEntity?.Data?.['AssignedToSysUsername']}'s role.`
                    : `System performed a/an ${dto?.auditEntity?.Action} action for ${dto?.auditEntity?.Data?.['AssignedToSysUsername']}'s role`;
                break;
            case SIEMLogEventType.PARENT_CREATED:
                description = `${dto?.attributes?.email} registered as a parent.`;
                break;
            case SIEMLogEventType.USER_CREATED:
                description = dto?.request
                    ? `${dto?.request?._authObject?.selectedRole?.Username}(SysUserID: ${dto.request?._authObject?.selectedRole?.SysUserID}, PersonID: ${dto.request?._authObject?.selectedRole?.PersonID}) issued a create action for ${dto?.attributes?.firstName}'s account.`
                    : `System issued a create action for ${dto?.attributes?.firstName}'s account.`;
                break;
            case SIEMLogEventType.USER_UPDATED:
                description = dto?.request
                    ? `${dto?.request?._authObject?.selectedRole?.Username}(SysUserID: ${dto.request?._authObject?.selectedRole?.SysUserID}, PersonID: ${dto.request?._authObject?.selectedRole?.PersonID}) issued an update action for ${dto?.attributes?.firstName}'s account.`
                    : `System issued an update action for ${dto?.attributes?.firstName}'s account.`;
                break;
            case SIEMLogEventType.USER_DELETED:
                description = dto?.request
                    ? `${dto?.request?._authObject?.selectedRole?.Username}(SysUserID: ${dto.request?._authObject?.selectedRole?.SysUserID}, PersonID: ${dto.request?._authObject?.selectedRole?.PersonID}) issued a delete action for ${dto?.attributes?.firstName}'s account.`
                    : `System issued a delete action for ${dto?.attributes?.firstName}'s account.`;
                break;
            case SIEMLogEventType.STUDENT_DISABLE:
                description = dto?.request
                    ? `${dto?.request?._authObject?.selectedRole?.Username}(SysUserID: ${dto.request?._authObject?.selectedRole?.SysUserID}, PersonID: ${dto.request?._authObject?.selectedRole?.PersonID}) issued a disable action for ${dto?.attributes?.firstName}'s account.`
                    : `System issued a disable action for ${dto?.attributes?.firstName}'s account.`;
                break;
            case SIEMLogEventType.SCHOOL_BOOK_ACCESS_CHANGE:
                description = dto?.request
                    ? `${dto?.request?._authObject?.selectedRole?.Username}(SysUserID: ${dto.request?._authObject?.selectedRole?.SysUserID}, PersonID: ${dto.request?._authObject?.selectedRole?.PersonID}) performed a/an ${dto?.auditEntity?.Action} action to update school book access for the user with Personal ID '${dto?.attributes.personID}'.`
                    : `System performed a/an ${dto?.auditEntity?.Action} action to update school book access for the user with Personal ID '${dto?.attributes.personID}'.`;
                break;
        }
        return description;
    }
}
