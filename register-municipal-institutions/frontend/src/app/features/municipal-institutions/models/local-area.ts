import { Town } from './town';

export interface LocalArea {
  LocalAreaID: number;
  Name: string;
  Town?: Town;
  TownCode?: number;
}
