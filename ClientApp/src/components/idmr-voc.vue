<template>
    <v-container grid-list-lg>
        <v-row>
            <v-col lg="2" offset-lg="1">
                <v-autocomplete v-model="selectedWaferId"
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
                <v-select v-if="selectedWaferId"  v-model="selectedDieTypeName"
                                :items="dieTypes"
                                no-data-text="Нет данных"
                                outlined
                                filled
                                item-text="name"
                                item-value="name"
                                label="Тип монитора">
                </v-select>
            </v-col>
            <v-col lg="3">
                <v-checkbox v-if="selectedWaferId" class="mt-3" v-model="showAllMeasurements" label="Показать все операции на пластине"></v-checkbox>
            </v-col>
            <v-col lg="3">
                <v-btn v-if="selectedWaferId" color="indigo" class="mt-3"
                       @click="goToStageTable(selectedWaferId)">Перейти к редактированию этапов</v-btn>
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
    <v-row justify="center">
        <chbx-dialog :initialArray="deleting.measurementRecordingList"
                     :state="deleting.dialog"
                     keyProp="id" labelProp="name"
                     title="Выберите измерения для удаления"
                     confirmText="Удалить"
                     @confirm="deleteSelectedMeasurements"
                     @cancel="wipeDeleting">
        </chbx-dialog>
  </v-row>
  <v-row justify="center">
    <v-dialog v-model="editing.dialog" persistent max-width="450px">
        <v-card>
        <v-card-title>
          <v-chip color="pink" label text-color="white"><v-icon left>warning</v-icon>Название операции вводить без оп.</v-chip>
        </v-card-title>
        <v-card-text style="height: 200px;">
            <v-text-field outlined label="Старое название операции" readonly v-model="editing.measurementRecording.name"></v-text-field>
            <v-text-field outlined label="Новое название операции" v-model="editing.newName"></v-text-field>
        </v-card-text>
        <v-card-actions class="d-flex justify-lg-space-between">
           <v-btn color="indigo" @click="wipeEditing()">Закрыть</v-btn>
           <v-btn v-if="editing.newName && editing.newName!==editing.measurementRecording.name"
                  color="success"
                  @click="updateMeasurementRecordingName(editing.measurementRecording, editing.newName)">
                  Обновить название
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
    </v-container>
</template>

<script>
import checkboxSelectDialog from './Dialog/checkboxselect-dialog.vue';

