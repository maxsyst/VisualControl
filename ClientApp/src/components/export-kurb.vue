<template>
    <v-container grid-list-lg>
        <v-layout v-if="initialDialog === false" row class="alwaysOnTop">
            <v-flex lg3 offset-lg1>
                <v-btn color="indigo" @click="createElement()">
                    Добавить элемент
                </v-btn>
                <v-btn v-if="elements.length > 0" color="indigo" @click="deleteDialog = true">
                    Удалить элемент
                </v-btn>
            </v-flex>
            <v-flex lg2>
                <v-menu
                    v-model="menu"
                    :nudge-width="200"
                    :close-on-click="false"
                    :close-on-content-click="false"
                    offset-x>
                <template v-slot:activator="{ on }">
                        <v-btn
                        color="indigo"
                        dark
                        v-on="on"
                        >
                        Автозаполнение
                        </v-btn>
                    </template>
                    <v-card>
                       <v-card-text>
                        <v-text-field v-model="waferId" :error-messages="waferId ? [] : 'Введите название пластины'"
                            label="Номер пластины"
                        ></v-text-field>
                         <v-btn color="primary" outlined @click="getAutoIdmr()">Заполнить шаблон</v-btn>
                       </v-card-text>
                    </v-card>
                </v-menu>

            </v-flex>
            <v-flex lg4>
                <v-btn block v-if="readyToExport" color="green" @click="exportDialog = true">
                    Экспорт
                </v-btn>
                <v-btn block v-else outlined color="pink">
                    Для экспорта необходимо заполнить все элементы
                </v-btn>
            </v-flex>
        </v-layout>
        <v-layout row>
           <v-flex lg11 offset-lg1>
               <v-stepper v-if="elements.length>0" v-model="e1" vertical>
                    <template v-for="(element, index) in elements">
                    <v-stepper-step
                        :key="`${index+1}-step`"
                        :step="index + 1"
                        :color="$store.getters['exportkurb/isThisElementisReady'](element.key) ? 'green' : 'red'"
                        complete
                        editable>
                        {{"Операция: " + element.operation.number + " Элемент: " + element.element.name}}
                    </v-stepper-step>
                    <v-stepper-content
                        :key="`${index+1}-content`"
                        :step="index + 1">
                        <v-card>
                            <v-card-text>
                               <export-element :ref="element.key" :id.sync="element.key" :parameters.sync="element.parameters" :dividers="dividers"
                                               :operation.sync="element.operation" :element.sync="element.element"></export-element>
                            </v-card-text>
                        </v-card>
                    </v-stepper-content>
                    </template>
            </v-stepper>
           </v-flex>
        </v-layout>
         <v-layout row>
            <v-dialog v-model="deleteDialog" persistent max-width="400">
               <v-card>
                    <v-card-title class="headline">Удаление</v-card-title>
                    <v-card-text>Вы действительно хотите удалить элемент?</v-card-text>
                    <v-spacer></v-spacer>
                    <v-card-actions>
                        <v-btn color="pink" text @click="deleteElement()">Удалить</v-btn>
                        <v-btn color="indigo" text @click="deleteDialog = false">Отмена</v-btn>
                    </v-card-actions>
                </v-card>
            </v-dialog>
        </v-layout>
         <v-layout row>
            <v-dialog v-model="exportDialog" persistent max-width="400">
               <v-card>
                    <v-card-title class="headline">Экспорт</v-card-title>
                    <v-card-text>
                        <v-layout row>
                            <v-flex lg10 offset-lg1>
                                <v-text-field v-model="waferId" :error-messages="waferId ? [] : 'Введите номер пластины'"
                                    label="Введите номер пластины"
                                ></v-text-field>
                            </v-flex>
                        </v-layout>
                        <v-layout row>
                            <v-flex lg10 offset-lg1>
                                <v-text-field v-model="mslNumber"
                                    label="Введите номер МСЛ"
                                ></v-text-field>
                            </v-flex>
                        </v-layout>
                        <v-layout row>
                            <v-flex lg10 offset-lg1>
                                <v-text-field v-model="currentDate"
                                 label="Текущая дата"
                                ></v-text-field>
                            </v-flex>
                        </v-layout>
                        <v-layout row>
                            <v-flex lg10 offset-lg1>
                                <v-text-field v-model="filename" :error-messages="filename ? [] : 'Введите название файла'"
                                    label="Введите имя файла"
                                ></v-text-field>
                            </v-flex>
                        </v-layout>
                    </v-card-text>
                    <v-spacer></v-spacer>
                    <v-card-actions>
                        <v-btn v-if="filename" color="green" text @click="exportK()">Сформировать файл</v-btn>
                        <v-btn color="pink" text @click="exportDialog = false">Отмена</v-btn>
                    </v-card-actions>
                </v-card>
            </v-dialog>
        </v-layout>
         <v-layout row>
            <v-dialog v-model="initialDialog" persistent max-width="400">
               <v-card class="mx-auto">
                    <v-card-text>
                       <v-select class="pt-8" v-model="selectedPattern"
                                    :items="patterns"
                                    no-data-text="Нет данных"
                                    outlined
                                    item-value="id"
                                    item-text="name"
                                    label="Выберите начальный шаблон:">
                        </v-select>
                    </v-card-text>
                    <v-spacer></v-spacer>
                    <v-card-actions>
                        <v-spacer></v-spacer>
                        <v-btn color="indigo" @click="selectPattern()">Выбрать</v-btn>
                   </v-card-actions>
                </v-card>
            </v-dialog>
        </v-layout>
        <v-dialog
      v-model="dialog"
      max-width="1000"
    >
      <div v-for="element in autoIdmrStatus" :key="element.key">
        <p>{{element.element + " " + element.operation + " " + element.done}}</p>
      </div>
    </v-dialog>
    </v-container>
