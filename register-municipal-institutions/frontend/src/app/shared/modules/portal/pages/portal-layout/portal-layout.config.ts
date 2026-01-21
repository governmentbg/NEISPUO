import { RoleEnum } from '@core/authentication/models/role.enum';
import { CONSTANTS } from '@shared/contstants';

export interface ISideMenuConfigItem {
  label: string;
  routerLink: string[];
  authRoles?: any[];
  data?: any;
  class?: string;
}
export interface ISideMenuConfig {
  id: number,
  label: string;
  show?: any[];
  icon?: string;
  items?: ISideMenuConfigItem[];
  routerLink?: string[];
}
export interface authRoles {
  isMon: string,
  isMunicipality: string;
  isRio?: any[];
  icon?: string;
  items?: ISideMenuConfigItem[];
  routerLink?: string[];
}

export const SideMenuConfig: ISideMenuConfig[] = [
  {
    id: 1,
    label: `${CONSTANTS.SIDEMENU_TITLE_MUNICIPAL_INSTITUTIONS
    }`,
    show: [RoleEnum.MON, RoleEnum.MUNICIPALITY, RoleEnum.RUO],
    items: [
      {
        label: `${CONSTANTS.SIDEMENU_TITLE_EXISTING_INSTITUTIONS
        }`,
        routerLink: [`${CONSTANTS.ROUTER_PATH_EXISTING_INSTITUTIONS}`],
        authRoles: [RoleEnum.MON, RoleEnum.MUNICIPALITY, RoleEnum.RUO],
        class: 'active-institutions',

      },
      {
        label: `${CONSTANTS.SIDEMENU_TITLE_CLOSED_INSTITUTIONS
        }`,
        routerLink: [`${CONSTANTS.ROUTER_PATH_CLOSED_INSTITUTIONS}`],
        authRoles: [RoleEnum.MON, RoleEnum.MUNICIPALITY, RoleEnum.RUO],
        class: 'closed-institutions',
      },
      {
        label: `${CONSTANTS.SIDEMENU_TITLE_CREATE_INSTITUTION
        }`,
        routerLink: [`${CONSTANTS.ROUTER_PATH_CREATE_INSTITUTION}`],
        authRoles: [RoleEnum.MUNICIPALITY],
        class: 'create-new-institution',
      },
      {
        label: `${CONSTANTS.SIDEMENU_TITLE_TRANSFORM_INSTITUTION
        }`,
        routerLink: [`${CONSTANTS.ROUTER_PATH_TRANSFORM_INSTITUTION}`],
        authRoles: [RoleEnum.MUNICIPALITY],
        class: 'procedures',
      },
    ],

  },
  {
    id: 2,
    label: `${CONSTANTS.SIDEMENU_TITLE_FLEX_FIELDS}`,
    show: [RoleEnum.MUNICIPALITY],
    items: [
      {
        label: `${CONSTANTS.SIDEMENU_TITLE_FLEX_FIELDS_LIST}`,
        routerLink: [`${CONSTANTS.ROUTER_PATH_FLEX_FIELDS_LIST}`],
        authRoles: [RoleEnum.MUNICIPALITY],
        class: 'flex-fields-list',
      },
      {
        label: `${CONSTANTS.SIDEMENU_TITLE_FLEX_FIELDS_CREATE}`,
        routerLink: [`${CONSTANTS.ROUTER_PATH_FLEX_FIELDS_CREATE}`],
        authRoles: [RoleEnum.MUNICIPALITY],
        class: 'create-new-flex-field',
      },
    ],

  },
  {
    id: 3,
    label: `${CONSTANTS.SIDEMENU_TITLE_PUBLIC_REGISTER}`,
    show: [RoleEnum.MON, RoleEnum.MUNICIPALITY, RoleEnum.RUO],
    routerLink: [`${CONSTANTS.ROUTER_PATH_PUBLIC_REGISTER_LIST}`],
  },
];
