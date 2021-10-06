const defaultState = () => ({
  smpArray: [],
  elements: [],
  stages: [],
});

export const smpstorage = {
  namespaced: true,
  state: defaultState(),
  actions: {
    reset({ commit }) {
      commit('reset');
    },
    resetSmp({ commit }) {
      commit('resetSmp');
    },
    createSmp({ commit }, smp) {
      commit('createSmp', smp);
    },
    deleteSmp({ commit }, guid) {
      commit('deleteSmp', guid);
    },
    updateElementSmp({ commit, getters }, { guid, element }) {
      commit('updateElementSmp', { smp: getters.currentSmp(guid), element });
      commit('updateName', { smp: getters.currentSmp(guid) });
    },
    updateMslNameSmp({ commit, getters }, { guid, mslName }) {
      commit('updateMslNameSmp', { smp: getters.currentSmp(guid), mslName });
    },
    updateStageSmp({ commit, getters }, { guid, stage }) {
      commit('updateStageSmp', { smp: getters.currentSmp(guid), stage });
      commit('updateName', { smp: getters.currentSmp(guid) });
    },
    updateDividerSmp({ commit, getters }, { guid, divider }) {
      commit('updateDividerSmp', { smp: getters.currentSmp(guid), divider });
      commit('updateName', { smp: getters.currentSmp(guid) });
    },
    addToKpList({ commit, getters }, { guid, kp }) {
      commit('addToKpList', { smp: getters.currentSmp(guid), kp });
    },
    deleteFromKpList({ commit, getters }, { guid, kp }) {
      commit('deleteFromKpList', { smp: getters.currentSmp(guid), key: kp.key });
    },
    updateKp({ commit, getters }, {
      objName, guid, kpKey, obj,
    }) {
      commit('updateKp', {
        objName, smp: getters.currentSmp(guid), kpKey, obj,
      });
    },
    async getElementsByDieType({ commit }, { ctx, selectedDieTypeId }) {
      await ctx.$http
        .get(`/api/element/dietype/id/${selectedDieTypeId}`)
        .then((response) => commit('updateElements', response.data), (error) => error);
    },
    async getStagesByProcessId({ commit }, { ctx, process }) {
      await ctx.$http
        .get(`/api/stage/process/${process.processId}`)
        .then((response) => commit('updateStages', response.data), (error) => error);
    },
    async getStandartParameters({ commit }, { ctx }) {
      await ctx.$http
        .get('/api/standartparameter/all')
        .then((response) => commit('updateStandartParameters', response.data), (error) => error);
    },
  },

  getters: {
    currentSmpArray: (state) => state.smpArray,

    existInSmpArray: (state) => (name) => state.smpArray.find((s) => s.name === name) || false,

    currentSmp: (state) => (guid) => state.smpArray.find((s) => s.guid === guid),

    elementsToCopy: (state) => (guid) => {
      const smp = state.smpArray.find((s) => s.guid === guid) || false;
      if (!smp) return [];
      const currentStage = smp.stage;
      const usedElementIds = state.smpArray
        .filter((s) => s.stage.stageId === currentStage.stageId).map((x) => x.element.elementId);
      return state.elements.filter((e) => !usedElementIds.includes(e.elementId));
    },

    patternValidation: (state, getters) => state.smpArray
      .every((x) => getters.validationIsCorrect(x.guid)),

    validationIsCorrect: (state) => (guid) => {
      const smp = state.smpArray.find((s) => s.guid === guid);
      return smp.kpList.every((k) => Object.values(k.validationRules).every((item) => item));
    },

    elements: (state) => state.elements,

    stages: (state) => state.stages,

    standartParameters: (state) => state.standartParameters,
  },

  mutations: {
    reset(state) {
      Object.assign(state, defaultState());
    },
    resetSmp(state) {
      state.smpArray = [];
    },
    createSmp(state, smp) {
      state.smpArray.push(smp);
    },
    deleteSmp(state, guid) {
      state.smpArray = state.smpArray.filter((s) => s.guid !== guid);
    },
    updateName(state, { smp }) {
      smp.name = `${smp.element.name}_${smp.stage.stageName.split(' ').join('+')}_${smp.divider.name === 'Нет' ? 'No' : smp.divider.name}µm`;
    },
    updateMslNameSmp(state, { smp, mslName }) {
      smp.mslName = mslName;
    },
    updateElementSmp(state, { smp, element }) {
      smp.element = { ...element };
    },
    updateStageSmp(state, { smp, stage }) {
      smp.stage = { ...stage };
    },
    updateDividerSmp(state, { smp, divider }) {
      smp.divider = { ...divider };
    },
    addToKpList(state, { smp, kp }) {
      smp.kpList.push(kp);
    },
    deleteFromKpList(state, { smp, key }) {
      smp.kpList = smp.kpList.filter((k) => k.key !== key);
    },
    updateKp(state, {
      objName, smp, kpKey, obj,
    }) {
      const kpOld = smp.kpList.find((k) => k.key === kpKey);
      kpOld[objName] = { ...kpOld[objName], ...obj };
    },
    updateElements(state, elements) {
      state.elements = [...elements];
    },
    updateStages(state, stages) {
      state.stages = [...stages];
    },
    updateStandartParameters(state, standartParameters) {
      state.standartParameters = [...standartParameters];
    },
  },
};
