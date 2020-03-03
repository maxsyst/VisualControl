class StandartMeasurementPatternFullViewModel {
    constructor(standartpattern, smpList) {
        this.standartPattern = standartpattern,
        this.standartMeasurementPatternList = [...smpList]
    }
}

class StandartPattern {
    constructor(name, dieTypeId, id = 0) {
        this.name = name,
        this.dieTypeId = dieTypeId
        this.id = id    
    }
}

class StandartMeasurementPattern {
    constructor(smp, kpList, id = 0) {
        this.elementId = smp.element.elementId
        this.stageId = smp.stage.stageId
        this.dividerId = smp.divider.id
        this.name = smp.name
        this.kpList = [...kpList]
        this.id = id    
    }
}

class KurbatovParameter {
    constructor(borders, parameter, id = 0) {
        this.kurbatovParameterBorders = borders
        this.standartParameter = parameter
        this.id = id
    }
}

class KurbatovParameterBorders {
    constructor(lower, upper, id = 0) {
      this.lower = lower
      this.upper = upper
      this.id = id
    }
}


export {StandartMeasurementPatternFullViewModel, StandartPattern, StandartMeasurementPattern, KurbatovParameter, KurbatovParameterBorders}