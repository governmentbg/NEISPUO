import { RoleEnum } from '@shared/enums/roles.enum';
import { CONSTANTS } from '../shared/constants';

export interface ISideMenuConfigItem {
    label: string;
    routerLink: string[];
    authRoles: RoleEnum[];
    data?: any;
}
export interface ISideMenuConfig {
    id: number;
    label: string;
    hidden: RoleEnum[];
    items: ISideMenuConfigItem[];
}

export const SideMenuConfig: ISideMenuConfig[] = [
    {
        id: 1,
        label: `${CONSTANTS.SIDEMENU_SECTION_TITLE_USERS_NEISPUO}`,
        // TODO: add all roles for which the menu is hidden so it does not load at all
        hidden: [RoleEnum.BUDGETING_INSTITUTION],
        items: [
            {
                label: `${CONSTANTS.SIDEMENU_TAB_TITLE_TEACHER_CLASSES}`,
                routerLink: [`/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_TEACHER_CLASSES}`],
                authRoles: [RoleEnum.TEACHER],
                data: { isLeadTeacher: true },
            },
            {
                label: `${CONSTANTS.SIDEMENU_TAB_TITLE_INSTITUTIONS}`,
                routerLink: [`/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_INSTITUTIONS}`],
                authRoles: [
                    RoleEnum.INSTITUTION,
                    RoleEnum.MON_ADMIN,
                    RoleEnum.CIOO,
                    RoleEnum.RUO,
                    RoleEnum.CONSORTIUM_HELPDESK,
                ],
            },
            {
                label: `${CONSTANTS.SIDEMENU_TAB_TITLE_TEACHERS}`,
                routerLink: [`/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_TEACHERS}`],
                authRoles: [
                    RoleEnum.INSTITUTION,
                    RoleEnum.MON_ADMIN,
                    RoleEnum.CIOO,
                    RoleEnum.RUO,
                    RoleEnum.CONSORTIUM_HELPDESK,
                ],
            },
            {
                label: `${CONSTANTS.SIDEMENU_TAB_TITLE_STUDENTS}`,
                routerLink: [`/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_STUDENTS}`],
                authRoles: [
                    RoleEnum.INSTITUTION,
                    RoleEnum.MON_ADMIN,
                    RoleEnum.CIOO,
                    RoleEnum.RUO,
                    RoleEnum.CONSORTIUM_HELPDESK,
                ],
            },
            {
                label: `${CONSTANTS.SIDEMENU_TAB_TITLE_PARENTS}`,
                routerLink: [`/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_PARENTS}`],
                authRoles: [
                    RoleEnum.INSTITUTION,
                    RoleEnum.MON_ADMIN,
                    RoleEnum.CIOO,
                    RoleEnum.RUO,
                    RoleEnum.CONSORTIUM_HELPDESK,
                ],
            },
            {
                label: `${CONSTANTS.SIDEMENU_TAB_TITLE_ROLE_AUDIT}`,
                routerLink: [
                    `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_AUDIT}/${CONSTANTS.ROUTER_PATH_ROLE_AUDIT}`,
                ],
                authRoles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK],
            },
            {
                label: `${CONSTANTS.SIDEMENU_TAB_TITLE_LOGIN_AUDIT}`,
                routerLink: [
                    `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_AUDIT}/${CONSTANTS.ROUTER_PATH_LOGIN_AUDIT}`,
                ],
                authRoles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK],
            },
        ],
    },
    {
        id: 2,
        label: `${CONSTANTS.SIDEMENU_SECTION_TITLE_USERS_EXTERNAL}`,
        // TODO: add all roles for which the menu is hidden so it does not load at all
        hidden: [RoleEnum.INSTITUTION, RoleEnum.BUDGETING_INSTITUTION, RoleEnum.TEACHER],
        items: [
            {
                label: `${CONSTANTS.SIDEMENU_TAB_TITLE_MON}`,
                routerLink: [`/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_MON}`],
                authRoles: [RoleEnum.MON_ADMIN, RoleEnum.CONSORTIUM_HELPDESK],
            },
            {
                label: `${CONSTANTS.SIDEMENU_TAB_TITLE_RUO}`,
                routerLink: [`/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_RUO}`],
                authRoles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.RUO, RoleEnum.CONSORTIUM_HELPDESK],
            },
            {
                label: `${CONSTANTS.SIDEMENU_TAB_TITLE_MUNICIPALITIES}`,
                routerLink: [`/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_MUNICIPALITIES}`],
                authRoles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.RUO, RoleEnum.CONSORTIUM_HELPDESK],
            },
            {
                label: `${CONSTANTS.SIDEMENU_TAB_TITLE_BUDGET_INSTITUTIONS}`,
                routerLink: [`/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_BUDGET_INSTITUTIONS}`],
                authRoles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK],
            },
            {
                label: `${CONSTANTS.SIDEMENU_TAB_TITLE_OTHER_USERS}`,
                routerLink: [`/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_OTHER_USERS}`],
                authRoles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK],
            },
        ],
    },
    {
        id: 3,
        label: `${CONSTANTS.SIDEMENU_SECTION_TITLE_USERS_SYNCRONIZATION}`,
        hidden: [RoleEnum.INSTITUTION, RoleEnum.RUO, RoleEnum.BUDGETING_INSTITUTION, RoleEnum.TEACHER],
        items: [
            {
                label: `${CONSTANTS.SIDEMENU_TAB_TITLE_USERS}`,
                routerLink: [`/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_USERS}`],
                authRoles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK],
            },
            {
                label: `${CONSTANTS.SIDEMENU_TAB_TITLE_CLASSES}`,
                routerLink: [`/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_CLASSES}`],
                authRoles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK],
            },
            {
                label: `${CONSTANTS.SIDEMENU_TAB_TITLE_ORGANIZATIONS}`,
                routerLink: [`/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_ORGANIZATIONS}`],
                authRoles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK],
            },
            {
                label: `${CONSTANTS.SIDEMENU_TAB_TITLE_ENROLLMENTS}`,
                routerLink: [`/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_ENROLLMENTS}`],
                authRoles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK],
            },
        ],
    },
    {
        id: 4,
        label: `${CONSTANTS.SIDEMENU_SECTION_TITLE_USERS_ADMIN}`,
        hidden: [
            RoleEnum.INSTITUTION,
            RoleEnum.RUO,
            RoleEnum.MUNICIPALITY,
            RoleEnum.BUDGETING_INSTITUTION,
            RoleEnum.TEACHER,
            RoleEnum.STUDENT,
            RoleEnum.PARENT,
            RoleEnum.CLASS_TEACHER,
            RoleEnum.RUO_EXPERT,
            RoleEnum.EXTERNAL_INSTITUTIONS_EXPERT,
            RoleEnum.MON_EXPERT,
            RoleEnum.MON_USER_ADMIN,
            RoleEnum.TECHNICAL_INSTITUTION,
            RoleEnum.MON_OBGUM,
            RoleEnum.MON_OBGUM_FINANCES,
            RoleEnum.MON_CHRAO,
            RoleEnum.CONSORTIUM_HELPDESK,
            RoleEnum.NIO,
            RoleEnum.ACCOUNTANT,
        ],
        items: [
            {
                label: `${CONSTANTS.SIDEMENU_TAB_TITLE_ERRORS}`,
                routerLink: [`/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_ERRORS}`],
                authRoles: [RoleEnum.MON_ADMIN],
            },
            // {
            //     label: `${CONSTANTS.SIDEMENU_TAB_TITLE_CREATE_USER}`,
            //     routerLink: [`/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_CREATE_USER}`],
            //     authRoles: [
            //         RoleEnum.MON_ADMIN,
            //         RoleEnum.CIOO,
            //         RoleEnum.RUO,
            //         RoleEnum.RUO_EXPERT,
            //         RoleEnum.CONSORTIUM_HELPDESK,
            //     ],
            // },
        ],
    },
];
