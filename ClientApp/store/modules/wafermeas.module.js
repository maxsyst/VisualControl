export const wafermeas = {
  namespaced: true,
  state: {
    selectedDies: [],
    selectedGraphics: [],
    measurements: {},
    colors: {green: 0.8, orange: 0.6, red: 0.1, indigo: 0},
    wafer: {id: 0, formedMap: {}},    
    divider: "",
    dirtyCells: {}
  },
  
  actions: {
    updateSelectedDies ({ commit }, { selectedDies }) {
      commit('updateSelectedDies', { selectedDies })
    },
    async updateSelectedWaferId ({commit}, {ctx, waferId}) {
      //formedMap
      let measurements = await ctx.$http.get(`/api/measurementrecording?waferId=${waferId}`).data 
           
      let wafer 
      
      commit('updateWafer', { wafer })
      commit('updateMeasurements', { measurements })
    }
  },

  getters: {
    selectedDies: state => state.selectedDies,
    colors: state => state.colors,
    wafer: state => state.wafer,
    measurements: state => state.measurements
  },

  mutations: {

    updateSelectedDies (state, selectedDies) {
      state.selectedDies = selectedDies
    },

    updateWafer (state, wafer) {
      state.wafer = wafer
    },

    updateMeasurements (state, measurements) {
      state.measurements = measurements
    }

  }
}
