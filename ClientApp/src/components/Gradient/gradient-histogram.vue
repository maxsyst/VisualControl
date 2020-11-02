<template>   
    <v-container>
        <bar-chart
          :chartdata="histogramData"
          :options="options"/>
    </v-container>
</template>
<script>
    import BarChart from './barchart-hstg.vue'
    export default {
        props: ['gradientSteps'],
        components: { BarChart },
        data() {
            return {
               loading: false,
               histogramData: {},
               options:{
                   legend:{display:false},
                   xAxis:{label:"Название шага",display:true},
                   yAxis:{label:"Количество",display:true},
                   responsive:false,
                   showLines:false,
                   responsiveAnimationDuration:0,
                   animation:{duration:0},
                   hover:{animationDuration:0}}
                }
    },

    methods: {
        initHistogramData(gradientSteps) {
            let histogramData = {
                labels: gradientSteps.map(x => x.name),
                datasets: [
                    {
                        backgroundColor: gradientSteps.map(x => x.color),
                        data: gradientSteps.map(x => x.dieList.length),
                        fill: false,
                        borderWidth: 1,
                        pointHoverRadius: 0,
                        pointRadius: 0
                    }
                ]
            }
            this.histogramData = histogramData
        }
    },

    watch: {
        gradientSteps: function(newVal) {
            this.initHistogramData(newVal)
        }
    },

    computed:
    {
      selectedDies() {
        return this.$store.getters['wafermeas/selectedDies']
      }
    },

    async mounted() {
        this.initHistogramData(this.gradientSteps)
    }
}
</script>