import { UserGuide } from 'src/features/user-guides/user-guides.entity';
import { EntityManager } from 'typeorm';

export class UserGuideModificationDTO {
  request: any;
  userGuide: UserGuide;
  auditAction: string;
  entityManager: EntityManager;
}
