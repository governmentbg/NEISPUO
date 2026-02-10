import { Module } from '@nestjs/common';
import { VariablesService } from './routing/variables.service';
import { VariablesRepository } from './variables.repository';

@Module({
    imports: [],
    providers: [VariablesService, VariablesRepository],
    exports: [VariablesService],
})
export class VariablesModule {}
