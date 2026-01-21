export enum SettingTypeEnum {}

type SettingMap = {
    [key in SettingTypeEnum]: any;
};

export type SettingValue = SettingMap;
