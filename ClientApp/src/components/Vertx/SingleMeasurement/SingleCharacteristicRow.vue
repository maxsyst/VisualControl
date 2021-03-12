<template>
  <div class="d-flex">
    <div class="flex-column">
      <v-chip class="chip" label x-large>
        <span>{{ characteristic.name }}</span>
      </v-chip>
      <div class="flex-row mt-2">
        <SingleCharacteristicChart
          :characteristic="characteristic"
          :chartData="chartData"
          :loaded="loaded"
        ></SingleCharacteristicChart>
        <SingleCharacteristicStatistic v-if="loaded" :chartData="chartData" :measurementId="measurementId"></SingleCharacteristicStatistic>
      </div>
    </div>
  </div>
</template>

<script>
import SingleCharacteristicChart from '../SingleMeasurement/SingleCharacteristicChart'
import SingleCharacteristicStatistic from '../SingleMeasurement/SingleCharacteristicStatistic'
export default {
  props: ['measurementId', 'characteristic'],
  components: {
    SingleCharacteristicChart,
    SingleCharacteristicStatistic
  },
  data () {
    return {
      siftedK: 200,
      withoutBadPoints: true,
      loaded: false,
      chartData: {}
    }
  },

  async created () {
    const data = (
      await this.$http.get(
        `/api/vertx/point/measurementId/${this.measurementId}/characteristicName/${this.characteristic.name}/sifted/${this.siftedK}/withoutbadpoints/${this.withoutBadPoints}`
      )
    ).data
    this.chartData = { ...data }
    this.loaded = true
  }
}
</script>
