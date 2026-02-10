import { INomenclature } from './nomenclature';

export interface CurrentYear extends INomenclature {
    CurrentYearID: number;
    Name: string;
    Description?: string;
    isValid: boolean;
    TempCurrentYearID: number;
    ChangeYearFrom: Date;
    ChangeYearTo: Date
}