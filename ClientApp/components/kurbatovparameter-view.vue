<template>
    <v-container>
         <v-row v-if="!initialDialog" class="alwaysOnTop">
            <v-flex lg="3" offset-lg="1">
                <v-btn color="indigo" @click="createStandartMeasurementPattern">
                    Добавить элемент
                </v-btn>   
            </v-flex>
        </v-row>
        <v-row>
            <v-flex lg11 offset-lg1>
               <v-stepper v-if="kpArray.length > 0" v-model="step" vertical>               
                    <template v-for="(kp, index) in kpArray">
                        <v-stepper-step 
                            :key="`${index+1}-step`"
                            :step="index + 1"
                            complete
                            editable>
                            {{}}
                        </v-stepper-step>
                        <v-stepper-content
                            :key="`${index+1}-content`"
                            :step="index + 1">
                            <v-card>
                                <v-card-text>
                                <single-kp></single-kp>
                                </v-card-text>
                            </v-card>                                        
                        </v-stepper-content>
                    </template>                          
            </v-stepper>
           </v-flex>
        </v-row>
        <v-row>
            <v-dialog v-model="smpCreateDialog" persistent max-width="400">
                <v-card>
                    <v-card-text>
                        <v-row>
                            <v-flex lg="11" offset-lg="1">
                                <v-text-field readonly v-model="smpName" label="Код"></v-text-field>
                            </v-flex>
                            <v-flex lg="11" offset-lg="1">
                                <v-select v-model="selectedElementSMP"
                                    :items="elementsArray"
                                    item-text="name"
                                    no-data-text="Нет данных"
                                    return-object
                                    outlined
                                    label="Выберите элемент:">
                                </v-select>
                            </v-flex>
                            <v-flex lg="11" offset-lg="1">
                                <v-select v-model="selectedStageSMP"
                                    :items="stagesArray"
                                    item-text="stageName"
                                    no-data-text="Нет данных"
                                    return-object
                                    outlined
                                    label="Выберите этап:">
                                </v-select>
                            </v-flex>
                            <v-flex lg="11" offset-lg="1">
                                <v-select v-model="selectedDividerSMP"
                                    :items="dividersArray"
                                    item-text="name"
                                    no-data-text="Нет данных"
                                    return-object
                                    outlined
                                    label="Выберите приведение:">
                                </v-select>
                            </v-flex>
                        </v-row>
                    </v-card-text>
                    <v-card-actions>
                        <v-spacer></v-spacer>
                        <v-btn color="indigo" @click="smpCreateDialog = false">Закрыть</v-btn>
                        <v-btn v-if="readyToCreateSMP" color="indigo" @click="createSMP">Добавить</v-btn>
                   </v-card-actions>
                </v-card>
            </v-dialog>
        </v-row>
        <v-row>
            <v-dialog v-model="initialDialog" persistent max-width="400">
               <v-card>
                    <v-card-text>
                        <v-row class="mt-6">
                            <v-flex lg="11" offset-lg="1">
                                <v-select v-model="selectedDieTypeId"
                                    :items="dieTypes"
                                    item-text="name"
                                    no-data-text="Нет данных"
                                    item-value="id"
                                    outlined
                                    label="Выберите тип кристалла:">
                                </v-select>
                            </v-flex>
                        </v-row>
                        <v-row class="mt-6"  v-if="selectedDieTypeId">
                            <v-flex lg="11" offset-lg="1">
                                <v-select v-if="mode==='updating'" v-model="selectedPattern"
                                    :items="patterns"
                                    no-data-text="Нет данных"
                                    outlined
                                    label="Выберите шаблон:">
                                </v-select>
                                <v-btn v-else block @click="goToCreatingMode" color="indigo">Создать новый шаблон</v-btn>
                            </v-flex>
                        </v-row>
                        <v-row class="mt-6" v-if="selectedDieTypeId">
                            <v-flex lg="11" offset-lg="1">
                                <v-btn v-if="mode==='updating'" block @click="goToUpdatingMode" color="indigo">Подтвердить выбор</v-btn>
                                <v-btn v-else block @click="goToUpdatingMode(selectedDieType)" color="indigo">Редактировать шаблон</v-btn>
                            </v-flex>
                        </v-row>
                    </v-card-text>
                </v-card>
            </v-dialog>
        </v-row>
        <v-snackbar v-model="snackbar.visible" top>
            {{ snackbar.text }}
            <v-btn color="pink" text @click="snackbar.visible = false">Закрыть</v-btn>
        </v-snackbar>
    </v-container>
