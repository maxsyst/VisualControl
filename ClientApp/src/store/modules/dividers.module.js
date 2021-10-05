export const dividers = {
  namespaced: true,
  state: {
    dividers: [],
  },

  actions: {
    async getAllDividers({ commit }, ctx) {
      await ctx.$http
        .get('/api/divider/all')
        .then((response) => commit('updateDividers', response.data), (error) => error);
    },
  },

  getters: {
    getAll: (state) => state.dividers,
    getById: (state) => (id) => state.dividers.find((x) => x.id === id),
  },

  mutations: {
    updateDividers(state, dividerArray) {
      state.dividers = [...dividerArray];
    },
  },
};
