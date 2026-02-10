import { Injectable } from '@nestjs/common';
import { DeactivateOldStudentUsernamesRepository } from './deactivate-old-student-usernames.repository';
import { Connection } from 'typeorm';

@Injectable()
export class DeactivateOldStudentUsernamesService {
    constructor(
        private deactivateOldStudentUsernamesRepository: DeactivateOldStudentUsernamesRepository,
        private connection: Connection,
    ) {}

    async run() {
        const newUsers = await this.deactivateOldStudentUsernamesRepository.getAllCompletedUsers();

        for (const user of newUsers) {
            await this.connection.transaction(async (manager) => {
                await this.deactivateOldStudentUsernamesRepository.updatePerson(user, manager);
                await this.deactivateOldStudentUsernamesRepository.setDeletedOn(user, manager);
            });
        }
    }
}
