import { Question, QuestionSet } from 'nest-commander';
import { SyncUserPosition } from './sync-azure-users.command';

@QuestionSet({ name: 'sync-users' })
export class SyncUserQuestion {
    @Question({
        type: 'confirm',
        name: 'position',
        message:
            'Are you sure you want to run the script with no arguments? This will lead to a full sync script execution.',
        default: SyncUserPosition.ALL,
    })
    parsePosition(val: SyncUserPosition) {
        return val;
    }
}
