import {
  CanActivate,
  ExecutionContext,
  Injectable,
  UnauthorizedException,
} from '@nestjs/common';
import crypto from 'crypto';

@Injectable()
export class ApiKeyGuard implements CanActivate {
  private readonly validKey: Buffer;

  constructor() {
    const keyCsv = process.env.INTERNAL_API_KEY.trim() || '';
    this.validKey = Buffer.from(keyCsv, 'utf-8');
  }

  canActivate(context: ExecutionContext): boolean {
    const request = context.switchToHttp().getRequest();

    const headerValue =
      request.headers['x-api-key'] ??
      request.headers['X-API-KEY'] ??
      request.headers['x-api-key'.toLowerCase()];

    if (!headerValue || Array.isArray(headerValue)) {
      throw new UnauthorizedException('Missing x-api-key header');
    }

    if (!this.isKeyValid(headerValue as string)) {
      throw new UnauthorizedException('Invalid API key');
    }

    return true;
  }

  private isKeyValid(incoming: string): boolean {
    const incomingBuf = Buffer.from(incoming, 'utf-8');

    return this.validKey.length === incomingBuf.length
      ? crypto.timingSafeEqual(this.validKey, incomingBuf)
      : false;
  }
}
