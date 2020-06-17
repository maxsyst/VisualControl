const defaultState = () => {
  return {
      selectedDies: [],
      avbGraphics: [],
      selectedGraphics: [],
      measurements: [],
      wafer: {id: 0, formedMapBig: {dies: [], orientation: ""}, formedMapMini: {dies: [], orientation: ""}},
      divider: "",
      colors: {green: 0.8, orange: 0.6, red: 0.1, indigo: 0},
      dirtyCells: []
  }
}

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
    dirtyCells: []
  },
  
  actions: {

    reset({commit}) {
      commit('reset')
    },

    updateMeasurementName({commit}, {id,name}) {
      commit('updateMeasurementName', {id,name})
    },

    updateMeasurementStage({commit}, {measurementRecordingId, stageId}) {
      commit('updateMeasurementStage', {id: measurementRecordingId, stageId})
    },

    deleteMeasurement({commit}, id) {
      commit('deleteMeasurement', id)
    },
    
    updateDirtyCellsSelectedNow({commit}, {keyGraphicState, dirtyCells}) {
      commit('updateDirtyCellsSelectedNow', {keyGraphicState, dirtyCells})
    },

    updateDirtyCellsFullWafer({commit}, {keyGraphicState, dirtyCells}) {
      commit('updateDirtyCellsFullWafer', {keyGraphicState, dirtyCells})
    },

    updateSelectedDies ({ commit }, selectedDies ) {
      commit('updateSelectedDies', selectedDies)
    },

    updateAvbGraphics({commit}, avbGraphics) {
      commit('updateAvbGraphics', avbGraphics)
    },

    async updateSelectedWaferId ({commit, state}, {ctx, waferId}) {
    
      let measurements = (await ctx.$http.get(`/api/measurementrecording?waferId=${waferId}`)).data
      measurements = measurements.map(m => ({...m, name: m.name.split('.')[1]}))
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
    calculateColor: state => statPercentage => {
      if(statPercentage >= state.colors.green) {
        return "green"
      }
      else {
        if(statPercentage >= state.colors.orange) {
          return "orange"
        }
        else {
          if(statPercentage >= state.colors.red) {
            return "pink"
          }
          else {
            return "indigo"
          }
        }
      } 
    },
    getGraphicByGraphicState: state => keyGraphicState => state.avbGraphics.find(g => g.keyGraphicState === keyGraphicState),
    getDirtyCellsByGraphic: state => keyGraphicState => state.dirtyCells.find(dc => dc.keyGraphicState === keyGraphicState),
    selectedDies: state => state.selectedDies,
    avbGraphics: state => state.avbGraphics,
    colors: state => state.colors,
    wafer: state => state.wafer,
    measurements: state => state.measurements,
    size: state => bigormini => { return bigormini === "mini" ? state.sizes.mini : state.sizes.big }
  },

  mutations: {

    reset(state) {
      Object.assign(state, defaultState())
    },

    updateDirtyCellsSelectedNow(state, {keyGraphicState, dirtyCells}) {
      let graphic = state.dirtyCells.find(dc => dc.keyGraphicState === keyGraphicState)
      if(graphic === undefined) {
        graphic = {keyGraphicState, selectedNow: {cells: [], percentage: 0}, fullWafer: {cells: [], percentage: 0}}
        state.dirtyCells.push(graphic)
      }
      graphic.selectedNow = {cells: [...dirtyCells.cellsId], percentage: dirtyCells.statPercentage}
    },

    updateDirtyCellsFullWafer(state, {keyGraphicState, dirtyCells}) {
      let graphic = state.dirtyCells.find(dc => dc.keyGraphicState === keyGraphicState)
      if(graphic === undefined) {
        graphic = {keyGraphicState, selectedNow: {cells: [], percentage: -1}, fullWafer: {cells: [], percentage: -1}}
        state.dirtyCells.push(graphic)
      }
      graphic.fullWafer = {cells: [...dirtyCells.cellsId], percentage: dirtyCells.statPercentage}
    },

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
    },

    updateMeasurementName(state, {id, name}) {
      let measurement = state.measurements.find(x => x.id === id)
      if(measurement) {
        measurement.name = name
      }
    },

    updateMeasurementStage(state, {id, stageId}) {
      let measurement = state.measurements.find(x => x.id === id)
      if(measurement) {
        measurement.stageId = stageId
      }  
    },

    deleteMeasurement(state, id) {
      state.measurements = state.measurements.filter(x => x.id !== id)
    }

  }
}
