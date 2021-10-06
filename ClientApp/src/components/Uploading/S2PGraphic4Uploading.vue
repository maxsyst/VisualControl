<template>
    <v-container>
        <v-row class="d-flex flex-row">
            <v-col class="d-flex flex-column">
                <v-autocomplete
                        v-model="selectedWafer"
                        :items="wafers"
                        no-data-text="Нет данных"
                        item-text="waferId"
                        item-value="waferId"
                        filled
                        outlined
                        label="Номер пластины"
                        color='primary'
                ></v-autocomplete>
                 <v-chip v-if="isWaferExistInDirectory" label color="success">
                      Папка загрузки найдена в директории
                  </v-chip>
                  <v-chip v-else label color="pink">
                      Папка с пластиной не найдена в директории
                  </v-chip>
            </v-col>
            <v-col class="d-flex flex-column">
              <v-select
                      v-model="selectedStage"
                      :items="stages"
                      no-data-text="Нет данных"
                      item-value="stageId"
                      item-text="stageName"
                      outlined
                      label="Выберите этап:">
              </v-select>
               <v-chip v-if="selectedStage" label color="success">
                      Этап выбран
                  </v-chip>
                  <v-chip v-else label color="pink">
                      Этап не выбран
                  </v-chip>
            </v-col>
            <v-col class="d-flex flex-column">
                 <v-select
                        v-model="selectedMonitor"
                        :items="monitors"
                        item-text="name"
                        item-value="dieTypeId"
                        outlined
                        no-data-text="Нет данных"
                        label="Выберите монитор:">
                    </v-select>
                    <v-chip v-if="selectedMonitor" label color="success">
                      Монитор выбран
                    </v-chip>
                    <v-chip v-else label color="pink">
                      Монитор не выбран
                    </v-chip>
            </v-col>
            <v-col class="d-flex flex-column">
              <v-select v-model="selectedElement"
                        :items="avElements"
                        no-data-text="Нет данных"
                        item-value="elementId"
                        item-text="name"
                        outlined
                        label="Выберите элемент">
              </v-select>
               <v-chip v-if="selectedElement" label color="success">
                      Элемент выбран
                    </v-chip>
                    <v-chip v-else label color="pink">
                      Элемент не выбран
                    </v-chip>
            </v-col>
        </v-row>
        <v-row>
            <v-col class="d-flex flex-column">
                <v-text-field   v-model="measurementRecordingName"
                                v-if="isMeasurementVisible"
                                @input="checkMeasurementRecording"
                                outlined label="Номер операции:">
                </v-text-field>
                 <div v-if="isMeasurementVisible">
                  <v-chip v-if="isMeasurementReady" label color="success">
                      Папка загрузки найдена в директории
                  </v-chip>
                  <v-chip v-else-if="isMeasurementUnknown" label color="pink">
                      Заполните номер загружаемой операции
                  </v-chip>
                  <v-chip v-else-if="isMeasurementNotExists" label color="pink">
                      Папка с номером операции не найдена в директории
                  </v-chip>
                  <v-chip v-else-if="isMeasurementAlreadyUploaded" label color="teal">
                      Измерение уже загружено
                  </v-chip>
                 </div>
            </v-col>
        </v-row>
          <v-row>
            <v-col class="d-flex flex-column">
                 <v-select
                    v-if="isMeasurementReady"
                    v-model="uploadingType"
                    :items="uploadingTypes"
                    return-object
                    no-data-text="Нет данных"
                    item-value="type"
                    item-text="type"
                    outlined
                    label="Выберите тип загрузки:">
                  </v-select>
            </v-col>
        </v-row>
         <v-row>
          <v-card v-if="uploadingType==='JUST_S2P'">
                <v-card-title>
                    <span class="headline">Выбор графиков</span>
                </v-card-title>
                <v-card-text>
        <v-row>
            <v-col class="d-flex">
             <v-select
                    v-model="currentGraphics.S21"
                    :items="availableGraphics"
                    no-data-text="Нет данных"
                    item-value="id"
                    item-text="name"
                    outlined
                    label="S21:">
                  </v-select>
            </v-col>
            <v-col class="d-flex">
              <v-select
                    v-model="currentGraphics.S22"
                    :items="availableGraphics"
                    no-data-text="Нет данных"
                     item-value="id"
                    item-text="name"
                    outlined
                    label="S22:">
                  </v-select>
            </v-col>
        </v-row>
        <v-row>
            <v-col class="d-flex">
              <v-select
                v-model="currentGraphics.S11"
                :items="availableGraphics"
                no-data-text="Нет данных"
                item-value="id"
                item-text="name"
                outlined
                label="S11:">
              </v-select>
            </v-col>
            <v-col class="d-flex">
              <v-select
                    v-model="currentGraphics.S12"
                    :items="availableGraphics"
                    no-data-text="Нет данных"
                    item-value="id"
                    item-text="name"
                    outlined
                    label="S12:">
                  </v-select>
            </v-col>
        </v-row>
                </v-card-text>
                <v-card-actions>
                    <v-select
                      :items="['DB', 'RI']"
                      outlined
                      v-model="s2pParserMode"
                      label="Выберите тип s2p-файла">
                    </v-select>
                </v-card-actions>
            </v-card>
        </v-row>
        <v-row>
            <v-col class="d-flex">
                 <v-btn v-if="isUploadVisible" color="indigo" block @click="upload">
                    Загрузить измерения
                </v-btn>
                 <v-btn v-if="isMeasurementAlreadyUploaded" color="teal" block @click="deleteMeasurement">
                    Удалить измерения
                </v-btn>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
