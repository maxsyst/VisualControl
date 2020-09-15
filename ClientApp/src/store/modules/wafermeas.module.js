const defaultState = () => {
  return {
    dieValues: {},
    selectedDies: [],
    keyGraphicStateModes: [],
    graphicSettings: [],
    hoveredDieId: {},
    dieColors: [],
    avbGraphics: [],
    selectedGraphics: [],
    unSelectedGraphics: [],
    measurements: [],
    wafer: {id: 0, formedMapBig: {dies: [], orientation: ""}, formedMapGradient: {dies: [], orientation: ""}, formedMapMini: {dies: [], orientation: ""}},
    divider: "",
    colors: {green: 0.8, orange: 0.6, red: 0.1, indigo: 0},
    dirtyCells: {fixedList: [], statList: [], fixedPercentageFullWafer: 0, fixedPercentageSelected: 0, statPercentageFullWafer: 0, statPercentageSelected: 0, singleGraphics: []}
  }
}

export const wafermeas = {
  namespaced: true,
  state: {
    dieValues: {},
    selectedDies: [],
    keyGraphicStateModes: [],
    hoveredDieId: {},
    graphicSettings: [],
    avbGraphics: [],
    dieColors: [],
    selectedGraphics: [],
    unSelectedGraphics: [],
    measurements: [],
    colors: {green: 0.8, orange: 0.6, red: 0.1, indigo: 0},
    wafer: {id: 0, formedMapBig: {dies: [], orientation: ""}, formedMapGradient: {dies: [], orientation: ""}, formedMapMini: {dies: [], orientation: ""}},    
    divider: "",
    sizes: {big: { streetSize: 3, fieldHeight: 420, fieldWidth: 420 }, gradient: { streetSize: 2, fieldHeight: 320, fieldWidth: 320 }, mini: { streetSize: 1, fieldHeight: 140, fieldWidth: 140 }},
    dirtyCells: {fixedList: [], statList: [], fixedPercentageFullWafer: 0, fixedPercentageSelected: 0, statPercentageFullWafer: 0, statPercentageSelected: 0},
    dirtyCellsStatSingleGraphics: [],
    dirtyCellsFixedSingleGraphics: []
  },
  
  actions: {

    reset({commit}) {
      commit('reset')
    },

    changeGraphicInitialSettings({commit}, {keyGraphicState, axisType, settings}) {
      commit('changeGraphicInitialSettings', {keyGraphicState, axisType, settings})
    },

    changeGraphicCurrentSettings({commit}, {keyGraphicState, axisType, settings}) {
      commit('changeGraphicCurrentSettings', {keyGraphicState, axisType, settings})
    },

    hoverWaferMini({commit}, {dieId, keyGraphicState}) {
      commit('hoverWaferMini', {dieId, keyGraphicState})
    },

    updateDieValues({commit}, dieValues) {
      commit('updateDieValues', dieValues)
    },

    clearDieValues({commit}) {
      commit('clearDieValues')
    },

    unHoverWaferMini({commit}) {
      commit('unHoverWaferMini')
    },

    changeKeyGraphicStateMode({commit}, {keyGraphicState, mode}) {
      commit('changeKeyGraphicStateMode', {keyGraphicState, mode})
    },

    changeKeyGraphicStateLog({commit}, {keyGraphicState, log}) {
      commit('changeKeyGraphicStateLog', {keyGraphicState, log})
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

    addToDirtyCells({commit}, {keyGraphicState, avbSelectedDies}) {
      commit('addToDirtyCells', {keyGraphicState, avbSelectedDies})
      if(Array.isArray(keyGraphicState)) {
        commit('updateUnSelectedGraphics', [])
      }
    },

    deleteFromDirtyCells({commit}, {keyGraphicState, avbSelectedDies}) {
      commit('deleteFromDirtyCellsStat', {keyGraphicState, avbSelectedDies})
      commit('deleteFromDirtyCellsFixed', {keyGraphicState, avbSelectedDies})
    },

    updateDirtyCellsSelectedNowSingleGraphic({commit}, {keyGraphicState, dirtyCells, viewMode}) {
      commit('updateDirtyCellsSelectedNowSingleGraphic', {keyGraphicState, dirtyCells, viewMode})
    },

    updateDirtyCellsFullWaferSingleGraphic({commit}, {keyGraphicState, dirtyCells}) {
      commit('updateDirtyCellsFullWaferSingleGraphicStat', {keyGraphicState, dirtyCells})
      commit('updateDirtyCellsFullWaferSingleGraphicFixed', {keyGraphicState, dirtyCells})
    },

    updateSelectedDies ({ commit }, selectedDies ) {
      commit('updateSelectedDies', selectedDies)
    },

    updateAvbGraphics({ commit }, avbGraphics) {
      commit('updateAvbGraphics', avbGraphics)
    },

    async updateDieColors({ commit }, {ctx, waferId}) {
      let dieColors = (await ctx.$http.get(`/api/die/GetColorsByWaferId?waferId=${waferId}`)).data
      commit('updateDieColors', dieColors) 
    },

    async updateSelectedWaferId ({commit, state}, {ctx, waferId}) {
    
      let measurements = (await ctx.$http.get(`/api/measurementrecording?waferId=${waferId}`)).data
      measurements = measurements.map(m => ({...m, name: m.name.includes('.') ? m.name.split('.')[1] : m.name}))
      commit('updateMeasurements', measurements) 

      let bigMap = (await ctx.$http
        .get(`/api/wafermap/getformedwafermap?waferMapFieldViewModelJSON=${JSON.stringify({waferId, fieldHeight: state.sizes.big.fieldHeight, fieldWidth: state.sizes.big.fieldWidth, streetSize: state.sizes.big.streetSize})}`)).data
      let gradientMap = (await ctx.$http
        .get(`/api/wafermap/getformedwafermap?waferMapFieldViewModelJSON=${JSON.stringify({waferId, fieldHeight: state.sizes.gradient.fieldHeight, fieldWidth: state.sizes.gradient.fieldWidth, streetSize: state.sizes.gradient.streetSize})}`)).data
      let miniMap = (await ctx.$http
        .get(`/api/wafermap/getformedwafermap?waferMapFieldViewModelJSON=${JSON.stringify({waferId, fieldHeight: state.sizes.mini.fieldHeight, fieldWidth: state.sizes.mini.fieldWidth, streetSize: state.sizes.mini.streetSize})}`)).data

      commit('updateFormedMap', {map: bigMap, mode: "big"})
      commit('updateFormedMap', {map: gradientMap, mode: "gradient"})
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
    getGraphicSettingsKeyGraphicStates: state => keyGraphicStates => keyGraphicStates.map(function (kgs) {
       return {
          graphicId: +kgs.split('_')[0],
          keyGraphicState: kgs,
          mode: state.keyGraphicStateModes.find(k => k.keyGraphicState === kgs).mode,
          isLog: state.keyGraphicStateModes.find(k => k.keyGraphicState === kgs).log
       }
    }),  
    getGraphicSettingsKeyGraphicState: state => keyGraphicState => state.graphicSettings.find(k => k.keyGraphicState === keyGraphicState).settings,
    getKeyGraphicStateMode: state => keyGraphicState => state.keyGraphicStateModes.find(k => k.keyGraphicState === keyGraphicState).mode,
    getKeyGraphicStateLog: state => keyGraphicState => state.keyGraphicStateModes.find(k => k.keyGraphicState === keyGraphicState).log,
    getDieValuesByKeyGraphicState: state => keyGraphicState => state.dieValues[keyGraphicState],
    getGraphicByGraphicState: state => keyGraphicState => state.avbGraphics.find(g => g.keyGraphicState === keyGraphicState),
    getDirtyCellsByGraphic: (state) => function (keyGraphicState, mode) 
                                       { return mode === "Мониторинг" 
                                                          ? state.dirtyCellsStatSingleGraphics.find(dc => dc.keyGraphicState === keyGraphicState) 
                                                          : state.dirtyCellsFixedSingleGraphics.find(dc => dc.keyGraphicState === keyGraphicState)},
    selectedDies: state => state.selectedDies,
    unSelectedGraphics: state => state.unSelectedGraphics,
    selectedGraphics: state => state.selectedGraphics,
    avbGraphics: state => state.avbGraphics,
    dieColors: state => state.dieColors,
    colors: state => state.colors,
    wafer: state => state.wafer,
    measurements: state => state.measurements,
    size: state => bigormini => { 
                                  if(bigormini === "mini") return state.sizes.mini
                                  if(bigormini === "big") return state.sizes.big  
                                  if(bigormini === "gradient") return state.sizes.gradient }
  },

  mutations: {

    reset(state) {
      Object.assign(state, defaultState())
    },

    changeGraphicInitialSettings(state, {keyGraphicState, axisType, settings}) {
      state.graphicSettings.push({settings: {xAxis: {}, yAxis: {}}, keyGraphicState: keyGraphicState})
      let thisSettings = state.graphicSettings.find(x => x.keyGraphicState === keyGraphicState).settings 
      thisSettings[axisType] = {initial: _.cloneDeep(settings), current: _.cloneDeep(settings)}
    },

    changeGraphicCurrentSettings(state, {keyGraphicState, axisType, settings}) {
      let thisSettings = state.graphicSettings.find(x => x.keyGraphicState === keyGraphicState).settings 
      thisSettings[axisType].current = _.cloneDeep(settings)
    },

    hoverWaferMini(state, {dieId, keyGraphicState}) {
      state.hoveredDieId = {dieId, keyGraphicState}
    },

    updateDieValues(state, dieValues) {
      state.dieValues = _.cloneDeep(dieValues)
    },

    clearDieValues(state) {
      state.dieValues = {}
    },

    updateKeyGraphicStateMode(state, selectedGraphics) {
      state.keyGraphicStateModes = selectedGraphics.map(x => ({keyGraphicState: x, mode: "initial", log: false}))
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

    changeKeyGraphicStateLog(state, {keyGraphicState, log}) {
      let keyGraphicStateMode = state.keyGraphicStateModes.find(x => x.keyGraphicState === keyGraphicState)
      if(keyGraphicStateMode !== undefined) {
        keyGraphicStateMode.log = log
      }
    },

    addKeyGraphicStateMode(state, keyGraphicState) {
      state.keyGraphicStateModes.push({keyGraphicState: keyGraphicState, mode: "initial", log: false})
    },

    deleteKeyGraphicStateMode(state, keyGraphicState) {
      state.keyGraphicStateModes = state.keyGraphicStateModes.filter(x => x.keyGraphicState !== keyGraphicState)
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
      let singleGraphicCellsStat = Array.isArray(keyGraphicState) 
                               ? [...new Set(keyGraphicState.reduce((p,c) => [...p, ...state.dirtyCellsStatSingleGraphics.find(dc => dc.keyGraphicState === c).fullWafer.cells], []))]
                               : state.dirtyCellsStatSingleGraphics.find(dc => dc.keyGraphicState === keyGraphicState).fullWafer.cells  
      let singleGraphicCellsFixed = Array.isArray(keyGraphicState) 
                               ? [...new Set(keyGraphicState.reduce((p,c) => [...p, ...state.dirtyCellsFixedSingleGraphics.find(dc => dc.keyGraphicState === c).fullWafer.cells], []))]
                               : state.dirtyCellsFixedSingleGraphics.find(dc => dc.keyGraphicState === keyGraphicState).fullWafer.cells  
      state.dirtyCells.statList = [...new Set([...state.dirtyCells.statList, ...singleGraphicCellsStat])]
      state.dirtyCells.statPercentageFullWafer = Math.ceil((1.0 - state.dirtyCells.statList.length / avbSelectedDies.length) * 100)
      state.dirtyCells.statPercentageSelected = Math.ceil((1.0 - state.selectedDies.filter(value => state.dirtyCells.statList.includes(value)).length / state.selectedDies.length) * 100) 
      state.dirtyCells.fixedList = [...new Set([...state.dirtyCells.fixedList, ...singleGraphicCellsFixed])]
      state.dirtyCells.fixedPercentageFullWafer = Math.ceil((1.0 - state.dirtyCells.fixedList.length / avbSelectedDies.length) * 100)
      state.dirtyCells.fixedPercentageSelected = Math.ceil((1.0 - state.selectedDies.filter(value => state.dirtyCells.fixedList.includes(value)).length / state.selectedDies.length) * 100)    
    },

    deleteFromDirtyCellsStat(state, {keyGraphicState, avbSelectedDies}) {
      let singleGraphicCells = state.dirtyCellsStatSingleGraphics.find(dc => dc.keyGraphicState === keyGraphicState).fullWafer.cells
      let dirtyCellsArray = state.dirtyCellsStatSingleGraphics.filter(dc => dc.keyGraphicState !== keyGraphicState && state.selectedGraphics.includes(dc.keyGraphicState))
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

    deleteFromDirtyCellsFixed(state, {keyGraphicState, avbSelectedDies}) {
      let singleGraphicCells = state.dirtyCellsFixedSingleGraphics.find(dc => dc.keyGraphicState === keyGraphicState).fullWafer.cells
      let dirtyCellsArray = state.dirtyCellsFixedSingleGraphics.filter(dc => dc.keyGraphicState !== keyGraphicState && state.selectedGraphics.includes(dc.keyGraphicState))
      let cellsArray = [...new Set(dirtyCellsArray.map(x => x.fullWafer.cells).reduce((p,c) => [...p, ...c]))]
      let readyToDelete = []
      singleGraphicCells.forEach(sgc => {
        if(!cellsArray.includes(sgc)) {
          readyToDelete.push(sgc)
      }})
      state.dirtyCells.fixedList = state.dirtyCells.fixedList.filter(dc => !readyToDelete.includes(dc))
      state.dirtyCells.fixedPercentageFullWafer = Math.ceil((1.0 - state.dirtyCells.fixedList.length / avbSelectedDies.length) * 100)
      state.dirtyCells.fixedPercentageSelected = Math.ceil((1.0 - state.selectedDies.filter(value => state.dirtyCells.fixedList.includes(value)).length / state.selectedDies.length) * 100)      
    },

    updateDirtyCellsPercentageSelected(state, {statPercentageSelected, fixedPercentageSelected}) {
      state.dirtyCells.statPercentageSelected = statPercentageSelected
      state.dirtyCells.fixedPercentageSelected = fixedPercentageSelected
    },

    updateDirtyCellsSelectedNowSingleGraphic(state, {keyGraphicState, dirtyCells, viewMode}) {
      let dirtyCellsSingleGraphics = viewMode === "Мониторинг" ? state.dirtyCellsStatSingleGraphics : state.dirtyCellsFixedSingleGraphics
      let graphic = dirtyCellsSingleGraphics.find(dc => dc.keyGraphicState === keyGraphicState)
      if(graphic === undefined) {
        graphic = {keyGraphicState, selectedNow: {cells: [], percentage: 0}, fullWafer: {cells: [], percentage: 0}}
        dirtyCellsSingleGraphics.push(graphic)
      }
      graphic.selectedNow = {cells: [...dirtyCells.cellsId], percentage: dirtyCells.percentage}
    },

    updateDirtyCellsFullWaferSingleGraphicStat(state, {keyGraphicState, dirtyCells}) {
      let dirtyCellsSingleGraphics =  state.dirtyCellsStatSingleGraphics
      let graphic = dirtyCellsSingleGraphics.find(dc => dc.keyGraphicState === keyGraphicState)
      if(graphic === undefined) {
        graphic = {keyGraphicState, selectedNow: {cells: [], percentage: 100}, fullWafer: {cells: [], percentage: -1}}
        dirtyCellsSingleGraphics.push(graphic)
      }
      graphic.fullWafer = {cells: [...dirtyCells.stat.cellsId], percentage: dirtyCells.stat.percentage}
    },

    updateDirtyCellsFullWaferSingleGraphicFixed(state, {keyGraphicState, dirtyCells}) {
      let dirtyCellsSingleGraphics = state.dirtyCellsFixedSingleGraphics
      let graphic = dirtyCellsSingleGraphics.find(dc => dc.keyGraphicState === keyGraphicState)
      if(graphic === undefined) {
        graphic = {keyGraphicState, selectedNow: {cells: [], percentage: 100}, fullWafer: {cells: [], percentage: -1}}
        dirtyCellsSingleGraphics.push(graphic)
      }
      graphic.fullWafer = {cells: [...dirtyCells.fixed.cellsId], percentage: dirtyCells.fixed.percentage}
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
      if(payload.mode === "gradient") {
        state.wafer.formedMapGradient = { dies: [...JSON.parse(payload.map.waferMapFormed)], orientation: payload.map.orientation}
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
