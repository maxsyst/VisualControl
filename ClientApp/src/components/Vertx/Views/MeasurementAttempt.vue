<template>
  <v-container fluid>
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
        />
      </v-card-text>
    </v-card>
  </div>
  </v-container>
</template>

<script>

import CharacteristicTabs from '../MeasurementAttempt/CharacteristicTabs';
import MeasurementTable from '../MeasurementAttempt/MeasurementTable';

export default {
  components: {
    MeasurementTable,
    CharacteristicTabs,
  },
  data() {
    return {
      loaded: false,
      measurementAttemptId: '',
      tableData: [],
      characteristicList: [],
    };
  },

  async mounted() {
    this.measurementAttemptId = this.$route.params.measurementAttemptId;
    this.tableData = (await this.$http.get(`/api/vertx/measurement/measurementAttemptId/${this.measurementAttemptId}`)).data;
    this.characteristicList = (await this.$http.get(`/api/vertx/measurement/id/${this.tableData[0].id}/characteristics`)).data;
    this.loaded = true;
  },
};
</script>

<style scoped>

</style>
