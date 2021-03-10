<template>
  <v-card>
    <TotalTimeAmChart
      v-if="loaded"
      :data="chartData"
      :settings="chartSettings"
    />
    <v-skeleton-loader
      v-else
      class="mx-auto"
      type="image"
    />
  </v-card>
</template>

<script>
import TotalTimeAmChart from '../../AmCharts/TotalTimeAmChart'
export default {
  name: 'TotalTimeChartTab',
  components: {
    TotalTimeAmChart
  },
  props: ['characteristic', 'sourceId', 'isSingleMeasurement'],
  data () {
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
          power: 8
        },
        axisY:
            {
              strictMinMax: false,
              min: 0,
              max: 0
            },
        colors:
            {
              backgroundColor: '#303030',
              textColor: '#ffffff',
              gridColor: '#ffcc00'
            }
      }
    }
  },
  async created () {
    const data = this.isSingleMeasurement
      ? (await this.$http.get(`/api/vertx/point/measurementId/${this.sourceId}/characteristicName/${this.characteristic.name}/sifted/${this.siftedK}/withoutbadpoints/${this.withoutBadPoints}`)).data
      : (await this.$http.get(`/api/vertx/point/measurementAttemptId/${this.sourceId}/characteristicName/${this.characteristic.name}/sifted/${this.siftedK}/withoutbadpoints/${this.withoutBadPoints}/date`)).data
    this.chartData = { ...data }
    this.chartSettings.characteristic = { ...this.characteristic }
    this.loaded = true
  }
}
</script>

<style scoped>

</style>
