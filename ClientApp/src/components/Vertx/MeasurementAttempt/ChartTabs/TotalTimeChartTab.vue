<template>
  <v-card>
    <TotalTimeAmChart
      v-if="loaded"
      :data="chartData"
      :settings="chartSettings"
    />
    <v-skeleton-loader
      v-else
      class="mx-auto hello"
      type="image"
    />
  </v-card>
</template>

<script>
import TotalTimeAmChart from '../../AmCharts/TotalTimeAmChart';

export default {
  name: 'TotalTimeChartTab',
  components: {
    TotalTimeAmChart,
  },
  props: ['characteristic', 'sourceId'],
  data() {
    return {
      loaded: false,
      withoutBadPoints: true,
      siftedK: 200,
      chartData: {},
      chartSettings: {
        characteristic: {},
        serieName: 'name',
        smoothing: {
          require: false,
          power: 8,
        },
        axisY:
            {
              strictMinMax: false,
              min: 0,
              max: 0,
            },
        colors:
            {
              backgroundColor: '#303030',
              textColor: '#ffffff',
              gridColor: '#ffcc00',
              chartColors: {
                first: '#40e0d0', middle: '#ff8c00', last: '#ff0080',
              },
            },
      },
    };
  },
  async created() {
    const { data } = await this.$http.get(`/api/vertx/point/measurementAttemptId/${this.sourceId}/characteristicName/${this.characteristic.name}/sifted/${this.siftedK}/withoutbadpoints/${this.withoutBadPoints}/date`);
    this.chartData = { ...data };
    this.chartSettings.characteristic = { ...this.characteristic };
    this.loaded = true;
  },
};
</script>

<style scoped>

</style>
