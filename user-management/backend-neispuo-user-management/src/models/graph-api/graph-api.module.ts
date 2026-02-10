import { HttpModule, Module } from '@nestjs/common';
import { BearerTokenModule } from '../bearer-token/bearer-token.module';
import { GraphApiService } from './routing/graph-api.service';
import { GraphApiController } from './routing/graph-api.controller';

@Module({
    imports: [HttpModule, BearerTokenModule],
    controllers: [GraphApiController],
    providers: [GraphApiService],
    exports: [GraphApiService],
})
export class GraphApiModule {}
