import { Type } from 'class-transformer';
import { IsDate, IsOptional } from 'class-validator';
import { getLastWorkdayEightAm } from 'src/features/email-template-type/utils/get-last-workday-eight-am.util';

export class SendCustomEmailDto {
  @IsDate()
  @IsOptional()
  @Type(() => Date)
  fromDate: Date = getLastWorkdayEightAm();

  @IsDate()
  @IsOptional()
  @Type(() => Date)
  toDate: Date = new Date();
}
