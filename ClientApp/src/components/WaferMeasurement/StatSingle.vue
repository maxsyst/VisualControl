<template>
    <v-container fluid>
      <toolbar :loading="loading"
               :graphicName="graphicName"
               :rowViewMode="rowViewMode"
               :keyGraphicState="keyGraphicState">
      </toolbar>
      <v-row>
        <v-col lg="12">
          <v-tabs :vertical="rowViewMode=='miniChart'" v-model="activeTab" color="primary" dark slider-color="indigo">
            <v-tab href="#commonTable">Сводная</v-tab>
            <v-tab
              v-for="stat in statArrayRWRK"
              :disabled="rowViewMode!=='miniChart'"
              :key="stat.shortStatisticsName"
              :href="'#' + stat.shortStatisticsName"
              v-html="stat.shortStatisticsName">
            </v-tab>
            <v-tab-item
              v-for="stat in statArrayRWRK"
              :key="stat.shortStatisticsName"
              :value="stat.shortStatisticsName">
              <StatParameterInfo :measurementId="measurementId"
                                 :keyGraphicState="keyGraphicState"
                                 :statParameter="stat"
                                 :divider="divider">
              </StatParameterInfo>
            </v-tab-item>
            <v-tab-item value="commonTable">
              <v-card flat>
                <v-card-text>
                  <v-row>
                    <v-col lg="12">
                      <v-data-table v-if="statArrayRWRK.length > 0"
                        :headers="headers"
                        :items="statArrayRWRK"
                        :items-per-page="maxRowCount"
                        loading-text="Загрузка данных..."
                        no-data-text="Нет данных"
                        class="elevation-2 pa-0"
                        :loading="loading"
                        hide-default-footer
                        dark>
                        <template v-slot:item.shortStatisticsName="{ item }">
                          <v-tooltip bottom>
                            <template v-slot:activator="{ on }">
                              <v-chip color="indigo" v-on="on"
                                      label v-html="item.unit.trim() ? item.shortStatisticsName + ', ' + item.unit : item.shortStatisticsName"
                                      dark>
                              </v-chip>
                            </template>
                            <span v-html="item.statisticsName"></span>
                          </v-tooltip>
                        </template>
                        <template v-slot:item.goodDiesPercentage="{item}">
                          <td class="text-center">
                            <v-progress-circular v-if="selectedDies.length > 0"
                              :rotate="360"
                              :size="45"
                              :width="2"
                              :value="item.goodDiesPercentage"
                              :color="$store.getters['wafermeas/calculateColor'](item.goodDiesPercentage/100)"
                            >{{ item.goodDiesPercentage + '%'}}</v-progress-circular>
                            <v-chip v-else>
                              <span>?</span>
                            </v-chip>
                          </td>
                        </template>
                        <template v-slot:item.dirtyCells="{item}">
                        <td class="text-center">
                          <v-progress-circular v-if="selectedDies.length > 0"
                            :rotate="360"
                            :size="45"
                            :width="2"
                            :value="item.dirtyCells.percentage"
                            color='primary'>
                            {{ item.dirtyCells.percentage  }}%
                          </v-progress-circular>
                          <v-chip v-else>
                            <span>?</span>
                          </v-chip>
                          <v-btn  v-if="item.dirtyCells.percentage != 100 && !isNaN(item.dirtyCells.percentage)"
                                  text icon color='primary' @click="delDirtyCells(item.dirtyCells.array)">
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
import StatParameterInfo from './StatParameterInfo.vue';

export default {
  props: ['keyGraphicState', 'measurementId', 'divider', 'rowViewMode'],
  components: {
    toolbar: Toolbar,
    StatParameterInfo,
  },
  data() {
    return {
      showPopover: false,
      PopoverX: 0,
      PopoverY: 0,
      maxRowCount: 256,
      statArrayRWRK: [],
      graphicName: '',
      activeTab: 'commonTable',
      loading: false,
      headersConfigArray: [
        /// MonitoringMini
        {
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
            value: 'goodDiesPercentage',
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
            value: 'goodDiesPercentage',
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
      ],
    };
  },

  async created() {
    this.graphicName = this.$store.getters['wafermeas/getGraphicByGraphicState'](this.keyGraphicState).graphicName;
    await this.getStatArray();
  },

  beforeDestroy() {
    this.statArrayRWRK = null;
  },

  methods: {
    delDirtyCells(dirtyCells) {
      const selectedDies = this.selectedDies.filter((el) => !dirtyCells.includes(el));
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
      if (this.measurementId !== 0) {
        const singlestatModel = {};
        singlestatModel.divider = this.divider;
        singlestatModel.keyGraphicState = this.keyGraphicState;
        singlestatModel.measurementId = this.measurementId;
        singlestatModel.dieIdList = this.selectedDies;
        const path = '/api/statrwrk/StatisticSingleGraphic?statisticSingleGraphicViewModelJSON';
        this.statArrayRWRK = (await this.$http.get(`${path}=${JSON.stringify(singlestatModel)}`)).data;
        this.statArrayRWRK = this.statArrayRWRK.map((s) => ({ ...s, goodDiesPercentage: this.dirtyCellsSnapshot.statNameDirtyCellsDictionary[s.statisticsName].goodDiesPercentage, dirtyCells: { array: [...this.dirtyCellsSnapshot.statNameDirtyCellsDictionary[s.statisticsName].badDirtyCells], percentage: Math.ceil((1.0 - (this.dirtyCellsSnapshot.statNameDirtyCellsDictionary[s.statisticsName].badDirtyCells.length - ([...new Set([...this.selectedDies, ...this.dirtyCellsSnapshot.statNameDirtyCellsDictionary[s.statisticsName].badDirtyCells])].length - this.selectedDies.length)) / this.selectedDies.length) * 100) } }));
      }
    },
  },

  watch: {
    async divider() {
      await this.getStatArray();
    },

    async selectedDies() {
      await this.getStatArray();
    },

    dirtyCellsSnapshot() {
      this.statArrayRWRK = this.statArrayRWRK.map((s) => ({
        ...s,
        goodDiesPercentage: this.dirtyCellsSnapshot.statNameDirtyCellsDictionary[s.statisticsName].goodDiesPercentage,
        dirtyCells: {
          array: [...this.dirtyCellsSnapshot.statNameDirtyCellsDictionary[s.statisticsName].badDirtyCells],
          percentage: Math.ceil((1.0 - (this.dirtyCellsSnapshot.statNameDirtyCellsDictionary[s.statisticsName].badDirtyCells.length - ([...new Set([...this.selectedDies, ...this.dirtyCellsSnapshot.statNameDirtyCellsDictionary[s.statisticsName].badDirtyCells])].length - this.selectedDies.length)) / this.selectedDies.length) * 100),
        },
      }));
    },
  },

  computed: {

    ...mapGetters({
      selectedDies: 'wafermeas/selectedDies',
    }),

    headers() {
      return this.headersConfigArray.find((x) => x.rowViewMode === this.rowViewMode).headers;
    },

    dirtyCellsSnapshot() {
      return this.$store.getters['wafermeas/getDirtyCellsSnapshotByKeyGraphicState'](this.keyGraphicState);
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
