const defaultState = () => ({
  selectedProcess: {},
  selectedStage: {},
  selectedStatParameter: {},
  wafersWithParcels: [],
  selectedWafers: [],
  stagesList: [],
  statParametersList: [],
});

export const controlCharts = {
  namespaced: true,
  state: {
    selectedProcess: {},
    selectedStage: {},
    selectedStatParameter: {},
    processesList: [],
    wafersWithParcels: [],
    selectedWafers: [],
    stagesList: [],
    statParametersList: [],
  },

  actions: {
    reset({ commit }) {
      commit('reset');
    },

    resetProcess({ commit }) {
      commit('resetProcess');
    },

    changeProcess({ commit }, process) {
      commit('changeProcess', process);
    },

    changeSelectedStage({ commit }, stage) {
      commit('changeSelectedStage', stage);
    },

    changeSelectedStatParameter({ commit }, statParameter) {
      commit('changeSelectedStatParameter', statParameter);
    },

    changeSelectedWafers({ commit }, wafersArray) {
      commit('changeSelectedWafers', wafersArray);
    },

    async getProcessesFromDb({ commit, getters }, { ctx }) {
      if (getters.processesList.length === 0) {
        const processesList = (await ctx.$http.get('/api/process/all')).data;
        commit('getProcessesFromDb', processesList);
      }
    },

    async getStagesByProcessId({ commit }, { ctx, processId }) {
      const stagesList = (await ctx.$http.get(`/api/stage/process/${processId}`)).data;
      commit('changeStagesList', stagesList);
    },

    async getStatParametersByStage({ commit }, { ctx, stageId }) {
      try {
        const statParametersList = (await ctx.$http.get(`/api/statisticparameter/stage/${stageId}`)).data;
        commit('changeStatParametersList', statParametersList);
      } catch {
        commit('changeStatParametersList', []);
      }
    },

    async getWafersWithParcels({ commit }, { ctx, selectedProcess }) {
      const wafersWithParcels = (await ctx.$http.get(`/api/parcel/processId/${selectedProcess.processId}`)).data;
      commit('changeWafersWithParcels', wafersWithParcels);
    },
  },

  getters: {
    isProcessSelected: (state) => Object.keys(state.selectedProcess).length === 0
                                  && state.selectedProcess.constructor === Object,
    selectedProcess: (state) => state.selectedProcess,
    processesList: (state) => [...state.processesList],
    stagesList: (state) => [...state.stagesList],
    statParametersList: (state) => [...state.statParametersList],
    selectedWafers: (state) => [...state.selectedWafers],
    wafersWithParcels: (state) => [...state.wafersWithParcels],
  },

  mutations: {

    reset(state) {
      Object.assign(state, defaultState());
    },

    resetProcess(state) {
      state.selectedProcess = {};
    },

    changeProcess(state, process) {
      state.selectedProcess = { ...process };
    },

    changeSelectedStage(state, selectedStage) {
      state.selectedStage = { ...selectedStage };
    },

    changeSelectedStatParameter(state, statParameter) {
      state.selectedStatParameter = { ...statParameter };
    },

    changeStagesList(state, stagesList) {
      state.stagesList = [...stagesList];
    },

    changeSelectedWafers(state, wafersArray) {
      state.selectedWafers = [...wafersArray];
    },

    changeStatParametersList(state, statParametersList) {
      state.statParametersList = [...statParametersList];
    },

    getProcessesFromDb(state, processesList) {
      state.processesList = [...processesList];
    },

    changeWafersWithParcels(state, wafersWithParcels) {
      state.wafersWithParcels = [...wafersWithParcels];
    },

  },
};
