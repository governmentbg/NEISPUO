import { Inject, Injectable, Logger, NotFoundException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { EmailTemplateType } from 'src/features/email-template-type/email-template-type.entity';
import { ContentProvider } from 'src/features/email-template-type/interfaces/content-provider.interface';
import { DataFetchResult } from 'src/features/email-template-type/interfaces/data-fetch-result.interface';
import { EmailTemplateTypeService } from 'src/features/email-template-type/routing/email-template-type.service';
import { EmailTemplate } from 'src/features/email-template/email-template.entity';
import { AuthedRequest } from 'src/shared/dto/authed-request.interface';
import { SendEmailResponse } from 'src/shared/interfaces/send-email-response.interface';
import { MailerWrapperService } from 'src/shared/services/mailer-wrapper/mailer-wrapper.service';
import { dedupeAndValidateStrings } from 'src/shared/utils/dedupe-and-validate-strings';
import { Repository } from 'typeorm';
import { CreateEmailTemplateDto } from '../dto/create-email-template.dto';
import { SendCustomEmailDto } from '../dto/send-custom-email.dto';
import { UpdateEmailTemplateDto } from '../dto/update-email-template.dto';

@Injectable()
export class EmailTemplateService {
  private readonly logger = new Logger(EmailTemplateService.name);
  private readonly relations = ['emailTemplateType'];

  constructor(
    @InjectRepository(EmailTemplate)
    private readonly emailTemplateRepo: Repository<EmailTemplate>,
    @Inject('CONTENT_PROVIDERS')
    private readonly providers: ContentProvider[],
    private readonly emailTemplateTypeService: EmailTemplateTypeService,
    private readonly mailerWrapperService: MailerWrapperService,
  ) {}

  async findAll() {
    return this.emailTemplateRepo
      .createQueryBuilder('emailTemplate')
      .leftJoin('emailTemplate.emailTemplateType', 'type')
      .addSelect(['type.displayName'])
      .getMany();
  }

  async findOneOrFail(id: number) {
    const template = await this.emailTemplateRepo.findOne(id, {
      relations: this.relations,
    });
    if (!template) {
      throw new NotFoundException(`EmailTemplate #${id} not found`);
    }
    return template;
  }

  async create(dto: CreateEmailTemplateDto, authedRequest: AuthedRequest) {
    const emailTemplateType = await this.emailTemplateTypeService.findOneOrFail(
      dto.emailTemplateTypeId,
    );

    const username = authedRequest?.user?.selected_role?.Username;

    const template = this.emailTemplateRepo.create({
      title: dto.title,
      content: dto.content,
      isActive: dto.isActive,
      recipients: dto?.recipients
        ? dedupeAndValidateStrings(dto.recipients)
        : [],
      emailTemplateType,
      createdBy: username,
      updatedBy: username,
    });

    return this.emailTemplateRepo.save(template);
  }

  async update(
    id: number,
    dto: UpdateEmailTemplateDto,
    authedRequest: AuthedRequest,
  ) {
    await this.findOneOrFail(id);

    const username = authedRequest?.user?.selected_role?.Username;

    if (dto.emailTemplateTypeId) {
      const type = await this.emailTemplateTypeService.findOneOrFail(
        dto.emailTemplateTypeId,
      );
      dto.emailTemplateType = type;
    }
    if (dto.recipients) {
      dto.recipients = dedupeAndValidateStrings(dto.recipients);
    }

    delete dto.emailTemplateTypeId;

    return this.emailTemplateRepo.save({ ...dto, updatedBy: username, id });
  }

  async delete(id: number) {
    const result = await this.emailTemplateRepo.delete(id);
    if (result.affected === 0) {
      throw new NotFoundException(`EmailTemplate #${id} not found`);
    }
  }

  async sendCustomEmailById(
    id: number,
    dto: SendCustomEmailDto,
  ): Promise<SendEmailResponse> {
    const template = await this.findOneOrFail(id);

    const provider = this.providers.find(p =>
      p.supports(template.emailTemplateType),
    );

    try {
      return await this.sendCustomEmail(template, dto, provider);
    } catch (error) {
      this.logger.error(error);
      return {
        success: false,
        messageBG: 'Възникна грешка по време на изпращането на имейла.',
      };
    }
  }

  async sendCustomEmails() {
    // Duplicate STOP_EMAILS check here so we can bail out immediately—
    // avoids loading templates, compiling content, and other upstream work
    // when emails are globally disabled via the environment.
    if (process.env.STOP_EMAILS === 'true') {
      this.logger.warn(
        'Emails are stopped by environment variable. No Emails will be sent.',
      );
      return;
    }

    const templateTypes = await this.emailTemplateTypeService
      .getRepository()
      .createQueryBuilder('templateType')
      .leftJoinAndSelect('templateType.emailTemplates', 'emailTemplate')
      .where('emailTemplate.isActive = :isActive', { isActive: true })
      .getMany();

    if (templateTypes?.length === 0) {
      this.logger.warn(`No active templates found. Skipping.`);
      return;
    }

    for (const templateType of templateTypes) {
      const provider = this.providers.find(p => p.supports(templateType));
      if (!provider) {
        this.logger.warn(
          `No provider for type: "${templateType.displayName}". Skipping all templates of this type.`,
        );
        continue;
      }

      const { fromDate, toDate } = new SendCustomEmailDto();
      const data = await provider.fetchData(fromDate, toDate);

      if (!provider.hasSufficientData(data)) {
        this.logger.warn(
          `No sufficient data for template type: "${templateType.displayName}". Skipping all templates of this type.`,
        );
        continue;
      }

      templateType.emailTemplates.forEach(template => {
        template.emailTemplateType = {
          variableMappings: templateType.variableMappings,
        } as EmailTemplateType;

        this.sendCustomEmailWithData(template, data, provider).catch(err => {
          this.logger.error(
            `Error sending "${template.title}": ${err?.message}`,
          );
        });
      });
    }
  }

  private async validateTemplate(
    template: EmailTemplate,
    provider: ContentProvider,
  ): Promise<{ isValid: boolean } & SendEmailResponse> {
    if (!template.recipients?.length) {
      return {
        isValid: false,
        success: true,
        skipped: true,
        messageBG: `Съобщението "${template.title}" няма получатели.`,
        messageENG: `Message "${template.title}" has no recipients. Skipping.`,
      };
    }

    if (!provider) {
      return {
        isValid: false,
        success: false,
        messageBG: `Няма доставчик на данни за тип: "${template.emailTemplateType.displayName}", в съобщение: "${template.title}".`,
        messageENG: `No data provider for type: "${template.emailTemplateType.displayName}", in message: "${template.title}". Skipping.`,
      };
    }

    return { isValid: true, success: true };
  }

  private async sendEmail(
    template: EmailTemplate,
    data: DataFetchResult,
    provider: ContentProvider,
  ): Promise<SendEmailResponse> {
    const html = await provider.getPopulatedTemplate(
      template.content,
      template.emailTemplateType.variableMappings,
      data,
    );

    return this.mailerWrapperService.sendMail({
      subject: template.title,
      html,
      from: process.env.EMAIL_FROM,
      to: template.recipients?.join(','),
    });
  }

  private async sendCustomEmailWithData(
    template: EmailTemplate,
    data: DataFetchResult,
    provider: ContentProvider,
  ) {
    const validation = await this.validateTemplate(template, provider);
    if (!validation.isValid) {
      this.logger.warn(validation.messageENG);
      return { success: false, messageBG: validation.messageBG };
    }

    return this.sendEmail(template, data, provider);
  }

  private async sendCustomEmail(
    template: EmailTemplate,
    dto: SendCustomEmailDto,
    provider: ContentProvider,
  ): Promise<SendEmailResponse> {
    const validation = await this.validateTemplate(template, provider);
    if (!validation.isValid) {
      this.logger.warn(validation.messageENG);
      return {
        success: validation.success,
        skipped: validation.skipped,
        messageBG: validation.messageBG,
      };
    }

    const { fromDate, toDate } = dto;
    const data = await provider.fetchData(fromDate, toDate);

    if (!provider.hasSufficientData(data)) {
      const messageENG = `Message "${template.title}" has no data this run.`;
      const messageBG = `Съобщението "${template.title}" няма данни за този период.`;
      this.logger.warn(messageENG);
      return { success: true, skipped: true, messageBG: messageBG };
    }

    return this.sendEmail(template, data, provider);
  }
}
