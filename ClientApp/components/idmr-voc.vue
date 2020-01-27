<template>
    <v-container grid-list-lg>
        <v-row>
            <v-col lg="3" offset-lg="1">
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
            <v-col lg="3">
                <v-select       v-model="selectedDieType" 
                                :items="dieTypes"
                                outlined
                                filled
                                item-text="name"
                                item-value="id"
                                label="Тип монитора">
                </v-select>
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
                            <v-btn color="indigo" class="mt-4" @click="deleteMeasurement(stage)">Удаление операций</v-btn>
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
                                        class="mt-4"                                       
                                        label
                                        color="indigo"
                                        text-color="white">
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
    </v-container>    
</template>

<script>
export default {
    data() {
        return {
           snackbar: {visible: false, text: ""},
           e1: 0,
           waferId: "",
           wafers: [],
           dieTypes: [],
           selectedDieType: {},
           allStagesArray: [],
           avElements: [],
           stagesArray: [],
           deleting: {dialog: false, measurementRecordingList: [], selectedMeasurements: []},
           loading: false
        }
    },

    methods: 
    {       
        showSnackbar(text) {
            this.snackbar.visible = true
            this.snackbar.text = text
        },

        wipeDeleting() {
              this.deleting.dialog = false
              this.deleting.measurementRecordingList = []
              this.deleting.selectedMeasurements = []
        },

        deleteMeasurement(stage) {
            this.deleting.dialog = true
            this.deleting.measurementRecordingList = _.cloneDeep(stage.measurementRecordingList)
        },

        async initialize() {
            await this.getWafers() 
        },

        async getStagesByWaferId(waferId) {
                await   this.$http
                        .get(`/api/measurementrecording/wafer/${waferId}/stage`)
                        .then(response => {
                            this.stagesArray = response.data
                        })
                        .catch((error) => {
                            alert(error)
                        });
        },        

        async getDieTypesByWaferId(waferId) {
            await   this.$http
                    .get(`/api/dietype/wafer/${waferId}`)
                    .then(response => {
                        this.dieTypes = response.data
                        this.selectedDieType = this.dieTypes[0]
                    })
                    .catch((error) => {
                        alert(error)
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

        async getAvElements(dieType) {             
            await   this.$http
                        .get(`/api/element/dietype/${dieType.id}`)
                        .then(response => {
                            this.avElements = response.data
                        })
                        .catch((error) => {
                            alert(error)
                        });
        },

        async getAllStages(waferId) {
            await   this.$http
                        .get(`/api/stage/getstagesbywaferid?waferId=${waferId}`)
                        .then(response => {
                            this.allStagesArray = response.data
                        })
                        .catch((error) => {
                            alert(error)
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
            this.loading = true
            await this.getDieTypesByWaferId(newVal).then(async () => await this.getAvElements(this.selectedDieType))
            await this.getAllStages(newVal)           
            await this.getStagesByWaferId(newVal).then(() => this.loading = false)
        },
    },

    async mounted() {
        this.initialize()
    }
}
</script>