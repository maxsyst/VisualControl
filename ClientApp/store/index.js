import Vue from 'vue'
import Vuex from 'vuex'
import defects from './modules/defects'
import { alert } from './modules/alert.module'
import { authentication } from './modules/authentication.module'
import { users } from './modules/users.module'
import { wafermeas } from './modules/wafermeas.module'
import { exportkurb } from './modules/exportkurb.module'
import { smpstorage } from './modules/smpstorage.module'
import { elements } from './modules/elements.module'
import { dividers } from './modules/dividers.module'

Vue.config.devtools = true
Vue.use(Vuex)

export default new Vuex.Store({
  modules: {
    defects, 
    alert, 
    authentication, 
    users, 
    wafermeas, 
    exportkurb,
    smpstorage,
    elements,
    dividers
  }

})
