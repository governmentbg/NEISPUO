import { Module } from '@nestjs/common';
import { VersionController } from './routes/version.controller';
import { VersionService } from './routes/version.service';

@Module({
  imports: [],
  controllers: [VersionController],
  providers: [VersionService],
})
export class VersionModule {}
