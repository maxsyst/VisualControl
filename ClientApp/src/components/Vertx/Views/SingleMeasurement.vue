<template>
  <v-container>
    <v-row v-for="(characteristic, index) in characteristicList" :key="index">
      <SingleMeasurementRow class="mt-8" :characteristic="characteristic" :measurementId="measurementId"></SingleMeasurementRow>
    </v-row>
  </v-container>
</template>

<script>
import SingleMeasurementRow from '../SingleMeasurement/SingleCharacteristicRow';

export default {
  components: {
    SingleMeasurementRow,
  },
  data() {
    return {
      measurementId: '',
      characteristicList: [],
    };
  },
  async mounted() {
    this.measurementId = this.$route.params.measurementId;
    const characteristicList = (await this.$http.get(`/api/vertx/measurement/id/${this.measurementId}/characteristics`)).data;
    this.characteristicList = [...characteristicList];
  },
};
</script>

<style scoped>

</style>
