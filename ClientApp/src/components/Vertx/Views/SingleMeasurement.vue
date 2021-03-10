<template>
  <v-row>
    <CharacteristicTabs
      :characteristic-data="characteristicList"
      :source-id="measurementId"
      :is-single-measurement="isSingleMeasurement"
    />
  </v-row>
</template>

<script>
import CharacteristicTabs from '../MeasurementAttempt/CharacteristicTabs'

export default {
  name: 'SingleMeasurement',
  components: {
    CharacteristicTabs
  },
  data () {
    return {
      measurementId: '602f98361a39ae85bb647d9a',
      characteristicList: [],
      isSingleMeasurement: true
    }
  },
  async mounted () {
    this.measurementId = this.$route.params.measurementId
    this.characteristicList = (await this.$http.get(`/api/vertx/measurement/id/${this.measurementId}/characteristics`)).data
  }
}
</script>

<style scoped>

</style>
