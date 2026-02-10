import { Module } from '@nestjs/common';
import { AggregatedResultsService } from './routes/aggregated-results.service';
import { AggregatedResultsController } from './routes/aggregated-results.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { AggregatedResults } from './aggregated-results.entity';

@Module({
    imports: [TypeOrmModule.forFeature([AggregatedResults])],
    exports: [TypeOrmModule],

    controllers: [AggregatedResultsController],
    providers: [AggregatedResultsService]
})
export class AggregatedResultsModule {}
