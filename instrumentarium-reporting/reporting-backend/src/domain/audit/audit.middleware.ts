// audit.middleware.ts
import { Injectable, NestMiddleware } from '@nestjs/common';
import { Response, NextFunction } from 'express';
import { AuditLoggerService } from './audit.service';
import { AuthedRequest } from 'src/shared/interfaces/authed-request.interface';

@Injectable()
export class AuditMiddleware implements NestMiddleware {
  constructor(private readonly auditLogger: AuditLoggerService) { }

  use(req: AuthedRequest, res: Response, next: NextFunction) {
    res.on('finish', async () => {
      try {
        await this.auditLogger.logRequest(req);
      } catch (err) {
        console.error('Audit logging failed:', err);
      }
    });

    next();
  }
}
