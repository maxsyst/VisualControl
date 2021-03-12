<template>
  <v-card>
    <v-card-text>
      <StartTimeAmChart
        v-if="loaded"
        :data="chartData"
        :settings="chartSettings"
      />
      <v-skeleton-loader
        v-else
        class="mx-auto"
        type="image, actions"
      />
    </v-card-text>
    <v-card-actions class="d-flex flex-column">
      <v-select
        v-model="chartSettings.serieName"
        :items="['name', 'date']"
        outlined
        label="Отображение измерения на легенде"
      />
      <v-text-field
        v-model="chartSettings.minutes"
        label="Период отображения в минутах"
      />
      <color-table :color-object="chartSettings.colors" ></color-table>
    </v-card-actions>
  </v-card>
</template>

<script>
import StartTimeAmChart from '../../AmCharts/StartTimeAmChart'
import ColorTable from '../../MeasurementAttempt/ChartTabs/Settings/Extra/ColorTable'

export default {
  name: 'StartTimeChartTab',
  components: {
    StartTimeAmChart,
    ColorTable
  },
  props: ['characteristic', 'sourceId'],
  data () {
    return {
      loaded: false,
      seconds: 43200,
      chartData: {},
      chartSettings: {
        characteristic: {},
        minutes: 10,
        serieName: 'name',
        smoothing: {
          require: false,
          power: 8
        },
        axisX:
            {
              strictMinMax: false,
              min: 0,
              max: 0

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
    const data = (await this.$http.get(`/api/vertx/point/measurementAttemptId/${this.sourceId}/characteristicName/${this.characteristic.name}/seconds/${this.seconds}`)).data
    this.chartData = { ...data }
    this.chartSettings.characteristic = { ...this.characteristic }
    this.loaded = true
  }

}

</script>

<style scoped>

</style>
