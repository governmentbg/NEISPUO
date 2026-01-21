import { Controller, Get, Query, Res } from '@nestjs/common';
import { VerifyEmailDTO } from './verify-email.dto';
import { VerifyEmailService } from './verify-email.service';
import { Response } from 'express';

@Controller('v1/auth/verify-email')
export class VerifyEmailController {
    constructor(private readonly verifyEmailService: VerifyEmailService) {}
    // @Get()
    // async verifyEmail(@Res() response: Response, @Query() verifyEmailDTO: VerifyEmailDTO) {
    //     const { verified, error } = await this.verifyEmailService.verifyEmail(verifyEmailDTO);
    //     return response.redirect(
    //         process.env.UI_BASE_URL +
    //             process.env.UI_VERIFY_EMAIL_REDIRECT +
    //             `?emailVerified=${verified}${error ? `&error=${error}` : ''}`
    //     );
    // }
}
