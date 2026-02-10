import {
  Body,
  Controller,
  Delete,
  Get,
  Param,
  Post,
  Put,
  UseGuards,
} from '@nestjs/common';
import { RoleEnum } from 'src/shared/enums/role.enum';
import { JwksGuard } from 'src/shared/guards/jwks.guard';
import { RoleGuard } from 'src/shared/guards/roles.guard';
import { SystemUserMessageDto } from '../dto/system-user-message.dto';
import { SystemUserMessageService } from './system-user-message.service';

@UseGuards(JwksGuard, RoleGuard([RoleEnum.MON_ADMIN]))
@Controller('/v1/system-user-message')
export class SystemUserMessageController {
  constructor(private readonly service: SystemUserMessageService) {}

  @Get()
  async getSystemUserMessages() {
    return this.service.getSystemUserMessages();
  }

  @Post()
  async createSystemUserMessage(@Body() message: SystemUserMessageDto) {
    return this.service.createSystemUserMessage(message);
  }

  @Put(':id')
  async updateSystemUserMessage(
    @Param('id') id: number,
    @Body() message: SystemUserMessageDto,
  ) {
    return this.service.updateSystemUserMessage(id, message);
  }

  @Delete(':id')
  async deleteSystemUserMessage(@Param('id') id: number) {
    return this.service.deleteSystemUserMessage(id);
  }
}
