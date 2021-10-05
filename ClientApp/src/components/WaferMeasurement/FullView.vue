<template>
  <v-container grid-list-lg>
    <div class="usefulButtons d-flex flex-column">
      <v-tooltip left>
        <template v-slot:activator="{ on }">
          <v-btn
            v-scroll="onScroll"
            v-show="fabToTop"
            fab
            small
            dark
            color="indigo"
            v-on="on"
            @click="toTop">
            <v-icon>keyboard_arrow_up</v-icon>
          </v-btn>
          </template>
          <span>Наверх страницы</span>
      </v-tooltip>
      <v-tooltip left>
        <template v-slot:activator="{ on }">
          <v-btn
            v-scroll="onScroll"
            v-show="fabToTop"
            fab
            small
            dark
            color="indigo"
            v-on="on"
            @click="selectAllDies(avbSelectedDies)">
            <v-icon>all_inclusive</v-icon>
          </v-btn>
          </template>
          <span>Выбрать все кристаллы</span>
      </v-tooltip>
      <v-tooltip left>
        <template v-slot:activator="{ on }">
          <v-btn
            v-scroll="onScroll"
            v-show="fabToTop"
            fab
            small
            dark
            color="indigo"
            v-on="on"
            @click="delDirtyCells(dirtyCellsSnapshot.badDies, selectedDies)">
            <v-icon>cached</v-icon>
          </v-btn>
          </template>
          <span>Отсеять по всем параметрам</span>
      </v-tooltip>
    </div>
    <loading
      :active.sync="loading"
      :can-cancel="false"
      color="#fc0"
      :width="50"
      :height="50"
      loader="bars"
      :is-full-page="true"
    ></loading>
    <v-row wrap>
      <v-col lg="6">
        <v-row justify-center column>
          <mini-report :waferId="selectedWafer" :selectedMeasurementId="selectedMeasurementId"></mini-report>
        </v-row>
        <v-row justify-center column>
          <v-col>
            <v-tabs v-model="activeTab"
                    background-color='indigo' dark slider-color="primary" icons-and-text>
              <v-tab href="#wafer">
                Выбор измерения
                <v-icon>table_chart</v-icon>
              </v-tab>
              <v-tab href="#extra">
                Доп.настройки
                <v-icon>timeline</v-icon>
              </v-tab>
              <v-tab href="#statistics">
                Статистика по годным
                <v-icon>equalizer</v-icon>
              </v-tab>
              <v-tab-item value="wafer">
                <v-card color="#303030" dark>
                  <v-card-text>
                    <v-layout justify-center column>
                      <v-autocomplete
                        v-model="selectedWafer"
                        :items="wafers"
                        no-data-text="Нет данных"
                        item-text="waferId"
                        item-value="waferId"
                        filled
                        readonly
                        outlined
                        label="Номер пластины"
                        color='primary'
                      ></v-autocomplete>
                      <v-select
                        v-if="measurementRecordings.length>0"
                        v-model="selectedMeasurementId"
                        :items="measurementRecordings"
                        no-data-text="Нет данных"
                        item-text="name"
                        item-value="id"
                        filled
                        outlined
                        color='primary'
                        @change="measurementRecordingIdChanged($event)"
                        label="Выберите измерение:">
                      </v-select>
                      <v-progress-circular class="ml-4"
                        v-else
                        :width="3"
                        indeterminate
                        color="primary">
                      </v-progress-circular>
                    </v-layout>
                  </v-card-text>
                </v-card>
              </v-tab-item>
              <v-tab-item value="extra">
                <v-card color="#303030" dark>
                  <v-card-text>
                    <v-row justify-center column>
                      <v-col lg="6">
                        <v-select
                          v-model="selectedDivider"
                          :items="dividers"
                          no-data-text="Нет данных"
                          item-text="name"
                          item-value="dividerK"
                          filled
                          outlined
                          label="Выберите приведение к мм"
                        ></v-select>
                      </v-col>
                    </v-row>
                    <v-row justify-center column v-if="loading">
                      <v-col lg="6">
                        <v-text-field outlined v-model="shortLinkSrv6" label="Короткая ссылка"></v-text-field>
                      </v-col>
                       <v-col lg="6">
                         <v-btn color="primary" class="mt-4" block outlined @click="handleShortLinkSrv6(shortLinkSrv6)">Обработать ссылку</v-btn>
                      </v-col>
                    </v-row>
                    <v-row justify-center column v-if="selectedMeasurementId>0">
                        <v-col lg="12">
                          <v-row>
                            <v-col lg="12 ">
                              <v-text-field id="shortLinkTextBox" v-model="shortLinkSrv6" outlined readonly label="Короткая ссылка"></v-text-field>
                            </v-col>
                          </v-row>
                           <v-row>
                              <v-col lg="4">
                                <v-select :items="['srv3', 'srv6']"
                                   outlined
                                   v-model="generateShortLinkMode"
                                   label="Выберите тип ссылки">
                                </v-select>
                              </v-col>
                              <v-col lg="8">
                                <v-btn color='primary'
                                       сlass="mt-4" block outlined @click="generateShortLink">Сгенерировать ссылку</v-btn>
                              </v-col>
                            </v-row>
                            <v-row>
                              <v-btn v-if="shortLinkSrv6.length>0" color="green" class="mt-4" block @click="copyShortLink">Скопировать ссылку</v-btn>
                            </v-row>
                      </v-col>
                    </v-row>
                  </v-card-text>
                </v-card>
              </v-tab-item>
              <v-tab-item value="statistics">
                <v-row>
                  <v-col lg="8">
                    <v-card dark>
                      <perfect-scrollbar>
                        <micro-row class="mt-2" v-for="graphic in availiableGraphics"
                                  :key="`kgs-${graphic.keyGraphicState}`" :keyGraphicState="graphic.keyGraphicState"></micro-row>
                      </perfect-scrollbar>
                    </v-card>
                  </v-col>
                  <v-col lg="4">
                  <v-btn color='primary' outlined small @click="selectAllGraphics">
                    Выбрать все графики
                  </v-btn>
                  <v-chip class="elevation-12 mt-4" color="#303030" dark>Годны по всей пластине</v-chip>
                  <v-card class="mr-2 mt-2 mb-4" color="#303030" dark>
                      <v-card-text>
                        <v-progress-circular v-if="selectedDies.length > 0"
                          :rotate="360"
                          :size="90"
                          :width="3"
                          :value="dirtyCellsSnapshot.goodDiesPercentage"
                          :color="$store.getters['wafermeas/calculateColor'](dirtyCellsSnapshot.goodDiesPercentage / 100)">
                                        {{ dirtyCellsSnapshot.goodDiesPercentage }}%
                        </v-progress-circular>
                        <v-chip v-else>
                            <span>?</span>
                          </v-chip>
                        <v-tooltip class="ml-8" v-model="showUnSelectedGraphics" v-if="unSelectedGraphics.length>0" top>
                          <template v-slot:activator="{ on, attrs }">
                            <v-btn icon v-bind="attrs" v-on="on">
                              <v-icon color="primary">report</v-icon>
                            </v-btn>
                          </template>
                          <div>
                            <p>
                              <span>Некоторые графики не выбраны:</span>
                            </p>
                            <p v-for="g in unSelectedGraphics" :key="'UNSLCTDGR_'+g.keyGraphicState">
                              <span :key="'SPAN_'+g.keyGraphicState">{{g.graphicName}}</span>
                            </p>
                          </div>
                      </v-tooltip>
                      </v-card-text>
                    </v-card>
                      <v-chip class="elevation-12 mt-4 ms-2" color="#303030" dark>Годны из выбранных</v-chip>
                      <v-card class="mr-2 mt-2" color="#303030" dark>
                        <v-card-text>
                          <v-progress-circular v-if="selectedDies.length > 0"
                            :rotate="360"
                            :size="90"
                            :width="3"
                            :value="dirtyCellsPercentage"
                            color='primary'>
                            {{ dirtyCellsPercentage }}%
                          </v-progress-circular>
                          <v-chip v-else>
                            <span>?</span>
                          </v-chip>
                          <v-btn v-if="dirtyCellsPercentage != 100 && !isNaN(dirtyCellsPercentage)" text icon color='primary'
                                 @click="delDirtyCells(dirtyCellsSnapshot.badDies, selectedDies)">
                            <v-icon>cached</v-icon>
                          </v-btn>
                        </v-card-text>
                      </v-card>
                  </v-col>
                </v-row>
              </v-tab-item>
            </v-tabs>
          </v-col>
        </v-row>
      </v-col>
      <v-col lg="5" offset-lg="1">
        <wafermap-svg>
        </wafermap-svg>
      </v-col>
    </v-row>
    <v-divider></v-divider>
    <graphic-row v-for="graphic in availiableGraphics.filter(x => selectedGraphics.includes(x.keyGraphicState))"
                 :id="`kgs-${graphic.keyGraphicState}`"
                 :key="`kgs-${graphic.keyGraphicState}`"
                 :selectedMeasurementId="selectedMeasurementId"
                 :keyGraphicState="graphic.keyGraphicState"
                 :selectedDivider="selectedDivider"
                 :selectedDiesLength="selectedDies.length">
    </graphic-row>
  </v-container>
