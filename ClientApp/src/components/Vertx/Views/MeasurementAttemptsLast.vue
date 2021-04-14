<template>
  <v-container fluid>
    <v-card
      color="#303030"
      class="elevation-8"
    >
        <v-card-title>
          <v-btn @click="$router.push({ name: 'livePoint' })">К измерениям в реальном времени</v-btn>
        </v-card-title>
        <v-card-text>
            <LastUpdates
              v-if="loaded"
              :table-data="tableData"
            />
            <v-skeleton-loader
              v-else
              type="table"
            />
        </v-card-text>
      </v-card>
    </v-container>
</template>

<script>
import LastUpdates from '../MeasurementAttempt/LastUpdates';

export default {
  components: {
    LastUpdates,
  },
  data() {
    return {
      loaded: false,
      tableData: [],
    };
  },
  async mounted() {
    const tableData = (await this.$http.get('/api/vertx/aggregation/lastupdates/all')).data;
    this.tableData = tableData.map((t) => ({
      lastUpdateDate: t.measurement.lastUpdate.date,
      measurementName: t.measurement.notes[t.measurement.notes.length - 1],
      measurementAttemptId: t.measurementAttempt.id,
      waferId: t.mdv.waferId,
      code: t.mdv.code,
    }));
    this.loaded = true;
  },
};
</script>

<style scoped>

</style>
