import { Module } from '@nestjs/common';
import { UserGuidesService } from './routes/user-guides.service';
import { UserGuidesController } from './routes/user-guides.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { UserGuide } from './user-guides.entity';
import { AuditService } from '../audit/routing/audit.service';
import { AuditEntity } from 'src/entities/audit.entity';

@Module({
  imports: [TypeOrmModule.forFeature([UserGuide, AuditEntity])],
  controllers: [UserGuidesController],
  providers: [UserGuidesService, AuditService],
  exports: [UserGuidesService],
})
export class UserGuideModule {}
