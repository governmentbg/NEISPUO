import { CPLRAreaType } from "./cplr-area-type";

export interface RICPLRArea {
  RICPLRAreaID: number;
  CPLRAreaType?: CPLRAreaType;
  ValidFrom?: number;
  ValidTo?: number;
}