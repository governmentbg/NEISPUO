module.exports = {
  extends: ['@commitlint/config-conventional'],
  plugins: [
    {
      rules: {
        ticket: ({ subject }) => {
          const ticketRegex = /(\w+-{1}\d+)/;
          const skipCiRegexp = /(skip ci)/gi;
          return [
            ticketRegex.test(subject) || skipCiRegexp.test(subject),
            'Your subject should include the ticket, for example PRJ-300.',
          ];
        },
      },
    },
  ],
  rules: {
    ticket: [2, 'always'],
    'subject-empty': [2, 'never'],
    'subject-full-stop': [2, 'never', '.'],
    'subject-case': [0],
    'type-case': [2, 'always', 'lower-case'],
    'type-empty': [2, 'never'],
    'type-enum': [
      2,
      'always',
      [
        'build',
        'chore',
        'ci',
        'docs',
        'feat',
        'fix',
        'perf',
        'refactor',
        'revert',
        'style',
        'test',
      ],
    ],
  },
};