</template>

<script>
import { mapGetters } from 'vuex';
import Loading from 'vue-loading-overlay';
import MiniReport from './MiniReport.vue';
import WaferMap from './WaferMap.vue';
import MiniGraphicRow from './MiniGraphicRow.vue';
import GraphicRowFullInfo from './GraphicRowFullInfo.vue';

export default {
  data() {
    return {
      toggle_exclusive: null,
      fabToTop: false,
      fabToNext: false,
      fabToPrev: false,
      loading: false,
      showUnSelectedGraphics: false,
      activeTab: 'wafer',
      dividers: [],
      wafers: [],
      selectedWafer: '',
      selectedDivider: '1.0',
      selectedMeasurementId: 0,
      shortLinkSrv6: '',
      generateShortLinkMode: 'srv3',
    };
  },

  components: {
    'graphic-row': GraphicRowFullInfo,
    'mini-report': MiniReport,
    'micro-row': MiniGraphicRow,
    'wafermap-svg': WaferMap,
    Loading,
  },

  async created() {
    this.dividers = (await this.$http.get('/api/divider/all')).data;
  },

  async mounted() {
    await this.routeHandler(this.$route.name);
  },

  computed: {

    ...mapGetters({
      dirtyCellsSnapshot: 'wafermeas/dirtyCellsSnapshot',
      avbSelectedDies: 'wafermeas/avbSelectedDies',
      selectedDies: 'wafermeas/selectedDies',
      selectedGraphics: 'wafermeas/selectedGraphics',
      unSelectedGraphics: 'wafermeas/unSelectedGraphics',
      availiableGraphics: 'wafermeas/avbGraphics',
      measurementRecordings: 'wafermeas/measurements',
    }),

    dirtyCellsPercentage() {
      return Math.ceil((1.0 - (this.dirtyCellsSnapshot.badDies.length - ([...new Set([...this.selectedDies, ...this.dirtyCellsSnapshot.badDies])].length - this.selectedDies.length)) / this.selectedDies.length) * 100);
    },

    selectedGraphicsIcon() {
      if (this.availiableGraphics.length === this.selectedGraphics.length) { return 'check_box'; }
      if (this.selectedGraphics.length > 0) return 'indeterminate_check_box';
      return 'check_box_outline_blank';
    },
  },

  methods: {

    showSnackbar(text) {
      this.$store.dispatch('alert/success', text);
    },

    onScroll(e) {
      if (typeof window === 'undefined') return;
      const top = window.pageYOffset || e.target.scrollTop || 0;
      this.fabToTop = top > 20;
    },

    toTop() {
      this.$vuetify.goTo(0);
    },

    copyShortLink() {
      const copyText = document.querySelector('#shortLinkTextBox');
      copyText.select();
      document.execCommand('copy');
    },

    async handleShortLinkSrv6(shortLink) {
      const shortLinkVm = (await this.$http.get(`/api/shortlink/guid/${shortLink.split('=')[1].trim()}`)).data;
    },

    async generateShortLink() {
      const generateShortLinkViewModel = {
        waferId: this.selectedWafer,
        measurementRecordingId: this.selectedMeasurementId,
        divider: this.selectedDivider,
        selectedDies: [...this.selectedDies],
        selectedGraphics: this.$store.getters['wafermeas/getGraphicSettingsKeyGraphicStates'](this.selectedGraphics),
      };
      await this.$http.post(`/api/shortlink/generate/${this.generateShortLinkMode}`, generateShortLinkViewModel)
        .then((response) => {
          this.shortLinkSrv6 = response.data.shortLink;
          this.showSnackbar('Ссылка успешно создана');
        })
        .catch(() => {
          this.showSnackbar('Ошибка при генерации ссылки');
        });
    },

    async routeHandler(routeName) {
      if (routeName === 'wafermeasurement-onlywafer') {
        this.selectedWafer = this.$route.params.waferId;
      }
      if (routeName === 'wafermeasurement-fullselected') {
        this.selectedWafer = this.$route.params.waferId;
        await this.$store.dispatch('wafermeas/updateSelectedWaferId', { ctx: this, waferId: this.$route.params.waferId }).then(async () => {
          this.selectedMeasurementId = this.measurementRecordings.find((x) => x.name === this.$route.params.measurementName).id;
          await this.measurementRecordingIdChanged(this.selectedMeasurementId);
        });
      }
      if (routeName === 'wafermeasurement-shortlink') {
        await this.resolveShortLink(this.$route.params);
      }
    },

    async resolveShortLink(params) {
      this.selectedWafer = params.waferId;
      this.loading = true;
      this.$store.dispatch('wafermeas/updateSelectedDies', []);
      this.$store.dispatch('wafermeas/clearSelectedGraphics');
      await this.$store.dispatch('wafermeas/updateSelectedWaferId', { ctx: this, waferId: params.waferId }).then(async () => {
        const selectedMeasurementId = this.measurementRecordings.find((x) => x.name === params.measurementName).id;
        const dirtyCellsSnapshot = (await this.$http.get(`/api/statrwrk/dirtycellssnapshot/${selectedMeasurementId}/initial`)).data;
        this.$store.dispatch('wafermeas/createDirtyCellsSnapshot', dirtyCellsSnapshot);
        this.$store.dispatch('wafermeas/createDcProfiles', dirtyCellsSnapshot.dirtyCellsProfiles);
        const keyGraphicStateJSON = JSON.stringify(Object.keys(dirtyCellsSnapshot.singleGraphicDirtyCellsDictionary));
        this.$store.dispatch('wafermeas/updateAvbSelectedDies', dirtyCellsSnapshot.selectedDies);
        this.selectedDivider = params.shortLinkVm.divider.dividerK;
        this.$store.dispatch('wafermeas/updateSelectedDies', params.shortLinkVm.selectedDies);
        this.$store.dispatch('wafermeas/updateSelectedGraphics', [...params.shortLinkVm.selectedGraphics.map((g) => g.keyGraphicState)]);
        this.selectedMeasurementId = selectedMeasurementId;
        const availiableGraphics = (await this.$http.get(`/api/graphicsrv6/GetAvailiableGraphicsByKeyGraphicStateList?keyGraphicStateJSON=${keyGraphicStateJSON}`)).data;
        this.$store.dispatch('wafermeas/updateAvbGraphics', availiableGraphics);
        this.selectAllGraphics();
        this.loading = false;
        this.activeTab = 'statistics';
      });
    },

    async measurementRecordingIdChanged(selectedMeasurementId) {
      this.loading = true;
      this.$store.dispatch('wafermeas/updateSelectedDies', []);
      this.$store.dispatch('wafermeas/clearSelectedGraphics');
      const dirtyCellsSnapshot = (await this.$http.get(`/api/statrwrk/dirtycellssnapshot/${selectedMeasurementId}/initial`)).data;
      this.$store.dispatch('wafermeas/createDirtyCellsSnapshot', dirtyCellsSnapshot);
      this.$store.dispatch('wafermeas/createDcProfiles', dirtyCellsSnapshot.dirtyCellsProfiles);
      const keyGraphicStateJSON = JSON.stringify(Object.keys(dirtyCellsSnapshot.singleGraphicDirtyCellsDictionary));
      this.$store.dispatch('wafermeas/updateAvbSelectedDies', dirtyCellsSnapshot.selectedDies);
      this.$store.dispatch('wafermeas/updateSelectedDies', dirtyCellsSnapshot.selectedDies);
      this.delDirtyCells(this.dirtyCellsSnapshot.badDies, this.avbSelectedDies);
      const availiableGraphics = (await this.$http.get(`/api/graphicsrv6/GetAvailiableGraphicsByKeyGraphicStateList?keyGraphicStateJSON=${keyGraphicStateJSON}`)).data;
      this.$store.dispatch('wafermeas/updateAvbGraphics', availiableGraphics);
      this.selectAllGraphics();
      this.loading = false;
      this.activeTab = 'statistics';
      await this.$router.push({
        name: 'wafermeasurement-fullselected',
        params: { waferId: this.selectedWafer, measurementName: this.measurementRecordings.find((x) => x.id === selectedMeasurementId).name },
      });
    },

    delDirtyCells(dirtyCellsList, selectedDies) {
      this.$store.dispatch('wafermeas/updateSelectedDies', selectedDies.filter((die) => !dirtyCellsList.includes(die)));
    },

    selectAllDies(avbSelectedDies) {
      this.$store.dispatch('wafermeas/updateSelectedDies', [...avbSelectedDies]);
    },

    selectAllGraphics() {
      this.$nextTick(() => {
        if (this.selectedGraphics.length !== this.availiableGraphics.length) {
          this.$store.dispatch('wafermeas/updateSelectedGraphics', [...this.availiableGraphics.map((g) => g.keyGraphicState)]);
          if (this.unSelectedGraphics.length > 0) {
            this.$store.dispatch('wafermeas/addToDirtyCells',
              { keyGraphicState: this.unSelectedGraphics.map((g) => g.keyGraphicState), avbSelectedDies: this.avbSelectedDies });
          }
        }
      });
    },
  },

  watch: {

    async selectedWafer(newValue) {
      this.wafers = [newValue];
      this.$store.dispatch('wafermeas/updateAvbGraphics', []);
      this.$store.dispatch('wafermeas/updateDieColors', { ctx: this, waferId: newValue });
      this.$store.dispatch('wafermeas/updateSelectedWaferId', { ctx: this, waferId: newValue });
    },

    availiableGraphics() {
      if (this.availiableGraphics.length === 0) {
        this.$store.dispatch('wafermeas/clearSelectedGraphics');
      }
    },
  },

  beforeDestroy() {
    this.$store.dispatch('wafermeas/reset');
  },
};
</script>

<style scoped>
  .ps {
    height: 400px;
  }
  .usefulButtons {
    bottom: 0;
    right: 0;
    position: fixed;
    z-index: 9999;
    opacity: 0.65;
  }
</style>
