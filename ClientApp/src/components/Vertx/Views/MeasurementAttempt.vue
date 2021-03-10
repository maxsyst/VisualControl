<template>
  <div class="d-flex flex-column mt-auto">
    <v-card
      color="#303030"
      class="elevation-8"
    >
      <v-card-text>
        <MeasurementTable
          v-if="loaded"
          :table-data="tableData"
        />
        <v-skeleton-loader
          v-else
          type="table"
        />
      </v-card-text>
    </v-card>
    <v-card
      color="#303030"
      class="elevation-8"
    >
      <v-card-text>
        <CharacteristicTabs
          v-if="characteristicList.length > 0"
          :characteristic-data="characteristicList"
          :source-id="measurementAttemptId"
          :is-single-measurement="isSingleMeasurement"
        />
      </v-card-text>
    </v-card>
  </div>
</template>

<script>

import CharacteristicTabs from '@/components/MeasurementAttempt/CharacteristicTabs';
import MeasurementTable from '@/components/MeasurementAttempt/MeasurementTable';

export default {
  components: {
    MeasurementTable,
    CharacteristicTabs,
  },
  data() {
    return {
      loaded: false,
      measurementAttemptId: '',
      isSingleMeasurement: false,
      tableData: [],
      characteristicList: [],
    };
  },

  async mounted() {
    this.measurementAttemptId = this.$route.params.measurementAttemptId;
    this.tableData = (await this.$axios.get(`/api/v1/measurement/measurementAttemptId/${this.measurementAttemptId}`)).data;
    this.characteristicList = (await this.$axios.get(`/api/v1/measurement/id/${this.tableData[0].id}/characteristics`)).data;
    this.loaded = true;
  },
};
</script>

<style scoped>

</style>
