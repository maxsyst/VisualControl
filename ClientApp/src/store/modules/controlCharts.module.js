const defaultState = () => {
    return {
        selectedProcess: {}
    }
  }
  
  export const controlCharts = {
    namespaced: true,
    state: {
        selectedProcess: {},
        processesList: []
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
      }
    },
  
    getters: {
        isProcessSelected: state => Object.keys(state.selectedProcess).length === 0 && state.selectedProcess .constructor === Object,
        selectedProcess: state => state.selectedProcess,
        processesList: state => [...state.processesList]
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
      }

    }
  }
  