import { NeispuoModule } from './neispuo-module.interface';
import { NeispuoUserGuide } from './neispuo-user-guide.interface';
export interface NeispuoCategory {
  id: number;
  name: string;
  description?: string;
  neispuoModules?: NeispuoModule[];
  userGuides?: NeispuoUserGuide[];
}
