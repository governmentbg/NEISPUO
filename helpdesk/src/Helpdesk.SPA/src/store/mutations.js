export default {
  setUser: (state, user) => {
    state['user'] = user;
  },
  setUserDetails: (state, userDetails) => {
    state['userDetails'] = userDetails;
  },
  updateGridOptions: (state, { options, refKey }) => {
    state['gridOptions'][refKey] = options;
  },
  updateIssuesFilter: (state, options) => {
    state['issuesFilter'] = options;
  },
  resetOptions: (state, refKey) => {
    state['gridOptions'][refKey] = {};
    state['issuesFilter'] = {};
  },
};
