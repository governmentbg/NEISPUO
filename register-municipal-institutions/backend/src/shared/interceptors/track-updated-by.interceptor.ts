import {
    Injectable,
    NestInterceptor,
    ExecutionContext,
    CallHandler,
    InternalServerErrorException
} from '@nestjs/common';
import { Observable } from 'rxjs';
import { AuthedRequest } from '../interfaces/authed-request.interface';
import { HttpMethodEnum } from '../enums/http-method.enum';

@Injectable()
export class TrackUpdatedByInterceptor implements NestInterceptor {
    intercept(context: ExecutionContext, next: CallHandler): Observable<any> {
        const req: AuthedRequest = context.switchToHttp().getRequest();
        if (
            req.method === HttpMethodEnum.POST ||
            req.method === HttpMethodEnum.PUT ||
            req.method === HttpMethodEnum.PATCH
        ) {
            const authObject = req._authObject;

            if (authObject) {
                const user = authObject.user;
                req.body.updatedBy = user;
                return next.handle();
            }
            if (req.originalUrl.indexOf('cms') !== -1) {
                throw new InternalServerErrorException(
                    'Cannot add updatedBy to request body, because user is not authenticated.'
                );
            }
        }
        return next.handle();
    }
}
