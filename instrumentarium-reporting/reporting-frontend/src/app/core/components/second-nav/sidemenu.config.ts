import { CONSTANTS } from '@shared/constants';

export interface ISideMenuConfigItem {
  label: string;
  routerLink: string[];
  authRoles?: any[];
  data?: any;
}
export interface ISideMenuConfig {
  id: number;
  label: string;
  hidden?: any[];
  icon?: string;
  items: ISideMenuConfigItem[];
}

export const SideMenuConfig: ISideMenuConfig[] = [
  {
    id: 1,
    label: `Данни`,
    items: [
      {
        label: `Данни`,
        routerLink: [`/reports`]
      }
    ]
  },
  {
    id: 2,
    label: `Моите справки`,
    items: [
      {
        label: `Моите справки`,
        routerLink: [`/saved-reports`]
      }
    ]
  },
  {
    id: 3,
    label: `Споделени справки`,
    items: [
      {
        label: `Споделени справки`,
        routerLink: [`/shared-reports`]
      }
    ]
  }
];
