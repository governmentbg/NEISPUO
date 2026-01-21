import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { SystemUserMessageService } from './routes/system-user-message.service';
import { SystemUserMessage } from './system-user-message.entity';
import { SysRole } from 'src/entities/sys-role.entity';
import { SystemUserMessageController } from './routes/system-user-message.controller';


@Module({
    imports: [TypeOrmModule.forFeature([SystemUserMessage, SysRole])],
    controllers: [SystemUserMessageController],
    providers: [SystemUserMessageService],
})
export class SystemUserMessageModule { }
