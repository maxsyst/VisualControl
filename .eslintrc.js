module.exports = {
  env: {
    browser: true,
    es6: true,
    node: true,
  },
  parserOptions: {
    parser: 'babel-eslint',
  },
  // https://github.com/feross/standard/blob/master/RULES.md#javascript-standard-style
  extends: ['airbnb', 'plugin:vue/strongly-recommended'],
  // We could also use the https://github.com/vuejs/eslint-plugin-vue
  // required to lint *.vue files
  // add your custom rules here
  rules: {
  },
};
