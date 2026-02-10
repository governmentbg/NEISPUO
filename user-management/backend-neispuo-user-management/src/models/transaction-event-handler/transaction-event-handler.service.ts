import { Injectable } from '@nestjs/common';
import { EventEmitter } from 'events';
import { EntityManager, QueryRunner } from 'typeorm';
import { SIEMLoggerService } from '../siem-logger/siem-logger.service';

@Injectable()
export class TransactionEventHandlerService {
    private eventEmitter = new EventEmitter();

    constructor(private readonly siemLoggerService: SIEMLoggerService) {}

    registerCommitCallback(manager: EntityManager, callback: (...args: any[]) => void, ...args: any[]) {
        const queryRunner = manager.queryRunner;

        if (!queryRunner) {
            // Immediately execute the callback if no transaction is active
            callback(...args, this.siemLoggerService);
            return;
        }

        const managerId = this.getManagerId(queryRunner);
        const originalCommit = queryRunner.commitTransaction.bind(queryRunner);

        // Override the commitTransaction method
        queryRunner.commitTransaction = async () => {
            await originalCommit(); // Perform the original commit
            this.eventEmitter.emit(`commit_${managerId}`, ...args, this.siemLoggerService);
        };

        // Register the callback to execute when the commit event fires, passing all necessary args
        this.eventEmitter.once(`commit_${managerId}`, () => {
            callback(...args, this.siemLoggerService);
        });
    }

    private getManagerId(queryRunner: QueryRunner): string {
        return queryRunner.connection.name || 'default';
    }
}
