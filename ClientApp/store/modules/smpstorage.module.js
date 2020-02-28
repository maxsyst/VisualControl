export const smpstorage = {
    namespaced: true,
    state: {
       smpArray: [],
       elements: [],
       stages: [],
       standartParameters: []
    },

    actions: {
        createSmp ({ commit }, smp) {
            commit('createSmp', smp)
        },
        deleteSmp ({ commit }, guid) {
            commit('deleteSmp', guid)
        },
        updateElementSmp ({ commit, getters }, {guid, element}) {
            commit('updateElementSmp', {smp: getters.currentSmp(guid), element})
            commit('updateName', {smp: getters.currentSmp(guid)})
        },
        updateStageSmp ({ commit, getters }, {guid, stage}) {
            commit('updateStageSmp', {smp: getters.currentSmp(guid), stage})
            commit('updateName', {smp: getters.currentSmp(guid)})
        },
        updateDividerSmp ({ commit, getters }, {guid, divider}) {
            commit('updateDividerSmp', {smp: getters.currentSmp(guid), divider})
            commit('updateName', {smp: getters.currentSmp(guid)})
        },
        addToKpList ({ commit, getters }, {guid, kp}) {
            commit('addToKpList', {smp: getters.currentSmp(guid), kp})
        },
        deleteFromKpList ({ commit, getters }, {guid, kp}) {
            commit('deleteFromKpList', {smp: getters.currentSmp(guid), key: kp.key})
        },
        updateKp ({ commit, getters }, {objName, guid, kpKey, obj}) {
            commit('updateKp', {objName, smp: getters.currentSmp(guid), kpKey, obj})
        },
        async getElementsByDieType({commit}, {ctx, selectedDieTypeId}) {
            await ctx.$http
            .get(`/api/element/dietype/${selectedDieTypeId}`)
            .then(response => commit('updateElements', response.data), error => error)
        },
        async getStagesByProcessId({commit}, {ctx, process}) {
            await ctx.$http
            .get(`/api/stage/process/${process.processId}`)
            .then(response => commit('updateStages', response.data), error => error)
        },
        async getStandartParameters({commit}, {ctx}) {
            await ctx.$http
            .get(`/api/standartparameter/all`)
            .then(response => commit('updateStandartParameters', response.data), error => error)
        }
    },
  
    getters: {
        currentSmpArray: state => {
            return state.smpArray
        },

        existInSmpArray: state => name => {
            return state.smpArray.find(s => s.name === name) || false
        },

        currentSmp: state => guid => {           
            return state.smpArray.find(s => s.guid === guid)
        },

        elementsToCopy: state => guid => {
            let smp = state.smpArray.find(s => s.guid === guid) || false
            if(!smp) return []
            let currentStage = smp.stage
            let usedElementIds = state.smpArray.filter(s => s.stage.stageId === currentStage.stageId).map(x => x.element.elementId);
            return state.elements.filter(e => !usedElementIds.includes(e.elementId))
        },

        validationIsCorrect: state => guid => {           
            let smp = state.smpArray.find(s => s.guid === guid)
            return smp.kpList.every(k => Object.values(k.validationRules).every(item => item))
        },

        elements: state => {
            return state.elements
        },

        stages: state => {
            return state.stages
        },

        standartParameters: state => {
            return state.standartParameters
        }
    },

    mutations: {
        createSmp(state, smp) {
            state.smpArray.push(smp)
        },
        deleteSmp(state, guid) {
            state.smpArray = state.smpArray.filter(s => s.guid !== guid)
        },
        updateName(state, {smp}) {
            smp.name = `${smp.element.name}_${smp.stage.stageName.split(' ').join('+')}_${smp.divider.name === "Нет" ? "No" : smp.divider.name}µm` 
        },
        updateElementSmp(state, {smp, element}) {
            smp.element = {...element}
        },
        updateStageSmp(state, {smp, stage}) {
            smp.stage = {...stage}
        },
        updateDividerSmp(state, {smp, divider}) {
            smp.divider = {...divider}
        },
        addToKpList(state, {smp, kp}) {
            smp.kpList.push(kp)
        },
        deleteFromKpList(state, {smp, key}) {
            smp.kpList = smp.kpList.filter(k => k.key !== key)
        },
        updateKp(state, {objName, smp, kpKey, obj}) {
            let kpOld = smp.kpList.find(k => k.key === kpKey)
            kpOld[objName] = {...kpOld[objName], ...obj}
        },
        updateElements(state, elements) {
            state.elements = [...elements]
        },
        updateStages(state, stages) {
            state.stages = [...stages]
        },
        updateStandartParameters(state, standartParameters) {
            state.standartParameters = [...standartParameters]
        }
    }
}
  