export enum CampaignType {
    EVALUATE = 1,
    SELFEVALUATE = 2,
    ESUI = 3,
}

// tslint:disable-next-line: no-namespace
export namespace CampaignType {
    export const toConvertType = (type: CampaignType) => {
        switch (type) {
            case CampaignType.EVALUATE:
                return 'Оценка';
            case CampaignType.SELFEVALUATE:
                return 'Самооценка';
            case CampaignType.ESUI:
                return 'ЕСУИ';
        }
    };
}