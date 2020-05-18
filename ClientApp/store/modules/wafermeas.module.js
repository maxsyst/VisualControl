export const wafermeas = {
  namespaced: true,
  state: {
    selectedDies: []
  },
  
  actions: {
    updateSelectedDies ({ commit }, { selectedDies }) {
      commit('updateSelectedDies', { selectedDies })
    }

  },

  getters: {
    selectedDies: state => state.selectedDies
  },

  mutations: {
    updateSelectedDies (state, selectedDies) {
      state.selectedDies = selectedDies
    }
  }
}
