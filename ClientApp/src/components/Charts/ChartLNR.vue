<template>
  <div class="d-flex flex-column" v-if="loaded">
    <div class="d-flex">
      <v-col class="d-flex">
        <v-card>
          <v-container>
            <line-chart
              :id="'LCHART_' + keyGraphicState"
              ref="linechart"
              :class="rowViewMode"
              :keyGraphicState="keyGraphicState"
              :divider="divider"
              :settings="settings"
              :options="options"/>
          </v-container>
        </v-card>
      </v-col>
      <div class="d-flex flex-column justify-space-between">
        <div class="d-flex">
          <v-col class="d-flex flex-column justify-start">
            <v-btn :color="log === false ? 'indigo' : 'grey darken-2'" fab x-small dark @click="changeLogMode(false)">
              Лин
            </v-btn>
            <v-btn :color="log === true ? 'indigo' : 'grey darken-2'" fab x-small dark @click="changeLogMode(true)">
              Лог
            </v-btn>
          </v-col>
        </div>
        <div class="d-flex flex-column">
          <v-col class="d-flex">
          <v-tooltip left>
            <template v-slot:activator="{ on }">
              <v-btn color='grey darken-2' fab x-small dark  v-on="on" @click="changeRowViewMode(rowViewMode)">
                <v-icon v-if="rowViewMode==='miniChart'">aspect_ratio</v-icon>
                <v-icon v-else>vertical_split</v-icon>
              </v-btn>
            </template>
            <span>{{rowViewMode==='miniChart' ? 'Увеличить график' : 'Минимизировать график'}}</span>
          </v-tooltip>
          </v-col>
          <v-col class="d-flex">
            <v-tooltip left>
              <template v-slot:activator="{ on }">
                <v-btn color='grey darken-2' fab x-small dark v-on="on" @click="resetChart">
                <v-icon>refresh</v-icon>
                </v-btn>
              </template>
              <span>Восстановить настройки графика</span>
            </v-tooltip>
          </v-col>
          <v-col class="d-flex">
            <v-tooltip left>
              <template v-slot:activator="{ on }">
              <v-btn :color="showSettings === true ? 'indigo' : 'grey darken-2'" v-on="on" fab x-small dark
                      @click="showSettingsContainer(showSettings)">
                <v-icon>settings</v-icon>
              </v-btn>
              </template>
              <span>Настройки графика</span>
            </v-tooltip>
          </v-col>
        </div>
      </div>
    </div>
    <div class="d-flex">
      <settings v-if="showSettings" :keyGraphicState="keyGraphicState" @settings-changed="settingsChanged"></settings>
    </div>
  </div>
  <v-progress-circular v-else :size="50" color="primary" indeterminate></v-progress-circular>
</template>

<script>
// eslint-disable-next-line no-unused-vars
import ChartJsZoom from 'chartjs-plugin-zoom';
import LineChart from './LineChart.vue';
import Settings from './ChartSettings/ChartSettingsLNR';

