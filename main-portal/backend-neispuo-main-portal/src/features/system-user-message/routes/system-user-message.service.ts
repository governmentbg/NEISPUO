import {
  Injectable,
  InternalServerErrorException,
  NotFoundException,
} from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { SystemUserMessage } from '../system-user-message.entity';
import { Repository } from 'typeorm';
import { SysRole } from 'src/entities/sys-role.entity';
import { SystemUserMessageWithRoles } from '../interfaces/system-user-message.interface';
import { SystemUserMessageDto } from '../dto/system-user-message.dto';

@Injectable()
export class SystemUserMessageService {
  constructor(
    @InjectRepository(SystemUserMessage)
    private readonly messageRepo: Repository<SystemUserMessage>,
    @InjectRepository(SysRole)
    private readonly roleRepo: Repository<SysRole>,
  ) {}

  async getSystemUserMessages() {
    try {
      const messages = await this.messageRepo
        .createQueryBuilder('message')
        .orderBy(
          `CASE 
           WHEN GETDATE() BETWEEN message.startDate AND message.endDate THEN 0
           ELSE 1
         END`,
          'ASC',
        )
        .addOrderBy('message.endDate', 'DESC')
        .addOrderBy('message.startDate', 'DESC')
        .getMany();

      if (!messages.length) {
        return [];
      }

      const allRoles = await this.roleRepo.find();

      return this.enrichMessagesWithRoles(messages, allRoles);
    } catch (err) {
      throw new InternalServerErrorException('Failed to fetch system messages');
    }
  }

  async createSystemUserMessage(message: SystemUserMessageDto) {
    try {
      const newMessage = this.messageRepo.create({
        title: message.title,
        content: message.content,
        roles: message.roles,
        startDate: message.startDate,
        endDate: message.endDate,
      });

      return await this.messageRepo.save(newMessage);
    } catch (err) {
      throw new InternalServerErrorException('Failed to create system message');
    }
  }

  async updateSystemUserMessage(id: number, message: SystemUserMessageDto) {
    try {
      const existingMessage = await this.messageRepo.findOne({ where: { id } });

      if (!existingMessage) {
        throw new NotFoundException(`System message with ID ${id} not found`);
      }

      const updatedMessage = {
        ...existingMessage,
        title: message.title,
        content: message.content,
        roles: message.roles,
        startDate: message.startDate,
        endDate: message.endDate,
      };

      return await this.messageRepo.save(updatedMessage);
    } catch (err) {
      if (err instanceof NotFoundException) {
        throw err;
      }
      throw new InternalServerErrorException('Failed to update system message');
    }
  }

  async deleteSystemUserMessage(id: number) {
    try {
      const result = await this.messageRepo.delete({ id });

      if (result.affected === 0) {
        throw new NotFoundException(`System message with ID ${id} not found`);
      }

      return { message: 'System message deleted successfully' };
    } catch (err) {
      if (err instanceof NotFoundException) {
        throw err;
      }
      throw new InternalServerErrorException('Failed to delete system message');
    }
  }

  private enrichMessagesWithRoles(
    messages: SystemUserMessage[],
    allRoles: SysRole[],
  ): SystemUserMessageWithRoles[] {
    return messages.map(msg => {
      const roleIds = this.parseRoleIds(msg.roles);
      const matchedRoles = this.matchRolesByIds(allRoles, roleIds);

      return {
        id: msg.id,
        title: msg.title,
        content: msg.content,
        startDate: msg.startDate,
        endDate: msg.endDate,
        roles: matchedRoles.map(role => this.mapRole(role)),
      };
    });
  }

  private parseRoleIds(roleString: string | null | undefined): number[] {
    if (!roleString?.trim()) return [];
    return roleString
      .split(',')
      .map(id => parseInt(id.trim(), 10))
      .filter(id => !isNaN(id));
  }

  private matchRolesByIds(roles: SysRole[], ids: number[]): SysRole[] {
    return roles.filter(role => ids.includes(role.id));
  }

  private mapRole(role: SysRole) {
    return {
      id: role.id,
      name: role.name,
      description: role.description,
    };
  }
}
