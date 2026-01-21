import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { AzureUsersEntity } from 'src/common/entities/azure-users.entity';
import { AzureUsersRepository } from './azure-users.repository';
import { AzureUsersCrudController } from './routing/azure-users-crud.controller';
import { AzureUsersCrudService } from './routing/azure-users-crud.service';
import { AzureUsersController } from './routing/azure-users.controller';
import { AzureUsersService } from './routing/azure-users.service';
import { UsersArchiveService } from './routing/users-archive.service';
import { UsersErrorService } from './routing/users-error.service';
import { UsersRestartService } from './routing/users-restart.service';

@Module({
    imports: [TypeOrmModule.forFeature([AzureUsersEntity])],
    controllers: [AzureUsersController, AzureUsersCrudController],
    providers: [
        AzureUsersService,
        AzureUsersRepository,
        AzureUsersCrudService,
        UsersArchiveService,
        UsersRestartService,
        UsersErrorService,
    ],
    exports: [AzureUsersService, UsersArchiveService, UsersRestartService, UsersErrorService],
})
export class AzureUsersModule {}