export default {

  props: ['keyGraphicState', 'measurementId', 'divider', 'rowViewMode'],
  components: { LineChart, Settings },
  data: () => ({
    loaded: false,
    showSettings: false,
    settings: {},
    options: {},
  }),

  async mounted() {
    await this.getChartData();
  },

  computed: {
    log() {
      return this.$store.getters['wafermeas/getKeyGraphicStateLog'](this.keyGraphicState);
    },

  },

  beforeDestroy() {
    this.settings = null;
    this.options = null;
  },

  watch: {
    async divider() {
      await this.getChartData();
    },
  },

  methods: {

    async getChartData() {
      const avbSelectedDies = this.$store.getters['wafermeas/avbSelectedDies'];
      this.loaded = false;
      const singlestatModel = {};
      singlestatModel.keyGraphicState = this.keyGraphicState;
      singlestatModel.measurementId = this.measurementId;
      singlestatModel.dieIdList = avbSelectedDies;
      singlestatModel.divider = this.divider;
      await this.$http
        .get(`/api/chartjs/GetLinearForMeasurement?statisticSingleGraphicViewModelJSON=${JSON.stringify(singlestatModel)}`)
        .then((response) => {
          const chart = response.data;
          chart.chartData.labels = [...chart.chartData.labels.map((x) => +x)];
          this.calculateOptions(chart.options,
            { min: chart.chartData.labels[0], max: chart.chartData.labels[chart.chartData.labels.length - 1], maxTicksLimit: 11 },
            { min: 0, max: 0, maxTicksLimit: 11 });
          this.$store.dispatch('wafermeas/changeGraphicInitialSettings', {
            keyGraphicState: this.keyGraphicState,
            axisType: 'xAxis',
            settings: { min: chart.chartData.labels[0], max: chart.chartData.labels[chart.chartData.labels.length - 1], maxTicksLimit: 11 },
          });
          this.$store.dispatch('wafermeas/changeGraphicInitialSettings', {
            keyGraphicState: this.keyGraphicState,
            axisType: 'yAxis',
            settings: { min: 'Авто', max: 'Авто', maxTicksLimit: 11 },
          });
          this.$store.dispatch('wafermeas/updateChartsData', { keyGraphicState: this.keyGraphicState, data: chart.chartData });
          // let ds = []
          // chart.chartData.datasets.forEach((dataset,index) => {
          //   let downsampled = this.downsample(dataset.data.map(function (d,index) { return {x: d, y: chart.chartData.labels[index]}}), 201)
          //   dataset.data = downsampled.map(x => x.x)
          //   ds.push({x: dataset.data, y: downsampled.map(x => x.y)})
          //   if(index === 0)
          //     chart.chartData.labels = downsampled.map(x => x.y)
          // });
          this.loaded = true;
        });
    },

    resetChart() {
      this.$refs.linechart.resetZoom();
      this.$refs.linechart.resetHighligted();
    },

    changeRowViewMode(rowViewMode) {
      this.rowViewMode = rowViewMode === 'miniChart' ? 'bigChart' : 'miniChart';
      this.$store.dispatch('wafermeas/changeKeyGraphicStateRowViewMode', { keyGraphicState: this.keyGraphicState, rowViewMode: this.rowViewMode });
    },

    downsample(data, threshold) {
      const { floor } = Math;
      const { abs } = Math;
      const dataLength = data.length;
      if (threshold >= dataLength || threshold <= 0) {
        return data; // nothing to do
      }
      const sampled = [];
      let sampledIndex = 0;

      // bucket size, leave room for start and end data points
      const every = (dataLength - 2) / (threshold - 2);

      let a = 0; // initially a is the first point in the triangle
      let maxAreaPoint;
      let maxArea;
      let area;
      let nextA;

      // always add the first point
      sampled[sampledIndex++] = data[a];

      for (let i = 0; i < threshold - 2; i++) {
        // Calculate point average for next bucket (containing c)
        let avgX = 0;
        let avgY = 0;
        let avgRangeStart = floor((i + 1) * every) + 1;
        let avgRangeEnd = floor((i + 2) * every) + 1;
        avgRangeEnd = avgRangeEnd < dataLength ? avgRangeEnd : dataLength;

        const avgRangeLength = avgRangeEnd - avgRangeStart;
        for (; avgRangeStart < avgRangeEnd; avgRangeStart++) {
          avgX += data[avgRangeStart].x * 1; // * 1 enforces Number (value may be Date)
          avgY += data[avgRangeStart].y * 1;
        }
        avgX /= avgRangeLength;
        avgY /= avgRangeLength;

        // Get the range for this bucket
        let rangeOffs = floor((i + 0) * every) + 1;
        const rangeTo = floor((i + 1) * every) + 1;

        // Point a
        const pointAX = data[a].x * 1; // enforce Number (value may be Date)
        const pointAY = data[a].y * 1;

        maxArea = area = -1;

        for (; rangeOffs < rangeTo; rangeOffs++) {
          // Calculate triangle area over three buckets
          area = abs((pointAX - avgX) * (data[rangeOffs].y - pointAY)
                        - (pointAX - data[rangeOffs].x) * (avgY - pointAY)) * 0.5;
          if (area > maxArea) {
            maxArea = area;
            maxAreaPoint = data[rangeOffs];
            nextA = rangeOffs; // Next a is this b
          }
        }

        sampled[sampledIndex++] = maxAreaPoint; // Pick this point from the bucket
        a = nextA; // This a is the next a (chosen b)
      }

      sampled[sampledIndex] = data[dataLength - 1]; // Always add last

      return sampled;
    },

    settingsChanged(e) {
      this.settings = _.cloneDeep(this.$store.getters['wafermeas/getGraphicSettingsKeyGraphicState'](this.keyGraphicState));
    },

    showSettingsContainer(showSettings) {
      this.showSettings = !showSettings;
    },

    changeLogMode(log) {
      this.$store.dispatch('wafermeas/changeKeyGraphicStateLog', { keyGraphicState: this.keyGraphicState, log });
    },

    calculateOptions(chartOptions, xAxisSettings, yAxisSettings) {
      const log = this.log === true ? 'logarithmic' : 'linear';
      this.options = {
        animation: chartOptions.animation,
        hover: chartOptions.hover,
        tooltips: chartOptions.tooltips,
        legend: {
          display: chartOptions.legend.display,
        },
        responsive: true,
        responsiveAnimationDuration: chartOptions.responsiveAnimationDuration,
        maintainAspectRatio: chartOptions.maintainAspectRatio,
        plugins: {
          deferred: {
            xOffset: 50,
            yOffset: '10%',
            delay: 100,
          },
          zoom: {
            zoom: {
              enabled: true,
              drag: true,
              mode: 'xy',
              rangeMin: {
                x: null,
                y: null,
              },
              rangeMax: {
                x: null,
                y: null,
              },
              speed: 0.1,
              threshold: 2,
              sensitivity: 3,
            },
          },
        },
        scales: {
          xAxes: [{
            scaleLabel: {
              display: chartOptions.xAxis.display,
              labelString: chartOptions.xAxis.label,
              fontColor: '#BDBDBD',
            },
            gridLines: {
              display: true,
              color: '#303030',
            },
            ticks: {
              fontColor: '#BDBDBD',
              min: xAxisSettings.min,
              max: xAxisSettings.max,
              maxTicksLimit: xAxisSettings.maxTicksLimit,

            },
          }],
          yAxes: [{
            type: log,
            scaleLabel: {
              display: chartOptions.yAxis.display,
              labelString: chartOptions.yAxis.label,
              fontColor: '#BDBDBD',
            },
            gridLines: {
              display: true,
              color: '#303030',
            },
            ticks: {
              fontColor: '#BDBDBD',
              maxTicksLimit: yAxisSettings.maxTicksLimit,
            },
          }],
        },
      };
    },
  },

};
</script>
<style scoped>
  .bigChart {
    position: relative;
    margin: auto;
    height: 70vh;
    width: 55vw;
  }
  .miniChart {
    position: relative;
    margin: auto;
    height: 400px;
    width: 400px;
  }
</style>
