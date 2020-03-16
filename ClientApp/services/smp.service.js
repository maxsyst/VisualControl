import store from '../store/index'
import { restoreKpListFromSmpVm } from '../services/kurbatovparameter.service.js'
import { newGuid } from '../services/guid.service.js'

export const restoreFromVm = (smpVmList) => {
    smpVmList.forEach(smpVm => {
        restoreFromSingleVm(smpVm)
    })
}

export const createSmp = (smp) => {
    let guid = newGuid()
    store.dispatch("smpstorage/createSmp", {guid, ...smp})
    return guid
}

const restoreFromSingleVm = (smpVm) => {
    const {elementId, stageId, dividerId, name} = smpVm;   
    let smp = {name, kpList: []}
    smp.element = store.state.smpstorage.elements.find(x => x.elementId === elementId)
    smp.stage = store.state.smpstorage.stages.find(x => x.stageId === stageId)
    smp.divider = store.state.dividers.dividers.find(x => x.id === dividerId)
    restoreKpListFromSmpVm(createSmp(smp), smpVm.kpList)    
}

