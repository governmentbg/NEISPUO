import userService from '@/services/user.service.js';

export default {
  setUser: async ({ commit }, user) => {
    commit('setUser', user);

    const userDetails = (await userService.getUserInfo()).data;
    commit('setUserDetails', userDetails);
  },
};
