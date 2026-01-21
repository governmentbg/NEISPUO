import { Injectable } from '@nestjs/common';
import * as winston from 'winston';
import DailyRotateFile = require("winston-daily-rotate-file");
import { utilities as nestWinstonModuleUtilities } from 'nest-winston';

@Injectable()
export class LoggerService {

    constructor(){

    }
    userLoggingLogger = winston.createLogger({
        transports: (process.env.LOG_FILE_USERS && process.env.LOG_TO_FILE_ALLOWED)
            ? [
                // log to file
                new DailyRotateFile({
                    filename: `${process.env.LOG_FILE_USERS}`,
                    maxSize: "1m",
                    zippedArchive: true,
                    datePattern: 'YYYY-MM-DD',
                    format: winston.format.combine(
                        winston.format.timestamp({
                            format: 'YYYY-MM-DD HH:mm:ss'
                        }),
                        winston.format.simple(),
                        winston.format.printf(info => {
                            return `[${info.timestamp}][${info.level}][${info.message}]`;
                        })
                    ),
                })
            ]
            : [
                // log to console
                new winston.transports.Console({
                    level: process.env.LOG_LEVEL,
                    format: winston.format.combine(
                        winston.format.timestamp(),
                        nestWinstonModuleUtilities.format.nestLike()
                    ),

                })
            ]
    })
    jobsLogger = winston.createLogger({
        transports: (process.env.LOG_FILE_JOBS && process.env.LOG_TO_FILE_ALLOWED)
            ? [
                // log to file
                new DailyRotateFile({
                    filename: `${process.env.LOG_FILE_JOBS}`,
                    maxSize: "1m",
                    zippedArchive: true,
                    datePattern: 'YYYY-MM-DD',
                    format: winston.format.combine(
                        winston.format.timestamp({
                            format: 'YYYY-MM-DD HH:mm:ss'
                        }),
                        winston.format.simple(),
                        winston.format.printf(info => {
                            return `[${info.timestamp}][${info.level}][${info.message}]`;
                        })
                    ),
                })
            ]
            : [
                // log to console
                new winston.transports.Console({
                    level: process.env.LOG_LEVEL,
                    format: winston.format.combine(
                        winston.format.timestamp(),
                        nestWinstonModuleUtilities.format.nestLike()
                    ),

                })
            ]
    });
    performanceLogger = winston.createLogger({
        transports: (process.env.LOG_FILE_PERFORMANCE && process.env.LOG_TO_FILE_ALLOWED)
            ? [
                // log to file
                new DailyRotateFile({
                    filename: `${process.env.LOG_FILE_PERFORMANCE}`,
                    maxSize: "1m",
                    zippedArchive: true,
                    datePattern: 'YYYY-MM-DD',
                    format: winston.format.combine(
                        winston.format.timestamp({
                            format: 'YYYY-MM-DD HH:mm:ss'
                        }),
                        winston.format.simple(),
                        winston.format.printf(info => {
                            return `[${info.timestamp}][${info.level}][${info.message}]`;
                        })
                    ),
                })
            ]
            : [
                // log to console
                new winston.transports.Console({
                    level: process.env.LOG_LEVEL,
                    format: winston.format.combine(
                        winston.format.timestamp(),
                        nestWinstonModuleUtilities.format.nestLike()
                    ),

                })
            ]
    });
}
