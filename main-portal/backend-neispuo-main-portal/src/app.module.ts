import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { WinstonModule } from 'nest-winston';
import { AppController } from './app.controller';
import { AppService } from './app.service';
import { EmailTemplateModule } from './features/email-template/email-template.module';
import { FileModule } from './features/file/file.module';
import { NesipuoCategoryModule } from './features/neispuo-category/neispuo-category.module';
import { NeispuoModuleModule } from './features/neispuo-module/neispuo-module.module';
import { ParentRegisterModule } from './features/parent-register/parent-register.module';
import { SysRoleModule } from './features/sys-roles/sys-role.module';
import { SystemUserMessageModule } from './features/system-user-message/system-user-message.module';
import { UserGuideModule } from './features/user-guides/user-guide.module';
import { UserProfileModule } from './features/user-profile/user-profile.module';
import { VersionModule } from './features/version/version.module';

import { SharedModule } from './shared/shared.module';
import { JobsModule } from './features/jobs/jobs.module';
@Module({
  imports: [
    TypeOrmModule.forRoot(),
    SharedModule,
    NeispuoModuleModule,
    NesipuoCategoryModule,
    UserGuideModule,
    FileModule,
    UserProfileModule,
    ParentRegisterModule,
    VersionModule,
    SystemUserMessageModule,
    SysRoleModule,
    WinstonModule.forRoot({}),
    EmailTemplateModule,
    // JobsModule,
    // IntervalsModule,
  ],
  controllers: [AppController],
  providers: [AppService],
})
export class AppModule {}
