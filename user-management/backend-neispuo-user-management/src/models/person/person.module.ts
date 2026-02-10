import { Module } from '@nestjs/common';
import { PersonRepository } from './person.repository';
import { PersonController } from './routing/person.controller';
import { PersonService } from './routing/person.service';

@Module({
    controllers: [PersonController],
    providers: [PersonService, PersonRepository],
    exports: [PersonService],
})
export class PersonModule {}
