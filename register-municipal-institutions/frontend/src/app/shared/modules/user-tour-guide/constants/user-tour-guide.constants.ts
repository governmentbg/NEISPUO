export interface GuideStepButton {
  text: string;
  type: 'cancel' | 'next' | 'back';
  secondary?: boolean;
  classes?: string;
}

const cancelButton: GuideStepButton = {
  text: 'Изход',
  type: 'cancel',
  secondary: true,
  classes: 'p-button p-button-outlined p-button-primary',
};

const backButton: GuideStepButton = {
  text: '<span><i class="bi bi-chevron-left" style="vertical-align:middle; height:auto; width:auto; font-size:10px; margin-right:5px;"></i>Назад</span>',

  type: 'back',
  classes: 'p-button',
};

const forwardButton: GuideStepButton = {
  text: '<span>Продължи<i class="bi bi-chevron-right" style="vertical-align:middle; height:auto; width:auto; font-size:10px; margin-left:5px;"></i></span>',
  type: 'next',
  classes: 'p-button',
};

const endButton: GuideStepButton = {
  text: 'Край',
  type: 'next',
  classes: 'p-button p-button-outlined p-button-primary',
};

const guideStepButtons: GuideStepButton[] = [
  cancelButton,
  backButton,
  forwardButton,
];

interface GuideStep {
  text: string;
  attachTo: {
    element: string;
    on: 'left' | 'top' | 'right' | 'bottom';
  };
  buttons: GuideStepButton[];
  classes?: string;
}

export type GuideSteps = GuideStep[];

export const checkBulstatSteps: GuideSteps = [
  {

    text: 'Тук можете да проверите по "Булстат/ЕИК", дали дадената институция е регистрирана в "Търговския регистър". Ако е регистрирана данните за нея ще се попълнят автоматично.',
    attachTo: {
      element: '.check-institution-bulstat',
      on: 'left',
    },
    buttons: [
      cancelButton,
      forwardButton,
    ],
  },
  {
    text: 'Натиснете върху "Напред", за да въведете ръчно данните за институцията.',
    attachTo: {
      element: '.my-primary-forward',
      on: 'top',
    },
    buttons: [
      backButton,
      endButton,
    ],
  },
];

export const createInstitutionSteps: GuideSteps = [
  {

    text: 'Въведете информация относно името на институцията и "Булстат/ЕИК" кода.',
    attachTo: {
      element: '.general-information',
      on: 'top',
    },
    buttons: [
      cancelButton,
      forwardButton,
    ],
  },
  {
    text: 'Въведете информация относно седалището и адреса на управление на институцията.',
    attachTo: {
      element: '.institution-address',
      on: 'top',
    },
    buttons: guideStepButtons,
  },
  {
    text: 'Въведете информация относно Директора на институцията.',

    attachTo: {
      element: '.institution-headmaster',
      on: 'top',
    },
    buttons: guideStepButtons,
  },
  {
    text: 'Въведете информация относно вида според подготовка и детайлния вид на институцията.',

    attachTo: {
      element: '.institution-type',
      on: 'top',
    },
    buttons: guideStepButtons,
  },
  {
    text: 'Въведете информация относно данните за дадената процедура и прикачете необходимите файлове.',

    attachTo: {
      element: '.institution-procedure-data',
      on: 'top',
    },
    buttons: guideStepButtons,
  },
  {
    text: 'Добавете флекс полета, ако е необходимо.',

    attachTo: {
      element: '.institution-flex-field',
      on: 'top',
    },
    buttons: guideStepButtons,
  },
  {
    text: 'Натиснете бутона "Създай", за да откриете успешно институцията.',
    classes: 'bottom-message-step',
    attachTo: {
      element: '.my-primary-submit',
      on: 'bottom',
    },
    buttons: [
      backButton,
      endButton,

    ],
  },
];

export const previewInstitutionSteps: GuideSteps = [
  {
    text: 'При натискане на иконката, можете да видите всички налични версии на Институцията.',
    attachTo: {
      element: '.history-icon',
      on: 'top',
    },
    buttons: [
      cancelButton,
      forwardButton,
    ],
  },
  {
    text: 'Информация относно името на институцията и "Булстат/ЕИК" кода.',
    attachTo: {
      element: '.general-information',
      on: 'top',
    },
    buttons: guideStepButtons,
  },
  {
    text: 'Информация относно седалището и адреса на управление на институцията.',
    attachTo: {
      element: '.institution-address',
      on: 'top',
    },
    buttons: guideStepButtons,
  },
  {
    text: 'Информация относно Директора на институцията.',

    attachTo: {
      element: '.institution-headmaster',
      on: 'top',
    },
    buttons: guideStepButtons,
  },
  {
    text: 'Информация относно вида според подготовка и детайлния вид на институцията.',

    attachTo: {
      element: '.institution-type',
      on: 'top',
    },
    buttons: guideStepButtons,
  },
  {
    text: 'Информация относно данните за дадената процедура и прикачете необходимите файлове.',

    attachTo: {
      element: '.institution-procedure-data',
      on: 'top',
    },
    buttons: guideStepButtons,
  },
  {
    text: 'При натискане на бутона "Промяна", вие можете да редактирате Институцията.',
    classes: 'bottom-message-step',
    attachTo: {
      element: '.my-primary-edit',
      on: 'bottom',
    },
    buttons: guideStepButtons,
  },
  {
    text: 'При натискане на бутона "Закриване", вие можете да закриете Институцията.',
    classes: 'bottom-message-step',
    attachTo: {
      element: '.my-primary-delete',
      on: 'bottom',
    },
    buttons: [
      backButton,
      endButton,

    ],
  },
];

