<template>
  <div class="d-flex">
    <div class="flex-column">
      <SingleCharacteristicStatistic v-if="loaded.fullPeriod" :startPeriod="startPeriod" :chartData="chartData.fullPeriod" :characteristic="characteristic" :measurementId="measurementId"></SingleCharacteristicStatistic>
      <div class="d-flex flex-row mt-2">
        <div class="flex-row">
          <SingleCharacteristicChart
            :characteristic="characteristic"
            :chartData="chartData.fullPeriod"
            :axisX="axisX.fullPeriod"
            :loaded="loaded.fullPeriod"
          ></SingleCharacteristicChart>
        </div>
        <div class="flex-row mx-2">
          <SingleCharacteristicStartPeriodChart
            :characteristic="characteristic"
            :chartData="chartData.startPeriod"
            :axisX="axisX.startPeriod"
            :loaded="loaded.startPeriod"
          ></SingleCharacteristicStartPeriodChart>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import moment from 'moment'
import 'moment-duration-format'
import SingleCharacteristicChart from '../SingleMeasurement/SingleCharacteristicChart'
import SingleCharacteristicStartPeriodChart from '../SingleMeasurement/SingleCharacteristicStartPeriodChart'
import SingleCharacteristicStatistic from '../SingleMeasurement/SingleCharacteristicStatistic'
export default {
  props: ['measurementId', 'characteristic'],
  components: {
    SingleCharacteristicChart,
    SingleCharacteristicStatistic,
    SingleCharacteristicStartPeriodChart
  },
  data () {
    return {
      siftedK: 200,
      startPeriod: 0,
      withoutBadPoints: true,
      loaded: {
        fullPeriod: false,
        startPeriod: false
      },
      axisX: {
        fullPeriod: {
          strictMinMax: false,
          min: 0
        },
        startPeriod: {
          strictMinMax: true,
          min: 0
        }
      },
      chartData: {
        startPeriod: {},
        fullPeriod: {}
      }
    }
  },

  async created () {
    let data = (
      await this.$http.get(
        `/api/vertx/point/measurementId/${this.measurementId}/characteristicName/${this.characteristic.name}/sifted/${this.siftedK}/withoutbadpoints/${this.withoutBadPoints}`
      )
    ).data
    this.loaded.fullPeriod = true
    const startPeriod = moment.duration(data[this.measurementId].points[0].fromStartDate).asSeconds()
    this.startPeriod = moment.duration(data[this.measurementId].points[0].fromStartDate).asMinutes()
    this.chartData.fullPeriod = { ...data }
    data = (
      await this.$http.get(
        `/api/vertx/point/measurementId/${this.measurementId}/characteristicName/${this.characteristic.name}/seconds/${startPeriod}`
      )
    ).data
    this.chartData.startPeriod = { ...data }
    this.loaded.startPeriod = true
  }
}
</script>
