import { InstType, UserRole } from '@/enums/enums';

import Constants from '@/common/constants';
import Helper from '../components/helper';
import { config } from '@/common/config';

export default {
  getAllStudents: (state) => {
    return state.students;
  },
  user: (state) => {
    return state.user;
  },
  userDetails: (state) => {
    return state.userDetails;
  },
  userSelectedRole: (state) => {
    return !!state.user && !!state.user.profile ? state.user.profile.selected_role : null;
  },
  userRoles: (state) => {
    return !!state.user && !!state.user.profile && !!state.user.profile.roles ? state.user.profile.roles : [];
  },
  userInstitutionId: (state) => {
    return !!state.user && !!state.user.profile & !!state.user.profile.selected_role
      ? state.user.profile.selected_role.InstitutionID
      : null;
  },
  userRegionId: (state) => {
    return !!state.user && !!state.user.profile & !!state.user.profile.selected_role
      ? state.user.profile.selected_role.RegionID
      : null;
  },
  language: (state) => state.language,
  gridItemsPerPageOptions: (state) => state.gridItemsPerPageOptions,
  isAuthenticated: (state) => state.user && !state.user.expired,
  loggedUserUsername: (state) => state.user && state.user.profile ? state.user.profile.sub : '',
  loggedUserFullName: (state) => state.user && state.user.profile
    ? `${state.user.profile.FirstName ?? ''} ${state.user.profile.LastName ?? ''}`.trim()
    : '',
  loggedUserAvatarText: (state) => {
    const username = state.user && state.user.profile ? state.user.profile.sub : '';
    const fullname = state.user && state.user.profile
      ? `${state.user.profile.FirstName ?? ''} ${state.user.profile.LastName ?? ''}`
      : '';
    if (fullname.trim()) {
      return Helper.getAvatarText(fullname.trim());
    }
    return Helper.getAvatarText(username.trim());
  },
  permissions: (state) => state.permissions,
  permissionsForStudent: (state) => state.permissionsForStudent,
  permissionsForInstitution: (state) => state.permissionsForInstitution,
  permissionsForClass: (state) => state.permissionsForClass,
  appMenu: (state) => state.appMenu,
  studentSearchModel: (state) => state.studentSearchModel,
  currentStudentSummary: (state) => state.currentStudentSummary,
  isInRole: (state) => (role) => {
    if (!state.user || !state.user.profile || !state.user.profile.selected_role) {
      return false;
    }

    if (isNaN(role)) {
      // Ролята идва като текст
      const sysRoleID = UserRole[role];
      return state.user.profile.selected_role.SysRoleID === sysRoleID;

    } else {
      // Ролята идва като число
      return state.user.profile.selected_role.SysRoleID === role;
    }
  },
  isInstType: (state) => (instType) => {
    if (!state.userDetails) {
      return false;
    }

    if (isNaN(instType)) {
      // InstType идва като текст
      const instTypeId = InstType[instType];
      return state.userDetails.instTypeId === instTypeId;

    } else {
      // InstType идва като число
      return state.userDetails.instTypeId === instType;
    }
  },
  egnAnonymization: () => config.egnAnonymization,
  mode: () => config.mode,
  demandingPermission: (state) => state.demandingPermission,
  selectedStudentClass: (state) => state.selectedStudentClass,
  allContextualInfo: (state) => state.contextualInfo,
  contextualInformation: (state) => (key) => {
    return state.contextualInfo ? state.contextualInfo[key] : '';
  },
  dynamicEntitiesSchema: (state) => state.dynamicEntitiesSchema,
  termOptions: (state) => state.termOptions,
  isDebug: () => true,
  hasPermission: (state) => (permission) => {
    return state.permissions && state.permissions.includes(permission);
  },
  hasStudentPermission: (state) => (permission) => {
    return state.permissionsForStudent && state.permissionsForStudent.includes(permission);
  },
  hasInstitutionPermission: (state) => (permission) => {
    return state.permissionsForInstitution && state.permissionsForInstitution.includes(permission);
  },
  hasClassPermission: (state) => (permission) => {
    return state.permissionsForClass && state.permissionsForClass.includes(permission);
  },
  manageContextualInformation: (statet) => statet.manageContextualInformation,
  studentFinalizedLods: (state) => state.studentFinalizedLods,
  isLodFinalized: (state) => (schoolYear) => {
    return state.studentFinalizedLods && schoolYear
      && state.studentFinalizedLods.includes(schoolYear);
  },
  getPersonMessagesCount: (state) => {
    return state.messagesCount;
  },
  apiErrors: (state) => {
    return state.apiErros;
  },
  isInStudentsModuleRole: (state) => {
    if (!state.user || !state.user.profile || !state.user.profile.selected_role) {
      return false;
    }

    return Constants.STUDENTS_MODULE_ALLOWED_ROLES.includes(state.user.profile.selected_role.SysRoleID);
  },
  turnOnRefugeeModule: () => {
    return true;
  },
  diplomaListSelectedTab: (state) => {
    return state.diplomaListSelectedTab;
  },
  turnOnOresModule: () => {
    return true;
  },
  dualEduFormId: (state) => {
    return state.dualEduFormId;
  },
  dualClassTypeId: (state) => {
    return state.dualClassTypeId;
  },
  currency: (state) => {
    return state.currency;
  },
  isDevelopment: () => config.mode === 'development',
  personalDevelopmentSuppert_v2: () => true,
};
