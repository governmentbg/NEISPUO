import { NeispuoModule } from './neispuo-module.interface';

export interface NeispuoCategory {
  id: number;
  name: string;
  description?: string;
  neispuoModules?: NeispuoModule[];
}
