export const wafermeas = {
  namespaced: true,
  state: {
    selectedDies: [],
    avbGraphics: [],
    selectedGraphics: [],
    measurements: [],
    colors: {green: 0.8, orange: 0.6, red: 0.1, indigo: 0},
    wafer: {id: 0, formedMapBig: {dies: [], orientation: ""}, formedMapMini: {dies: [], orientation: ""}},    
    divider: "",
    sizes: {big: { streetSize: 3, fieldHeight: 420, fieldWidth: 420 }, mini: { streetSize: 1, fieldHeight: 140, fieldWidth: 140 }},
    dirtyCells: {}
  },
  
  actions: {
    updateSelectedDies ({ commit }, selectedDies ) {
      commit('updateSelectedDies', selectedDies)
    },

    updateAvbGraphics({commit}, avbGraphics) {
      commit('updateAvbGraphics', avbGraphics)
    },

    async updateSelectedWaferId ({commit, state}, {ctx, waferId}) {
    
      let measurements = (await ctx.$http.get(`/api/measurementrecording?waferId=${waferId}`)).data
      commit('updateMeasurements', measurements) 
      
      let bigMap = (await ctx.$http({
        method: "post",
        url: `/api/wafermap/getformedwafermap`, data: {waferId, fieldHeight: state.sizes.big.fieldHeight, fieldWidth: state.sizes.big.fieldWidth, streetSize: state.sizes.big.streetSize}, config: {
        headers: {
          'Accept': "application/json",
          'Content-Type': "application/json"
          }
        }
      })).data

      let miniMap = (await ctx.$http({
        method: "post",
        url: `/api/wafermap/getformedwafermap`, data: {waferId, fieldHeight: state.sizes.mini.fieldHeight, fieldWidth: state.sizes.mini.fieldWidth, streetSize: state.sizes.mini.streetSize}, config: {
          headers: {
            'Accept': "application/json",
            'Content-Type': "application/json"
          }
        }
      })).data

      commit('updateFormedMap', {map: bigMap, mode: "big"})
      commit('updateFormedMap', {map: miniMap, mode: "mini"})
      commit('updateWaferId', waferId)
    }
  },

  getters: {
    getGraphicByGraphicState: state => keyGraphicState => state.avbGraphics.find(g => g.keyGraphicState === keyGraphicState),
    selectedDies: state => state.selectedDies,
    avbGraphics: state => state.avbGraphics,
    colors: state => state.colors,
    wafer: state => state.wafer,
    measurements: state => state.measurements,
    size: state => bigormini => { return bigormini === "mini" ? state.sizes.mini : state.sizes.big }
  },

  mutations: {

    updateSelectedDies (state, selectedDies) {
      state.selectedDies = [...selectedDies]
    },

    updateAvbGraphics(state, avbGraphics) {
      state.avbGraphics = [...avbGraphics]
    },

    updateWaferId(state, waferId) {
      state.wafer.id = waferId
    },

    updateFormedMap (state, payload) {
      if(payload.mode === "mini") {
        state.wafer.formedMapMini = { dies: [...JSON.parse(payload.map.waferMapFormed)], orientation: payload.map.orientation}
      }
      if(payload.mode === "big") {
        state.wafer.formedMapBig = { dies: [...JSON.parse(payload.map.waferMapFormed)], orientation: payload.map.orientation}
      }
    },

    updateMeasurements (state, measurements) {
      state.measurements = [...measurements]
    }

  }
}
