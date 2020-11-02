import Vue from 'vue'
import axios from 'axios'
import App from './components/app-root.vue'
import './registerServiceWorker'
import router from './router/index'
import store from './store'
import { sync } from 'vuex-router-sync'
import lodash from 'lodash';  
import Vuetify from 'vuetify'
import Lightbox from 'vue-my-photos'
import Vuelidate from 'vuelidate'
import AsyncComputed from 'vue-async-computed'
import UUID from 'vue-uuid'
import PerfectScrollbar from 'vue2-perfect-scrollbar'
import qs from 'qs'
import 'material-design-icons-iconfont/dist/material-design-icons.css'
import 'vuetify/dist/vuetify.min.css'
import 'vue2-perfect-scrollbar/dist/vue2-perfect-scrollbar.css'

Vue.config.productionTip = false
Vue.component('lightbox', Lightbox)
Vue.use(AsyncComputed)
Vue.config.devtools = true
Vue.config.performance = true
Vue.use(Lightbox)
Vue.use(Vuelidate)
Vue.use(UUID)
Vue.use(PerfectScrollbar)
Vue.use(Vuetify)
Vue.prototype.$http = axios;
Vue.prototype._ = lodash
Vue.prototype.$qs = qs
sync(store, router);

const opts =  {
  icons: {
    iconfont: 'md',
  },
  theme:
  {
    dark: true,
    themes: {
      dark: {
        primary: '#fc0'
      }
    }
  }
}

new Vue({
  vuetify: new Vuetify(opts),
  router,
  store,
  render: h => h(App)
}).$mount('#app')
