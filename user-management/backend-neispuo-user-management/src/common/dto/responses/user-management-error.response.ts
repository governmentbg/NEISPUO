import { HttpStatus } from '@nestjs/common';
import { ApiModelProperty } from '@nestjs/swagger';
import { DTO } from './dto.interface';

interface UserManagementErrorResponseParams {
    stack?: any;
    status?: HttpStatus;
    message?: string;
    validationErrors?: any;
}

export class UserManagementErrorResponse implements DTO {
    @ApiModelProperty()
    status: HttpStatus;

    @ApiModelProperty()
    message: string;

    @ApiModelProperty()
    validationErrors: string[];

    stack: any;

    constructor(params: UserManagementErrorResponseParams) {
        this.status = params.status;
        this.message = params.message;
        this.validationErrors = params.validationErrors;
        this.stack = params.stack;
    }
}
