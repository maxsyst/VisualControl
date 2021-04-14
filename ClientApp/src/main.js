import Vue from 'vue';
import axios from 'axios';
import moment from 'moment';
import './registerServiceWorker';
import { sync } from 'vuex-router-sync';
import lodash from 'lodash';
import Vuetify from 'vuetify';
import Lightbox from 'vue-my-photos';
import Vuelidate from 'vuelidate';
import AsyncComputed from 'vue-async-computed';
import UUID from 'vue-uuid';
import PerfectScrollbar from 'vue2-perfect-scrollbar';
import qs from 'qs';
import 'material-design-icons-iconfont/dist/material-design-icons.css';
import 'vuetify/dist/vuetify.min.css';
import 'vue2-perfect-scrollbar/dist/vue2-perfect-scrollbar.css';
import 'vue-swatches/dist/vue-swatches.min.css';
import 'moment-duration-format';
import * as am4core from '@amcharts/amcharts4/core';
import * as am4charts from '@amcharts/amcharts4/charts';
import am4lang from '@amcharts/amcharts4/lang/ru_RU';
import store from './store';
import router from './router/index';
import App from './components/app-root.vue';
import 'moment/locale/ru';

moment.locale('ru');

Vue.config.productionTip = false;
Vue.component('lightbox', Lightbox);
Vue.use(AsyncComputed);
Vue.config.devtools = true;
Vue.config.performance = true;
Vue.use(Lightbox);
Vue.use(Vuelidate);
Vue.use(UUID);
Vue.use(PerfectScrollbar);
Vue.use(Vuetify);
Vue.prototype.$am4core = () => ({
  am4core,
  am4charts,
  am4lang,
});
Vue.prototype.$http = axios;
Vue.prototype._ = lodash;
Vue.prototype.$qs = qs;
Vue.prototype.moment = moment;
sync(store, router);

const opts = {
  icons: {
    iconfont: 'md',
  },
  theme:
  {
    dark: true,
    themes: {
      dark: {
        primary: '#fc0',
      },
    },
  },
};

new Vue({
  vuetify: new Vuetify(opts),
  router,
  store,
  render: (h) => h(App),
}).$mount('#app');
