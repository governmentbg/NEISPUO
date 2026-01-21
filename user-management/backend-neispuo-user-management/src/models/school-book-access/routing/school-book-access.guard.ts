import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import { HttpMethodEnum } from 'src/common/constants/enum/http-method.enum';
import { Connection } from 'typeorm';
import { AuthObject, AuthedRequest } from 'src/common/dto/authed-request.interface';

@Injectable()
export class SchoolBookAccessGuard implements CanActivate {
    constructor(private connection: Connection) {}

    canActivate(ctx: ExecutionContext): Promise<boolean> {
        const authedRequest: AuthedRequest = ctx.switchToHttp().getRequest();
        const authObject: AuthObject = authedRequest._authObject;
        const method = authedRequest.method.toUpperCase() as HttpMethodEnum;

        return this.grantAccess(method, authObject, authedRequest);
    }

    private async canInstitutionModifyRows(rowIDs: number[], institutionID: number) {
        const requestedRows = await this.connection
            .createQueryBuilder()
            .select('personnelSchoolBookAccess.RowID', 'rowID')
            .from('school_books.PersonnelSchoolBookAccess', 'personnelSchoolBookAccess')
            .innerJoin('personnelSchoolBookAccess.classBook', 'classBook', 'classBook.instId = :instId', {
                instId: institutionID,
            })
            .whereInIds(rowIDs)
            .orderBy('rowID', 'ASC')
            .execute()
            .then((r) => r.map((r) => r.rowID));
        return (
            rowIDs.length === rowIDs.length &&
            rowIDs.sort((a, b) => a - b).every((value, index) => value === requestedRows[index])
        );
    }

    private async canRUOModifyRows(rowIDs: number[], regionID: number) {
        const requestedRows = await this.connection
            .createQueryBuilder()
            .select('personnelSchoolBookAccess.RowID', 'rowID')
            .from('school_books.PersonnelSchoolBookAccess', 'personnelSchoolBookAccess')
            .innerJoin('personnelSchoolBookAccess.classBook', 'classBook')
            .innerJoin('classBook.institution', 'institution')
            .innerJoin('institution.town', 'town')
            .innerJoin('town.municipality', 'municipality', 'municipality.regionID IN (:regionID)', {
                regionID,
            })
            .whereInIds(rowIDs)
            .orderBy('rowID', 'ASC')
            .execute()
            .then((r) => r.map((r) => r.rowID));
        return (
            rowIDs.length === requestedRows.length &&
            rowIDs.sort((a, b) => a - b).every((value, index) => value === requestedRows[index])
        );
    }

    private async canRUOCreateRows(classBookIDs: number[], personID: number, schoolYears: number[], regionID: number) {
        const eduStates = await this.connection
            .createQueryBuilder()
            .select('es.PersonID', 'personID')
            .from('core.EducationalState', 'es')
            .innerJoin('es.institution', 'institution')
            .innerJoin('institution.town', 'town')
            .innerJoin('town.municipality', 'municipality', 'municipality.regionID IN (:regionID)', {
                regionID,
            })
            .where(`es.PersonID =:personID`, { personID })
            .orderBy('PersonID', 'ASC')
            .execute()
            .then((r) => r.map((r) => r.personID));

        const allowedClassBooks = await this.connection
            .createQueryBuilder()
            .select('classBook.ClassBookID', 'classBookID')
            .from('school_books.ClassBook', 'classBook')
            .innerJoin('classBook.institution', 'institution')
            .innerJoin('institution.town', 'town')
            .innerJoin('town.municipality', 'municipality', 'municipality.regionID IN (:regionID)', {
                regionID,
            })
            .where('classBook.schoolYear IN (:...schoolYears)', { schoolYears: schoolYears })
            .andWhere('classBook.classBookID  IN (:...classBookIDs)', { classBookIDs: classBookIDs })
            .orderBy('classBookID', 'ASC')
            .execute()
            .then((r) => r.map((r) => r.classBookID));
        return (
            allowedClassBooks.length === classBookIDs.length &&
            classBookIDs.sort((a, b) => a - b).every((value, index) => value === allowedClassBooks[index]) &&
            eduStates.includes(personID)
        );
    }

