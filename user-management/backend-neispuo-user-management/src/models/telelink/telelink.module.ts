import { HttpModule, Module } from '@nestjs/common';
import { BearerTokenModule } from '../bearer-token/bearer-token.module';
import { TelelinkService } from './routing/telelink.service';

@Module({
    imports: [HttpModule, BearerTokenModule, HttpModule],
    providers: [TelelinkService],
    controllers: [],
    exports: [TelelinkService],
})
export class TelelinkModule {}
