import {
 Body, Controller, Post, Req, UseGuards,
} from '@nestjs/common';
import { AuthedRequest } from '@shared/interfaces/authed-request.interface';
import { RIInstitutionGuard } from '../ri-institution.guard';
import { RIInstitutionDetachService } from './ri-institution-detach.service';

@UseGuards(RIInstitutionGuard)
@Controller('v1/ri-institution-detach')
export class RIInstitutionDetachController {
    constructor(public riInstitutionDetachService: RIInstitutionDetachService) {}

    /**
     * Преобразуване чрез отделяне
     *
     * @param req
     * @param params
     * @param body
     */
    @Post()
    async detachInstitutions(@Req() authedRequest: AuthedRequest, @Body() body: any) {
        return await this.riInstitutionDetachService.detachInstitutions(authedRequest, body);
    }
}
