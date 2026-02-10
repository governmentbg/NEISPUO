import { Injectable } from '@nestjs/common';
import { LogEntity } from 'src/common/entities/log.entity';
import { LogRepository } from '../log.repository';

@Injectable()
export class LogService {
    constructor(private logRepository: LogRepository) {}

    async insertLog?(dto: LogEntity) {
        await this.logRepository.insertLog(dto);
    }

    async areThereErrorsFromLastWeek() {
        const result = await this.logRepository.getLastWeekLogs();
        if (result?.length > 0) return true;
        return false;
    }
}
