import { HttpException, HttpStatus } from '@nestjs/common';
import { CONSTANTS } from '../constants/constants';

export class SchoolBookAccessNotFoundException extends HttpException {
    // USECASE: This exception should be used when row the row does not exist in the database and notify the client
    constructor() {
        super(CONSTANTS.RESPONSE_PARAM_ERROR_MESSAGE_SCHOOL_BOOK_ACCESS_NOT_FOUND, HttpStatus.INTERNAL_SERVER_ERROR);
    }
}