export const dashboardMenuSteps: GuideSteps = [
  {
    text: 'Съдържа списък с всички действащи институции в съответния район.',
    attachTo: {
      element: '.active-institutions',
      on: 'right',
    },
    buttons: [
      cancelButton,
      forwardButton,
    ],
  },
  {
    text: 'Съдържа списък с всички закрити институции в съответния район.',
    attachTo: {
      element: '.closed-institutions',
      on: 'right',
    },
    buttons: guideStepButtons,
  },
  {
    text: 'Процедура по създаване на нова институция.',

    attachTo: {
      element: '.create-new-institution',
      on: 'right',
    },
    buttons: guideStepButtons,
  },
  {
    text: 'Съдържа списък с процедурите по преобразуване.',

    attachTo: {
      element: '.procedures',
      on: 'right',
    },
    buttons: guideStepButtons,
  },
  {
    text: 'Съдържа списък със създадените флекс полета.',

    attachTo: {
      element: '.flex-fields-list',
      on: 'right',
    },
    buttons: guideStepButtons,
  },
  {
    text: 'Процедура по създаване на нови флекс полета.',

    attachTo: {
      element: '.create-new-flex-field',
      on: 'right',
    },
    buttons: guideStepButtons,
  },
  {
    text: 'Съдържа списък с публичните общински партиди.',

    attachTo: {
      element: '.public-register',
      on: 'right',
    },
    buttons: [
      backButton,
      endButton,

    ],
  },
];

export const deleteInstitutionSteps: GuideSteps = [
  {

    text: 'Информация относно името на институцията и "Булстат/ЕИК" кода.',
    attachTo: {
      element: '.general-information',
      on: 'top',
    },
    buttons: [
      cancelButton,
      forwardButton,
    ],
  },
  {
    text: 'Информация относно седалището и адреса на управление на институцията.',
    attachTo: {
      element: '.institution-address',
      on: 'top',
    },
    buttons: guideStepButtons,
  },
  {
    text: 'Информация относно Директора на институцията.',

    attachTo: {
      element: '.institution-headmaster',
      on: 'top',
    },
    buttons: guideStepButtons,
  },
  {
    text: 'Информация относно вида според подготовка и детайлния вид на институцията.',

    attachTo: {
      element: '.institution-type',
      on: 'top',
    },
    buttons: guideStepButtons,
  },
  {
    text: 'Въведете информация относно данните за дадената процедура и прикачете необходимите файлове.',

    attachTo: {
      element: '.institution-procedure-data',
      on: 'top',
    },
    buttons: guideStepButtons,
  },
  {
    text: 'При натискане на бутона "Закриване", вие можете да закриете Институцията.',

    attachTo: {
      element: '.my-primary-delete',
      on: 'top',
    },
    buttons: [
      backButton,
      endButton],
  },
];

export const procedureChoiceSteps: GuideSteps = [
  {

    text: 'Процедурата позволява избраните от вас институции да бъдат закрити и да се "влеят" в избрана институция.',
    attachTo: {
      element: '.join-procedure',
      on: 'right',
    },
    buttons: [
      cancelButton,
      forwardButton,
    ],
  },
  {

    text: 'Процедурата позволява избраните от вас институции да бъдат закрити и да се "слеят" в нова институция.',
    attachTo: {
      element: '.merge-procedure',
      on: 'right',
    },
    buttons: guideStepButtons,
  },
  {

    text: 'Процедурата позволява избраните от вас институция да бъде закрити, а нейно място ще бъдат създадени нови.',
    attachTo: {
      element: '.divide-procedure',
      on: 'right',
    },
    buttons: guideStepButtons,
  },
  {

    text: 'Процедурата позволява избраните от вас избраната институция да бъде актуализирана и да бъдат създадени нови.',
    attachTo: {
      element: '.detach-procedure',
      on: 'right',
    },
    buttons: [
      backButton,
      endButton,
    ],
  },
];

export enum GuideName {
  BULSTAT_CHECK,
  CREATE_INSTITUTION,
  PROCEDURE_CHOICE,
  DASHBOARD_MENU,
  PREVIEW_INSTITUTION,
  DELETE_INSTITUTION,
}

export const GUIDE_STEPS_BY_NAME: Record<GuideName, GuideSteps> = {
  [GuideName.BULSTAT_CHECK]: checkBulstatSteps,
  [GuideName.CREATE_INSTITUTION]: createInstitutionSteps,
  [GuideName.PROCEDURE_CHOICE]: procedureChoiceSteps,
  [GuideName.DELETE_INSTITUTION]: deleteInstitutionSteps,
  [GuideName.DASHBOARD_MENU]: dashboardMenuSteps,
  [GuideName.PREVIEW_INSTITUTION]: previewInstitutionSteps,
};
