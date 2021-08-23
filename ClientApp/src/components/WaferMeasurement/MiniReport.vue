<template>
    <v-container>
        <v-row dense>
            <v-col lg="6">
                <v-row>
                    <v-col lg="12">
                        <v-card class="elevation-8" color="#303030">
                            <v-card-text>
                                <v-chip class="d-lg-flex"
                                        color="indigo"
                                        @click="$router.push({ name: 'wafer-path', params: { waferId: waferId } })"
                                        x-large label
                                        v-html="waferId" dark>
                                </v-chip>
                            </v-card-text>
                        </v-card>
                    </v-col>
                </v-row>
                <v-row>

                </v-row>
                <v-row>
                    <v-col lg="12">
                        <v-card class="elevation-8" color="#303030">
                            <v-card-text>
                                <p>Партия</p>
                                <v-chip class="d-lg-block"
                                        v-if="parcel.name==='Неизвестно'"
                                        color="pink darken-1"
                                        label v-html="parcel.name" dark>
                                </v-chip>
                                <v-chip class="d-lg-block" v-else color="indigo" label v-html="parcel.name" dark></v-chip>
                            </v-card-text>
                            <v-card-actions>
                                    <v-btn
                                    fab
                                    dark
                                    x-small
                                    outlined
                                    color="green"
                                    @click="modes.measurement.edit=true">
                                    <v-icon>edit</v-icon>
                                </v-btn>
                            </v-card-actions>
                        </v-card>
                    </v-col>
                </v-row>
            </v-col>
            <v-col lg="6">
                <v-row>
                    <v-col lg="12">
                        <v-card class="elevation-8" color="#303030">
                            <v-card-text>
                                <p>Шаблон</p>
                                <v-chip class="d-lg-block"
                                        v-if="codeProduct.name==='Неизвестно'"
                                        color="pink darken-1" label
                                        v-html="codeProduct.name" dark>
                                </v-chip>
                                <v-chip class="d-lg-block" v-else color="indigo" label v-html="codeProduct.name" dark></v-chip>
                            </v-card-text>
                        </v-card>
                    </v-col>
                </v-row>
                <v-row>
                    <v-col lg="12">
                        <selectedDiesInfo :selectedMeasurementId="selectedMeasurementId">
                        </selectedDiesInfo>
                    </v-col>
                </v-row>
                <v-row>
                    <v-col lg="12">
                        <v-card class="elevation-8" color="#303030">
                            <v-card-text>
                                <p>Этап</p>
                                <v-chip class="d-lg-block"
                                        v-if="stage.stageName === 'Неизвестно'"
                                        color="pink darken-1" label v-html="stage.stageName" dark>
                                </v-chip>
                                <v-chip class="d-lg-block" v-else color="indigo" label v-html="stage.stageName" dark></v-chip>
                            </v-card-text>
                            <v-card-actions>
                                    <v-btn
                                    fab
                                    dark
                                    x-small
                                    outlined
                                    color="green"
                                    @click="modes.measurement.edit=true">
                                    <v-icon>edit</v-icon>
                                </v-btn>
                            </v-card-actions>
                        </v-card>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>
        <v-row dense>
            <v-col lg="6">
                <v-card class="elevation-8" color="#303030">
                    <v-card-text>
                        <p>Операция</p>
                        <v-chip class="d-lg-block"
                                v-if="measurement.name === 'Не выбрано'" color="pink darken-1" label v-html="measurement.name" dark>
                        </v-chip>
                        <v-chip class="d-lg-block" v-else color="indigo" label v-html="measurement.name" dark></v-chip>
                    </v-card-text>
                    <v-card-actions>
                        <v-btn
                            fab
                            dark
                            x-small
                            outlined
                            color="green"
                            @click="modes.measurement.edit=true">
                            <v-icon>edit</v-icon>
                        </v-btn>
                        <v-btn
                            fab
                            dark
                            x-small
                            outlined
                            color="pink"
                            @click="modes.measurement.delete=true">
                            <v-icon>delete</v-icon>
                        </v-btn>
                    </v-card-actions>
                    </v-card>
            </v-col>
            <v-col lg="6">
                <v-card class="elevation-8" color="#303030">
                    <v-card-text>
                        <p>Элемент</p>
                        <v-chip class="d-lg-block"
                                v-if="element.name === 'Неизвестно'"
                                color="pink darken-1" label v-html="element.name" dark>
                        </v-chip>
                        <v-chip class="d-lg-block" v-else color="indigo" label v-html="element.name" dark></v-chip>
                    </v-card-text>
                    <v-card-actions>
                        <v-btn
                            fab
                            dark
                            x-small
                            outlined
                            color="green"
                            @click="modes.element.edit=true">
                            <v-icon>edit</v-icon>
                        </v-btn>
                    </v-card-actions>
                </v-card>
            </v-col>
        </v-row>
        <v-row dense>
            <v-col lg="12">

            </v-col>
        </v-row>
        <v-row>
            <v-col lg="12">
            </v-col>
        </v-row>

        <v-row dense v-if="modes.measurement.edit || modes.measurement.delete">
                <v-col lg="12">

                <v-card class="elevation-8" color="#303030" v-if="modes.measurement.edit">
                    <v-row>
                        <v-col lg="4" offset-lg="1" class="mt-4">
                            <v-text-field
                                v-model="measurement.name"
                                outlined
                                readonly
                                label="Текущее название">
                            </v-text-field>
                        </v-col>
                        <v-col lg="4" class="mt-4">
                            <v-text-field
                                v-model="modes.measurement.newName"
                                :error-messages="validationNewMeasurementName"
                                outlined
                                label="Новое название">
                            </v-text-field>
                        </v-col>
                        <v-col lg="2">
                            <v-btn v-if="Array.isArray(validationNewMeasurementName)"
                                   outlined block color="green" class="d-flex mt-2" @click="editMeasurement">
                               Изменить
                            </v-btn>
                             <v-btn outlined block color="pink" class="d-flex mt-2" @click="cancelMeasurementEdit">
                               Отменить
                            </v-btn>
                        </v-col>
                    </v-row>
                </v-card>
                <v-card class="elevation-8" color="#303030" v-if="modes.measurement.delete">
                    <v-row>
                        <v-col lg="6" offset-lg="1" class="mt-4">
                             <v-text-field
                                v-model="modes.measurement.deletePassword"
                                type="password"
                                outlined
                                label="Пароль для удаления">
                            </v-text-field>
                        </v-col>
                        <v-col lg="2">
                            <v-btn v-if="modes.measurement.deletePassword"
                                    :error-messages="!modes.measurement.deletePassword
                                      ? 'Введите пароль' : []"  outlined block color="green" class="d-flex mt-2"
                                      @click="deleteMeasurement">
                               Удалить измерение
                            </v-btn>
                             <v-btn outlined block color="pink" class="d-flex mt-2" @click="cancelMeasurementEdit">
                               Отменить удаление
                            </v-btn>
                        </v-col>
                    </v-row>
                </v-card>
            </v-col>
        </v-row>
        <v-row dense v-if="modes.stage.edit">
            <v-col lg="12">
                <v-card class="elevation-8" color="#303030">
                    <v-row>
                        <v-col lg="6" offset-lg="1" class="mt-4">
                            <v-select
                                v-model="modes.stage.selected"
                                :items="modes.stage.avStages"
                                no-data-text="Нет данных"
                                item-text="stageName"
                                item-value="stageId"
                                outlined
                                label="Выберите новый этап">
                            </v-select>
                        </v-col>
                        <v-col lg="4">
                            <v-btn v-if="modes.stage.selected" outlined block color="green" class="d-flex mt-2" @click="editStage">
                                Изменить этап
                            </v-btn>
                             <v-btn outlined block color="pink" class="d-flex mt-2" @click="cancelStageEdit">
                               Отменить изменение
                            </v-btn>
                        </v-col>
                    </v-row>
                </v-card>
            </v-col>
        </v-row>
        <v-row dense v-if="modes.element.edit">
            <v-col lg="12">
                <v-card class="elevation-8" color="#303030">
                    <v-row>
                        <v-col lg="6" offset-lg="1" class="mt-4">
                            <v-select
                                v-model="modes.element.selectedDieType"
                                :items="modes.element.dieTypes"
                                no-data-text="Нет данных"
                                item-text="name"
                                item-value="dieTypeId"
                                outlined
                                label="Выберите тип монитора">
                            </v-select>
                        </v-col>
                        <v-col lg="4">
                            <v-btn v-if="modes.element.selectedElement" outlined block color="green" class="d-flex mt-2" @click="editElement">
                                Изменить элемент
                            </v-btn>
                            <v-btn outlined block color="pink" class="d-flex mt-2" @click="cancelElementEdit">
                               Отменить изменение
                            </v-btn>
                        </v-col>
                    </v-row>
                    <v-row>
                        <v-col lg="6" offset-lg="1" class="mt-4">
                            <v-select
                                v-model="modes.element.selectedElement"
                                :items="modes.element.avElements"
                                no-data-text="Нет данных"
                                item-text="name"
                                item-value="elementId"
                                outlined
                                label="Выберите элемент">
                            </v-select>
                        </v-col>
                    </v-row>
                </v-card>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
