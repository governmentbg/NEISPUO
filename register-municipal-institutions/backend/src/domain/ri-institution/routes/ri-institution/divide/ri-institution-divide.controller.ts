import {
 Body, Controller, Post, Req, UseGuards,
} from '@nestjs/common';
import { AuthedRequest } from '@shared/interfaces/authed-request.interface';
import { RIInstitutionGuard } from '../ri-institution.guard';
import { RIInstitutionDivideService } from './ri-institution-divide.service';

@UseGuards(RIInstitutionGuard)
@Controller('v1/ri-institution-divide')
export class RIInstitutionDivideController {
    constructor(public riInstitutionDivideService: RIInstitutionDivideService) {}

    /**
     * Преобразуване чрез разделяне
     *
     * @param req
     * @param params
     * @param body
     */
    @Post()
    async divideInstitutions(@Req() authedRequest: AuthedRequest, @Body() body: any) {
        return await this.riInstitutionDivideService.divideInstitutions(authedRequest, body);
    }
}
