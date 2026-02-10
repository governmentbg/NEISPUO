import { HttpStatus } from '@nestjs/common';
import { ApiModelProperty } from '@nestjs/swagger';
import { DTO } from './dto.interface';

export class UserManagementResponse implements DTO {
    @ApiModelProperty()
    status: HttpStatus;

    @ApiModelProperty()
    message: string;

    @ApiModelProperty()
    payload: any;

    constructor(payload: any = {}) {
        this.status = HttpStatus.CREATED;
        this.message = '';
        this.payload = payload;
    }

    setStatus(status: HttpStatus) {
        this.status = status;
    }

    setMessage(message: string) {
        this.message = message;
    }

    setPayload(payload: any) {
        this.payload = payload;
    }
}
