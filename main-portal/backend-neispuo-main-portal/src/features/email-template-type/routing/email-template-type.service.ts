import { Injectable, NotFoundException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { EmailTemplateType } from '../email-template-type.entity';

@Injectable()
export class EmailTemplateTypeService {
  constructor(
    @InjectRepository(EmailTemplateType)
    private readonly emailTemplateTypeRepo: Repository<EmailTemplateType>,
  ) {}

  getRepository() {
    return this.emailTemplateTypeRepo;
  }

  async findOneOrFail(id: number) {
    const emailTemplateType = await this.emailTemplateTypeRepo.findOne(id);
    if (!emailTemplateType) {
      throw new NotFoundException(`EmailTemplate #${id} not found`);
    }
    return emailTemplateType;
  }

  async findMany() {
    return this.emailTemplateTypeRepo.find({
      select: ['id', 'displayName', 'variableMappings'],
    });
  }
}
