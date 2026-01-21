import { Module } from '@nestjs/common';
import { FileController } from './routes/file.controller';
import { UserGuideModule } from '../user-guides/user-guide.module';
import { AuditService } from '../audit/routing/audit.service';

@Module({
  imports: [UserGuideModule],
  controllers: [FileController],
  providers: [AuditService],
  exports: [],
})
export class FileModule {}
