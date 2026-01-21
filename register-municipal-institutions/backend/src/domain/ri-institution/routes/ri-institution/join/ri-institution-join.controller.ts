import {
 Body, Controller, Post, Req, UseGuards,
} from '@nestjs/common';
import { AuthedRequest } from '@shared/interfaces/authed-request.interface';
import { RIInstitutionGuard } from '../ri-institution.guard';
import { RIInstitutionJoinService } from './ri-institution-join.service';

@UseGuards(RIInstitutionGuard)
@Controller('v1/ri-institution-join')
export class RIInstitutionJoinController {
    constructor(public riInstitutionJoinService: RIInstitutionJoinService) {}

    /**
     * Вливане
     *
     * @param req
     * @param params
     * @param body
     */
    @Post()
    async joinInstitutions(@Req() authedRequest: AuthedRequest, @Body() reqBody: any) {
        return await this.riInstitutionJoinService.joinInstitutions(authedRequest, reqBody);
    }
}
