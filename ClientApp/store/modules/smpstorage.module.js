export const smpstorage = {
    namespaced: true,
    state: {
       smpArray: []
    },

    actions: {
        createSmp ({ commit }, smp) {
            commit('createSmp', smp)
        },
        deleteSmp ({ commit, getters }, guid) {
            commit('deleteSmp', getters.currentSmp(guid))
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
        }
    },
  
    getters: {
        currentSmp: state => guid => {           
            return state.smpArray.find(s => s.guid === guid)
        }
    },

    mutations: {
        createSmp(state, {smp}) {
            state.smpArray.push(smp)
        },
        deleteSmp(state, {smp}) {
            state.smpArray = state.smpArray.filter(s => s.guid !== smp.guid)
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
        }
    }
}
  