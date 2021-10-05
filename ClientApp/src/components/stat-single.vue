<template>
    <v-container fluid>
      <toolbar :loading="loading"
               :graphicName="graphicName"
               :dirtyCellsFullWafer="dirtyCellsFullWafer"
               :dirtyCellsStatPercentage="dirtyCellsStatPercentage"
               :dirtyCellsFixedPercentage="dirtyCellsFixedPercentage"
               :rowViewMode="rowViewMode"
               :dirtyCells="dirtyCells"
               :viewMode="viewMode"
               :keyGraphicState="keyGraphicState">
      </toolbar>
      <v-row>
        <v-col lg="12">
          <v-tabs v-model="activeTab" color="primary" dark slider-color="indigo">
            <v-tab href="#commonTable">Сводная таблица</v-tab>
            <v-tab
              v-for="stat in statArray"
              :disabled="rowViewMode!=='miniChart'"
              :key="stat.shortStatisticsName"
              :href="'#' + stat.shortStatisticsName"
              v-html="stat.shortStatisticsName">
            </v-tab>
            <v-tab-item
              v-for="stat in statArray"
              :key="stat.shortStatisticsName"
              :value="stat.shortStatisticsName">
              <gradient-full :key="'GRF_' + stat.shortStatisticsName + keyGraphicState"
                             :measurementId="measurementId"
                             :keyGraphicState="keyGraphicState"
                             :statParameter="stat"
                             :divider="divider"
                             :statisticKf="statisticKf">
              </gradient-full>
            </v-tab-item>
            <v-tab-item value="commonTable">
              <v-card flat>
                <v-card-text>
                  <v-row>
                    <v-col lg="12">
                      <v-data-table v-if="statArray.length > 0"
                        :headers="headers"
                        :items="statArray"
                        loading-text="Загрузка данных..."
                        no-data-text="Нет данных"
                        class="elevation-2 pa-0"
                        :loading="loading"
                        hide-default-footer
                        dark>
                        <template v-slot:item.shortStatisticsName="{ item }">
                          <v-tooltip bottom>
                            <template v-slot:activator="{ on }">
                              <v-chip color="indigo" v-on="on" label v-html="item.unit.trim() ? item.shortStatisticsName + ', ' + item.unit : item.shortStatisticsName" dark></v-chip>
                            </template>
                            <span v-html="item.statisticsName"></span>
                          </v-tooltip>
                        </template>
                        <template v-slot:item.fwPercentage="{item}">
                          <td v-if="viewMode==='Мониторинг'" class="text-xs-center">
                            <v-progress-circular
                              :rotate="360"
                              :size="45"
                              :width="2"
                              :value="item.fwPercentage.stat"
                              :color="$store.getters['wafermeas/calculateColor'](item.fwPercentage.stat/100)"
                            >{{ item.fwPercentage.stat + '%'}}</v-progress-circular>
                          </td>
                          <td v-else class="text-xs-center">
                            <v-progress-circular
                              :rotate="360"
                              :size="45"
                              :width="2"
                              :value="item.fwPercentage.fixed"
                              :color="$store.getters['wafermeas/calculateColor'](item.fwPercentage.fixed/100)"
                            >{{ item.fwPercentage.fixed + '%'}}</v-progress-circular>
                          </td>
                        </template>
                        <template v-slot:item.dirtyCells="{item}">
                        <td class="text-xs-center">
                          <v-progress-circular
                            :rotate="360"
                            :size="45"
                            :width="2"
                            :value = "viewMode === `Мониторинг` ? item.dirtyCells.statPercentageFullWafer : item.dirtyCells.fixedPercentageFullWafer"
                            :color= "viewMode === `Мониторинг` ? 'primary' : '#80DEEA'"
                          >{{ viewMode === `Мониторинг` ? item.dirtyCells.statPercentageFullWafer + '%' : item.dirtyCells.fixedPercentageFullWafer + '%' }}</v-progress-circular>
                          <v-btn text icon :color="viewMode === `Мониторинг` ? 'primary' : '#80DEEA'" @click="delDirtyCells(item.dirtyCells)">
                            <v-icon>cached</v-icon>
                          </v-btn>
                        </td>
                        </template>
                      </v-data-table>
                      <div v-else>
                       <v-skeleton-loader v-if="loading"
                          class="mx-auto"
                          type="table-tbody"
                        ></v-skeleton-loader>
                        <p v-else>Не найдено статистических параметров на данном графике</p>
                      </div>
                    </v-col>
                  </v-row>
                </v-card-text>
              </v-card>
            </v-tab-item>
          </v-tabs>
        </v-col>
      </v-row>
    </v-container>
</template>

<script>
import { mapGetters } from 'vuex';
import Toolbar from './GraphicRowFullInfoToolbar.vue';
import GradientFull from './Gradient/gradient-full.vue';

