import { Logger } from '@nestjs/common';
import { LogDtoFactory } from '../factories/log-dto.factory';

// we did this in order to have optional named params in the decorator
interface EnableDatabaseLoggingParams {
    excludedMethods?: string[];
    includedMethods?: string[];
}
//this decorator is placed on top of a class and all methods except the excluded ones are being loged in the DB
export function EnableDatabaseLogging(params?: EnableDatabaseLoggingParams) {
    return (target: Function) => {
        for (const propertyName of Object.getOwnPropertyNames(target.prototype)) {
            const descriptor = Object.getOwnPropertyDescriptor(target.prototype, propertyName);

            if (!descriptor) {
                continue;
            }

            const originalMethod = descriptor.value;

            const isMethod = originalMethod instanceof Function;

            if (!isMethod) {
                continue;
            }

            descriptor.value = function (...args: any[]) {
                //this will log the class method args and authObject if any to the logger
                const log = new Logger(target.name);
                // request object always has to be number 1 in the parameter list of a method
                //if the method name matched some of the excluded ones then no log is logged
                if (
                    (params?.excludedMethods && !params?.excludedMethods?.includes(propertyName)) ||
                    (params?.includedMethods && params?.includedMethods?.includes(propertyName)) ||
                    (!params?.excludedMethods && !params?.includedMethods)
                ) {
                    const logDTO = LogDtoFactory.createFromMethodArguments(target.name, propertyName, args);
                    log.log(logDTO);
                }
                const result = originalMethod.apply(this, args);
                return result;
            };

            Object.defineProperty(target.prototype, propertyName, descriptor);
        }
    };
}
