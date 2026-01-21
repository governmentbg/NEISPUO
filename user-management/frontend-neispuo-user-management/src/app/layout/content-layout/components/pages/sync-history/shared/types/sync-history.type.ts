export type EntityType = 'institutions' | 'teachers' | 'students' | 'parents';

export enum SyncHistoryEntityEnum {
    INSTITUTION = 'institutions',
    TEACHER = 'teachers',
    STUDENT = 'students',
    PARENT = 'parents',
}

export interface UserLoadMethodState {
    azureID: string | null;
    publicEduNumber: string | null;
    loadMethod: 'azureID' | 'publicEduNumber';
}
