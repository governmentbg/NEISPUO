import { Module } from '@nestjs/common';
import { UpdateJobInterval } from 'src/intervals/update-job.interval';
import { JobsModule } from '../jobs/jobs.module';

@Module({
    imports: [JobsModule],
    providers: [UpdateJobInterval],
})
export class IntervalsModule {}
