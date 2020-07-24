<template>
  <div class="d-flex flex-column" v-if="loaded">
    <div class="d-flex">
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
        <div class="d-flex">
          <v-col class="d-flex">
            <v-btn :color="showSettings === true ? 'indigo' : 'grey darken-2'" fab x-small dark @click="showSettingsContainer(showSettings)">
              <v-icon>settings</v-icon>
            </v-btn>
          </v-col>
        </div>
      </div>
    </div>
    <div class="d-flex"> 
      <settings v-if="showSettings" :keyGraphicState="keyGraphicState"></settings>
    </div>
  </div>
  <v-progress-circular v-else :size="50" color="primary" indeterminate></v-progress-circular>
</template>

<script>  
import LineChart from './linechart-cjs.vue'
import Settings from './graphicsettings-lnr.vue'
export default {
  
  props: ["keyGraphicState", "measurementId", "divider"],
  components: { LineChart, Settings },
  data: () => ({
    loaded: false,
    showSettings: false,
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
            chart.chartData.labels = [...chart.chartData.labels.map(x => +x)]
            this.calculateOptions(chart.options, 
                                  {min: chart.chartData.labels[0], max: chart.chartData.labels[chart.chartData.labels.length -1], maxTicksLimit: 11}, 
                                  {min: 0, max: 0, maxTicksLimit: 11})
            this.$store.dispatch("wafermeas/changeGraphicInitialSettings", {keyGraphicState: this.keyGraphicState, axisType: "xAxis", settings: {min: chart.chartData.labels[0], max: chart.chartData.labels[chart.chartData.labels.length -1], maxTicksLimit: 11}})
            this.$store.dispatch("wafermeas/changeGraphicInitialSettings", {keyGraphicState: this.keyGraphicState, axisType: "yAxis", settings: {min: "Авто", max: "Авто", maxTicksLimit: 11}})
            this.chartdata = chart.chartData
            this.loaded = true       
        })
        .catch(error => {});
      },

      showSettingsContainer(showSettings) {
        this.showSettings = !showSettings
      },

      changeLogMode(log) {
        this.$store.dispatch("wafermeas/changeKeyGraphicStateLog", {keyGraphicState: this.keyGraphicState, log: log});
      },

        calculateOptions(chartOptions, xAxisSettings, yAxisSettings) {
          let log = this.log === true ? 'logarithmic' : 'linear'
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
                  fontColor: '#BDBDBD',
                  min: xAxisSettings.min,
                  max: xAxisSettings.max,
                  maxTicksLimit: xAxisSettings.maxTicksLimit
                
                }
              }],
              yAxes: [{
                type: log,
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
                  fontColor: '#BDBDBD',
                  maxTicksLimit: yAxisSettings.maxTicksLimit
                }
              }]
            }
          }   
        }
    }

    
}
</script>
