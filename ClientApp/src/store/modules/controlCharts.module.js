const defaultState = () => {
    return {
        selectedProcess: {},
        wafersWithParcels: [],
        selectedWafers: []
    }
  }
  
  export const controlCharts = {
    namespaced: true,
    state: {
        selectedProcess: {},
        processesList: [],
        wafersWithParcels: [],
        selectedWafers: []
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

      changeSelectedWafers({commit}, wafersArray) {
        commit('changeSelectedWafers', wafersArray)
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
        selectedWafers: state => [...state.selectedWafers],
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
      
      changeSelectedWafers(state, wafersArray) {
        state.selectedWafers = [...wafersArray]
      },

      getProcessesFromDb(state, processesList) {
        state.processesList = [...processesList]  
      },

      changeWafersWithParcels(state, wafersWithParcels) {
        state.wafersWithParcels = [...wafersWithParcels]  
      }

    }
  }
  