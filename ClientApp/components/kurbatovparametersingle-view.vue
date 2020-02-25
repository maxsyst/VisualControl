<template>
    <v-container>
        <v-row>
            <v-col lg="2">
                <v-select :value="smp.element"
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
                <v-select :value="smp.stage"
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
                <v-select :value="smp.divider"
                    :items="$store.getters['dividers/getAll']"
                    item-text="name"
                    no-data-text="Нет данных"
                    return-object
                    outlined
                    @change="updateDivider($event)"
                    label="Выберите периферию:">
                </v-select>
            </v-col>
        </v-row>
        <v-row>
            <v-col lg="12">
                <v-stepper v-model="step">
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
                                                        :items="standartParameters"
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
                                                            readonly outlined label="Расширенное название:">
                                            </v-text-field>
                                        </v-col>
                                        <v-col lg="4">
                                            <v-text-field   :value="parameter.standartParameter.parameterNameStat" 
                                                            readonly outlined label="Системное название:">
                                            </v-text-field>
                                        </v-col>
                                    </v-row>
                                    <v-row>                                        
                                         <v-col lg="2">
                                            <v-text-field   :value="parameter.bounds.lower" 
                                                            @change="updateBounds($event, 'lower', parameter)" outlined label="Нижняя граница:">
                                            </v-text-field>
                                        </v-col>
                                        <v-col lg="2">
                                            <v-text-field   :value="parameter.bounds.upper" 
                                                            @change="updateBounds($event, 'upper', parameter)" outlined label="Верхняя граница:">
                                            </v-text-field>
                                        </v-col>        
                                        <v-col lg="3">
                                             <v-switch
                                                v-model="parameter.withBounds"
                                                color='primary'
                                                :label="parameter.withBounds ? `Включить границы` : `Не включать границы`">
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
import { uuid } from 'vue-uuid';
export default {
    props: {
        guid: String
    },

    data() {
       return {
           step: 0
       }
    },

    methods: {
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
            let kp = {
                standartParameter: {parameterName: "", russianParameterName: "", parameterNameStat: "", specialRon: false, dividerNeed: false},
                bounds: {lower: "", upper: ""},
                key: this.$uuid.v1()
            }
            this.$store.dispatch("smpstorage/addToKpList", {guid: this.guid, kp})
            this.step++
        },

        updateStandartParameter(standartParameter, kp) {
            this.$store.dispatch("smpstorage/updateKp", {objName: 'standartParameter', guid: this.guid, kpKey: kp.key, obj: standartParameter})
        },

        updateBounds(newBound, bound, kp) {
            let bounds = bound === "upper" ? {lower: kp.bounds.lower, upper: newBound} : {lower: newBound, upper: kp.bounds.upper}
            this.$store.dispatch("smpstorage/updateKp", {objName: 'bounds', guid: this.guid, kpKey: kp.key, obj: bounds})
        },

        deleteParameter(step) {
            let kp = this.smp.kpList[step - 1]
            this.$store.dispatch("smpstorage/deleteFromKpList", {guid: this.guid, kp})
            this.step--            
        },        

        nextStep (n) {       
            this.step = n === this.smp.kpList.length ? 1 : n + 1
        },
        
        prevStep (n) {              
            this.step = n === 1 ? this.smp.kpList.length : n - 1
        }
    },

    computed: {
        smp() {
            return this.$store.getters['smpstorage/currentSmp'](this.guid)
        },

        standartParameters() {
             return this.$store.getters['smpstorage/standartParameters']
        }
    },

    mounted() {
        this.createParameter()
    }
}
</script>

