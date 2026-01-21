export default {
  appendStudents: (state, payload) => {
    state['students'] = payload;
  },
  setUser: (state, user) => {
    state['user'] = user;
  },
  setUserDetails: (state, userDetails) => {
    state['userDetails'] = userDetails;
  },
  setPermissions: (state, permissions) => {
    state['permissions'] = permissions ?? [];
  },
  clearPermissions: (state) => {
    state['permissions'] = [];
  },
  clearPermissionsForStudent: (state) => {
    state['permissionsForStudent'] = [];
  },
  setPermissionsForStudent: (state, permissions) => {
    state['permissionsForStudent'] = permissions ?? [];
  },
  clearPermissionsForInstitution: (state) => {
    state['permissionsForInstitution'] = [];
  },
  setPermissionsForInstitution: (state, permissions) => {
    state['permissionsForInstitution'] = permissions ?? [];
  },
  clearPermissionsForClass: (state) => {
    state['permissionsForClass'] = [];
  },
  setPermissionsForClass: (state, permissions) => {
    state['permissionsForClass'] = permissions ?? [];
  },
  setStudentFinalizedLods: (state, lods) => {
    state['studentFinalizedLods'] = lods ?? [];
  },
  clearStudentFinalizedLods: (state) => {
    state['studentFinalizedLods'] = [];
  },
  setPersonMessagesCount: (state, messagesCount) => {
    state['messagesCount'] = messagesCount ?? 0;
  },
  updatePersonMessagesCount: (state, messagesCount) => {
    state['messagesCount'] = messagesCount;
  },
  hideMainMenu: (state) => {
    state.appMenu.showMainMenu = false;
  },
  showMainMenu: (state) => {
    state.appMenu.showMainMenu = true;
  },
  setStudentSearchModel: (state, model) => {
    state['studentSearchModel'] = model;
  },
  claerStudentSearchModel: (state) => {
    state['studentSearchModel'] = null;
  },
  setCurrentStudentSummary: (state, model) => {
    state['currentStudentSummary'] = model;
  },
  clearCurrentStudentSummary: (state) => {
    state['currentStudentSummary'] = null;
  },
  demandingPermission: (state, val) => {
    state['demandingPermission'] = val;
  },
  setSelectedStudentClass: (state, selectedClass) => {
    state['selectedStudentClass'] = selectedClass;
  },
  setContextualInfo: (state, contextualInfo) => {
    state['contextualInfo'] = contextualInfo;
  },
  setDynamicEntitiesSchema: (state, dynamicEntitiesSchema) => {
    state['dynamicEntitiesSchema'] = dynamicEntitiesSchema;
  },
  setManageContextualInformation: (state, val) => {
    state['manageContextualInformation'] = val;
  },
  addApiError: (state, error) => {
    state['apiErros'].unshift(error);
  },
  setGridState: (state, { key, gridState }) => {
    state.gridStates[key] = gridState;
  },
  setDiplomaListSelectedTab: (state, selectedTab) => {
    state['diplomaListSelectedTab'] = selectedTab;
  },
  updateGridOptions: (state, { options, refKey }) => {
    state['gridOptions'][refKey] = options;
  },
  updateGridFilter: (state, { options, refKey }) => {
    state['gridFilters'][refKey] = options;
  },
  setCurrency: (state, currency) => {
    state['currency'] = currency;
  },
};
