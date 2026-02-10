export interface IAzureSyncUserSearchInput {
    institutionID: number;
    position: 'teacher' | 'student';
    enableUserManagementSync?: boolean;
    fromPersonCreationDate: string | Date;
}
