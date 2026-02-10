import { Region } from './region';

export interface Municipality {
  MunicipalityID: number;
  Name: string;
  Region?: Region;
}
