import { Module } from '@nestjs/common';
import { Report } from './report.entity';
import { TypeOrmModule } from '@nestjs/typeorm';
import { ReportController } from './personal-report/report.controller';
import { ReportService } from './report.service';
import { SharedReportController } from './shared-report/shared-report.controller';

@Module({
  imports: [TypeOrmModule.forFeature([Report])],
  controllers: [ReportController, SharedReportController],
  providers: [ReportService],
  exports: [ReportService],
})
export class ReportModule {}
