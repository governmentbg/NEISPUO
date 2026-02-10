import { PartialType } from '@nestjs/mapped-types';
import { CreateEmailTemplateDto } from './create-email-template.dto';
import { EmailTemplateType } from 'src/features/email-template-type/email-template-type.entity';

export class UpdateEmailTemplateDto extends PartialType(
  CreateEmailTemplateDto,
) {
  emailTemplateType?: EmailTemplateType
}
