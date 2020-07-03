const defaultState = () => {
  return {
    
      selectedDies: [],
      keyGraphicStateModes: [],
      hoveredDieId: {},
      dieColors: [],
      avbGraphics: [],
      selectedGraphics: [],
      unSelectedGraphics: [],
      measurements: [],
      wafer: {id: 0, formedMapBig: {dies: [], orientation: ""}, formedMapMini: {dies: [], orientation: ""}},
      divider: "",
      colors: {green: 0.8, orange: 0.6, red: 0.1, indigo: 0},
      dirtyCells: {fixedList: [], statList: [], fixedPercentageFullWafer: 0, fixedPercentageSelected: 0, statPercentageFullWafer: 0, statPercentageSelected: 0, singleGraphics: []}
  }
}

export const wafermeas = {
  namespaced: true,
  state: {
    selectedDies: [],
    keyGraphicStateModes: [],
    hoveredDieId: {},
    avbGraphics: [],
    dieColors: [],
    selectedGraphics: [],
    unSelectedGraphics: [],
    measurements: [],
    colors: {green: 0.8, orange: 0.6, red: 0.1, indigo: 0},
    wafer: {id: 0, formedMapBig: {dies: [], orientation: ""}, formedMapMini: {dies: [], orientation: ""}},    
    divider: "",
    sizes: {big: { streetSize: 3, fieldHeight: 420, fieldWidth: 420 }, mini: { streetSize: 1, fieldHeight: 140, fieldWidth: 140 }},
    dirtyCells: {fixedList: [], statList: [], fixedPercentageFullWafer: 0, fixedPercentageSelected: 0, statPercentageFullWafer: 0, statPercentageSelected: 0},
    dirtyCellsSingleGraphics: []
  },
  
  actions: {

    reset({commit}) {
      commit('reset')
    },

    hoverWaferMini({commit}, {dieId, keyGraphicState}) {
      commit('hoverWaferMini', {dieId, keyGraphicState})
    },

    unHoverWaferMini({commit}) {
      commit('unHoverWaferMini')
    },

    changeKeyGraphicStateMode({commit}, {keyGraphicState, mode}) {
      commit('changeKeyGraphicStateMode', {keyGraphicState, mode})
    },

    clearSelectedGraphics({commit}) {
      commit('updateSelectedGraphics', [])
      commit('clearKeyGraphicStateMode')
    },

    updateSelectedGraphics({commit}, selectedGraphics) {
      commit('updateSelectedGraphics', selectedGraphics)
      commit('updateKeyGraphicStateMode', selectedGraphics)
    },

    updateUnSelectedGraphics({commit}, unSelectedGraphics) {
      commit('updateUnSelectedGraphics', unSelectedGraphics)
    },

    addSelectedGraphic({commit}, keyGraphicState) {
      commit('addSelectedGraphic', keyGraphicState)
      commit('addKeyGraphicStateMode', keyGraphicState)
    },

    deleteSelectedGraphic({commit}, keyGraphicState) {
      commit('deleteSelectedGraphic', keyGraphicState)
      commit('deleteKeyGraphicStateMode', keyGraphicState)
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

    updateDirtyCells({commit}, dirtyCells) {
      commit('updateDirtyCells', dirtyCells)
    },

    updateDirtyCellsPercentageSelected({commit}, {statPercentageSelected, fixedPercentageSelected}) {
      commit('updateDirtyCellsPercentageSelected', {statPercentageSelected, fixedPercentageSelected})
    },

    addToDirtyCellsStat({commit}, {keyGraphicState, avbSelectedDies}) {
      commit('addToDirtyCellsStat', {keyGraphicState, avbSelectedDies})
      if(Array.isArray(keyGraphicState)) {
        commit('updateUnSelectedGraphics', [])
      }
    },

    deleteFromDirtyCellsStat({commit}, {keyGraphicState, avbSelectedDies}) {
      commit('deleteFromDirtyCellsStat', {keyGraphicState, avbSelectedDies})
    },
    
    updateDirtyCellsSelectedNowSingleGraphic({commit}, {keyGraphicState, dirtyCells}) {
      commit('updateDirtyCellsSelectedNowSingleGraphic', {keyGraphicState, dirtyCells})
    },

    updateDirtyCellsFullWaferSingleGraphic({commit}, {keyGraphicState, dirtyCells}) {
      commit('updateDirtyCellsFullWaferSingleGraphic', {keyGraphicState, dirtyCells})
    },

    updateSelectedDies ({ commit }, selectedDies ) {
      commit('updateSelectedDies', selectedDies)
    },

    updateAvbGraphics({commit}, avbGraphics) {
      commit('updateAvbGraphics', avbGraphics)
    },

    async updateDieColors({commit}, {ctx, waferId}) {
      let dieColors = (await ctx.$http.get(`/api/die/GetColorsByWaferId?waferId=${waferId}`)).data
      commit('updateDieColors', dieColors) 
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
    dirtyCells: state => state.dirtyCells,
    hoveredDieId: state => state.hoveredDieId,
    getKeyGraphicStateMode: state => keyGraphicState => state.keyGraphicStateModes.find(k => k.keyGraphicState === keyGraphicState).mode,
    getGraphicByGraphicState: state => keyGraphicState => state.avbGraphics.find(g => g.keyGraphicState === keyGraphicState),
    getDirtyCellsByGraphic: state => keyGraphicState => state.dirtyCellsSingleGraphics.find(dc => dc.keyGraphicState === keyGraphicState),
    selectedDies: state => state.selectedDies,
    unSelectedGraphics: state => state.unSelectedGraphics,
    selectedGraphics: state => state.selectedGraphics,
    avbGraphics: state => state.avbGraphics,
    dieColors: state => state.dieColors,
    colors: state => state.colors,
    wafer: state => state.wafer,
    measurements: state => state.measurements,
    size: state => bigormini => { return bigormini === "mini" ? state.sizes.mini : state.sizes.big }
  },

  mutations: {

    reset(state) {
      Object.assign(state, defaultState())
    },

    hoverWaferMini(state, {dieId, keyGraphicState}) {
      state.hoveredDieId = {dieId, keyGraphicState}
    },

    updateKeyGraphicStateMode(state, selectedGraphics) {
      state.keyGraphicStateModes = selectedGraphics.map(x => ({keyGraphicState: x, mode: "selected"}))
    },

    clearKeyGraphicStateMode(state) {
      state.keyGraphicStateModes = []
    },

    changeKeyGraphicStateMode(state, {keyGraphicState, mode}) {
      let keyGraphicStateMode = state.keyGraphicStateModes.find(x => x.keyGraphicState === keyGraphicState)
      if(keyGraphicStateMode !== undefined) {
        keyGraphicStateMode.mode = mode
      }
    },

    addKeyGraphicStateMode(state, keyGraphicState) {
      state.keyGraphicStateModes.push({keyGraphicState: keyGraphicState, mode: "selected"})
    },

    deleteKeyGraphicStateMode(state, keyGraphicState) {
      state.keyGraphicStateModes = state.keyGraphicStateModes.filter(x => x.keyGraphicState!==keyGraphicState)
    },

    unHoverWaferMini(state) {
      state.hoveredDieId = {}
    },

    updateDieColors(state, dieColors) {
      state.dieColors = [...dieColors]
    },

    updateDirtyCells(state, dirtyCells) {
      state.dirtyCells = _.cloneDeep(dirtyCells)
    },

    addToDirtyCellsStat(state, {keyGraphicState, avbSelectedDies}) {
      let singleGraphicCells = Array.isArray(keyGraphicState) 
                               ? [...new Set(keyGraphicState.reduce((p,c) => [...p, ...state.dirtyCellsSingleGraphics.find(dc => dc.keyGraphicState === c).fullWafer.cells], []))]
                               : state.dirtyCellsSingleGraphics.find(dc => dc.keyGraphicState === keyGraphicState).fullWafer.cells  
      state.dirtyCells.statList = [...new Set([...state.dirtyCells.statList, ...singleGraphicCells])]
      state.dirtyCells.statPercentageFullWafer = Math.ceil((1.0 - state.dirtyCells.statList.length / avbSelectedDies.length) * 100)
      state.dirtyCells.statPercentageSelected = Math.ceil((1.0 - state.selectedDies.filter(value => state.dirtyCells.statList.includes(value)).length / state.selectedDies.length) * 100)    
    },

    deleteFromDirtyCellsStat(state, {keyGraphicState, avbSelectedDies}) {
      let singleGraphicCells = state.dirtyCellsSingleGraphics.find(dc => dc.keyGraphicState === keyGraphicState).fullWafer.cells
      let dirtyCellsArray = state.dirtyCellsSingleGraphics.filter(dc => dc.keyGraphicState !== keyGraphicState && state.selectedGraphics.includes(dc.keyGraphicState))
      let cellsArray = [...new Set(dirtyCellsArray.map(x => x.fullWafer.cells).reduce((p,c) => [...p, ...c]))]
      let readyToDelete = []
      singleGraphicCells.forEach(sgc => {
        if(!cellsArray.includes(sgc)) {
          readyToDelete.push(sgc)
      }})
      state.dirtyCells.statList = state.dirtyCells.statList.filter(dc => !readyToDelete.includes(dc))
      state.dirtyCells.statPercentageFullWafer = Math.ceil((1.0 - state.dirtyCells.statList.length / avbSelectedDies.length) * 100)
      state.dirtyCells.statPercentageSelected = Math.ceil((1.0 - state.selectedDies.filter(value => state.dirtyCells.statList.includes(value)).length / state.selectedDies.length) * 100)      
    },

    updateDirtyCellsPercentageSelected(state, {statPercentageSelected, fixedPercentageSelected}) {
      state.dirtyCells.statPercentageSelected = statPercentageSelected
      state.dirtyCells.fixedPercentageSelected = fixedPercentageSelected
    },

    updateDirtyCellsSelectedNowSingleGraphic(state, {keyGraphicState, dirtyCells}) {
      let graphic = state.dirtyCellsSingleGraphics.find(dc => dc.keyGraphicState === keyGraphicState)
      if(graphic === undefined) {
        graphic = {keyGraphicState, selectedNow: {cells: [], percentage: 0}, fullWafer: {cells: [], percentage: 0}}
        state.dirtyCellsSingleGraphics.push(graphic)
      }
      graphic.selectedNow = {cells: [...dirtyCells.cellsId], percentage: dirtyCells.statPercentage}
    },

    updateDirtyCellsFullWaferSingleGraphic(state, {keyGraphicState, dirtyCells}) {
      let graphic = state.dirtyCellsSingleGraphics.find(dc => dc.keyGraphicState === keyGraphicState)
      if(graphic === undefined) {
        graphic = {keyGraphicState, selectedNow: {cells: [], percentage: 100}, fullWafer: {cells: [], percentage: -1}}
        state.dirtyCellsSingleGraphics.push(graphic)
      }
      graphic.fullWafer = {cells: [...dirtyCells.cellsId], percentage: dirtyCells.statPercentage}
    },

    updateSelectedGraphics(state, selectedGraphics) {
      state.selectedGraphics = [...selectedGraphics]
    },

    updateUnSelectedGraphics(state, unSelectedGraphics) {
      state.unSelectedGraphics = [...unSelectedGraphics]
    },

    addSelectedGraphic(state, keyGraphicState) {
      state.selectedGraphics.push(keyGraphicState)
      state.unSelectedGraphics = state.unSelectedGraphics.filter(x => x.keyGraphicState!==keyGraphicState)
    },

    deleteSelectedGraphic(state, keyGraphicState) {
      state.selectedGraphics = state.selectedGraphics.filter(x => x!==keyGraphicState)
      state.unSelectedGraphics.push(state.avbGraphics.find(x=>x.keyGraphicState === keyGraphicState))
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
