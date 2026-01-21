import { Municipality } from './municipality';

export interface Town {
  TownID: number;
  Name: string;
  Municipality?: Municipality
  Code: number;
}
