import { Injectable, Logger, NestMiddleware } from '@nestjs/common';
import { Request } from 'express';
import { RequestContext } from 'nestjs-request-context';

/**
 * Checks if request contains a non-expired JWT.
 * If it does, attaches authenticatedUser to the request object, including its roles.
 */
import { RequestIDGeneratorService } from '../services/request-id-generator/request-id-generator.service';

@Injectable()
export class RequestIDMiddleware implements NestMiddleware {
    private readonly logger = new Logger(RequestIDMiddleware.name);

    private ATTRIBUTE_NAME = 'requestID';

    private generator = RequestIDGeneratorService.getRequestID;

    private headerName = 'X-Request-Id';

    private setHeader = false;

    async use(request: Request, response: any, next: any) {
        const oldValue = request.get(this.headerName);
        const id = oldValue === undefined ? this.generator() : oldValue;

        if (this.setHeader) {
            response.set(this.headerName, id);
        }

        request[this.ATTRIBUTE_NAME] = id;
        RequestContext.cls.run(new RequestContext(request, response), next);
    }
}
