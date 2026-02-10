import { IsNumber, IsString } from 'class-validator';

export class CronJobConfigResponseDTO {
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
