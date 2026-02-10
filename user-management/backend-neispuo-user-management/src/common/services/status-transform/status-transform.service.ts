import { EventStatus } from 'src/common/constants/enum/event-status.enum';

export class StatusTransformService {
    static transformOne(valueToTransform: string) {
        // recieves a string like so '0' and return a string like so 'AWAITING_CREATION'
        return EventStatus[valueToTransform];
    }

    static transformMany(valueToTransform: any[]) {
        // recieves a string like so '0' and return a string like so 'AWAITING_CREATION'
        for (const item of valueToTransform) {
            item.status = EventStatus[item.status];
        }
        return valueToTransform;
    }
}
