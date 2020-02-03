<template>
    <v-container grid-list-lg>
        <v-row>
            <v-col lg="2" offset-lg="1">
                <v-autocomplete v-model="waferId"
                                    :items="wafers"
                                    no-data-text="Нет данных"
                                    item-text="waferId"
                                    item-value="waferId"
                                    filled
                                    outlined
                                    label="Номер пластины">
                </v-autocomplete>
            </v-col>
            <v-col lg="2">
                <v-select v-if="waferId"  v-model="selectedDieType" 
                                :items="dieTypes"
                                no-data-text="Нет данных"
                                outlined
                                filled
                                item-text="name"
                                item-value="id"
                                label="Тип монитора">
                </v-select>
            </v-col>
            <v-col lg="3">
                <v-checkbox v-if="waferId" class="mt-3" v-model="showAllMeasurements" label="Показать все операции на пластине"></v-checkbox>
            </v-col>
            <v-col lg="3">
                <v-btn v-if="waferId" color="indigo" class="mt-3" @click="goToStageTable(waferId)">Перейти к редактированию этапов</v-btn>
            </v-col>
        </v-row>
       
        <v-row>
            <v-col lg="10" offset-lg="1">
               <v-stepper v-if="stagesArray.length>0" v-model="e1" vertical>               
                    <template v-for="(stage, index) in stagesArray">
                    <v-row :key="`${index+1}-step`">
                        <v-col lg="10">
                            <v-stepper-step                            
                                :step="index + 1"
                                color="indigo"
                                complete
                                editable>
                                {{stage.name}}
                                
                            </v-stepper-step>
                        </v-col>
                        <v-col lg="2">
                            <v-btn v-if="index===e1-1" color="indigo" class="mt-4" @click="deleteMeasurement(stage)">Удаление операций</v-btn>
                        </v-col>
                    </v-row>
                    
                    <v-stepper-content
                        :key="`${index+1}-content`"
                        :step="index + 1">
                       
                        <v-card style="max-height: 450px"
                        class="overflow-y-auto">
                            <v-card-text>                              
                           
                             <v-row v-for="idmr in stage.measurementRecordingList" :key="idmr.id">
                                <v-col lg="2">                                    
                                    <v-chip
                                        close
                                        close-icon="edit"
                                        class="mt-4"                                       
                                        label
                                        color="indigo"
                                        text-color="white"
                                        @click:close="updateName(idmr)">
                                    {{idmr.name}}
                                    </v-chip>
                                </v-col>
                                <v-col lg="6">
                                    <v-select :value="stage.id"
                                              :items="allStagesArray"
                                              no-data-text="Нет данных"
                                              item-value="stageId" 
                                              item-text="stageName"
                                              outlined
                                              label="Выберите этап"
                                              @change="updateStageOnIdmr(idmr.id, stage.id, $event)">
                                    </v-select>                                        
                                </v-col>
                                 <v-col lg="4">
                                    <v-select v-model="idmr.element"
                                              :items="avElements"
                                              no-data-text="Нет данных"
                                              item-value="elementId" 
                                              item-text="name"
                                              outlined
                                              label="Выберите элемент"
                                              @change="updateElementOnIdmr(idmr.id, idmr.element)">
                                    </v-select>                                     
                                </v-col>                              
                            </v-row>
                          </v-card-text>
                        </v-card>                                        
                    </v-stepper-content>
                    </template>                          
            </v-stepper>
            <v-alert v-else color="indigo">               
                <div>Нет этапов для отображения</div>
            </v-alert>
           </v-col>
        </v-row>
        <v-snackbar v-model="snackbar.visible" top>
        {{ snackbar.text }}
        <v-btn color="pink" text @click="snackbar.visible = false">Закрыть</v-btn>
        </v-snackbar>
        <v-dialog
            v-model="loading"
            hide-overlay
            persistent
            width="300">
        <v-card color="indigo" dark>
            <v-card-text>
            Формирование этапов измерений
            <v-progress-linear
                indeterminate
                color="white"
                class="mb-0"
            ></v-progress-linear>
            </v-card-text>
        </v-card>
    </v-dialog>
    <v-row justify="center">
    <v-dialog v-model="deleting.dialog" scrollable max-width="450px">
      <v-card>
        <v-card-title>Выберите измерения для удаления</v-card-title>
        <v-divider></v-divider>
        <v-card-text style="height: 300px;" >
            <v-checkbox v-for="measurement in deleting.measurementRecordingList" :key="measurement.id" v-model="deleting.selectedMeasurements" :label="measurement.name" :value="measurement.id"></v-checkbox>
        </v-card-text>
        <v-divider></v-divider>
        <v-card-actions class="d-flex justify-lg-space-between">
          <v-btn color="indigo" @click="wipeDeleting()">Закрыть</v-btn>
          <v-btn color="success" v-if="deleting.selectedMeasurements.length>0" @click="deleteSelectedMeasurements(deleting.selectedMeasurements)">Удалить</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
  <v-row justify="center">
    <v-dialog v-model="editing.dialog" persistent max-width="450px">
        <v-card>
        <v-card-title><v-chip color="pink" label text-color="white"><v-icon left>warning</v-icon>Название операции вводить без оп.</v-chip></v-card-title>
        <v-card-text style="height: 200px;">           
            <v-text-field outlined label="Старое название операции" readonly v-model="editing.measurementRecording.name"></v-text-field>          
            <v-text-field outlined label="Новое название операции" v-model="editing.newName"></v-text-field>
        </v-card-text>
        <v-card-actions class="d-flex justify-lg-space-between">          
           <v-btn color="indigo" @click="wipeEditing()">Закрыть</v-btn>
           <v-btn v-if="editing.newName && editing.newName!==editing.measurementRecording.name" color="success" @click="updateMeasurementRecordingName(editing.measurementRecording, editing.newName)">Обновить название</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
    </v-container>    
