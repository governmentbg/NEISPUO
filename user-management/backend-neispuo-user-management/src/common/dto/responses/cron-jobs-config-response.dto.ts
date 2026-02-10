import { IsNumber, IsString } from 'class-validator';
import { DTO } from './dto.interface';

export class CronJobConfigResponseDTO implements DTO {
    @IsNumber()
    jobID?: number;

    @IsString()
    name?: string;

    @IsString()
    cron?: string;

    @IsNumber()
    isRunning?: number;

    @IsNumber()
    isActive?: number;

    @IsNumber()
    markedForRestart?: number;
}
