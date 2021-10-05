<template>
    <v-container>
        <bar-chart
          :chartdata="histogramData"
          :options="options"/>
    </v-container>
</template>
<script>
import { mapGetters } from 'vuex';
import BarChart from './barchart-hstg.vue';

export default {
  props: ['gradientSteps'],
  components: { BarChart },
  data() {
    return {
      loading: false,
      histogramData: {},
      options: {
        legend: { display: false },
        xAxis: { label: 'Название шага', display: true },
        yAxis: { label: 'Количество', display: true },
        responsive: false,
        showLines: false,
        responsiveAnimationDuration: 0,
        animation: { duration: 0 },
        hover: { animationDuration: 0 },
      },
    };
  },

  methods: {
    initHistogramData(gradientSteps, selectedDies) {
      const selectedDiesSet = new Set([...selectedDies]);
      const histogramData = {
        labels: gradientSteps.map((x) => x.name),
        datasets: [
          {
            backgroundColor: gradientSteps.map((x) => x.color),
            data: gradientSteps.map((x) => x.dieList.filter((d) => selectedDiesSet.has(d)).length),
            fill: false,
            borderWidth: 1,
            pointHoverRadius: 0,
            pointRadius: 0,
          },
        ],
      };
      this.histogramData = histogramData;
    },
  },

  watch: {
    gradientSteps(gradientSteps) {
      this.initHistogramData(gradientSteps, this.selectedDies);
    },
    selectedDies(selectedDies) {
      this.initHistogramData(this.gradientSteps, selectedDies);
    },
  },

  computed:
    {
      ...mapGetters({
        selectedDies: 'wafermeas/selectedDies',
      }),
    },

  async mounted() {
    this.initHistogramData(this.gradientSteps, this.selectedDies);
  },
};
</script>
