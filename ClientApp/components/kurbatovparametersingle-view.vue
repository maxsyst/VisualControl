<template>
    <v-container>
        <v-row>
            <v-col lg="2">
                <v-select 
                    :value="smp.element"
                    :items="this.$store.getters['smpstorage/elements']"
                    item-text="name"
                    no-data-text="Нет данных"
                    return-object
                    outlined
                    @change="updateElement($event)"
                    label="Выберите элемент:">
                </v-select>
            </v-col>
            <v-col lg="4">
                <v-select 
                    :value="smp.stage"
                    :items="this.$store.getters['smpstorage/stages']"
                    item-text="stageName"
                    no-data-text="Нет данных"
                    return-object
                    outlined
                    @change="updateStage($event)"
                    label="Выберите этап:">
                </v-select>
            </v-col>
            <v-col lg="2">
                <v-text-field  
                    :value="smp.mslName"
                    :key="`msl-${guid}`"
                    @change="updateMslName($event)"
                    outlined label="Номер операции в МСЛ:">
                </v-text-field>
            </v-col>    
            <v-col lg="2">
                <v-select 
                    :value="smp.divider"
                    :items="$store.getters['dividers/getAll']"
                    item-text="name"
                    no-data-text="Нет данных"
                    return-object
                    outlined
                    @change="updateDivider($event)"
                    label="Выберите периферию:">
                </v-select>
            </v-col>
            <v-col lg="1">                
                <v-btn v-if="validationIsCorrect" fab dark small color="indigo" @click="$emit('chbx-dialog', guid)">
                    <v-icon dark color="primary">file_copy</v-icon>
                </v-btn>
                 <v-btn fab dark small color="indigo" @click="$emit('delete-smp', guid)">
                    <v-icon dark color="primary">delete</v-icon>
                </v-btn>
            </v-col>
            <v-col lg="1">
                <v-btn v-if="!validationIsCorrect" fab dark small color="pink"><v-icon dark color="white">error_outline</v-icon></v-btn>
                <v-btn v-else fab dark small color="green"><v-icon dark color="white">done_outline</v-icon></v-btn>      
            </v-col>
        </v-row>
        <v-row>
            <v-col lg="12">
                <v-stepper :key="stepperKey" v-model="step">
                    <v-stepper-header>
                        <template v-for="(parameter, index) in smp.kpList">
                            <v-stepper-step 
                                :key="`${parameter.key}-step`"
                                :step="index + 1"
                                color="indigo"
                                editable>
                                {{parameter.standartParameter.parameterName}}
                            </v-stepper-step>
                            <v-divider :key="index + 1"></v-divider>
                        </template>
                    </v-stepper-header>
                    <v-stepper-items>
                        <v-stepper-content
                            v-for="(parameter, index) in smp.kpList"
                            :key="`${parameter.key}-content`"
                            :step="index + 1">
                            <div>
                                    <v-row>
                                        <v-col lg="3">
                                            <v-select   :value="parameter.standartParameter"
                                                        :items="forbiddenStandartParameters.length === 0 
                                                                    ? [] 
                                                                    : standartParameters
                                                                        .filter(x => !forbiddenStandartParameters.find(f => f.key === parameter.key).forbiddenIds.includes(x.id))"
                                                        no-data-text="Нет данных"
                                                        return-object
                                                        item-text="parameterName"
                                                        outlined
                                                        @change="updateStandartParameter($event, parameter)"
                                                        label="Выберите параметр:">
                                            </v-select>
                                        </v-col>
                                        <v-col lg="4">
                                            <v-text-field   :value="parameter.standartParameter.russianParameterName"
                                                            :error-messages="parameter.validationRules.parameterRq 
                                                                                ? []                                                                                                          
                                                                                : 'Выберите параметр'"
                                                            readonly outlined label="Расширенное название:">
                                            </v-text-field>
                                        </v-col>
                                        <v-col lg="5">
                                            <v-text-field   :value="parameter.standartParameter.parameterNameStat" 
                                                            :error-messages="parameter.validationRules.parameterRq 
                                                                                ? []                                                                                                          
                                                                                : 'Выберите параметр'"
                                                            readonly outlined label="Системное название:">
                                            </v-text-field>
                                        </v-col>
                                    </v-row>
                                    <v-row>                                        
                                         <v-col lg="2">
                                            <v-text-field :key="`bndl-`+ parameter.key" :value="parameter.bounds.lower" :error-messages="validationBoundsErrors(parameter.validationRules, `lower`)"
                                                            :readonly="!parameter.withBounds.value"
                                                            @change="updateBounds($event, 'lower', parameter)" outlined label="Нижняя граница:">
                                            </v-text-field>
                                        </v-col>
                                        <v-col lg="2">
                                            <v-text-field :key="`bndu-`+ parameter.key" :value="parameter.bounds.upper" :error-messages="validationBoundsErrors(parameter.validationRules, `upper`)"
                                                            :readonly="!parameter.withBounds.value"
                                                            @change="updateBounds($event, 'upper', parameter)" outlined label="Верхняя граница:">
                                            </v-text-field>
                                        </v-col>        
                                        <v-col lg="3">
                                             <v-switch
                                                :input-value="parameter.withBounds.value"
                                                color='primary'
                                                @change="updateWithBounds($event, parameter)"
                                                :label="parameter.withBounds.value ? `Включить границы` : `Не включать границы`">
                                            </v-switch>
                                        </v-col>      
                                    </v-row>      
                            <v-tooltip v-if="index > 0" bottom>
                                <template v-slot:activator="{ on }">
                                    <v-btn fab dark small color="indigo" v-on="on" @click="prevStep(index + 1)">
                                        <v-icon dark color="primary">skip_previous</v-icon>
                                    </v-btn>
                                </template>
                                <span>Предыдущий параметр</span>
                            </v-tooltip>                        
                            <v-tooltip v-if="index < smp.kpList.length - 1" bottom>
                                <template v-slot:activator="{ on }">
                                    <v-btn fab dark small color="indigo" v-on="on" @click="nextStep(index + 1)">
                                        <v-icon dark color="primary">skip_next</v-icon>
                                    </v-btn>
                                </template>
                                <span>Следующий параметр</span>
                            </v-tooltip>    
                          </div>                         
                        </v-stepper-content>
                    </v-stepper-items> 
                <div class="parameter-actions"> 
                    <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                            <v-btn fab dark small color="indigo" v-on="on" @click="createParameter">
                                <v-icon dark color="primary">add</v-icon>
                            </v-btn>
                        </template>
                        <span>Добавить параметр</span>
                    </v-tooltip>
                    <v-tooltip bottom>
                        <template v-slot:activator="{ on }">
                            <v-btn v-if="smp.kpList.length > 0" fab dark small color="indigo" v-on="on" @click="deleteParameter(step)">
                                <v-icon dark color="primary">delete</v-icon>
                            </v-btn>
                        </template>
                        <span>Удалить параметр</span>
                    </v-tooltip>               
               </div>
            </v-stepper>
          </v-col>
         </v-row>
    </v-container>
