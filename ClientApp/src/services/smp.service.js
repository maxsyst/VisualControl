import store from '../store/index';
import { restoreKpListFromSmpVm } from './kurbatovparameter.service.js';
import { newGuid } from './guid.service.js';

export const restoreFromVm = (smpVmList) => {
  smpVmList.forEach((smpVm) => {
    restoreFromSingleVm(smpVm);
  });
};

export const createSmp = (smp) => {
  const guid = newGuid();
  store.dispatch('smpstorage/createSmp', { guid, ...smp });
  return guid;
};

const restoreFromSingleVm = (smpVm) => {
  const {
    elementId, stageId, dividerId, mslName, name, id,
  } = smpVm;
  const smp = { name, mslName, kpList: [] };
  smp.element = store.state.smpstorage.elements.find((x) => x.elementId === elementId);
  smp.stage = store.state.smpstorage.stages.find((x) => x.stageId === stageId);
  smp.divider = store.state.dividers.dividers.find((x) => x.id === dividerId);
  smp.id = id;
  restoreKpListFromSmpVm(createSmp(smp), smpVm.kpList);
};
