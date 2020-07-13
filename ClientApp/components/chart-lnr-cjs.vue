<template>
<div class="d-flex" v-if="loaded">

<v-col class="d-flex">
  <v-card>
    <v-container>
          <line-chart
            :id="'LCHART_' + keyGraphicState"
            :keyGraphicState="keyGraphicState"
            :chartdata="chartdata"
            :options="options"/>
    </v-container>
  </v-card>
</v-col>
<v-col class="d-flex flex-column justify-start">
            <v-btn :color="log === false ? 'indigo' : 'grey darken-2'" fab x-small dark @click="changeLogMode(false)">
              Лин
            </v-btn>
            <v-btn :color="log === true ? 'indigo' : 'grey darken-2'" fab x-small dark @click="changeLogMode(true)">
              Лог
            </v-btn>
</v-col>
</div>
<v-progress-circular v-else
          :size="50"
          color="primary"
          indeterminate>
</v-progress-circular>
</template>

<script>  
import LineChart from './linechart-cjs.vue'

export default {
  
  props: ["keyGraphicState", "measurementId", "divider"],
  components: { LineChart },
  data: () => ({
    loaded: false,
    chartdata: {},
    options: {}
  }),

  async mounted() {
    await this.getChartData(this.selectedDies);
  },

   computed: {
      selectedDies() {
        return this.$store.getters['wafermeas/selectedDies']
      },
      log() {
        return this.$store.getters['wafermeas/getKeyGraphicStateLog'](this.keyGraphicState)
      },

    },

    watch: {
      divider: async function() {
        await this.getChartData(this.selectedDies);
      }
    },

    methods: {

      async getChartData(selectedDies) {
        this.loaded = false     
        let singlestatModel = {};
        singlestatModel.divider = this.divider;
        singlestatModel.keyGraphicState = this.keyGraphicState;
        singlestatModel.measurementId = this.measurementId;
        singlestatModel.dieIdList = selectedDies;
        await this.$http
              .get(`/api/chartjs/GetLinearForMeasurement?statisticSingleGraphicViewModelJSON=${JSON.stringify(singlestatModel)}`)
              .then(response => {
                let chart = response.data
                this.calculateOptions(chart.options)
                this.chartdata = chart.chartData
                this.loaded = true       
        })
        .catch(error => {});
      },

      changeLogMode(log) {
        this.$store.dispatch("wafermeas/changeKeyGraphicStateLog", {keyGraphicState: this.keyGraphicState, log: log});
      },

        calculateOptions(chartOptions) {
          this.options = {
            animation: chartOptions.animation,
            hover: chartOptions.hover,
            tooltips: chartOptions.tooltips,
            legend: {
              display: chartOptions.legend.display
            },
            responsive: chartOptions.responsive,
            responsiveAnimationDuration: chartOptions.responsiveAnimationDuration,
            scales: {
              xAxes: [{
                scaleLabel: {
                  display: chartOptions.xAxis.display,
                  labelString: chartOptions.xAxis.label,
                  fontColor: '#BDBDBD'
                },
                gridLines: {
                  display: true,
                  color: '#303030'
                },
                ticks: {
                  fontColor: '#BDBDBD'
                }
              }],
              yAxes: [{
                type: 'linear',
                scaleLabel: {
                  display: chartOptions.yAxis.display,
                  labelString: chartOptions.yAxis.label,
                  fontColor: '#BDBDBD'
                },
                gridLines: {
                  display: true,
                  color: '#303030'
                },
                ticks: {
                  fontColor: '#BDBDBD'
                }
              }]
            }
          }   
        }
    }

    
}
</script>
