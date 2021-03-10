<template>
  <div class="d-flex flex-column mt-auto">
    <v-card
      color="#303030"
      class="elevation-8"
    >
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
  </div>
</template>

<script>
import LastUpdates from '@/components/MeasurementAttempt/LastUpdates';

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
    const tableData = (await this.$axios.get('/api/v1/aggregation/lastupdates/all')).data;
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
