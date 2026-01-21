const buttons = [
  {
    text: 'Изход',
    type: 'cancel',
    secondary: true,
    classes: 'left'
  },
  {
    text: '<span><i class="bi bi-chevron-left"></i>Назад</span>',
    type: 'back',
    classes: 'btn btn-primary'
  },
  {
    text: '<span>Продължи<i class="bi bi-chevron-right"></i></span>',
    type: 'next'
  }
];

export const steps = [
  {
    title: 'Добре дошли',
    text: 'От таблото за управление можете да изберете категория и към нея ще се визуализират съответните модули.',
    attachTo: {
      element: '.sidebar-step',
      on: screen.width > 620 ? 'right' : 'bottom'
    },
    buttons: [
      {
        text: 'Изход',
        type: 'cancel',
        secondary: true,
        classes: 'left'
      },
      {
        text: '<span>Продължи<i class="bi bi-chevron-right"></i></span>',
        type: 'next'
      }
    ]
  },
  {
    text: 'Натиснете върху реда "Към модула", за да бъдетете пренасочени в нов подпрозорец към сътоветния модул.',
    attachTo: {
      element: '.module-link',
      on: 'bottom'
    },
    buttons: buttons
  },
  {
    text: 'Тук можете да изтеглите ръководствата за работа с модулите на НЕИСПУО.',
    classes: 'bottom-message-step',
    attachTo: {
      element: '.user-guides-menu-btn',
      on: 'bottom'
    },
    buttons: buttons
  },
  {
    text: 'Достъп до категориите и модулите имате и от контекстното меню.',
    classes: 'bottom-message-step',
    attachTo: {
      element: '.dashboard-btn',
      on: 'bottom'
    },
    buttons: buttons
  },
  {
    text: 'Тук може да ги прегледате детайли за вашия профил или да излезете от системата на НЕИСПУО.',
    classes: 'bottom-message-step',
    attachTo: {
      element: '.dropdown',
      on: 'bottom'
    },
    buttons: [
      {
        text: '<span><i class="bi bi-chevron-left"></i>Назад</span>',
        type: 'back'
      },
      {
        text: 'Край',
        type: 'next'
      }
    ]
  }
];
