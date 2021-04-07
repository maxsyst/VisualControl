<template>
    <v-container>
         <v-row v-if="!initialDialog" class="alwaysOnTop">
            <v-col lg="2" offset-lg="1">
                <v-btn v-if="validation && patternName && smpArray.length > 0" color="success" block @click="savePattern(smpArray)">
                    {{mode==='creating' ? 'Сохранить шаблон' : 'Обновить шаблон'}}
                </v-btn>
            </v-col>
            <v-col lg="2">
                <v-btn v-if="mode==='updating'" color="pink" block @click="deletePattern(selectedPattern)">
                    Удалить шаблон
                </v-btn>
            </v-col>
            <v-col lg="2" offset-lg="1">
                <v-text-field v-if="mode==='creating'" outlined v-model="patternName" :error-messages="patternName ? []
                                                                            : 'Введите название шаблона'" label="Название шаблона"></v-text-field>
                 <v-select  v-else
                            v-model="selectedPattern"
                            :items="patterns"
                            no-data-text="Нет данных"
                            return-object
                            item-text="name"
                            outlined
                            @change="changeSelectedPattern(selectedPattern)"
                            label="Выберите шаблон:">
                </v-select>
            </v-col>
             <v-col lg="2">
                <v-btn color="indigo" block @click="createStandartMeasurementPattern">
                    Добавить элемент
                </v-btn>
            </v-col>
        </v-row>
        <v-row style="margin-top: 64px;">
            <v-col lg="11" offset-lg="1">
               <v-stepper v-if="smpArray.length > 0" v-model="step" vertical>
                    <template v-for="(smp, index) in smpArray">
                        <v-stepper-step
                            :key="`${index+1}-step`"
                            :step="index + 1"
                            :color="$store.getters['smpstorage/validationIsCorrect'](smp.guid) ? 'green' : 'red'"
                            complete
                            editable>
                            {{smp.name}}
                        </v-stepper-step>
                        <v-stepper-content
                            :key="`${index+1}-content`"
                            :step="index + 1">
                            <v-card>
                                <v-card-text>
                                <single-kp :guid="smp.guid" @chbx-dialog="showCopyDialog" @delete-smp="deleteSmp"></single-kp>
                                </v-card-text>
                            </v-card>
                        </v-stepper-content>
                    </template>
            </v-stepper>
           </v-col>
        </v-row>
        <v-row>
            <v-dialog v-model="smpCreateDialog" persistent max-width="400">
                <v-card>
                    <v-card-text>
                        <v-row>
                            <v-col lg="12">
                                <v-text-field readonly v-model="smpName" label="Код"></v-text-field>
                            </v-col>
                            <v-col lg="12">
                                <v-select v-model="selectedElementSMP"
                                    :items="elementsArray"
                                    item-text="name"
                                    no-data-text="Нет данных"
                                    return-object
                                    outlined
                                    label="Выберите элемент:">
                                </v-select>
                            </v-col>
                            <v-col lg="12">
                                <v-select v-model.trim="selectedStageSMP"
                                    :items="stagesArray"
                                    item-text="stageName"
                                    no-data-text="Нет данных"
                                    return-object
                                    outlined
                                    label="Выберите этап:">
                                </v-select>
                            </v-col>
                            <v-col lg="12">
                                <v-select v-model="selectedDividerSMP"
                                    :items="dividersArray"
                                    item-text="name"
                                    no-data-text="Нет данных"
                                    return-object
                                    outlined
                                    label="Выберите периферию:">
                                </v-select>
                            </v-col>
                        </v-row>
                    </v-card-text>
                    <v-card-actions>
                        <v-spacer></v-spacer>
                        <v-btn color="pink" @click="smpCreateDialog = false">Закрыть</v-btn>
                        <v-btn v-if="readyToCreateSMP" color="indigo" @click="createSmp">Добавить</v-btn>
                   </v-card-actions>
                </v-card>
            </v-dialog>
        </v-row>
        <v-row justify="center">
            <chbx-dialog :initialArray="$store.getters['smpstorage/elementsToCopy'](copyguid)"
                         :state="copyDialog"
                         keyProp="elementId" valueProp="elementId"
                         title="Выберите элементы для копирования"
                         confirmText="Скопировать"
                         @confirm="copySmp"
                         @cancel="wipeCopy">
            </chbx-dialog>
         </v-row>
        <v-row>
            <v-dialog v-model="initialDialog" persistent max-width="400">
               <v-card>
                    <v-card-text>
                        <v-row class="mt-6">
                            <v-col lg="12">
                                <v-select v-model="selectedDieTypeId"
                                    :items="dieTypes"
                                    item-text="name"
                                    no-data-text="Нет данных"
                                    item-value="id"
                                    outlined
                                    @change="selectedDieTypeIdChanged($event)"
                                    label="Выберите тип кристалла:">
                                </v-select>
                            </v-col>
                        </v-row>
                        <v-row class="mt-6" v-if="selectedDieTypeId">
                            <v-col lg="12">
                                <v-select v-if="mode==='updating'" v-model="selectedPattern"
                                    :items="patterns"
                                    no-data-text="Нет данных"
                                    return-object
                                    item-text="name"
                                    outlined
                                    label="Выберите шаблон:">
                                </v-select>
                                <v-btn v-else block @click="goToCreatingMode" color="indigo">Создать новый шаблон</v-btn>
                            </v-col>
                        </v-row>
                        <v-row class="mt-6" v-if="selectedDieTypeId">
                            <v-col lg="12">
                                <v-btn v-if="mode==='updating'" block @click="changeSelectedPattern(selectedPattern)" color="indigo">Подтвердить выбор</v-btn>
                                <v-btn v-else block @click="goToUpdatingMode(selectedDieTypeId)" color="indigo">Редактировать шаблон</v-btn>
                            </v-col>
                        </v-row>
                    </v-card-text>
                </v-card>
            </v-dialog>
        </v-row>
    </v-container>