export default {
  data() {
    return {
      wafers: [],
      stages: [],
      monitors: [],
      avElements: [],
      uploadingTypes: [],
      availableGraphics: [],
      selectedWafer: '',
      selectedMonitor: '',
      selectedElement: '',
      selectedStage: '',
      measurementRecordingName: '',
      uploadingType: '',
      isWaferExistInDirectory: false,
      measurementRecordingStatus: 'unknown',
      currentGraphics: {},
      s2pParserMode: 'DB',
    };
  },

  computed: {
    isUploadVisible() {
      if (this.uploadingType === 'JUST_S2P') {
        return this.isMeasurementReady && this.isGraphicsSelected && this.uploadingType;
      }
      return this.isMeasurementReady && this.uploadingType;
    },
    isMeasurementVisible() {
      return this.isWaferExistInDirectory && this.selectedStage && this.selectedMonitor && this.selectedElement;
    },
    isMeasurementUnknown() {
      return this.measurementRecordingStatus.status === 'unknown';
    },
    isMeasurementReady() {
      return this.measurementRecordingStatus.status === 'ready';
    },
    isMeasurementAlreadyUploaded() {
      return this.measurementRecordingStatus.status === 'alreadyUploaded';
    },
    isMeasurementNotExists() {
      return this.measurementRecordingStatus.status === 'notExists';
    },
    isGraphicsSelected() {
      return !Object.keys(this.currentGraphics).every((k) => this.currentGraphics[k] === '0');
    },
    userName() {
      return `${this.$store.state.authentication.user.firstName} ${this.$store.state.authentication.user.surname}`;
    },
  },

  watch: {
    async selectedWafer(selectedWafer) {
      if (selectedWafer) {
        this.isWaferExistInDirectory = (await this.$http.get(`/api/folder/iswaferexist/graphic4/${selectedWafer}`)).data;
        this.measurementRecordingName = '';
        this.measurementRecordingStatus = { status: 'unknown', measurementRecordingId: 0 };
        this.availableGraphics = [];
        this.currentGraphics = {};
        this.selectedMonitor = '';
        this.selectedElement = '';
        this.selectedStage = '';
        this.avElements = [];
        await this.getAllStages(selectedWafer);
        await this.initMonitors(selectedWafer);
      }
    },

    async selectedMonitor(selectedMonitor) {
      await this.getAvElements(selectedMonitor);
    },

    async uploadingType(uploadingType) {
      if (uploadingType === 'JUST_S2P') {
        const { data } = await this.$http.get(`/api/uploadingtype/availiableGraphics/${this.selectedWafer}`);
        this.availableGraphics = [{ id: '0', name: 'Не загружать' }, ...data.availableGraphics];
        this.currentGraphics = data.currentGraphics;
        Object.keys(this.currentGraphics).forEach((key) => {
          if (this.currentGraphics[key] == null) {
            this.currentGraphics[key] = '0';
          }
        });
      }
    },
  },

  async created() {
    await this.getWafers();
    await this.getUploadingTypes();
  },

  methods: {
    async getWafers() {
      const response = await this.$http.get('/api/wafer/all');
      this.wafers = response.data;
    },
    async getUploadingTypes() {
      const response = await this.$http.get('/api/uploadingtype/all');
      this.uploadingTypes = response.data;
    },

    async initMonitors(selectedWafer) {
      await this.$http
        .get(`/api/dietype/wafer/${selectedWafer}`)
        .then((response) => {
          this.monitors = response.data;
        })
        .catch(() => this.showSnackBar('Ошибка при загрузке списка мониторов'));
    },

    async getAllStages(waferId) {
      await this.$http
        .get(`/api/stage/wafer/${waferId}`)
        .then((response) => {
          this.stages = response.data;
        })
        .catch(() => {
          this.showSnackBar('Ошибка при загрузке этапов');
        });
    },

    async getAvElements(selectedMonitor) {
      await this.$http.get(`/api/element/dietype/id/${selectedMonitor}`)
        .then((response) => {
          this.avElements = response.data;
        })
        .catch((error) => {
          this.showSnackBar(error);
        });
    },

    async checkMeasurementRecording() {
      const response = await this.$http.get(`/api/uploading/graphic4/checkStatus/${this.selectedWafer}/${this.measurementRecordingName}`);
      this.measurementRecordingStatus = response.data;
    },

    async deleteMeasurement() {
      const { measurementRecordingId } = this.measurementRecordingStatus;
      await this.$http.delete(`/api/measurementrecording/delete/graphic4/${measurementRecordingId}`)
        .then(() => {
          this.showSnackBar('Успешно удалено');
          this.reset();
        })
        .catch(() => {
          this.showSnackBar('Ошибка при удалении');
        });
    },

    showSnackBar(text) {
      this.$store.dispatch('alert/success', text);
    },

    showLoading(text) {
      this.$store.dispatch('loading/show', text);
    },

    closeLoading() {
      this.$store.dispatch('loading/cloak');
    },

    reset() {
      this.selectedWafer = '';
      this.selectedMonitor = '';
      this.selectedElement = '';
      this.measurementRecordingName = '';
      this.uploadingType = '';
      this.isWaferExistInDirectory = false;
      this.measurementRecordingStatus = { status: 'unknown', measurementRecordingId: 0 };
      this.currentGraphics = {};
      this.s2pParserMode = 'DB';
      this.selectedStage = '';
    },

    async upload() {
      this.showLoading('Загрузка измерения...');
      const uploadingFileViewModel = {
        measurementRecordingName: this.measurementRecordingName,
        waferId: this.selectedWafer,
        uploadingType: this.uploadingType,
        s2pParserMode: this.s2pParserMode,
        elementId: this.selectedElement,
        stageId: this.selectedStage,
        userName: this.userName,
      };
      await this.$http.post('/api/uploading/graphic4', uploadingFileViewModel)
        .then((response) => {
          this.closeLoading();
          this.reset();
          this.showSnackBar(`Файл успешно загружен. Операция ${response.data}`);
        })
        .catch(() => {
          this.closeLoading();
          this.showSnackBar('Ошибка при загрузке файла');
        });
    },
  },
};
</script>