import SelectedDiesInfo from './SelectedDiesInfo.vue';

export default {

  props: ['waferId', 'selectedMeasurementId'],

  data() {
    return {
      modes: {
        measurement: {
          edit: false, newName: '', deletePassword: '', delete: false,
        },
        stage: { edit: false, avStages: [], selected: {} },
        element: {
          edit: false, selectedElement: '', selectedDieType: '', dieTypes: [], avElements: [],
        },
      },
      fab: {
        measurement: false, element: false, stage: false, parcel: false,
      },
      measurement: { name: 'Не выбрано' },
      codeProduct: { name: 'Неизвестно' },
      element: { name: 'Неизвестно' },
      stage: { stageName: 'Неизвестно' },
      parcel: { name: 'Неизвестно' },
    };
  },

  components: {
    selectedDiesInfo: SelectedDiesInfo,
  },

  methods: {
    async editMeasurement() {
      const measurementViewModel = { id: this.selectedMeasurementId, name: this.modes.measurement.newName };
      try {
        await this.$http.post('/api/measurementrecording/edit/name', measurementViewModel);
        this.$store.dispatch('wafermeas/updateMeasurementName', measurementViewModel);
        this.showSnackBar('Название успешно изменено');
        this.cancelMeasurementEdit();
      } catch (ex) {
        this.showSnackBar('Ошибка при изменении названия');
      }
    },

    async editStage() {
      const measurementViewModel = { measurementRecordingId: this.selectedMeasurementId, stageId: this.modes.stage.selected };
      try {
        await this.$http.post('/api/measurementrecording/update-stage', measurementViewModel);
        this.$store.dispatch('wafermeas/updateMeasurementStage', measurementViewModel);
        this.showSnackBar('Этап успешно изменен');
        Object.assign(this.stage, this.modes.stage.avStages.find((x) => x.stageId === this.modes.stage.selected));
        this.cancelStageEdit();
      } catch (ex) {
        this.showSnackBar('Ошибка при изменении этапа');
      }
    },

    async editElement() {
      const response = await this.$http({
        method: 'post',
        url: '/api/element/updateElementOnIdmr',
        data: { elementId: this.modes.element.selectedElement, measurementRecordingId: this.selectedMeasurementId },
        config: {
          headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
          },
        },
      })
        .then(() => {
          Object.assign(this.element, response.data);
          this.cancelElementEdit();
          this.showSnackBar(`Элемент успешно изменен на ${this.element.name}`);
        })
        .catch(() => {
          this.showSnackBar('Произошла ошибка при изменении элемента');
        });
    },

    async deleteMeasurement() {
      await this.$http.delete(`/api/measurementrecording/delete/${this.selectedMeasurementId}?superuser=${this.modes.measurement.deletePassword}`)
        .then(() => {
          this.$store.dispatch('wafermeas/deleteMeasurement', this.selectedMeasurementId);
          this.cancelMeasurementEdit();
          this.showSnackBar('Операция успешно удалена');
        })
        .catch((error) => {
          error && error.response.status === 403 ? this.showSnackBar('Запрещено удаление') : this.showSnackBar('Ошибка при удалении');
        });
    },

    cancelMeasurementEdit() {
      this.modes.measurement.edit = false;
      this.modes.measurement.delete = false;
      this.modes.measurement.newName = '';
      this.modes.measurement.deletePassword = '';
    },

    cancelStageEdit() {
      this.modes.stage.edit = false;
      this.modes.stage.selected = Object.assign(this.modes.stage.selected, this.stage);
    },

    cancelElementEdit() {
      this.modes.element.edit = false;
    },

    async getCodeProduct(waferId) {
      const response = await this.$http.get(`/api/codeproduct/getbywaferid?waferId=${waferId}`);
      if (response.status === 200) {
        return response.data;
      }
      if (response.status === 404) {
        return { codeProductName: 'Неизвестно' };
      }
      this.showSnackBar(response.data);
    },

    async getParcel(waferId) {
      const response = await this.$http.get(`/api/parcel/waferId/${waferId}`);
      if (response.status === 200) {
        return response.data;
      }
      this.showSnackBar(response.data);
    },

    async getElement(measurementRecordingId) {
      try {
        const response = await this.$http.get(`/api/element/getbyidmr?idmr=${measurementRecordingId}`);
        return response.data[0].elementId === 0 ? { name: 'Неизвестно' } : response.data[0];
      } catch (error) {
        this.showSnackBar(error);
      }
    },

    async getStage(stageId) {
      if (stageId) {
        try {
          const response = await this.$http.get(`/api/stage/id/${stageId}`);
          return response.data.stageId === 0 ? { stageName: 'Неизвестно' } : response.data;
        } catch (error) {
          this.showSnackBar(error);
        }
      } else {
        return { stageName: 'Неизвестно' };
      }
    },

    showSnackBar(text) {
      this.$store.dispatch('alert/success', text);
    },
  },

  watch: {
    async waferId(waferId) {
      this.codeProduct = await this.getCodeProduct(waferId);
      this.modes.stage.avStages = (await this.$http.get(`/api/stage/wafer/${waferId}`)).data;
      this.modes.element.dieTypes = (await this.$http.get(`/api/dietype/wafer/${waferId}`)).data;
      this.parcel = await this.getParcel(waferId);
      if (this.modes.element.dieTypes.length > 0) {
        this.modes.element.selectedDieType = this.modes.element.dieTypes[0].dieTypeId;
      }
    },

    async selectedMeasurementId(newValue) {
      this.measurement = this.$store.getters['wafermeas/measurements'].find((x) => x.id === newValue);
      this.element = await this.getElement(this.measurement.id);
      this.stage = await this.getStage(this.measurement.stageId);
      Object.assign(this.modes.stage.selected, this.stage);
    },

    // eslint-disable-next-line func-names
    'modes.element.selectedDieType': async function (newValue) {
      if (newValue !== 'undefined') {
        const dieType = this.modes.element.dieTypes.find((x) => x.name === newValue);
        if (dieType) {
          this.modes.element.avElements = (await this.$http.get(`/api/element/dietype/${dieType.id}`)).data;
          if (this.modes.element.avElements.length > 0) {
            this.modes.element.selectedElement = this.modes.element.avElements[0].elementId;
          }
        }
      }
    },
  },

  computed: {
    validationNewMeasurementName() {
      if (this.modes.measurement.edit === false) {
        return [];
      }
      if (this.modes.measurement.newName.length === 0) {
        return 'Введите название измерения';
      }
      if (this.measurementRecordings.some((x) => x.name === this.modes.measurement.newName)) {
        return 'Измерение с таким названием уже существует';
      }
      return [];
    },

    measurementRecordings() {
      return this.$store.getters['wafermeas/measurements'];
    },
  },

};
</script>
