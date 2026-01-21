import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { UserProfileController } from './routes/user-profile/user-profile.controller';
import { UserProfileService } from './routes/user-profile/user-profile.service';
import { UserProfile } from './user-profile.entity';

@Module({
  imports: [TypeOrmModule.forFeature([UserProfile])],
  providers: [UserProfileService],
  exports: [],
  controllers: [UserProfileController],
})
export class UserProfileModule {}
