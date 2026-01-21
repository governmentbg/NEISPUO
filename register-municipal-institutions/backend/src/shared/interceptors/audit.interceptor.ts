import {
    Injectable,
    NestInterceptor,
    ExecutionContext,
    CallHandler,
    BadGatewayException,
    HttpException
} from '@nestjs/common';
import { Observable, throwError, Subject } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import * as util from 'util';
import { AuthedRequest } from '../interfaces/authed-request.interface';
import { Audit } from 'src/domain/audit/audit.entity';
import { ConfigService } from '../services/config/config.service';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';

@Injectable()
export class AuditInterceptor implements NestInterceptor {
    constructor(
        private configService: ConfigService,
        @InjectRepository(Audit) private readonly auditRepo: Repository<Audit>
    ) {}

    intercept(context: ExecutionContext, next: CallHandler): Observable<any> {
        // get Request and auth object for user
        const req: AuthedRequest = context.switchToHttp().getRequest();
        if (
            !this.configService
                .get('AUDIT_METHODS')
                ?.split(',')
                .includes(req.method)
        ) {
            return next.handle();
        }

        const authObject = req._authObject;

        const requestBody = { ...req.body };

        // remove passwords from audit
        for (const exclude of this.configService.get('EXCLUDE_FROM_AUDIT')?.split(',')) {
            if (requestBody.hasOwnProperty(exclude)) {
                delete requestBody[exclude];
            }
        }

        // create entry for insert
        let auditEntry: Audit = {
            requestHeaders: JSON.stringify(req.rawHeaders),
            requestBody: JSON.stringify(requestBody),
            statusCode: undefined,
            originalUrl: req.originalUrl,
            responseBody: '',
            method: req.method // POST/PUT/PATCH/DELETE
        };

        // add user to audit entry if user is logged in
        if (authObject && authObject.user) {
            auditEntry = {
                ...auditEntry,
                // user: authObject.user
            };
        }

        return next.handle().pipe(
            map(data => {
                auditEntry.responseBody = JSON.stringify(data) || '';
                auditEntry.statusCode = context.switchToHttp().getResponse().statusCode;
                // this.auditRepo.insert(auditEntry);

                return data;
            }),
            catchError(err => {
                auditEntry.responseBody = JSON.stringify(err.response) || '';
                if (err.response) {
                    auditEntry.statusCode = err.response.statusCode;
                } else {
                    auditEntry.statusCode = 500;
                }
                // this.auditRepo.insert(auditEntry);
                return throwError(err);
            })
        );
    }
}
