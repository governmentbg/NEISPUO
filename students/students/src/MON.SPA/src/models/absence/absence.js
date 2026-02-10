export class AbsenceFileWithDetails {
    constructor(obj = {}) {
        this.importedFileDetails = obj.importedFileRecords;
        this.itemsCount = obj.totalRecords;
        this.month = obj.month;
        this.schoolYear = obj.schoolYear;
        this.dateCreated = obj.dateImported;
        this.importedFilesCount = obj.recordsCount;
    }
}