</template>

<script>
export default {
    data() {
        return {
           snackbar: {visible: false, text: ""},
           e1: 1,
           showAllMeasurements: false,
           waferId: "",
           wafers: [],
           dieTypes: [],
           selectedDieType: {},
           allStagesArray: [],
           avElements: [],
           stagesArray: [],
           deleting: {dialog: false, measurementRecordingList: [], selectedMeasurements: []},
           editing: {dialog: false, measurementRecording: {}, newName: ""},
           loading: false
        }
    },

    methods: 
    {       
        showSnackbar(text) {
            this.snackbar.visible = true
            this.snackbar.text = text
        },

        async goToStageTable(waferId) {
            await this.$http.get(`/api/process/waferid/${waferId}`)
            .then(response => {
                let processId = response.data.processId
                this.$router.push({ name: 'stagetable', params: {processId: processId}})
            })
            .catch((error) => {
                this.showSnackbar("Ошибка серевера")
            });
        },

        wipeDeleting() {
              this.deleting.dialog = false
              this.deleting.measurementRecordingList = []
              this.deleting.selectedMeasurements = []
        },

        wipeEditing() {
              this.editing.dialog = false
              this.editing.newName = ""
        },

        updateName(measurementRecording) {
            this.editing.dialog = true
            this.editing.measurementRecording = measurementRecording
        },

        deleteMeasurement(stage) {
            this.deleting.dialog = true
            this.deleting.measurementRecordingList = _.cloneDeep(stage.measurementRecordingList)
        },

        async initialize() {
            await this.getWafers() 
        },

        async getStagesByWaferId(waferId, dieTypeId) {
            if(this.showAllMeasurements) {
                 dieTypeId = 0
            }               
            await this.$http.get(`/api/measurementrecording/wafer/${waferId}/dietype/${dieTypeId}`)
            .then(response => {
                if(response.status === 200) {
                    this.stagesArray = response.data
                }
                if(response.status === 204) {
                    this.stagesArray = []
                }                            
            })
            .catch((error) => {
                this.showSnackbar(error)
            });
        },        

        async getDieTypesByWaferId(waferId) {
            await this.$http.get(`/api/dietype/wafer/${waferId}`)
            .then(response => {
                this.stagesArray = []
                this.dieTypes = response.data               
            })
            .catch((error) => {
                this.showSnackbar(error)
            });
        },

        async deleteSelectedMeasurements(selectedMeasurements) {
            await this.$http.delete('/api/measurementrecording/delete/list', { data: selectedMeasurements })
            .then(response => {           
                this.stagesArray[this.e1 - 1].measurementRecordingList = this.stagesArray[this.e1 - 1].measurementRecordingList.filter(x => !selectedMeasurements.includes(x.id))                  
                this.showSnackbar(`Удалено измерений -> ${selectedMeasurements.length}`)
                this.wipeDeleting()
            })
            .catch((error) => {
                this.showSnackbar(`Ошибка при удалении`)   
            });
        },

        async updateMeasurementRecordingName(measurementRecording, newName) {
            let measurementRecordingViewModel = {id: measurementRecording.id, name: newName}
            await this.$http.post('/api/measurementrecording/edit/name', measurementRecordingViewModel)
            .then((response) => {
                this.showSnackbar("Название изменено")
                this.stagesArray[this.e1 - 1].measurementRecordingList.find(x => x.id == response.data.id).name = response.data.name
                this.wipeEditing()
            })
            .catch((error) => {
                this.showSnackbar("Ошибка при изменении названия")
            });
        },

        async getAvElements(dieTypeId) {             
            await this.$http.get(`/api/element/dietype/${dieTypeId}`)
            .then(response => {
                this.avElements = response.data
            })
            .catch((error) => {
                this.showSnackbar(error)
            });
        },

        async getAllStages(waferId) {
            await this.$http.get(`/api/stage/wafer/${waferId}`)
            .then(response => {
                this.allStagesArray = response.data
            })
            .catch((error) => {
               this.showSnackbar(error)
            });
        },

        async getWafers() {
            await this.$http.get(`/api/wafer/all`).then(response => {
                 this.wafers = response.data;
            });
        },

        async updateStageOnIdmr(idmr, oldStageId, newStageId) {
            let measurementRecording = this.stagesArray.find(x => x.id === oldStageId).measurementRecordingList.find(x => x.id === idmr)
            let newStage = this.stagesArray.find(x => x.id === newStageId)
            if(!newStage) {
                let stage = this.allStagesArray.find(x => x.stageId === newStageId);
                this.stagesArray.push({id: stage.stageId, name: stage.stageName, measurementRecordingList: Array(1).fill(measurementRecording)})
                this.e1 = this.stagesArray.length
            } else {
                newStage.measurementRecordingList.push(measurementRecording)
                this.e1 = this.stagesArray.findIndex(x => x.id === newStageId) + 1
            }
            
            let newOldStage = this.stagesArray.find(x => x.id === oldStageId).measurementRecordingList.filter(x => x.id != idmr)
            if(newOldStage.length === 0) {
                this.stagesArray = this.stagesArray.filter(x => x.id !== oldStageId)
            } else {
                this.stagesArray.find(x => x.id === oldStageId).measurementRecordingList = newOldStage
            } 
          
           
            const response = await this.$http({
                method: "post",
                url: `/api/measurementrecording/update-stage`, 
                data: { stageId: newStageId, measurementRecordingId: idmr}, 
                config: {
                    headers: {
                                'Accept': "application/json",
                                'Content-Type': "application/json"
                             }
                }
            })
            .then(response => {
                this.showSnackbar(`Этап успешно изменен`)
            })
            .catch(error => {
                this.showSnackbar("Произошла ошибка при изменении этапа")
            });
        },

        async updateElementOnIdmr(idmr, elementId) {
            const response = await this.$http({
                method: "post",
                url: `/api/element/updateElementOnIdmr`, 
                data: { elementId: elementId, measurementRecordingId: idmr}, 
                config: {
                    headers: {
                                'Accept': "application/json",
                                'Content-Type': "application/json"
                             }
                }
            })
            .then(response => {
                this.showSnackbar(`Элемент успешно изменен на ${response.data.name}`)
            })
            .catch(error => {
                this.showSnackbar("Произошла ошибка при изменении элемента")
            });
        }
    },

    watch: {
        waferId: async function(newVal, oldVal) {
            this.selectedDieType = 0
            this.$router.push({ name: `idmrvoc`, params: {waferId: newVal, dieTypeId: ""} })
            await this.getDieTypesByWaferId(newVal).then(() => this.selectedDieType = this.dieTypes[0].id)
            await this.getAllStages(newVal)              
        },

        selectedDieType: async function(newVal, oldVal) {
            if(newVal !== 0) {
                this.loading = true
                this.$router.push({ name: `idmrvoc`, params: {waferId: this.waferId, dieTypeId: newVal} })
                await this.getStagesByWaferId(this.waferId, newVal).then(async () => await this.getAvElements(newVal)).then(() => this.loading = false)
            }
         
        },

        showAllMeasurements: async function(newVal) {
            this.loading = true
            await this.getStagesByWaferId(this.waferId, this.selectedDieType).then(async () => await this.getAvElements(this.selectedDieType)).then(() => this.loading = false)
        }
    },

    async mounted() {
        this.initialize()
    }
}
</script>