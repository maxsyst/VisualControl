<template>
  <v-row>
      <v-col :lg="rowViewOptions.statSingleFlexSize" class="d-flex">
        <stat-single :id="'ss_' + keyGraphicState"
          :measurementId="selectedMeasurementId"
          :keyGraphicState="keyGraphicState"
          :divider="selectedDivider"
          :rowViewMode="rowViewMode"
          :viewMode="viewMode"
          :statisticKf="statisticKf"
        ></stat-single>
        <v-divider light></v-divider>
      </v-col>
      <v-col v-if="selectedDiesLength < 200" :lg="rowViewOptions.chartFlexSize" class="d-flex align-self-center">
        <chart-lnr
          v-if="keyGraphicState.includes(`LNR`)"
          :measurementId="selectedMeasurementId"
          :keyGraphicState="keyGraphicState"
          :rowViewMode="rowViewMode"
          :viewMode="viewMode"
          :divider="selectedDivider"
        ></chart-lnr>
        <chart-hstg
          v-else
          :measurementId="selectedMeasurementId"
          :keyGraphicState="keyGraphicState"
          :rowViewMode="rowViewMode"
          :viewMode="viewMode"
          :divider="selectedDivider"
        ></chart-hstg>
        <v-divider light></v-divider>
      </v-col>
      <v-col v-else :lg="rowViewOptions.chartFlexSize" class="d-flex align-self-center">
        <v-card>
          <v-card-text>
            <div>Графиков для отображения: {{selectedDiesLength}}</div>
            <p>
              Для повышения производительности графики не будут отображены. Выберите менее 200 графиков.
            </p>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
</template>

<script>
import ChartLNR from '../Charts/ChartLNR.vue';
import ChartHSTG from '../Charts/ChartHSTG.vue';
import StatSingle from './StatSingle.vue';

export default {
  props: ['selectedMeasurementId', 'keyGraphicState', 'viewMode', 'selectedDivider', 'statisticKf', 'selectedDiesLength'],
  data() {
    return {

    };
  },
  components: {
    'stat-single': StatSingle,
    'chart-lnr': ChartLNR,
    'chart-hstg': ChartHSTG,
  },
  computed: {

    rowViewMode() {
      return this.$store.getters['wafermeas/getKeyGraphicStateRowViewMode'](this.keyGraphicState);
    },

    rowViewOptions() {
      if (this.rowViewMode === 'miniChart') {
        return {
          statSingleFlexSize: 8,
          chartFlexSize: 4,
        };
      }
      return {
        statSingleFlexSize: 4,
        chartFlexSize: 8,
      };
    },
  },
};
</script>

<style>

</style>