</template>

<script>
import { createKurbatovParameter as createKurbatovParameterFromService } from '../services/kurbatovparameter.service.js'

export default {
    props: {
        guid: String
    },

    data() {
       return {
           step: 0,
           stepperKey: 0,
           forbiddenStandartParameters: []
       }
    },

    methods: {
        createKurbatovParameter: createKurbatovParameterFromService,

        updateMslName(mslName) {
            console.log(mslName)
            this.$store.dispatch("smpstorage/updateMslNameSmp", {guid: this.guid, mslName})      
        },

        updateElement(element) {
            this.$store.dispatch("smpstorage/updateElementSmp", {guid: this.guid, element})        
        },

        updateStage(stage) {
            this.$store.dispatch("smpstorage/updateStageSmp", {guid: this.guid, stage})        
        },

        updateDivider(divider) {
            this.$store.dispatch("smpstorage/updateDividerSmp", {guid: this.guid, divider})
        },

        createParameter() {
            let kp = this.createKurbatovParameter(this.guid)
            this.step = this.smp.kpList.length
            this.stepperKeyUpdate()
            this.forbiddenStandartParameters.push({key: kp.key, forbiddenIds: this.forbiddenStandartParameters.map(x => x.selectedId), selectedId: 0})
        },

        updateStandartParameter(standartParameter, kp) {
            let oldStandartParameterId = kp.standartParameter.id
            this.forbiddenStandartParameters.forEach(f => {
                f.forbiddenIds = f.forbiddenIds.filter(x => x !== oldStandartParameterId)
                if(f.key !== kp.key) {
                    f.forbiddenIds.push(standartParameter.id)
                } else {
                    f.selectedId = standartParameter.id
                }
            })
            this.$store.dispatch("smpstorage/updateKp", {objName: 'standartParameter', guid: this.guid, kpKey: kp.key, obj: standartParameter})
            this.$store.dispatch("smpstorage/updateKp", {objName: 'validationRules', guid: this.guid, kpKey: kp.key, obj: {parameterRq: true}})
        },

        updateWithBounds(withBounds, kp) {
            let validationRules = (withBounds === null || !withBounds) ? {boundsRq: true, lowerBoundIsNumeric: true, upperBoundIsNumeric: true, lowerBoundLowerThanUpperBound: true} : _.isEmpty(kp.bounds.lower) && _.isEmpty(kp.bounds.upper) ? {boundsRq: false} : {}
            validationRules = withBounds && kp.bounds.lower !== "" && kp.bounds.upper !== "" && +kp.bounds.lower >= +kp.bounds.upper ? {...validationRules, lowerBoundLowerThanUpperBound: false} : {...validationRules}
            validationRules = withBounds && isNaN(kp.bounds.lower) ? {...validationRules, lowerBoundIsNumeric: false} : {...validationRules}
            validationRules = withBounds && isNaN(kp.bounds.upper) ? {...validationRules, upperBoundIsNumeric: false} : {...validationRules}
            this.$store.dispatch("smpstorage/updateKp", {objName: 'withBounds', guid: this.guid, kpKey: kp.key, obj: {value: withBounds}})
            this.$store.dispatch("smpstorage/updateKp", {objName: 'validationRules', guid: this.guid, kpKey: kp.key, obj: validationRules})
        },


        updateBounds(newBound, bound, kp) {
            let bounds = bound === "upper" ? {lower: kp.bounds.lower, upper: newBound} : {lower: newBound, upper: kp.bounds.upper}
            let validationRules = bound === "upper" 
                                    ? _.isEmpty(kp.bounds.lower) && _.isEmpty(newBound) ? {boundsRq: false} : !_.isEmpty(kp.bounds.lower) && !_.isEmpty(newBound) && +kp.bounds.lower >= +newBound ? {boundsRq: true, lowerBoundLowerThanUpperBound: false} : {boundsRq: true, upperBoundIsNumeric: true, lowerBoundLowerThanUpperBound: true}
                                    : _.isEmpty(kp.bounds.upper) && _.isEmpty(newBound) ? {boundsRq: false} : !_.isEmpty(kp.bounds.upper) && !_.isEmpty(newBound) && +kp.bounds.upper <= +newBound ? {boundsRq: true, lowerBoundLowerThanUpperBound: false} : {boundsRq: true, lowerBoundIsNumeric: true, lowerBoundLowerThanUpperBound: true}
            validationRules = bound === "upper" && isNaN(newBound) ? {...validationRules, upperBoundIsNumeric: false} : {...validationRules}   
            validationRules = bound === "lower" && isNaN(newBound) ? {...validationRules, lowerBoundIsNumeric: false} : {...validationRules}  
            this.$store.dispatch("smpstorage/updateKp", {objName: 'bounds', guid: this.guid, kpKey: kp.key, obj: bounds})
            this.$store.dispatch("smpstorage/updateKp", {objName: 'validationRules', guid: this.guid, kpKey: kp.key, obj: validationRules})
               
        },      

        deleteParameter(step) {
            let kp = this.smp.kpList[step - 1]
            this.$store.dispatch("smpstorage/deleteFromKpList", {guid: this.guid, kp})
            this.step--            
            this.forbiddenStandartParameters = this.forbiddenStandartParameters.filter(x => x.key !== kp.key)
            this.forbiddenStandartParameters.forEach(f => {
                f.forbiddenIds = f.forbiddenIds.filter(x => x !== kp.standartParameter.id)
            })
        },

        stepperKeyUpdate() {
            this.stepperKey = this.stepperKey + '-steppervalid' 
        },
        
        nextStep (n) {       
            this.step = n === this.smp.kpList.length ? 1 : n + 1
        },
        
        prevStep (n) {              
            this.step = n === 1 ? this.smp.kpList.length : n - 1
        },

        recalculateForbiddenStandartParameters(kpList) {
            let selectedStandartParameters = kpList.map(x => x.standartParameter.id)
            return kpList.map(kp => ({forbiddenIds: selectedStandartParameters.filter(x => x !== kp.standartParameter.id), key: kp.key, selectedId: kp.standartParameter.id}))            
        },

        validationBoundsErrors(validationRules, bound) {
            if(!validationRules.boundsRq)
                return ["Выберите хотя бы одну границу"]
            if(!validationRules.lowerBoundLowerThanUpperBound)
                return ["Нижняя граница должна быть меньше"]
            if(!validationRules.lowerBoundIsNumeric && bound === 'lower')
                return ["Значение границы должно быть числом"]
            if(!validationRules.upperBoundIsNumeric && bound === 'upper')
                return ["Значение границы должно быть числом"]
            return []
        }
    },

    computed: {
        smp() {
            return this.$store.getters['smpstorage/currentSmp'](this.guid)
        },
      
        validationIsCorrect() {
            return this.$store.getters['smpstorage/validationIsCorrect'](this.guid)
        },

        standartParameters() {
             return this.$store.getters['smpstorage/standartParameters']
        }
    },

    mounted() {
        this.smp.kpList.length === 0 
            ? this.createParameter() 
            : this.forbiddenStandartParameters = [...this.forbiddenStandartParameters, ...this.recalculateForbiddenStandartParameters(this.smp.kpList)]
    }
}
</script>


