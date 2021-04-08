<template>
  <v-card>
    <StartTimeAmChart
      v-if="loaded"
      :data="chartData"
      :settings="chartSettings"
    />
    <v-skeleton-loader
      v-else
      class="mx-auto hello"
      type="image, actions"
    />
  </v-card>
</template>

<script>
import StartTimeAmChart from '../../AmCharts/StartTimeAmChart.vue';

export default {
  name: 'TestTimeChartTab',
  components: {
    StartTimeAmChart,
  },
  props: ['characteristic', 'sourceId'],
  data() {
    return {
      loaded: false,
      withoutBadPoints: true,
      siftedK: 200,
      chartData: {},
      chartSettings: {
        type: 'attemptTestTime',
        characteristic: {},
        minutes: 0,
        serieName: 'name',
        smoothing: {
          require: false,
          power: 8,
        },
        axisX: {
          strictMinMax: true,
          min: 0,
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
    const { data } = await this.$http
      .get(`/api/vertx/point/measurementAttemptId/${this.sourceId}
                            /characteristicName/${this.characteristic.name}
                            /sifted/${this.siftedK}
                            /withoutbadpoints/${this.withoutBadPoints}
                            /duration`);
    this.chartData = { ...data };
    this.chartSettings.characteristic = { ...this.characteristic };
    this.loaded = true;
  },
};
</script>

<style scoped>

</style>
