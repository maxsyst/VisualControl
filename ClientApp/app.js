import Vue from 'vue'
import axios from 'axios'
import router from './router/index'
import store from './store'
import { sync } from 'vuex-router-sync'
import App from 'components/app-root'
import { FontAwesomeIcon } from './icons'
import VueSweetalert2 from 'vue-sweetalert2';
import svgSpriteLoader from 'svg-sprite-loader';


// Registration of global components
Vue.component('icon', FontAwesomeIcon);
Vue.use(VueSweetalert2);

const __svg__ = { path: '/images/icons/*.svg', name: '/images/[hash].sprite.svg' }
svgSpriteLoader(__svg__.filename);


Vue.prototype.$http = axios;

sync(store, router);

const app = new Vue({
  store,
  router,
  ...App
});

export {
  app,
  router,
  store
}
