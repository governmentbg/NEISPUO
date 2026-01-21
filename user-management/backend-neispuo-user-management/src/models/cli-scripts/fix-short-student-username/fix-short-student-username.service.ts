import { Injectable } from '@nestjs/common';
import { FixShortStudentUsernameRepository } from './fix-short-student-username.repository';
import { GraphApiService } from 'src/models/graph-api/routing/graph-api.service';
import { Connection } from 'typeorm';

@Injectable()
export class FixShortStudentUsernameService {
    constructor(
        private fixShortStudentUsernameRepository: FixShortStudentUsernameRepository,
        private graphService: GraphApiService,
        private connection: Connection,
    ) {}

    async getAllStudents() {
        const studentsWithWrongLons = await this.fixShortStudentUsernameRepository.getAllStudents();

        for (const user of studentsWithWrongLons) {
            let secondChar = '';

            //get first charachter from last name
            if (user.surname) {
                secondChar = await this.getTranslatedChar(user.surname);
            }
            //get first charachter from middle name IF couldnt get from last name
            if (user.middleName && !secondChar) {
                secondChar = await this.getTranslatedChar(user.middleName);
            }
            //log error if second char was not generated
            if (!secondChar) {
                await this.fixShortStudentUsernameRepository.createNewUsernameRecord(
                    user,
                    '',
                    '',
                    'Second character was not generated',
                );
            } else {
                //check user in Telelink
                const resp = await this.graphService
                    .callGetExternalUsersInfoByUsernameEndpoint(user.username)
                    .catch(async (err) => {
                        //log error if user do not exist in telelink
                        await this.fixShortStudentUsernameRepository.createNewUsernameRecord(
                            user,
                            '',
                            '',
                            'User do not exist in Graph API',
                        );
                    });
                //if user exist in telelink
                if (resp) {
                    user.azureID = resp.data.id;
                    const { username: oldUsename } = user;

                    //generate new LON
                    const newEdu = `${oldUsename[0]}${secondChar}${oldUsename.substring(1, oldUsename.indexOf('@'))}`
                        .replace('-', '')
                        .replace('_', '')
                        .replace('.', '');

                    //generate new username
                    const newUsername = `${newEdu}@edu.mon.bg`;

                    //open connection
                    await this.connection.transaction(async (manager) => {
                        // insert new user in SysUser and GeneratedUsers tables
                        const userInserted = await this.fixShortStudentUsernameRepository.createNewUser(
                            user,
                            newUsername,
                            newEdu,
                            manager,
                        );
                        //insert new record in NewUsernames table
                        await this.fixShortStudentUsernameRepository.createNewUsernameRecord(
                            user,
                            newUsername,
                            newEdu,
                            userInserted ? 'OK' : 'User was not created. Username already exist!',
                            manager,
                        );
                    });
                }
            }
        }
        return { data: studentsWithWrongLons };
    }

    async getTranslatedChar(word: string) {
        const translatedChar = await this.fixShortStudentUsernameRepository.translateFromCyrilic(word);
        const latin = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'.toLowerCase();
        if (translatedChar && latin.indexOf(translatedChar[0].toLowerCase()) >= 0) {
            return translatedChar[0].toLowerCase();
        }
        return '';
    }
}