export default {
  components: {
    'chbx-dialog': checkboxSelectDialog,
  },
  props: {
    waferId: String,
    dieTypeName: String,
  },

  data() {
    return {
      e1: 1,
      selectedWaferId: this.waferId,
      selectedDieTypeName: this.dieTypeName,
      showAllMeasurements: false,
      wafers: [],
      dieTypes: [],
      allStagesArray: [],
      avElements: [],
      stagesArray: [],
      deleting: { dialog: false, measurementRecordingList: [], selectedMeasurements: [] },
      editing: { dialog: false, measurementRecording: {}, newName: '' },
    };
  },

  methods:
    {
      showSnackbar(text) {
        this.$store.dispatch('alert/success', text);
      },

      showLoading(text) {
        this.$store.dispatch('loading/show', text);
      },

      closeLoading() {
        this.$store.dispatch('loading/cloak');
      },

      async goToStageTable(waferId) {
        await this.$http.get(`/api/process/waferid/${waferId}`)
          .then((response) => {
            const { processId } = response.data;
            this.$router.push({ name: 'stagetable', params: { processId } });
          })
          .catch(() => {
            this.showSnackbar('Ошибка сервера');
          });
      },

      wipeDeleting() {
        this.deleting.dialog = false;
        this.deleting.measurementRecordingList = [];
        this.deleting.selectedMeasurements = [];
      },

      wipeEditing() {
        this.editing.dialog = false;
        this.editing.newName = '';
      },

      updateName(measurementRecording) {
        this.editing.dialog = true;
        this.editing.measurementRecording = measurementRecording;
      },

      deleteMeasurement(stage) {
        this.deleting.dialog = true;
        this.deleting.measurementRecordingList = _.cloneDeep(stage.measurementRecordingList);
      },

      async initialize() {
        await this.getWafers();
      },

      async getStagesByWaferId(waferId) {
        let mode = this.selectedDieTypeName;
        if (this.showAllMeasurements) {
          mode = 'all';
        }
        await this.$http.get(`/api/measurementrecording/wafer/${waferId}/dietype/${mode}`)
          .then((response) => {
            if (response.status === 200) {
              this.stagesArray = response.data;
            }
            if (response.status === 204) {
              this.stagesArray = [];
            }
          })
          .catch((error) => {
            this.showSnackbar(error);
          });
      },

      async getDieTypesByWaferId(waferId) {
        await this.$http.get(`/api/dietype/wafer/${waferId}`)
          .then((response) => {
            this.stagesArray = [];
            this.dieTypes = response.data;
          })
          .catch((error) => {
            this.showSnackbar(error);
          });
      },

      async deleteSelectedMeasurements(selectedMeasurements) {
        await this.$http.delete('/api/measurementrecording/delete/list', { data: selectedMeasurements })
          .then(() => {
            this.stagesArray[this.e1 - 1].measurementRecordingList = this.stagesArray[this.e1 - 1]
              .measurementRecordingList.filter((x) => !selectedMeasurements.includes(x.id));
            this.showSnackbar(`Удалено измерений -> ${selectedMeasurements.length}`);
            this.wipeDeleting();
          })
          .catch(() => {
            this.showSnackbar('Ошибка при удалении');
          });
      },

      async updateMeasurementRecordingName(measurementRecording, newName) {
        const measurementRecordingViewModel = { id: measurementRecording.id, name: newName };
        await this.$http.post('/api/measurementrecording/edit/name', measurementRecordingViewModel)
          .then((response) => {
            this.showSnackbar('Название изменено');
            this.stagesArray[this.e1 - 1].measurementRecordingList.find((x) => x.id === response.data.id).name = response.data.name;
            this.wipeEditing();
          })
          .catch(() => {
            this.showSnackbar('Ошибка при изменении названия');
          });
      },

      async getAvElements(dieTypeName) {
        await this.$http.get(`/api/element/dietype/name/${dieTypeName}`)
          .then((response) => {
            this.avElements = response.data;
          })
          .catch((error) => {
            this.showSnackbar(error);
          });
      },

      async getAllStages(waferId) {
        await this.$http.get(`/api/stage/wafer/${waferId}`)
          .then((response) => {
            this.allStagesArray = response.data;
          })
          .catch((error) => {
            this.showSnackbar(error);
          });
      },

      async getWafers() {
        await this.$http.get('/api/wafer/all').then((response) => {
          this.wafers = response.data;
        });
      },

      async updateStageOnIdmr(idmr, oldStageId, newStageId) {
        const measurementRecording = this.stagesArray.find((x) => x.id === oldStageId).measurementRecordingList.find((x) => x.id === idmr);
        const newStage = this.stagesArray.find((x) => x.id === newStageId);
        if (!newStage) {
          const stage = this.allStagesArray.find((x) => x.stageId === newStageId);
          this.stagesArray.push({ id: stage.stageId, name: stage.stageName, measurementRecordingList: Array(1).fill(measurementRecording) });
        } else {
          newStage.measurementRecordingList.push(measurementRecording);
        }

        const newOldStage = this.stagesArray.find((x) => x.id === oldStageId).measurementRecordingList.filter((x) => x.id !== idmr);
        if (newOldStage.length === 0) {
          this.stagesArray = this.stagesArray.filter((x) => x.id !== oldStageId);
        } else {
          this.stagesArray.find((x) => x.id === oldStageId).measurementRecordingList = newOldStage;
        }

        await this.$http({
          method: 'post',
          url: '/api/measurementrecording/update-stage',
          data: { stageId: newStageId, measurementRecordingId: idmr },
          config: {
            headers: {
              Accept: 'application/json',
              'Content-Type': 'application/json',
            },
          },
        })
          .then(() => {
            this.showSnackbar('Этап успешно изменен');
          })
          .catch(() => {
            this.showSnackbar('Произошла ошибка при изменении этапа');
          });
      },

      async updateElementOnIdmr(idmr, elementId) {
        await this.$http({
          method: 'post',
          url: '/api/element/updateElementOnIdmr',
          data: { elementId, measurementRecordingId: idmr },
          config: {
            headers: {
              Accept: 'application/json',
              'Content-Type': 'application/json',
            },
          },
        })
          .then((response) => {
            this.showSnackbar(`Элемент успешно изменен на ${response.data.name}`);
          })
          .catch(() => {
            this.showSnackbar('Произошла ошибка при изменении элемента');
          });
      },
    },

  watch: {
    selectedWaferId: {
      immediate: true,
      async handler(newVal, oldVal) {
        if (newVal) {
          if (oldVal) {
            await this.getDieTypesByWaferId(newVal);
            this.selectedDieTypeName = this.dieTypes[0].name;
          } else {
            await this.getDieTypesByWaferId(newVal);
          }
          await this.getAllStages(newVal);
        }
      },
    },

    async selectedDieTypeName(selectedDieTypeName) {
      if (selectedDieTypeName !== '') {
        this.showLoading('Загрузка...');
        this.$router.push({ name: 'idmrvoc', params: { waferId: this.selectedWaferId, dieTypeName: selectedDieTypeName } });
        await this.getStagesByWaferId(this.selectedWaferId);
        await this.getAvElements(selectedDieTypeName);
        this.closeLoading();
      }
    },

    async showAllMeasurements() {
      this.showLoading('Загрузка...');
      await this.getStagesByWaferId(this.selectedWaferId);
      await this.getAvElements(this.selectedDieTypeName);
      this.closeLoading();
    },
  },

  async mounted() {
    this.initialize();
  },
};
</script>
