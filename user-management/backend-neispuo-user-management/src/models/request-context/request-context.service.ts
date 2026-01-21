import { Injectable } from '@nestjs/common';
import { RequestContext } from 'nestjs-request-context';

import { Request } from 'express';
@Injectable()
export class RequestContextService {
    getRequest() {
        const req: Request = RequestContext?.currentContext?.req;
        return req;
    }
}
