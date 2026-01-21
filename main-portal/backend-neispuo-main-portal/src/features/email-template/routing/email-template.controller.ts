import {
  Body,
  Controller,
  Delete,
  Get,
  HttpCode,
  HttpStatus,
  Logger,
  Param,
  ParseIntPipe,
  Post,
  Put,
  Req,
  UseGuards,
} from '@nestjs/common';
import { AuthedRequest } from 'src/shared/dto/authed-request.interface';
import { CreateEmailTemplateDto } from '../dto/create-email-template.dto';
import { SendCustomEmailDto } from '../dto/send-custom-email.dto';
import { UpdateEmailTemplateDto } from '../dto/update-email-template.dto';
import { EmailTemplateService } from './email-template.service';
import { ApiKeyGuard } from 'src/shared/guards/api-key.guard';
import { JwksGuard } from 'src/shared/guards/jwks.guard';
import { RoleGuard } from 'src/shared/guards/roles.guard';
import { RoleEnum } from 'src/shared/enums/role.enum';

@Controller('v1/email-template')
export class EmailTemplateController {
  private readonly logger = new Logger(EmailTemplateController.name);

  constructor(private readonly service: EmailTemplateService) {}

  @UseGuards(ApiKeyGuard)
  @Get('send-all')
  @HttpCode(HttpStatus.NO_CONTENT)
  sendAll() {
    return this.service.sendCustomEmails();
  }

  @UseGuards(JwksGuard, RoleGuard([RoleEnum.MON_ADMIN]))
  @Get()
  findAll() {
    return this.service.findAll();
  }

  @UseGuards(JwksGuard, RoleGuard([RoleEnum.MON_ADMIN]))
  @Get(':id')
  findOne(@Param('id', ParseIntPipe) id: number) {
    return this.service.findOneOrFail(id);
  }

  @UseGuards(JwksGuard, RoleGuard([RoleEnum.MON_ADMIN]))
  @Post()
  create(
    @Body() dto: CreateEmailTemplateDto,
    @Req() authedRequest: AuthedRequest,
  ) {
    return this.service.create(dto, authedRequest);
  }

  @UseGuards(JwksGuard, RoleGuard([RoleEnum.MON_ADMIN]))
  @Put(':id')
  update( 
    @Param('id', ParseIntPipe) id: number,
    @Body() dto: UpdateEmailTemplateDto,
    @Req() authedRequest: AuthedRequest,
  ) {
    return this.service.update(id, dto, authedRequest);
  }

  @UseGuards(JwksGuard, RoleGuard([RoleEnum.MON_ADMIN]))
  @Delete(':id')
  @HttpCode(HttpStatus.NO_CONTENT)
  remove(@Param('id', ParseIntPipe) id: number) {
    return this.service.delete(id);
  }

  @UseGuards(JwksGuard, RoleGuard([RoleEnum.MON_ADMIN]))
  @Post(':id/send')
  sendOne(
    @Param('id', ParseIntPipe) id: number,
    @Body() dto: SendCustomEmailDto,
  ) {
    return this.service.sendCustomEmailById(id, dto);
  }
}
