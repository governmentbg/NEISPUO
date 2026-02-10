import { Injectable, Logger, OnModuleInit } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { EnableDatabaseLogging } from 'src/common/decorators/enable-database-logging.decorator';
import { SysUserResponseDTO } from 'src/common/dto/responses/sys-user.response.dto';
import { RedisService } from 'src/models/redis/redis.service';
import { SystemUserRepository } from '../system-user.repository';

@Injectable()
@EnableDatabaseLogging({
    includedMethods: ['getSystemUserFromDatabase'],
})
export class SystemUserService implements OnModuleInit {
    constructor(private readonly redisService: RedisService, private sysUserRepository: SystemUserRepository) {}

    async onModuleInit() {
        await this.loadSystemUser();
    }

    private readonly logger = new Logger(SystemUserService.name);

    async loadSystemUser() {
        const systemUser = await this.getSystemUserFromDatabase();
        await this.setSystemUser(systemUser);
    }

    async getSystemUserFromDatabase() {
        return this.sysUserRepository.getSystemUserFromDatabase();
    }

    async setSystemUser(dto: SysUserResponseDTO) {
        await this.redisService.set(CONSTANTS.CACHE_STORAGE_SYSTEM_USER_USERNAME, dto);
    }

    async getSystemUser() {
        const dto: SysUserResponseDTO = await this.redisService.get(CONSTANTS.CACHE_STORAGE_SYSTEM_USER_USERNAME);
        return dto;
    }
}
