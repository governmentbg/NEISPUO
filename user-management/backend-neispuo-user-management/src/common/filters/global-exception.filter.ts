import { ArgumentsHost, Catch, ExceptionFilter, HttpStatus, Logger } from '@nestjs/common';
import { Request, Response } from 'express';
import { CONSTANTS } from '../constants/constants';
import { UserManagementErrorResponse } from '../dto/responses/user-management-error.response';
import { LogDtoFactory } from '../factories/log-dto.factory';

@Catch()
export class GlobalExceptionFilter implements ExceptionFilter {
    private logger = new Logger(GlobalExceptionFilter.name);

    catch(exception: any, host: ArgumentsHost) {
        //  stackErrorParse(exception.stack);
        const ctx = host.switchToHttp();
        const response = ctx.getResponse<Response>();
        const request = ctx.getRequest<Request>();
        const validationErrors = exception?.response?.message ? Object.values(exception?.response?.message) : null;
        const status = exception.status ? exception.status : HttpStatus.INTERNAL_SERVER_ERROR;
        const message = exception.message ? exception.message : CONSTANTS.RESPONSE_PARAM_ERROR_MESSAGE_DEFAULT;
        const stack = exception.stack ? exception.stack : CONSTANTS.RESPONSE_PARAM_ERROR_MESSAGE_DEFAULT;
        // eslint-disable-next-line prefer-const
        let clientResponseObject: any = {
            status: status,
            message: message,
        };

        if (exception?.status === HttpStatus.BAD_REQUEST) {
            clientResponseObject.validationErrors = validationErrors;
        }
        const clientResponse = new UserManagementErrorResponse(clientResponseObject);
        const databaseLogResponse: UserManagementErrorResponse = { ...clientResponse, stack: stack };
        const logDTO = LogDtoFactory.createFromException(databaseLogResponse);
        this.logger.error(logDTO);
        response.status(status).json(clientResponse);
    }
}
