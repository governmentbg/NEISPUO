import {
    Injectable,
    NestInterceptor,
    ExecutionContext,
    CallHandler,
    InternalServerErrorException,
    BadRequestException
} from '@nestjs/common';
import { Observable } from 'rxjs';
import { Connection } from 'typeorm';
import { File } from '../file.entity';
import { AuthedRequest } from '../../../shared/middleware/auth.middleware';
import { ConfigService } from '../../../shared/services/config/config.service';

@Injectable()
export class MaxFileUploadInterceptor implements NestInterceptor {
    constructor(private connection: Connection, private configService: ConfigService) {}

    async intercept(context: ExecutionContext, next: CallHandler): Promise<Observable<any>> {
        const req: AuthedRequest = context.switchToHttp().getRequest();
        const reqUploadSize = +req.headers['content-length'];
        const user = req._authObject.user;

        // Get total size of persisted files
        const fileRepo = this.connection.getRepository(File);
        const { sumPersistedBytes } = await fileRepo
            .createQueryBuilder('File')
            .select('SUM(File.fileSize)', 'sumPersistedBytes')
            .where('userId = :id', { id: user.SysUserID })
            .getRawOne();

        // Compare total size of persisted files after current upload is complete
        const totalUploadSizeIfRequestAllowed = reqUploadSize + +sumPersistedBytes;
        const limitExceeded =
            totalUploadSizeIfRequestAllowed >
            +this.configService.get('MAX_TOTAL_FILE_SIZE_PER_USER');
        if (limitExceeded) {
            // throw if current upload would exceed MAX_TOTAL_FILE_SIZE_PER_USER
            throw new BadRequestException(`Upload limit exceeded.`);
        }

        return next.handle();
    }
}
