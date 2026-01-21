export class DiplomasDataModel{
    constructor(obj = {}){
        this.totalFinalizedDiplomasCount = obj.totalFinalizedDiplomasCount || 0;
        this.diplomasFinalizedForCurrentYearCount = obj.diplomasFinalizedForCurrentYearCount || 0;
    }
}