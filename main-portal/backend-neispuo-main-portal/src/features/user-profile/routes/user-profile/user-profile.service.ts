import {
  ForbiddenException,
  Inject,
  Injectable,
  NotFoundException,
  Scope,
} from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { UserProfile } from '../../user-profile.entity';
import { REQUEST } from '@nestjs/core';
import { AuthedRequest } from 'src/shared/dto/authed-request.interface';

@Injectable({ scope: Scope.REQUEST })
@Injectable()
export class UserProfileService {
  constructor(
    @InjectRepository(UserProfile) private repo,
    @Inject(REQUEST) private readonly request: AuthedRequest,
  ) {}

  async findOwnProfile() {
    const selectedRole = this.request.user?.selected_role;
    let profileDto: UserProfile = {
      SysRoleID: selectedRole.SysRoleID,
      SysUserID: selectedRole.SysUserID,
      RegionID: selectedRole.RegionID,
      MunicipalityID: selectedRole.MunicipalityID,
      InstitutionID: selectedRole.InstitutionID,
      BudgetingInstitutionID: selectedRole.BudgetingInstitutionID,
      PositionID: selectedRole.PositionID,
    } as UserProfile;
    if (!selectedRole) {
      throw new ForbiddenException(`Invalid JWT payload.`);
    }
    const profile = await this.repo.findOne({
      where: { ...profileDto },
    });
    if (!profile) {
      throw new NotFoundException(
        `Could not find profile for SysUserID ${JSON.stringify(selectedRole)}`,
      );
    }
    return profile;
  }
}
