import { defineConfig } from 'eslint/config';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import js from '@eslint/js';
import { FlatCompat } from '@eslint/eslintrc';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const compat = new FlatCompat({
  baseDirectory: __dirname,
  recommendedConfig: js.configs.recommended,
  allConfig: js.configs.all
});

export default defineConfig([
  {
    ignores: ['eslint.config.mjs', 'dist/**', 'node_modules/**']
  },
  {
    languageOptions: {
      ecmaVersion: 2022,
      sourceType: 'module'
    }
  },
  {
    files: ['**/*.ts'],

    extends: compat.extends('eslint:recommended', 'plugin:@typescript-eslint/recommended', 'prettier'),

    languageOptions: {
      ecmaVersion: 2022,
      sourceType: 'module',

      parserOptions: {
        createDefaultProgram: true
      }
    },

    rules: {
      quotes: [
        'error',
        'single',
        {
          avoidEscape: true
        }
      ],

      eqeqeq: [
        'error',
        'always',
        {
          null: 'never'
        }
      ],

      '@typescript-eslint/no-unused-vars': [
        'error',
        {
          args: 'none'
        }
      ],

      '@typescript-eslint/no-non-null-assertion': ['off'],
      '@typescript-eslint/no-explicit-any': ['off'],

      '@typescript-eslint/ban-ts-comment': [
        'error',
        {
          'ts-ignore': 'allow-with-description'
        }
      ]
    }
  }
]);
