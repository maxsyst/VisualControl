const defaultState = () => {
    return {
        selectedProcess: {},
        wafersWithParcels: []
    }
  }
  
  export const controlCharts = {
    namespaced: true,
    state: {
        selectedProcess: {},
        processesList: [],
        wafersWithParcels: []
    },
    
    actions: {
      reset({commit}) {
        commit('reset')
      },

      resetProcess({commit}) {
        commit('resetProcess')
      },

      changeProcess({commit}, process) {
        commit('changeProcess', process)
      },

      async getProcessesFromDb({commit, getters}, {ctx}) {
        if(getters.processesList.length === 0) {
            let processesList = (await ctx.$http.get(`/api/process/all`)).data
            commit('getProcessesFromDb', processesList) 
        }
      },

      async getWafersWithParcels({commit}, {ctx, selectedProcess}) {
        let wafersWithParcels = (await ctx.$http.get(`/api/parcel/processId/${selectedProcess.processId}`)).data
        commit('changeWafersWithParcels', wafersWithParcels) 
      }
    },
  
    getters: {
        isProcessSelected: state => Object.keys(state.selectedProcess).length === 0 && state.selectedProcess .constructor === Object,
        selectedProcess: state => state.selectedProcess,
        processesList: state => [...state.processesList],
        wafersWithParcels: state => [...state.wafersWithParcels]
    },
  
    mutations: {
      
      reset(state) {
        Object.assign(state, defaultState())
      },

      resetProcess(state) {
        state.selectedProcess = {}
      },

      changeProcess(state, process) {
        state.selectedProcess = Object.assign({}, process)
      },

      getProcessesFromDb(state, processesList) {
        state.processesList = [...processesList]  
      },

      changeWafersWithParcels(state, wafersWithParcels) {
        state.wafersWithParcels = [...wafersWithParcels]  
      }

    }
  }
  