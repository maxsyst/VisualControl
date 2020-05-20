<template>
<v-card>
    <v-container> 
        <bar-chart
            v-if="loaded"
            :chartdata="chartdata"
            :options="options"/>
        <v-progress-circular v-else
        :size="50"
        color="primary"
        indeterminate
        ></v-progress-circular>
    </v-container>
  </v-card>
</template>

<script>
import BarChart from './barchart-cjs.vue'
export default {
  props: ["keyGraphicState", "measurementId", "divider"],
  components: { BarChart },
  data: () => ({
    loaded: false,
    chartdata: {},
    options: {}
  }),

  async mounted() {
    await this.getChartData(this.selectedDies);
  },

   computed:
    {
        selectedDies() {
            return this.$store.getters['wafermeas/selectedDies']
        }
    },

    watch:
    {
        selectedDies: async function() {
            await this.getChartData(this.selectedDies);
        },

        divider: async function() {
            await this.getChartData(this.selectedDies);
        }
    },

    methods:
    {
         async getChartData() {
                this.loaded = false     
                let singlestatModel = {};
                singlestatModel.divider = this.divider;
                singlestatModel.keyGraphicState = this.keyGraphicState;
                singlestatModel.measurementId = this.measurementId;
                singlestatModel.dieIdList = this.selectedDies;
                await this.$http
                    .get(`api/chartjs/GetHistogramForMeasurement?statisticSingleGraphicViewModelJSON=${JSON.stringify(singlestatModel)}`)
                    .then(response => {
                        let chart = response.data;
                        this.chartdata = chart.chartData
                        this.calculateOptions(chart.options)                                        
                        this.loaded = true       
                  
                })
                .catch(error => {});     
         },

         calculateOptions(chartOptions) {
          this.options = {
            legend: {
              display: chartOptions.legend.display
            },
            scales: {
              xAxes: [{
                scaleLabel: {
                  display: chartOptions.xAxis.display,
                  labelString: chartOptions.xAxis.label
                }
              }],
              yAxes: [{
                scaleLabel: {
                  display: chartOptions.yAxis.display,
                  labelString: chartOptions.yAxis.label
                }
              }]
            }
          }   
        }
    }

    
}
</script>
