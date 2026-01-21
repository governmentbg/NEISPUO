import { HttpException, HttpStatus } from '@nestjs/common';

export class CustomException extends HttpException {
    // USECASE: This exception should be used if you need to throw a custom exception that would be used only once
    constructor(message: string) {
        super(message, HttpStatus.INTERNAL_SERVER_ERROR);
    }
}
