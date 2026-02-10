export enum CampaignDateFilter {
    CURRENT_YEAR = new Date().getFullYear(),
    PREVIOUS_YEAR = new Date().getFullYear() - 1,
    TWOYEARS_BEFORE = new Date().getFullYear() - 2
}