    private async canInstitutionCreateRows(
        classBookIDs: number[],
        personID: number,
        schoolYears: number[],
        institutionID: number,
    ) {
        const eduStates = await this.connection
            .createQueryBuilder()
            .select('es.PersonID', 'personID')
            .from('core.EducationalState', 'es')
            .where(`es.PersonID =:personID`, { personID })
            .andWhere(`es.InstitutionID =:institutionID`, { institutionID })
            .orderBy('PersonID', 'ASC')
            .execute()
            .then((r) => r.map((r) => r.personID));

        const allowedClassBooks = await this.connection
            .createQueryBuilder()
            .select('classBook.ClassBookID', 'classBookID')
            .from('school_books.ClassBook', 'classBook')
            .innerJoin('classBook.institution', 'institution')
            .where('institution.institutionID = :institutionID', { institutionID })
            .andWhere('classBook.schoolYear IN (:...schoolYears)', { schoolYears: schoolYears })
            .andWhere('classBook.classBookID  IN (:...classBookIDs)', { classBookIDs: classBookIDs })
            .orderBy('classBookID', 'ASC')
            .execute()
            .then((r) => r.map((r) => r.classBookID));
        return (
            allowedClassBooks.length === classBookIDs.length &&
            classBookIDs.sort((a, b) => a - b).every((value, index) => value === allowedClassBooks[index]) &&
            eduStates.includes(personID)
        );
    }

    private async grantAccess(reqMethod: HttpMethodEnum, authObject: AuthObject, req: AuthedRequest) {
        let accessGranted = false;
        if (!authObject?.selectedRole) {
            return false;
        }

        if (reqMethod === HttpMethodEnum.GET) {
            accessGranted = this.grantReadAccess(authObject, req);
        } else if (reqMethod === HttpMethodEnum.POST) {
            accessGranted = await this.grantCreateAccess(authObject, req);
        } else if (reqMethod === HttpMethodEnum.PUT || reqMethod === HttpMethodEnum.PATCH) {
            accessGranted = await this.grantUpdateAccess(authObject, req);
        } else if (reqMethod === HttpMethodEnum.DELETE) {
            accessGranted = await this.grantDeleteAccess(authObject, req);
        }

        return accessGranted;
    }

    private grantReadAccess(authObject: AuthObject, req: AuthedRequest) {
        const { isMon, isRuo, isSchool } = authObject;
        if (isMon) {
            return true;
        } else if (isRuo) {
            return true;
        } else if (isSchool) {
            return true;
        }

        return false;
    }

    private scopeAccessForInstitution(authObject: AuthObject, req: AuthedRequest) {
        return true;
    }

    private scopeAccessForRuo(authObject: AuthObject, req: AuthedRequest) {
        return true;
    }

    private async grantCreateAccess(authObject: AuthObject, req: AuthedRequest) {
        const { isMon, isRuo, isSchool, selectedRole } = authObject;
        let canModify = false;
        const { body } = req;
        const personID = body[0]?.personID;
        const classBookIDs = body.map((el) => +el.classBookID);
        const schoolYears = body.map((el) => +el.schoolYear);
        if (isMon) {
            canModify = true;
        } else if (isRuo) {
            const regionID = selectedRole.RegionID;
            canModify = await this.canRUOCreateRows(classBookIDs, personID, schoolYears, regionID);
        } else if (isSchool) {
            const institutionID = selectedRole.InstitutionID;
            canModify = await this.canInstitutionCreateRows(classBookIDs, personID, schoolYears, institutionID);
        }
        return canModify;
    }

    private async grantUpdateAccess(authObject: AuthObject, req: AuthedRequest) {
        const { isMon, isRuo, isSchool } = authObject;
        const rowIDs = [req.body.rowID];
        let canModify;
        if (isMon) {
            canModify = true;
        } else if (isRuo) {
            const regionID = authObject.selectedRole.RegionID;
            canModify = await this.canRUOModifyRows(rowIDs, regionID);
        } else if (isSchool) {
            const institutionID = authObject.selectedRole.InstitutionID;
            canModify = await this.canInstitutionModifyRows(rowIDs, institutionID);
        }

        return canModify;
    }

    private async grantDeleteAccess(authObject: AuthObject, req: AuthedRequest) {
        const { isMon, isRuo, isSchool } = authObject;
        const rowIDs = (req.query.rowIDs as string)?.split(',').map((r) => +r);
        let canModify;
        if (isMon) {
            canModify = true;
        } else if (isRuo) {
            const regionID = authObject.selectedRole.RegionID;
            canModify = await this.canRUOModifyRows(rowIDs, regionID);
        } else if (isSchool) {
            const institutionID = authObject.selectedRole.InstitutionID;
            canModify = await this.canInstitutionModifyRows(rowIDs, institutionID);
        }

        return canModify;
    }
}
