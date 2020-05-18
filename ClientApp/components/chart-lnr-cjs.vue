<template>
<v-card>
  <v-container> 
    <line-chart
      v-if="loaded"
      :chartdata="chartdata"
      :options="options"/>
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

   computed:
    {
        selectedDies()
        {
            return this.$store.state.wafermeas.selectedDies;
        }
    },

    watch:
    {
        selectedDies: async function()
        {
            await this.getChartData();
        },

        divider: async function()
        {
            await this.getChartData();
        }
    },

    methods:
    {
        async getChartData()
         {
               this.loaded = false
     
                var singlestatModel = {};
                singlestatModel.divider = this.divider;
                singlestatModel.keyGraphicState = this.keyGraphicState;
                singlestatModel.measurementId = this.measurementId;
                singlestatModel.dieIdList = this.selectedDies;
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
