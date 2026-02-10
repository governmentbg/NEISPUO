import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { ExternalUserViewEntity } from 'src/common/entities/external-user-view.entity';
import { OtherAzureUsersCrudController } from './routing/other-azure-users-crud.controller';
import { OtherAzureUsersCrudService } from './routing/other-azure-users-crud.service';

@Module({
    imports: [TypeOrmModule.forFeature([ExternalUserViewEntity])],
    providers: [OtherAzureUsersCrudService],
    controllers: [OtherAzureUsersCrudController],
    exports: [OtherAzureUsersCrudService],
})
export class OtherAzureUsersModule {}
