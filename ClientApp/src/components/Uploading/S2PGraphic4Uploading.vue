<template>
    <v-container>
        <v-row class="d-flex flex-row">
            <v-col class="d-flex">
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
            </v-col>
            <v-col class="d-flex">
                 <v-chip v-if="isWaferExistInDirectory" label color="success">
                    Папка загрузки найдена в директории
                </v-chip>
                 <v-chip v-else label color="pink">
                    Папка с пластиной не найдена в директории
                </v-chip>
            </v-col>
        </v-row>
        <v-row>
            <v-col class="d-flex">
                <v-text-field   v-model="measurementRecordingName"
                                v-if="isWaferExistInDirectory"
                                @input="checkMeasurementRecording"
                                outlined label="Номер операции:">
                </v-text-field>
            </v-col>
             <v-col class="d-flex">
                 <v-chip v-if="isMeasurementReady" label color="success">
                    Папка загрузки найдена в директории
                </v-chip>
                <v-chip v-else-if="isMeasurementUnknown && isWaferExistInDirectory" label color="pink">
                    Заполните номер загружаемой операции
                </v-chip>
                 <v-chip v-else-if="isMeasurementNotExists" label color="pink">
                    Папка с номером операции не найдена в директории
                </v-chip>
                <v-chip v-else-if="isMeasurementAlreadyUploaded" label color="teal">
                    Измерение уже загружено
                </v-chip>
            </v-col>
        </v-row>
          <v-row>
            <v-col class="d-flex">
                 <v-select
                    v-if="isMeasurementReady"
                    v-model="uploadingType"
                    :items="uploadingTypes"
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
            </v-card>
        </v-row>
        <v-row>
            <v-col class="d-flex">
                 <v-btn v-if="isMeasurementReady" color="indigo" block>
                    Загрузить измерения
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
      selectedWafer: '',
      measurementRecordingName: '',
      uploadingTypes: [],
      uploadingType: '',
      isWaferExistInDirectory: false,
      measurementRecordingStatus: 'unknown',
      availableGraphics: [],
      currentGraphics: {},
    };
  },

  computed: {
    isMeasurementUnknown() {
      return this.measurementRecordingStatus === 'unknown';
    },
    isMeasurementReady() {
      return this.measurementRecordingStatus === 'ready';
    },
    isMeasurementAlreadyUploaded() {
      return this.measurementRecordingStatus === 'alreadyUploaded';
    },
    isMeasurementNotExists() {
      return this.measurementRecordingStatus === 'notExists';
    },
  },

  watch: {
    async selectedWafer(selectedWafer) {
      this.isWaferExistInDirectory = (await this.$http.get(`/api/folder/iswaferexist/graphic4/${selectedWafer}`)).data;
      this.measurementRecordingName = '';
      this.measurementRecordingStatus = 'unknown';
      this.availableGraphics = [];
      this.currentGraphics = {};
    },

    async uploadingType(uploadingType) {
      if (uploadingType === 'JUST_S2P') {
        const { data } = await this.$http.get(`/api/uploadingtype/availiableGraphics/${this.selectedWafer}`);
        this.availableGraphics = [{ id: '0', name: 'Не загружать' }, ...data.availableGraphics];
        this.currentGraphics = data.currentGraphics;
        Object.keys(this.currentGraphics).forEach((key) => {
          if (this.currentGraphics[key] == null) {
            this.currentGraphics[key] = { id: '0', name: 'Не загружать' };
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
      [this.uploadingType] = this.uploadingTypes;
    },

    async checkMeasurementRecording() {
      const response = await this.$http.get(`/api/uploading/graphic4/checkStatus/${this.selectedWafer}/${this.measurementRecordingName}`);
      this.measurementRecordingStatus = response.data;
    },
  },
};
</script>
