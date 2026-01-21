import { HttpException, HttpStatus } from '@nestjs/common';
import { CONSTANTS } from '../constants/constants';

export class ParentAlreadyExistsException extends HttpException {
    // USECASE: This exception should be used you could not insert a row in the azure tables in NEIPSUO
    constructor() {
        super(CONSTANTS.RESPONSE_PARAM_ERROR_MESSAGE_USER_ALREADY_EXISTS, HttpStatus.CONFLICT);
    }
}
