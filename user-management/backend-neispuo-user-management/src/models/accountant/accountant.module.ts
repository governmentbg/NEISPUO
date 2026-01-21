import { Module } from '@nestjs/common';
import { AccountantRepository } from './accountant.repository';
import { AccountantService } from './routing/accountant.service';

@Module({
    imports: [],
    providers: [AccountantService, AccountantRepository],
    exports: [AccountantService],
})
export class AccountantModule {}
