import administrationService from '@/services/administration.service';
import http from '@/services/http.service';
import userService from '@/services/user.service.js';

const studentSearchModelKey = 'studentSearchModel';

export default {
  setAllStudents: ({ commit }, payload) => {
    commit('appendStudents', payload);
  },
  setUser: async ({ commit }, user) => {
    commit('setUser', user);
    // console.log('loaded user has been set to the state');

    const permissions = await http.get('/api/authorization/GetUserPermissions');
    commit('setPermissions', permissions.data ?? []);
    // console.log('Permissions has been set to the state');

    const userDetails = (await userService.getUserInfo()).data;
    commit('setUserDetails', userDetails);
  },
  clearPermissions: ({ commit }) => {
    commit('clearPermissions');
  },
  setPermissions: ({ commit }, permissions) => {
    commit('setPermissions', permissions);
  },
  clearPermissionsForStudent: ({ commit }) => {
    commit('clearPermissionsForStudent');
  },
  loadPermissionsForStudent: async ({ commit }, personId) => {
    const permissions = await http.get(`/api/authorization/GetUserPermissionsForStudent?personId=${personId}`);
    commit('setPermissionsForStudent', permissions.data ?? []);
  },
  clearPermissionsForInstitution: ({ commit }) => {
    commit('clearPermissionsForInstitution');
  },
  loadPermissionsForInstitution: async ({ commit }, institutionId) => {
    const permissions = await http.get(`/api/authorization/GetUserPermissionsForInstitution?institutionId=${institutionId}`);
    commit('setPermissionsForInstitution', permissions.data ?? []);
  },
  loadPermissionsForInstitutionForLoggedUser: async ({ commit }) => {
    const permissions = await http.get(`/api/authorization/GetUserPermissionsForInstitutionForLoggedUser`);
    commit('setPermissionsForInstitution', permissions.data ?? []);
  },
  clearPermissionsForClass: ({ commit }) => {
    commit('clearPermissionsForClass');
  },
  loadPermissionsForClass: async ({ commit }, classId) => {
    const permissions = await http.get(`/api/authorization/GetUserPermissionsForClass?classId=${classId}`);
    commit('setPermissionsForClass', permissions.data ?? []);
  },
  loadStudentFinalizedLods: async({ commit }, personId) => {
    const lods = await http.get(`/api/lodFinalization/GetStudentFinalizedLods?personId=${personId}`);
    commit('setStudentFinalizedLods', lods.data ?? []);
  },
  countMyUnreadMessages: async({ commit }) => {
    const messagesCount = await http.get(`/api/message/CountMyUnreadMessages`);
    commit('setPersonMessagesCount', messagesCount.data ?? 0);
  },
  updatePersonMessagesCount: async ({commit}, count) => {
    commit('updatePersonMessagesCount', count);
  },
  clearStudentFinalizedLods: ({ commit }) => {
    commit('clearStudentFinalizedLods');
  },
  loadStudentSearchModel: ({ commit }) => {
    try {
      const sm = localStorage.getItem(studentSearchModelKey);
      if (sm) {
        const model = JSON.parse(sm);
        commit('setStudentSearchModel', model);
      }
    } catch {
      // намя localStorage
    }
  },
  setStudentSearchModel: ({ commit }, model) => {
    try {
      localStorage.setItem(studentSearchModelKey, model ? JSON.stringify(model) : '');
      commit('setStudentSearchModel', model);
    } catch {
      // намя localStorage
    }
  },
  claerStudentSearchModel: ({ commit }) => {
    try {
      localStorage.removeItem(studentSearchModelKey);
      commit('claerStudentSearchModel');
    } catch {
      // намя localStorage
    }
  },
  setCurrentStudentSummary: async ({ commit }, id) => {
    try {
      const studentSummary = await http.get(`/api/student/GetSummaryById/?id=${id}`);
      commit('setCurrentStudentSummary', studentSummary.data);
    } catch (error) {
      console.log(error);
    }
  },
  clearCurrentStudentSummary: ({ commit }) => {
    commit('clearCurrentStudentSummary');
  },
  authorize: ({ commit }, model) => {
    return new Promise((resolve, reject) => {
      commit('demandingPermission', true);

      http.post('api/authorization/Authorize', model)
      .then(resp => {
        commit('demandingPermission', false);
        resolve(resp.data);
      })
      .catch(err => {
        commit('demandingPermission', false);
        reject(err);
      });
    });
  },
  setSelectedStudentClass: ({ commit }, selectedClass) => {
    commit('setSelectedStudentClass', selectedClass);
  },
  loadContextualInfo: async ({ commit }) => {
    // console.log('Loading contextual info');
    try {
      const contextualInfo = await administrationService.getContextualInformation();
      commit('setContextualInfo', contextualInfo.data);
      // console.log('Contextual info loaded');
    } catch (error) {
      console.log(error.response);
      throw new Error(error);
    }
  },
};
