<template>
<v-card>
    <v-container> 
        <bar-chart
        v-if="loaded"
        :chartdata="chartdata"
        :options="options"/>
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
                let singlestatModel = {};
                singlestatModel.divider = this.divider;
                singlestatModel.keyGraphicState = this.keyGraphicState;
                singlestatModel.measurementId = this.measurementId;
                singlestatModel.dieIdList = this.selectedDies;
                await this.$http
                    .get(`api/chartjs/GetHistogramForMeasurement?statisticSingleGraphicViewModelJSON=${JSON.stringify(singlestatModel)}`)
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

<style>
   
</style>
