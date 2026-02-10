export default {
  editMode: false,
  user: null,
  userDetails: null,
  language: localStorage.getItem('culture') || 'bg',
  gridItemsPerPageOptions: [5,10,15,50,100,-1],
  priorityColors: {
    1: 'primary',
    2: 'warning',
    3: 'error',
  },
  statusColors: {
    1: '',
    2: 'info',
    3: 'success',
  },
  statusText: {
    1: 'Нов',
    2: 'В процес на обработка',
    3: 'Приключен',
  },
  gridOptions: {
    'issuesGrid': {},
    'questionsGrid': {}
  },
  issuesFilter: {}
};