</template>

<script>
import singleKp from './kurbatovparametersingle-view.vue';
export default {
    data() {
        return {
            snackbar: {visible: false, text: ""},
            initialDialog: true,
            selectedDieTypeId: "",
            dieTypes: [],
            selectedPattern: {},
            patterns: [],
            step: 0,
            kpArray: [],           
            mode: "",

            //SMP
            smpCreateDialog: false,
            selectedElementSMP: {},
            selectedStageSMP: {},
            selectedDividerSMP: {},
            process: {},
            elementsArray: [],
            stagesArray: [],
            dividersArray: []
        }
    },

    components:
    {
        "single-kp": singleKp,
    },

    methods: {
        async initialize() {
            await this.getAllDieTypes()
        },

        showSnackbar(text) {
            this.snackbar.visible = true
            this.snackbar.text = text
        },

        createStandartMeasurementPattern() {
            this.smpCreateDialog = true
        },

        async goToCreatingMode() {
            this.initialDialog = false
            this.mode = 'creating'
            await this.getProcessByDieId(this.selectedDieTypeId)
                 .then(async () => await this.getElementsByDieType(this.selectedDieTypeId))
                 .then(async () => await this.getStagesByProcessId(this.process))
            await this.getDividers()
        },

        async goToUpdatingMode(selectedDieType) {
            this.mode = 'updating'
            await this.$http
            .get(`/api/standartpattern/dietype/${selectedDieType.id}`)
            .then(response => {this.patterns = response.data; this.selectedPattern = response.data[0] || {}})
            .catch(error => this.showSnackbar("Шаблоны не найдены в БД"))
        },

        async getAllDieTypes() {
            await this.$http
            .get(`/api/dietype/all`)
            .then(response => {this.dieTypes = response.data})
            .catch(error => this.showSnackbar("Типы кристаллов не найдены в БД"))
        },

        async getProcessByDieId(selectedDieTypeId) {
            await this.$http
            .get(`/api/process/dietype/${selectedDieTypeId}`)
            .then(response => this.process = response.data)
            .catch(error => this.showSnackbar(error.response.data[0].message))
        },

        async getElementsByDieType(selectedDieTypeId) {
            await this.$http
            .get(`/api/element/dietype/${selectedDieTypeId}`)
            .then(response => this.elementsArray = response.data)
            .catch(error => this.showSnackbar(error.response.data[0].message))
        },

        async getStagesByProcessId(process) {
            await this.$http
            .get(`/api/stage/process/${process.processId}`)
            .then(response => this.stagesArray = response.data)
            .catch(error => this.showSnackbar(error.response.data[0].message))
        },

        async getDividers() {
            await this.$http
            .get(`/api/divider/all`)
            .then(response => this.dividersArray = response.data)
            .catch(error => this.showSnackbar(error.response.data[0].message))
        }
    },

    computed: {
        readyToCreateSMP() {
            return !_.isEmpty(this.selectedElementSMP) && !_.isEmpty(this.selectedStageSMP) && !_.isEmpty(this.selectedDividerSMP)
        },

        smpName() {
            if(this.readyToCreateSMP)
                return `${this.selectedElementSMP.name}_D${this.selectedDividerSMP.name}_${this.selectedStageSMP.stageName.split(' ').join('')}` 
            return ""
        }
    },

    async mounted() {
        await this.initialize()
    }
}
</script>


<style>
    .alwaysOnTop{
        top: 64px;
        width: 100%;
        position: fixed;
        z-index: 9999;
        opacity: 0.85;
        background-color: #303030;
    }
</style>
