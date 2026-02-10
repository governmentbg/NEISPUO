export class StudentClassEditModel {
  constructor(obj = {}) {
      this.id = obj.id || 0;
      this.schoolYear = obj.schoolYear || '';
      this.schoolYearName = obj.schoolYearName || '';
      this.classId = obj.classId || 0;
      this.personId = obj.personId || 0;
      this.classNumber = obj.classNumber || 0;
      this.hasIndividualStudyPlan = obj.hasIndividualStudyPlan || false;
      this.repeaterId = obj.repeaterId || 0;
      this.hasSupportiveEnvironment = obj.hasSupportiveEnvironment || false;
      this.supportiveEnvironment = obj.supportiveEnvironment || '';
      this.classGroup = obj.classGroup || null;
      this.isNotForSubmissionToNra = obj.isNotForSubmissionToNra || false;
      this.commuterTypeId = obj.commuterTypeId || 0;
      this.oresTypeId = obj.oresTypeId || '';
      this.oresTypeName = obj.oresTypeName || '';
      this.availableArchitecture = obj.availableArchitecture || null;

      this.buildingAreas = (() => {
        if(obj.buildingAreas){
            if(obj.buildingAreas.length !== undefined){
                if(obj.buildingAreas.length === 0){
                    return null;
                }
                return obj.buildingAreas;
            }
        }
        return null;
     })();

       this.buildingRooms = (() => {
            if(obj.buildingRooms){
                if(obj.buildingRooms.length !== undefined){
                    if(this.buildingAreas === null){
                       return [];
                    }
                    else if(obj.buildingRooms.length === 0){
                        return null;
                    }
                    return obj.buildingRooms;
                }
            }
            return null;
       })();

       this.specialEquipment = (() => {
        if(obj.specialEquipment){
            if(obj.specialEquipment.length !== undefined){
                if(this.buildingAreas === null){
                    return [];
                }
                else if(obj.specialEquipment.length === 0){
                    return null;
                }
                return obj.specialEquipment;
            }
        }
        return null;
   })();



  }
}
