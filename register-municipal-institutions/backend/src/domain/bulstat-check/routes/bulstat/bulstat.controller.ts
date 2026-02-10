import { Controller, Get, Param } from '@nestjs/common';
import { BulstatService } from './bulstat.service';

@Controller('v1/bulstat')
export class BulstatController {
    constructor(public bulstatService: BulstatService) {}

    @Get('bulstat-check/:bulstat')
    async checkBulstat(@Param('bulstat') bulstat: number) {
        return this.bulstatService.getBulstat(bulstat);
    }
}
