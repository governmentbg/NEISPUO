import { ExceptionFilter, Catch, ArgumentsHost } from '@nestjs/common';
import { SystemError } from '../exceptions/catch.exception';

@Catch()
export class HttpExceptionFilter<T extends SystemError> implements ExceptionFilter {
  catch(exception: T, host: ArgumentsHost) {
    const ctx = host.switchToHttp();
    const response = ctx.getResponse();

    response
      .status(exception.status)
      .json({
        statusCode: exception.status,
        message: exception.message,
      });
  }
}