</template>

<script>
import singleKp from './kurbatovparametersingle-view.vue';
import { createSmp as createSmpFromService, restoreFromVm as restoreFromVmFromService } from '../services/smp.service.js';
import {
  StandartMeasurementPatternFullViewModel, StandartPattern, StandartMeasurementPattern, KurbatovParameter, KurbatovParameterBorders,
} from '../models/kurbatovparameter.js';
import checkboxSelectDialog from './Dialog/checkboxselect-dialog.vue';

export default {
  data() {
    return {
      initialDialog: true,
      copyDialog: false,
      selectedDieTypeId: '',
      dieTypes: [],
      selectedPattern: {},
      patterns: [],
      step: 0,
      mode: '',
      patternName: '',
      // SMP
      smpCreateDialog: false,
      selectedElementSMP: {},
      selectedStageSMP: {},
      selectedDividerSMP: {},
      process: {},
      copyguid: '',
    };
  },

  components:
    {
      'single-kp': singleKp,
      'chbx-dialog': checkboxSelectDialog,
    },

  methods: {

    createSmpFromService,
    restoreFromVm: restoreFromVmFromService,

    async initialize() {
      await this.getAllDieTypes();
      await this.getStandartParameters();
    },

    showSnackbar(text) {
      this.$store.dispatch('alert/success', text);
    },

    showLoading(text) {
      this.$store.dispatch('loading/show', text);
    },

    closeLoading() {
      this.$store.dispatch('loading/cloak');
    },

    wipeCopy() {
      this.copyDialog = false;
      this.showSnackbar('Копирование отменено');
    },

    showCopyDialog(guid) {
      this.copyDialog = true;
      this.copyguid = guid;
    },

    selectedDieTypeIdChanged(dieTypeId) {
      this.$router.push({ name: 'kurbatovparameter-initial-typeisselected', params: { dieType: dieTypeId } });
    },

    copySmp(selectedElementIds) {
      const parentSmp = this.$store.getters['smpstorage/currentSmp'](this.copyguid);
      selectedElementIds.forEach((elementId) => {
        const element = this.elementsArray.find((e) => e.elementId === elementId);
        const name = `${element.name}_${parentSmp.stage.stageName.split(' ').join('+')}_${parentSmp.divider.name === 'Нет' ? 'No' : parentSmp.divider.name}µm`;
        this.createSmpFromService({
          name, mslName: '', element: { ...element }, stage: { ...parentSmp.stage }, divider: { ...parentSmp.divider }, kpList: [...parentSmp.kpList],
        });
      });
      this.showSnackbar('Копирование завершено');
      this.copyDialog = false;
      this.copyguid = '';
    },

    createStandartMeasurementPattern() {
      this.smpCreateDialog = true;
    },

    createSmp() {
      if (!this.$store.getters['smpstorage/existInSmpArray'](this.smpName)) {
        this.createSmpFromService({
          name: this.smpName, mslName: '', element: this.selectedElementSMP, stage: this.selectedStageSMP, divider: this.selectedDividerSMP, kpList: [],
        });
        this.smpCreateDialog = false;
      } else {
        this.showSnackbar('Параметр с таким именем уже добавлен');
      }
    },

    deleteSmp(guid) {
      this.$store.dispatch('smpstorage/deleteSmp', guid);
      this.step = 0;
      this.showSnackbar('Удаление успешно');
    },

    reset() {
      this.mode = '';
      this.initialDialog = true;
      this.$router.push({ name: 'kurbatovparameter' });
      this.$store.dispatch('smpstorage/reset');
    },

    async routeHandler(routeName) {
      if (routeName === 'kurbatovparameter') {
        this.initialDialog = true;
        this.selectedDieTypeId = '';
      }
      if (routeName === 'kurbatovparameter-initial-typeisselected') {
        this.initialDialog = true;
        this.selectedDieTypeId = +this.$route.params.dieType;
      }
      if (routeName === 'kurbatovparameter-creating') {
        this.initialDialog = false;
        this.selectedDieTypeId = +this.$route.params.dieType;
        this.dieTypes
          .map((x) => x.id)
          .includes(this.selectedDieTypeId)
          ? await this.goToCreatingMode()
          : this.reset();
      }
      if (routeName === 'kurbatovparameter-updating') {
        this.initialDialog = false;
        this.selectedDieTypeId = +this.$route.params.dieType;
        const selectedPatternId = +this.$route.params.patternId;
        this.mode = 'updating';
        await this.$http
          .get(`/api/standartpattern/dietype/${this.selectedDieTypeId}`)
          .then((response) => { this.patterns = response.data; this.selectedPattern = this.patterns.find((x) => x.id === selectedPatternId) || {}; })
          .then(() => { if (!this.patterns.map((x) => x.id).includes(this.selectedPattern.id)) throw new Error(); })
          .then(async () => await this.getSelectedPattern(this.selectedPattern))
          .catch(() => { this.showSnackbar('Ошибка пути'); this.reset(); });
      }
    },

    async changeSelectedPattern(selectedPattern) {
      this.$router.push({ name: 'kurbatovparameter-updating', params: { dieType: this.selectedDieTypeId, patternId: selectedPattern.id } });
      await this.getSelectedPattern(selectedPattern);
    },

    async savePattern(smpArray) {
      const { mode } = this;
      this.showLoading('Сохранение...');
      await this.$http({
        method: 'post',
        url: `/api/standartpattern/${mode}`,
        data: new StandartMeasurementPatternFullViewModel(new StandartPattern(this.patternName, this.selectedDieTypeId, this.selectedPattern.id), smpArray.map((smp) => new StandartMeasurementPattern(smp, smp.kpList.map((k) => new KurbatovParameter(new KurbatovParameterBorders(k.bounds.lower, k.bounds.upper, k.bounds.id), k.standartParameter, k.id)), this.selectedPattern.id, smp.id))),
        config: {
          headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
          },
        },
      })
        .then((response) => {
          this.closeLoading();
          return response.data;
        })
        .then(async (pattern) => {
          await this.goToUpdatingMode(this.selectedDieTypeId);
          this.selectedPattern = pattern;
          this.$router.push({ name: 'kurbatovparameter-updating', params: { dieType: this.selectedDieTypeId, patternId: this.selectedPattern.id } });
          this.showSnackbar('Успешно сохранено');
        })
        .catch((error) => {
          if (error.response.status === 403) {
            this.showSnackbar('Шаблон с таким именем уже существует');
          } else {
            this.showSnackbar('Ошибка соединения с БД');
          }
          this.closeLoading();
        });
    },

    async deletePattern(selectedPattern) {
      await this.$http({
        method: 'delete',
        url: `/api/standartpattern/${selectedPattern.id}`,
        config: {
          headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
          },
        },
      })
        .then((response) => {
          this.patterns = this.patterns.filter((x) => x.id !== selectedPattern.id);
          this.selectedPattern = this.patterns[0] || {};
          this.showSnackbar('Успешно удалено');
          return this.selectedPattern;
        })
        .then(async (selectedPattern) => {
          _.isEmpty(selectedPattern)
            ? await this.changeSelectedPattern(selectedPattern)
            : Promise.resolve(this.reset()).then(() => this.showSnackbar('Удален последний шаблон'));
        })
        .catch((error) => {
          this.showSnackbar('Ошибка при удалении');
        });
    },

    async goToCreatingMode() {
      this.initialDialog = false;
      this.mode = 'creating';
      await this.fillSmpStorage().then((r) => this.$router.push({ name: 'kurbatovparameter-creating', params: { dieType: this.selectedDieTypeId } }));
    },

    async fillSmpStorage() {
      await this.getProcessByDieId(this.selectedDieTypeId)
        .then(async () => await this.getElementsByDieType(this.selectedDieTypeId))
        .then(async () => await this.getStagesByProcessId(this.process));
      await this.getDividers();
    },

    async goToUpdatingMode(selectedDieTypeId) {
      this.mode = 'updating';
      await this.$http
        .get(`/api/standartpattern/dietype/${selectedDieTypeId}`)
        .then((response) => {
          this.patterns = response.data;
          this.selectedPattern = response.data[0] || {};
        })
        .then(() => {
          if (_.isEmpty(this.selectedPattern)) {
            this.showSnackbar('Шаблоны не найдены');
            this.reset();
          }
        })
        .catch((error) => reset());
    },

    async getSelectedPattern(selectedPattern) {
      this.showLoading('Загрузка...');
      this.$store.dispatch('smpstorage/resetSmp');
      await this.fillSmpStorage();
      await this.$http.get(`/api/standartpattern/smp/${selectedPattern.id}`)
        .then((response) => {
          this.patternName = response.data.standartPattern.name;
          this.restoreFromVm(response.data.standartMeasurementPatternList);
          this.initialDialog = false;
          this.closeLoading();
        })
        .catch(() => { this.showSnackbar('В шаблоне не содержатся данные'); this.closeLoading(); });
    },

    async getAllDieTypes() {
      await this.$http
        .get('/api/dietype/all')
        .then((response) => { this.dieTypes = response.data; })
        .catch(() => this.showSnackbar('Типы кристаллов не найдены в БД'));
    },

    async getProcessByDieId(selectedDieTypeId) {
      await this.$http
        .get(`/api/process/dietype/${selectedDieTypeId}`)
        .then((response) => this.process = response.data)
        .catch((error) => this.showSnackbar(error.response.data[0].message));
    },

    async getElementsByDieType(selectedDieTypeId) {
      this.$store.dispatch('smpstorage/getElementsByDieType', { ctx: this, selectedDieTypeId });
    },

    async getStagesByProcessId(process) {
      this.$store.dispatch('smpstorage/getStagesByProcessId', { ctx: this, process });
    },

    async getStandartParameters() {
      this.$store.dispatch('smpstorage/getStandartParameters', { ctx: this });
    },

    getDividers() {
      this.$store.dispatch('dividers/getAllDividers', this);
    },
  },

  computed: {
    readyToCreateSMP() {
      return !_.isEmpty(this.selectedElementSMP) && !_.isEmpty(this.selectedStageSMP) && !_.isEmpty(this.selectedDividerSMP);
    },

    smpArray() {
      return this.$store.getters['smpstorage/currentSmpArray'];
    },

    validation() {
      return this.$store.getters['smpstorage/patternValidation'];
    },

    dividersArray() {
      return this.$store.getters['dividers/getAll'];
    },

    elementsArray() {
      return this.$store.getters['smpstorage/elements'];
    },

    stagesArray() {
      return this.$store.getters['smpstorage/stages'];
    },

    smpName() {
      if (this.readyToCreateSMP) { return `${this.selectedElementSMP.name}_${this.selectedStageSMP.stageName.split(' ').join('+')}_${this.selectedDividerSMP.name === 'Нет' ? 'No' : this.selectedDividerSMP.name}µm`; }
      return '';
    },
  },

  async mounted() {
    this.showLoading('Загрузка...');
    await this.initialize().then(async () => await this.routeHandler(this.$route.name)).then(() => this.closeLoading());
  },

  beforeDestroy() {
    this.$store.dispatch('smpstorage/reset');
  },
};
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
