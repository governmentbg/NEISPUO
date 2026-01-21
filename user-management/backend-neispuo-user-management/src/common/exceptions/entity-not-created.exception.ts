import { HttpException, HttpStatus } from '@nestjs/common';
import { CONSTANTS } from '../constants/constants';

export class EntityNotCreatedException extends HttpException {
    // USECASE: This exception should be used you could not insert a row in the azure tables in NEIPSUO
    constructor() {
        super(CONSTANTS.RESPONSE_PARAM_ERROR_MESSAGE_JOB_ENTITY_NOT_CREATED, HttpStatus.INTERNAL_SERVER_ERROR);
    }
}
