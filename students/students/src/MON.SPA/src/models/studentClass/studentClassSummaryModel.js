export class StudentClassSummaryModel{
    constructor(obj = {}){
        this.schoolYear = obj?.schoolYear || 0;
        this.institutionId = obj?.institutionId || 0;
        this.institutionName = obj?.institutionName || 'НЯМА';
        this.className = obj?.className || 'НЯМА';
        this.basicClassName = obj?.basicClassName || '';
        this.classId = obj?.classId || null;
        this.classTypeName = obj?.classTypeName || '';
        this.classSpecialityName = obj?.classSpecialityName || '';
        this.classEduFormName = obj?.classEduFormName || '';
        this.isCurrent = obj.isCurrent || false;
        this.status = obj.status || 0;
        this.enrollmentDate = (obj?.enrollmentDate && this.$moment) ? this.$moment(obj.enrollmentDate).format("MM/DD/YYYY") : '';
    }
}