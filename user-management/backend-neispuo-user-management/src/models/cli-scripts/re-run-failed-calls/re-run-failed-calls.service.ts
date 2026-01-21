import { Injectable } from '@nestjs/common';
import axios from 'axios';
import * as https from 'https';
import { DBErrorsDTO } from 'src/common/dto/responses/db-errors.dto';
import { ReRunFailedCallsRepository } from './re-run-failed-calls.repository';

@Injectable()
export class ReRunFailedCallsService {
    constructor(private reRunFailedCallsRepository: ReRunFailedCallsRepository) {}

    async reRunAllFailedCalls() {
        console.log(`Started: ${Date.now()}`);
        let count = 0;
        const failedCalls = await this.reRunFailedCallsRepository.getAllFailedCalls();
        console.log(`TotalCount: ${failedCalls.length}`);
        for (const failedCall of failedCalls) {
            count++;
            console.log(`CurrentCount: ${count}`);
            await this.reRunFailedCall(failedCall);
        }
        console.log(`Ended: ${Date.now()}`);
    }

    async reRunFailedCall(dto: DBErrorsDTO) {
        const httpsAgent = new https.Agent({ rejectUnauthorized: false });
        const httpOptions = {
            headers: {
                'Content-Type': 'application/json',
                Authorization: '',
                'x-api-key': process.env.INTERNAL_API_KEY,
            },
            httpsAgent: httpsAgent,
        };
        const url = dto.errorProcedure;
        const body = JSON.parse(dto.data);
        await axios
            .post(url, body, httpOptions)
            .then((result) => {
                console.log(`Successfully inserted record.: ${dto.data}`);
            })
            .catch((err) => {
                console.log(err);
            });
    }
}
