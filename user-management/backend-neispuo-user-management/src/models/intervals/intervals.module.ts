import { Module } from '@nestjs/common';
import { TokenRefreshInterval } from 'src/intervals/token-refresh.interval';
import { UpdateJobTopValuesInterval } from 'src/intervals/update-job-top-values.interval';
import { UpdateJobInterval } from 'src/intervals/update-job.interval';

@Module({
    imports: [],
    providers: [TokenRefreshInterval, UpdateJobInterval, UpdateJobTopValuesInterval],
})
export class IntervalsModule {}
