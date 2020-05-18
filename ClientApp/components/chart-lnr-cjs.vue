<template>
<v-card>
  <v-container> 
    <line-chart
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
import LineChart from './linechart-cjs.vue'

export default {
  
  props: ["keyGraphicState", "measurementId", "divider"],
  components: { LineChart },
  data: () => ({
    loaded: false,
    chartdata: null,
    options: null
   
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
        async getChartData(selectedDies) {
                this.loaded = false     
                let singlestatModel = {};
                singlestatModel.divider = this.divider;
                singlestatModel.keyGraphicState = this.keyGraphicState;
                singlestatModel.measurementId = this.measurementId;
                singlestatModel.dieIdList = selectedDies;
                await this.$http
                    .get(`api/chartjs/GetLinearForMeasurement?statisticSingleGraphicViewModelJSON=${JSON.stringify(singlestatModel)}`)
                    .then(response => {
                    let chart = response.data;
                    this.chartdata = chart.chartData;
                    this.options = chart.options;                                                   
                    this.loaded = true       
                  
                })
                .catch(error => {});
       
         }
    }

    
}
</script>
