import { HttpException, HttpStatus } from '@nestjs/common';
import { CONSTANTS } from '../constants/constants';

export class PersonIsAttendingException extends HttpException {
    // USECASE: This exception should be used when the query from the database has returned an empty array and you need to stop further execution of the method and notify the client
    constructor() {
        super(CONSTANTS.RESPONSE_PARAM_ERROR_MESSAGE_PERSON_IS_ATTENDING, HttpStatus.INTERNAL_SERVER_ERROR);
    }
}
