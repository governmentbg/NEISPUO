import { Controller, Get, Logger, UseGuards } from '@nestjs/common';
import { RoleEnum } from 'src/shared/enums/role.enum';
import { JwksGuard } from 'src/shared/guards/jwks.guard';
import { RoleGuard } from 'src/shared/guards/roles.guard';
import { EmailTemplateTypeService } from './email-template-type.service';

@UseGuards(JwksGuard, RoleGuard([RoleEnum.MON_ADMIN]))
@Controller('v1/email-template-type')
export class EmailTemplateTypeController {
  private readonly logger = new Logger(EmailTemplateTypeController.name);

  constructor(private readonly service: EmailTemplateTypeService) {}

  @Get()
  async findAll() {
    return this.service.findMany();
  }
}
