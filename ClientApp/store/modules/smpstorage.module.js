export const smpstorage = {
    namespaced: true,
    state: {
       smpArray: [],
       elements: [],
       stages: []
    },

    actions: {
        createSmp ({ commit }, smp) {
            commit('createSmp', smp)
        },
        deleteSmp ({ commit }, smp) {
            commit('deleteSmp', smp)
        },
        updateElementSmp ({ commit, getters }, {guid, element}) {
            commit('updateElementSmp', {smp: getters.currentSmp(guid), element})
        },
        updateStageSmp ({ commit, getters }, {guid, stage}) {
            commit('updateStageSmp', {smp: getters.currentSmp(guid), stage})
        },
        updateDividerSmp ({ commit, getters }, {guid, divider}) {
            commit('updateDividerSmp', {smp: getters.currentSmp(guid), divider})
        },
        addToKpList ({ commit, getters }, {guid, kp}) {
            commit('addToKpList', {smp: getters.currentSmp(guid), kp})
        },
        deleteFromKpList ({ commit, getters }, {guid, kp}) {
            commit('deleteFromKpList', {smp: getters.currentSmp(guid), key: kp.key})
        },
        updateKpList ({ commit, getters }, {guid, kp}) {
            commit('updateKpList', {smp: getters.currentSmp(guid), kp})
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

        elements: state => {
            return state.elements
        },

        stages: state => {
            return state.stages
        }
    },

    mutations: {
        createSmp(state, smp) {
            state.smpArray.push(smp)
        },
        deleteSmp(state, {guid}) {
            state.smpArray = state.smpArray.filter(s => s.guid !== guid)
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
            smp.kpList = smp.kpList.filter(k => k.key === key)
        },
        updateKpList(state, {smp, kp}) {
            let kpNew = smp.kpList.find(k => k.key === kp.key)
            kpNew = {...kp}
        },
        updateElements(state, elements) {
            state.elements = [...elements]
        },
        updateStages(state, stages) {
            state.stages = [...stages]
        }
    }
}
  