</template>

<script>
import exportElement from './export-element.vue';

export default {
  data() {
    return {
      e1: 0,
      waferId: 'PAS23_BIBLIO',
      mslNumber: '',
      menu: false,
      filename: '',
      currentDate: new Date().toLocaleDateString('ru-RU'),
      elements: [],
      patterns: ['Пустой', 'PHEMT05_СМКК', 'PHEMT05_ВП', 'CKBA_PASSIVE', 'SKY', 'SKYVP', 'VA50N_NU', 'VA50N_CLIMATE', 'ZION', 'ZIONVP'],
      selectedPattern: 'Пустой',
      dividers: [],
      initialDialog: true,
      deleteDialog: false,
      exportDialog: false,
      elementsVM: [],
      dialog: false,
    };
  },
  components:
    {
      'export-element': exportElement,
    },
  computed:
    {
      readyToExport() {
        return this.$store.getters['exportkurb/isReadyForExport'];
      },
      autoIdmrStatus() {
        return this.$store.getters['exportkurb/elementsAutoIdmrStatus'];
      },

    },
  methods:
    {
      async getAutoIdmr() {
        this.$store.commit('exportkurb/clearAutoIdmr');
        this.dialog = true;
        this.$store.commit('exportkurb/initAutoIdmr', {
          elements: this.elements.map((e) => ({
            key: e.key, operation: e.operation.number, element: e.element.name, done: 'loading',
          })),
        });
        this.elements.forEach((e) => {
          const { key, stageName } = e;
          e.operation.waferId = this.waferId;
          this.$refs[key][0].getAutoIdmr(this.waferId, stageName);
        });
        await this.getAvStages(this.waferId, this.elements);
      },

      async getAvStages(waferId, elements) {
        await this.$http
          .get(`/api/stage/wafer/${waferId}`)
          .then((response) => {
            elements.forEach((e) => e.operation.avStages = response.data);
          })
          .catch((error) => {
            console.log(error);
          });
      },
      async exportK() {
        const response = await this.$http({
          method: 'post',
          url: '/api/export/create-kurb',
          data: {
            waferId: this.waferId, mslNumber: this.mslNumber, date: this.currentDate, kurbatovXLSViewModelList: this.mapElementsToElementsVM(),
          },
          responseType: 'blob',
          config: {
            headers: {
              Accept: 'application/json',
              'Content-Type': 'application/json',
            },
          },
        })
          .then(() => {
            const url = window.URL.createObjectURL(new Blob([response.data]));
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', `${this.filename}.xlsx`);
            document.body.appendChild(link);
            link.click();
            this.exportDialog = false;
          });
      },
      async selectPattern() {
        let path = '';
        if (this.selectedPattern === 'PHEMT05_СМКК') path = 'kurb';
        if (this.selectedPattern === 'PHEMT05_ВП') path = 'vp';
        if (this.selectedPattern === 'CKBA_PASSIVE') path = 'ckba';
        if (this.selectedPattern === 'SKY') path = 'sky';
        if (this.selectedPattern === 'SKYVP') path = 'skyVP';
        if (this.selectedPattern === 'VA50N_NU') path = 'va50n_NU';
        if (this.selectedPattern === 'VA50N_CLIMATE') path = 'va50n_CLIMATE';
        if (this.selectedPattern === 'ZION') path = 'zionpcm';
        if (this.selectedPattern === 'ZIONVP') path = 'zion';
        if (this.selectedPattern !== 'Пустой') {
          await this.$http
            .get(`/api/export/pattern/${path}`)
            .then((response) => {
              const elementsVm = response.data;
              this.elements = this.mapElementsVMToElements(elementsVm);
              this.e1 = 1;
            })
            .catch((error) => {
              console.log(error);
            });
        }
        this.initialDialog = false;
      },
      mapElementsToElementsVM() {
        const elements = JSON.parse(JSON.stringify(this.elements));
        return elements.map((x) => {
          x.parameters = x.parameters.map((p) => {
            const pVm = {
              lower: p.bounds.lower.value,
              upper: p.bounds.upper.value,
              parameterName: p.parameterName.value,
              russianParameterName: p.russianParameterName.value,
              parameterNameStat: p.selectedStatParameter,
              dividerId: p.dividerId,
              divider: p.divider,
              measurementRecordingId: p.measurementRecording,
            };
            return pVm;
          });
          x.isAddedToCommonWorksheet = x.element.isAddedToCommonWorksheet;
          x.elementName = x.element.name;
          x.operationNumber = x.operation.number;
          delete x.operation;
          delete x.element;
          return x;
        });
      },
      mapElementsVMToElements(elementsVm) {
        const elements = JSON.parse(JSON.stringify(elementsVm));
        return elements.map((x) => {
          x.parameters = x.parameters.map((p) => {
            p.lower = +p.lower ? p.lower : '';
            p.upper = +p.upper ? p.upper : '';
            const pVm = {
              parameterName: { value: p.parameterName, isValidDirty: false, isValid: true },
              russianParameterName: { value: p.russianParameterName, isValidDirty: false, isValid: true },
              waferId: '',
              measurementRecording: 0,
              selectedStatParameter: p.parameterNameStat,
              bounds: {
                lower: {
                  value: p.lower, isValidDirty: false, isValid: true, errorMessages: [],
                },
                upper: {
                  value: p.upper, isValidDirty: false, isValid: true, errorMessages: [],
                },
              },
              dividerId: p.dividerId,
              divider: p.divider.toFixed(3),
              statParameterArray: [],
              shortLink: { value: '', success: '', errorMessage: '' },
            };
            return pVm;
          });
          x.operation = {
            number: x.operationNumber, waferId: '', stageName: x.stageName, avStages: [], errorMessage: 'Введите номер операции',
          },
          x.element = { name: x.elementName, isAddedToCommonWorksheet: x.isAddedToCommonWorksheet, errorMessage: 'Введите название элемента' },
          x.key = `f${(~~(Math.random() * 1e8)).toString(16)}`;
          delete x.operationNumber;
          delete x.elementName;
          delete x.isAddedToCommonWorksheet;
          return x;
        });
      },
      createElement() {
        const element = {
          key: `f${(~~(Math.random() * 1e8)).toString(16)}`,
          operation: { number: '', avStages: [], errorMessage: 'Введите номер операции' },
          element: { name: '', errorMessage: 'Введите название элемента' },
          parameters: [],

        };
        this.elements.splice(this.e1, 0, element);
        this.e1 += 1;
      },
      deleteElement() {
        this.$store.commit('exportkurb/deleteFromElementsReady', this.elements[this.e1 - 1].key);
        this.elements.splice(this.e1 - 1, 1);
        this.deleteDialog = false;
        this.e1 -= 1;
      },
    },
  async created() {
    await this.$http
      .get('/api/divider/all')
      .then((response) => {
        this.dividers = response.data;
      });
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
