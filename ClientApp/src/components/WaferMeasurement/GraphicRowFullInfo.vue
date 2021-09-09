<template>
  <v-row>
      <v-col :lg="rowViewOptions.statSingleFlexSize" class="d-flex">
        <stat-single :id="'ss_' + keyGraphicState"
          :measurementId="selectedMeasurementId"
          :keyGraphicState="keyGraphicState"
          :divider="selectedDivider"
          :rowViewMode="rowViewMode"
        ></stat-single>
        <v-divider light></v-divider>
      </v-col>
      <v-col v-if="selectedDiesLength > 0" :lg="rowViewOptions.chartFlexSize" class="d-flex align-self-center">
        <chart-lnr
          v-if="keyGraphicState.includes(`LNR`)"
          :measurementId="selectedMeasurementId"
          :keyGraphicState="keyGraphicState"
          :rowViewMode="rowViewMode"
          :divider="selectedDivider"
        ></chart-lnr>
        <chart-hstg
          v-else
          :measurementId="selectedMeasurementId"
          :keyGraphicState="keyGraphicState"
          :rowViewMode="rowViewMode"
          :divider="selectedDivider"
        ></chart-hstg>
          <v-divider light></v-divider>
        </v-col>
        <v-col v-else :lg="rowViewOptions.chartFlexSize" class="d-flex align-self-center">
        <v-card>
          <v-card-text>
            <p>
              Не выбраны кристаллы для отображения.
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
  props: ['selectedMeasurementId', 'keyGraphicState', 'selectedDivider', 'selectedDiesLength'],
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
