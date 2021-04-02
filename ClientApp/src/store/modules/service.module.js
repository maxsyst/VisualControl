export const service = {
  namespaced: true,
  state: {
    drawer: false,
  },
  actions: {
    hideDrawer({ commit }) {
      commit('hideDrawer');
    },
    changeDrawer({ commit }, drawerState) {
      commit('changeDrawer', drawerState);
    },
  },
  getters: {
    drawer: (state) => state.drawer,
  },
  mutations: {
    hideDrawer(state) {
      state.drawer = false;
    },
    changeDrawer(state, drawerState) {
      state.drawer = !drawerState;
    },
  },
};
