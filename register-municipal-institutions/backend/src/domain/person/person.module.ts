import { Module } from '@nestjs/common';
import { PersonService } from './routes/person/person.service';
import { PersonController } from './routes/person/person.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { Person } from './person.entity';

@Module({
    imports: [TypeOrmModule.forFeature([Person])],
    exports: [TypeOrmModule],

    controllers: [PersonController],
    providers: [PersonService]
})
export class PersonModule {}
