import { NeispuoCategory } from './neispuo-category.interface';

export interface NeispuoUserGuide {
  id: number;
  name: string;
  neispuoCategories?: NeispuoCategory[];
  fileContent: any;
  filename: string;
  URLOverride: string;
}
