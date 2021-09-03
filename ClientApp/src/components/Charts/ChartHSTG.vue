<template>
<v-card>
    <v-container>
        <bar-chart
          v-if="loaded"
          :keyGraphicState="keyGraphicState"
          :divider="divider"
          :options="options"/>
        <v-progress-circular v-else
          :size="50"
          color="primary"
          indeterminate>
        </v-progress-circular>
    </v-container>
  </v-card>
</template>

<script>
import BarChart from './BarChart.vue';

export default {
  props: ['keyGraphicState', 'measurementId', 'divider'],
  components: { BarChart },
  data: () => ({
    loaded: false,
    chartdata: {},
    options: {},
  }),

  async mounted() {
    await this.getChartData();
  },

  computed: {

  },

  watch: {
  },

  methods: {
    async getChartData() {
      this.loaded = false;
      const avbSelectedDies = this.$store.getters['wafermeas/avbSelectedDies'];
      const singlestatModel = {};
      singlestatModel.keyGraphicState = this.keyGraphicState;
      singlestatModel.measurementId = this.measurementId;
      singlestatModel.dieIdList = avbSelectedDies;
      singlestatModel.divider = this.divider;
      await this.$http
        .get(`/api/chartjs/GetHistogramForMeasurement?statisticSingleGraphicViewModelJSON=${JSON.stringify(singlestatModel)}`)
        .then((response) => {
          const chart = response.data;
          this.chartdata = chart.chartData;
          this.$store.dispatch('wafermeas/updateChartsData', { keyGraphicState: this.keyGraphicState, data: chart.chartData });
          this.calculateOptions(chart.options);
          this.loaded = true;
        });
    },

    calculateOptions(chartOptions) {
      this.options = {
        animation: chartOptions.animation,
        hover: chartOptions.hover,
        legend: {
          display: chartOptions.legend.display,
        },
        responsive: chartOptions.responsive,
        responsiveAnimationDuration: chartOptions.responsiveAnimationDuration,
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
            },
          }],
          yAxes: [{
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
    height: 80vh;
    width: 55vw;
  }
  .miniChart {
    position: relative;
    margin: auto;
    height: 400px;
    width: 400px;
  }
</style>
