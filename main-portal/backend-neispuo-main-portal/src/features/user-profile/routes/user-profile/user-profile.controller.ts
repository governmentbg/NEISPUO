import { Controller, Get, UseGuards } from '@nestjs/common';
import { JwksGuard } from 'src/shared/guards/jwks.guard';
import { UserProfileService } from './user-profile.service';

@UseGuards(JwksGuard)
@Controller('v1/user-profile')
export class UserProfileController {
  constructor(public service: UserProfileService) {}

  @Get()
  async getOwnProfile() {
    return await this.service.findOwnProfile();
  }
}
