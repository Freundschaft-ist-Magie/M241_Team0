import stylistic from '@stylistic/eslint-plugin'
import vue from 'eslint-plugin-vue'
import typescript from '@typescript-eslint/eslint-plugin'

/** @type {import('eslint').Linter.FlatConfig[]} */
export default [
  {
    files: [
      '**/*.ts',
      '**/*.vue',
    ],
    plugins: {
      '@style': stylistic,
      vue,
      '@typescript-eslint': typescript,
    },
    languageOptions: {
      parser: '@typescript-eslint/parser',
      parserOptions: {
        ecmaVersion: 2020,
        sourceType: 'module',
        ecmaFeatures: {
          jsx: false, // Set to false for Vue
        },
      },
    },
    rules: {
      /* warnings */

      'no-await-in-loop': 'warn',
      'no-unused-vars': 'warn',
      'object-shorthand': 'warn',
      'prefer-arrow-callback': 'warn',
      'prefer-const': 'warn',
      'prefer-destructuring': 'warn',
      'prefer-object-spread': 'warn',
      'prefer-spread': 'warn',
      'yoda': [ 'warn', 'always' ],

      /* errors */

      'array-callback-return': 'error',
      'getter-return': 'error',
      'no-class-assign': 'error',
      'no-setter-return': 'error',

      /* stylistic */

      '@style/quotes': [ 'warn', 'single' ],
      '@style/semi': [ 'warn', 'never' ],

      '@style/array-bracket-spacing': [ 'warn', 'always' ],
      '@style/arrow-spacing': 'warn',
      '@style/block-spacing': 'warn',
      '@style/comma-spacing': 'warn',
      '@style/function-call-spacing': 'warn',
      '@style/generator-star-spacing': 'warn',
      '@style/key-spacing': 'warn',
      '@style/keyword-spacing': 'warn',
      '@style/no-multi-spaces': 'warn',
      '@style/no-trailing-spaces': 'warn',
      '@style/no-whitespace-before-property': 'warn',
      '@style/object-curly-spacing': [ 'warn', 'always' ],
      '@style/rest-spread-spacing': [ 'warn', 'never' ],
      '@style/semi-spacing': 'warn',
      '@style/space-before-blocks': 'warn',
      '@style/space-before-function-paren': [ 'warn', 'never' ],
      '@style/space-in-parens': 'warn',
      '@style/space-infix-ops': 'warn',
      '@style/space-unary-ops': 'warn',
      '@style/spaced-comment': 'warn',
      '@style/switch-colon-spacing': 'warn',
      '@style/template-curly-spacing': [ 'warn', 'always' ],
      '@style/template-tag-spacing': 'warn',
      '@style/yield-star-spacing': 'warn',

      '@style/brace-style': 'warn',
      '@style/comma-dangle': [ 'warn', 'always-multiline' ],

      /* TypeScript specific */

      '@typescript-eslint/no-unused-vars': 'warn',
      '@typescript-eslint/explicit-module-boundary-types': 'off', // Customize this rule as needed
    },
  },
  {
    files: [
      'src/components/**/*.vue',
      'src/pages/**/*.vue',
    ],
    plugins: {
      vue,
    },
    rules: {
      'vue/jsx-uses-vars': 'warn',
      'vue/no-unused-vars': 'warn',
      'vue/require-prop-types': 'warn',
    },
  },
]
