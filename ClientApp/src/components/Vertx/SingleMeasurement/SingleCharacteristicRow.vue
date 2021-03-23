<template>
  <v-container>
   <v-expansion-panels>
    <v-expansion-panel @change="expandPanel">
      <v-expansion-panel-header>
         <SingleCharacteristicStatistic v-if="loaded.fullPeriod" :startPeriod="startPeriod" :chartData="chartData.fullPeriod" :characteristic="characteristic" :measurementId="measurementId"></SingleCharacteristicStatistic>
      </v-expansion-panel-header>
      <v-expansion-panel-content>
        <v-card class="mt-2 elevation-8">
         <SingleCharacteristicChart
            :characteristic="characteristic"
            :siftedK="siftedK"
            :withoutBadPoints="withoutBadPoints"
            :chartData="chartData.fullPeriod"
            :axisX="axisX.fullPeriod"
            :loaded="loaded.fullPeriod"
            @withoutbadpoint-change="withoutBadPointsChange"
            @siftedK-change="siftedKChange"
          ></SingleCharacteristicChart>
          </v-card>
          <v-card class="mt-2 elevation-8">
          <SingleCharacteristicStartPeriodChart
            :characteristic="characteristic"
            :chartData="chartData.startPeriod"
            :axisX="axisX.startPeriod"
            :loaded="loaded.startPeriod"
          ></SingleCharacteristicStartPeriodChart>
          </v-card>
      </v-expansion-panel-content>
    </v-expansion-panel>
  </v-expansion-panels>
  </v-container>
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
      withoutBadPoints: true,
      startPeriod: 0,
      loaded: {
        fullPeriod: false,
        startPeriod: false
      },
      axisX: {
        fullPeriod: {
          strictMinMax: true,
          min: 0
        },
        startPeriod: {
          min: 0
        }
      },
      chartData: {
        startPeriod: {},
        fullPeriod: {}
      }
    }
  },

  methods: {
    expandPanel: async function () {
      this.loaded.fullPeriod = false
      const temp = {...this.chartData.fullPeriod }
      this.chartData.fullPeriod = {}
      this.chartData.fullPeriod = temp
      this.loaded.fullPeriod = true
    },

    withoutBadPointsChange: async function (withoutBadPoints) {
      this.withoutBadPoints = withoutBadPoints
      this.loaded.fullPeriod = false
      const data = (
        await this.$http.get(
          `/api/vertx/point/measurementId/${this.measurementId}/characteristicName/${this.characteristic.name}/sifted/${this.siftedK}/withoutbadpoints/${this.withoutBadPoints}`
        )
      ).data
      this.chartData.fullPeriod = { ...data }
      this.loaded.fullPeriod = true
    },

    siftedKChange: async function (k) {
      this.loaded.fullPeriod = false
      this.siftedK = k
      const data = (
        await this.$http.get(
          `/api/vertx/point/measurementId/${this.measurementId}/characteristicName/${this.characteristic.name}/sifted/${this.siftedK}/withoutbadpoints/${this.withoutBadPoints}`
        )
      ).data
      this.chartData.fullPeriod = { ...data }
      this.loaded.fullPeriod = true
    }
  },

  async created () {
    let data = (
      await this.$http.get(
        `/api/vertx/point/measurementId/${this.measurementId}/characteristicName/${this.characteristic.name}/sifted/${this.siftedK}/withoutbadpoints/${this.withoutBadPoints}`
      )
    ).data
    const startPeriod = moment.duration(data[this.measurementId].points[0].fromStartDate).asSeconds()
    this.startPeriod = moment.duration(data[this.measurementId].points[0].fromStartDate).asMinutes()
    this.chartData.fullPeriod = { ...data }
    this.loaded.fullPeriod = true
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
