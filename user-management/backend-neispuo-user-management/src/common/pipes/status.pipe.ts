import { Injectable, PipeTransform } from '@nestjs/common';
import { EventStatus } from '../constants/enum/event-status.enum';

@Injectable()
export class StatusPipe implements PipeTransform {
    transform(value: string) {
        return EventStatus[value];
    }
}
