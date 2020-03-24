class StandartMeasurementPatternFullViewModel {
    constructor(standartpattern, smpList = []) {
        this.standartPattern = standartpattern,
        this.standartMeasurementPatternList = [...smpList]
    }

}

class StandartPattern {
    constructor(name, dieTypeId, id) {
        this.name = name,
        this.dieTypeId = dieTypeId
        this.id = id || 0
    }
}

class StandartMeasurementPattern {
    constructor(smp, kpList, patternId, id) {
        this.elementId = smp.element.elementId
        this.stageId = smp.stage.stageId
        this.dividerId = smp.divider.id
        this.name = smp.name
        this.patternId = patternId || 0
        this.kpList = [...kpList]
        this.id = id || 0   
    }
}

class KurbatovParameter {
    constructor(borders, parameter, id) {
        this.kurbatovParameterBorders = borders
        this.standartParameter = parameter
        this.id = id || 0
    }
}

class KurbatovParameterBorders {
    constructor(lower, upper, id) {
      this.lower = lower || null
      this.upper = upper || null
      this.id = id || 0
    }
}


export {StandartMeasurementPatternFullViewModel, StandartPattern, StandartMeasurementPattern, KurbatovParameter, KurbatovParameterBorders}