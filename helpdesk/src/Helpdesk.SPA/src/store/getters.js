import { UserRole } from '@/enums/enums';
import Helper from '../components/helper';

export default {
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
  priorityColor: (state) => (priorityId) => {
    return state.priorityColors[priorityId];
  },
  statusColor: (state) => (statusId) => {
    return state.statusColors[statusId];
  },
  statusText: (state) => (statusId) => {
    return state.statusText[statusId];
  }
};
