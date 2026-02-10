export class Campaign {
    name: string;
    type: number;
    startDate: Date;
    endDate: Date;
    isActive: boolean;
    isLocked: boolean;
    createdBy: number;
    updatedAt: Date;
    userId?: number;
    schoolId?: number;
}