export default {
  props: ['keyGraphicState', 'measurementId', 'viewMode', 'divider', 'statisticKf', 'rowViewMode'],
  components: {
    'gradient-full': GradientFull,
    toolbar: Toolbar,
  },
  data() {
    return {
      showPopover: false,
      PopoverX: 0,
      PopoverY: 0,
      statArray: [],
      fullWaferStatArray: [],
      dirtyCellsFullWafer: { fixed: { cellsId: [], percentage: -1 }, stat: { cellsId: [], percentage: -1 } },
      graphicName: '',
      activeTab: 'commonTable',
      loading: true,
      headersConfigArray: [
        /// MonitoringMini
        {
          viewMode: 'Мониторинг',
          rowViewMode: 'miniChart',
          headers: [{
            text: 'Название',
            align: 'center',
            sortable: false,
            value: 'shortStatisticsName',
            width: '20%',
          },
          {
            text: 'μ',
            align: 'center',
            sortable: false,
            value: 'expectedValue',
            width: '10%',
          },
          {
            text: 'σ',
            align: 'center',
            sortable: false,
            value: 'standartDeviation',
            width: '10%',
          },
          {
            text: 'Мин',
            align: 'center',
            sortable: false,
            value: 'minimum',
            width: '10%',
          },
          {
            text: 'Макс',
            align: 'center',
            sortable: false,
            value: 'maximum',
            width: '10%',
          },
          {
            text: 'Медиана',
            align: 'center',
            sortable: false,
            value: 'median',
            width: '10%',
          },
          {

            text: 'Годны по всей пластине, %',
            align: 'start',
            sortable: false,
            value: 'fwPercentage',
            width: '15%',
          },
          {
            text: 'Годны из выбранных, %',
            align: 'center',
            sortable: false,
            value: 'dirtyCells',
            width: '15%',
          },
          ],
        },

        /// MonitoringBig
        {
          viewMode: 'Мониторинг',
          rowViewMode: 'bigChart',
          headers: [{
            text: 'Название',
            align: 'center',
            sortable: false,
            value: 'shortStatisticsName',
            width: '25%',
          },
          {
            text: 'μ',
            align: 'center',
            sortable: false,
            value: 'expectedValue',
            width: '15%',
          },
          {
            text: 'σ',
            align: 'center',
            sortable: false,
            value: 'standartDeviation',
            width: '15%',
          },
          {

            text: 'Годны по всей пластине, %',
            align: 'start',
            sortable: false,
            value: 'fwPercentage',
            width: '15%',
          },
          {
            text: 'Годны из выбранных, %',
            align: 'center',
            sortable: false,
            value: 'dirtyCells',
            width: '30%',
          }],
        },

        /// SupplyMini
        {
          viewMode: 'Поставка',
          rowViewMode: 'miniChart',
          headers: [{
            text: 'Название',
            align: 'center',
            sortable: false,
            value: 'shortStatisticsName',
            width: '30%',
          },
          {
            text: 'Мин',
            align: 'center',
            sortable: false,
            value: 'lowBorderFixed',
            width: '20%',
          },
          {
            text: 'Макс',
            align: 'center',
            sortable: false,
            value: 'topBorderFixed',
            width: '20%',
          },
          {
            text: 'Годны по всей пластине, %',
            align: 'start',
            sortable: false,
            value: 'fwPercentage',
            width: '15%',
          },
          {
            text: 'Годны из выбранных, %',
            align: 'center',
            sortable: false,
            value: 'dirtyCells',
            width: '15%',
          },
          ],
        },
        {
          viewMode: 'Поставка',
          rowViewMode: 'bigChart',
          headers: [],
        },
      ],
    };
  },

  async created() {
    this.graphicName = this.$store.getters['wafermeas/getGraphicByGraphicState'](this.keyGraphicState).graphicName;
    this.fullWaferStatArray = (await this.$http
      .get(`/api/statistic/GetStatisticSingleGraphicFullWafer?measurementRecordingId=${this.measurementId}&keyGraphicState=${this.keyGraphicState}&k=${this.statisticKf}`)).data;
    this.calculateFullWaferDirtyCells(this.fullWaferStatArray);
    await this.getStatArray();
  },

  methods: {
    delDirtyCells(dirtyCells) {
      const deletedDies = this.viewMode === 'Мониторинг' ? dirtyCells.statList : dirtyCells.fixedList;
      const selectedDies = this.selectedDies.filter((el) => !deletedDies.includes(el));
      this.$store.dispatch('wafermeas/updateSelectedDies', selectedDies);
    },

    showStatTab(statisticsName) {
      this.activeTab = statisticsName;
    },

    showPopoverClick(e) {
      e.preventDefault();
      this.showPopover = false;
      this.PopoverX = e.clientX;
      this.PopoverY = e.clientY;
      this.$nextTick(() => {
        this.showPopover = true;
      });
    },

    async getStatArray() {
      if (this.measurementId != 0 && this.selectedDies.length > 0) {
        this.loading = true;
        const singlestatModel = {};
        singlestatModel.k = this.statisticKf;
        singlestatModel.divider = this.divider;
        singlestatModel.keyGraphicState = this.keyGraphicState;
        singlestatModel.measurementId = this.measurementId;
        singlestatModel.dieIdList = this.selectedDies;
        this.statArray = (await this.$http
          .get(`/api/statistic/GetStatisticSingleGraphic?statisticSingleGraphicViewModelJSON=${JSON.stringify(singlestatModel)}`)).data;
        this.statArray = this.statArray.map((s) => ({ ...s, fwPercentage: { fixed: this.fullWaferStatArray.find((f) => f.parameterID === s.parameterID).dirtyCells.fixedPercentageFullWafer, stat: this.fullWaferStatArray.find((f) => f.parameterID === s.parameterID).dirtyCells.statPercentageFullWafer } }));
        this.loading = false;
      }
    },

    calculateFullWaferDirtyCells(fullWaferStatArray) {
      this.dirtyCellsFullWafer.stat.cellsId = [...new Set(fullWaferStatArray.reduce((p, c) => [...p, ...c.dirtyCells.statList], []))];
      this.dirtyCellsFullWafer.fixed.cellsId = [...new Set(fullWaferStatArray.reduce((p, c) => [...p, ...c.dirtyCells.fixedList], []))];
      this.dirtyCellsFullWafer.stat.percentage = Math.ceil((1.0 - (this.dirtyCellsFullWafer.stat.cellsId.length / this.avbSelectedDies.length)) * 100);
      this.dirtyCellsFullWafer.fixed.percentage = Math.ceil((1.0 - (this.dirtyCellsFullWafer.fixed.cellsId.length / this.avbSelectedDies.length)) * 100);
      this.$store.dispatch('wafermeas/updateDirtyCellsFullWaferSingleGraphic', {
        keyGraphicState: this.keyGraphicState,
        dirtyCells: {
          fixed: { cellsId: [...this.dirtyCellsFullWafer.fixed.cellsId], percentage: this.dirtyCellsFullWafer.fixed.percentage },
          stat: { cellsId: [...this.dirtyCellsFullWafer.stat.cellsId], percentage: this.dirtyCellsFullWafer.stat.percentage },
        },
      });
    },
  },

  watch: {
    async divider() {
      await this.getStatArray();
    },

    async statisticKf(newValue) {
      this.fullWaferStatArray = (await this.$http
        .get(`/api/statistic/GetStatisticSingleGraphicFullWafer?measurementRecordingId=${this.measurementId}&keyGraphicState=${this.keyGraphicState}&k=${newValue}`)).data;
      this.calculateFullWaferDirtyCells(this.fullWaferStatArray);
      await this.getStatArray();
    },

    async selectedDies() {
      await this.getStatArray();
    },
  },

  computed: {

    ...mapGetters({
      selectedDies: 'wafermeas/selectedDies',
      avbSelectedDies: 'wafermeas/avbSelectedDies',
    }),

    headers() {
      return this.headersConfigArray.find((x) => x.viewMode === this.viewMode && x.rowViewMode === this.rowViewMode).headers;
    },

    dirtyCells() {
      let statArray = [];
      let fixedArray = [];
      this.statArray.forEach((s) => {
        (statArray = statArray.concat(s.dirtyCells.statList)),
        (fixedArray = fixedArray.concat(s.dirtyCells.fixedList));
      });
      return {
        statList: [...new Set(statArray)],
        fixedList: [...new Set(fixedArray)],
      };
    },

    dirtyCellsStatPercentage() {
      const dirtyCellsList = this.viewMode === 'Мониторинг' ? this.dirtyCells.statList : this.dirtyCells.fixedList;
      const percentage = Math.ceil((1.0 - dirtyCellsList.length / this.selectedDies.length) * 100);
      this.$store.dispatch('wafermeas/updateDirtyCellsSelectedNowSingleGraphic', {
        keyGraphicState: this.keyGraphicState,
        dirtyCells: { cellsId: [...dirtyCellsList], percentage },
        viewMode: this.viewMode,
      });
      return isNaN(percentage) ? 0 : percentage;
    },

    dirtyCellsFixedPercentage() {
      const percentage = Math.ceil((1.0 - this.dirtyCells.fixedList.length / this.selectedDies.length) * 100);
      return isNaN(percentage) ? 0 : percentage;
    },
  },
};
</script>

<style>
.card-shadow {
  --box-shadow-color: palegoldenrod;
  box-shadow: 1px 2px 3px var(--box-shadow-color);
}
</style>
