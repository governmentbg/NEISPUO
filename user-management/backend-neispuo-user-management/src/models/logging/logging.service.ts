import { HttpStatus, Injectable, Logger, LoggerService } from '@nestjs/common';
import { LogLevelEnum } from 'src/common/constants/enum/log-level.enum';
import { LogEntity } from 'src/common/entities/log.entity';
import { LogDtoFactory } from 'src/common/factories/log-dto.factory';
import { RequestContextService } from 'src/models/request-context/request-context.service';
import { LogService } from '../logs/log/routing/log.service';

@Injectable()
export class LoggingService implements LoggerService {
    constructor(private logService: LogService, private requestContextService: RequestContextService) {}

    async log(message: any | LogEntity, ...optionalParams: any[]) {
        await this.initiateLog(message, LogLevelEnum.INFO, optionalParams);
    }

    async error(message: any, ...optionalParams: any[]) {
        await this.initiateLog(message, LogLevelEnum.ERROR, optionalParams);
    }

    async warn(message: any, ...optionalParams: any[]) {
        await this.initiateLog(message, LogLevelEnum.WARNING, optionalParams);
    }

    async debug?(message: any, ...optionalParams: any[]) {
        await this.initiateLog(message, LogLevelEnum.DEBUG, optionalParams);
    }

    async verbose?(message: any, ...optionalParams: any[]) {
        await this.initiateLog(message, LogLevelEnum.VERBOSE, optionalParams);
    }

    private transformToConsoleString(logDTO: LogEntity) {
        return `[Nest] ${process.pid}   - ${new Date(Date.now()).toLocaleString()}   ${logDTO.message}`;
    }

    private useDefaultLogger(loggerName: string, level: LogLevelEnum, message: string) {
        const levelMap = {
            [LogLevelEnum.ERROR]: 'error',
            [LogLevelEnum.WARNING]: 'warn',
            [LogLevelEnum.INFO]: 'log',
            [LogLevelEnum.DEBUG]: 'debug',
            [LogLevelEnum.VERBOSE]: 'verbose',
        };
        const loggerMethod = levelMap[level];
        if (loggerMethod && Logger[loggerMethod]) {
            if (loggerMethod === 'error') {
                Logger[loggerMethod](loggerName, message);
            } else {
                Logger[loggerMethod](message, loggerName);
            }
        } else {
            Logger.log(message, loggerName);
        }
    }

    private async initiateLog(message: any, level: LogLevelEnum, optionalParams?: any[]) {
        const dto = this.preLogHook(message, level);
        await this.logHook(dto, level, optionalParams);
    }

    private preLogHook(message: any, level: LogLevelEnum) {
        let dto = message;
        //checks if  message is string
        if (typeof message == 'string') dto = LogDtoFactory.createFromString(message);
        //checks if message is error
        if (message instanceof Error)
            dto = LogDtoFactory.createFromException({
                message: message?.message,
                status: HttpStatus.INTERNAL_SERVER_ERROR,
                stack: message?.stack,
                validationErrors: [],
            });
        //if message is neither string nor error then in must be logEntity
        dto.level = level;
        return dto;
    }

    private async logHook(dto: any, level: LogLevelEnum, optionalParams?: any[]) {
        const lowerCaseLevel = level?.toLowerCase();
        let loggerName: string;

        if (optionalParams && Array.isArray(optionalParams)) {
            loggerName = optionalParams.find((param) => param && param !== '');
        }

        if (loggerName) {
            this.useDefaultLogger(loggerName, level, dto.message);
        } else {
            console[lowerCaseLevel](this.transformToConsoleString(dto));
        }

        await this.logService.insertLog(dto);
    }
}
