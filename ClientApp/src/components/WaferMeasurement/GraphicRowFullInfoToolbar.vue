<template>
  <v-row :class="responsiveSettings.rowClass">
        <v-col :lg="responsiveSettings.vToolbarFlexSize">
          <v-toolbar extended>
              <v-container>
                <v-row>
                  <v-col lg="6" class="d-flex align-center justify-start">
                    <v-chip class="elevation-8" label x-large color="#303030">
                      {{graphicName}}
                    </v-chip>
                  </v-col>
                  <v-col lg="6" class="d-flex align-center justify-space-around">
                  <div class="d-flex flex-column" >
                    <v-chip class="elevation-8" color="#303030">
                      Годны по всей пластине
                    </v-chip>
                    <div class="d-flex flex-row mt-4">
                      <v-progress-circular
                        :rotate="360"
                        :size="60"
                        :width="2"
                        :value="dirtyCellsSnapshot.goodDiesPercentage"
                        :color="this.$store.getters['wafermeas/calculateColor'](dirtyCellsSnapshot.goodDiesPercentage / 100)">
                      {{dirtyCellsSnapshot.goodDiesPercentage}}%
                      </v-progress-circular>
                    </div>
                  </div>
                <div class="d-flex flex-column align-self-center">
                  <v-chip class="elevation-8" color="#303030">
                    Годны из выбранных
                  </v-chip>
                  <div class="d-flex flex-row mt-4">
                    <v-progress-circular
                      :rotate="360"
                      :size="60"
                      :width="2"
                      :value="dirtyCellsPercentage"
                      color='primary'>
                      {{ dirtyCellsPercentage }}%
                    </v-progress-circular>
                    <v-btn text icon color='primary' @click="delDirtyCells(dirtyCellsSnapshot.badDies)">
                      <v-icon>cached</v-icon>
                    </v-btn>
                </div>
                </div>
                <!-- <v-switch color="primary" v-model="switchMode" :label="mode"></v-switch> -->
                </v-col>
                </v-row>
              </v-container>
          </v-toolbar>
        </v-col>
        <v-col :lg="responsiveSettings.waferMiniFlexSize">
          <wafer-mini v-if="(dirtyCellsPercentage >= 0)"
            :keyGraphicState="keyGraphicState"
            :rowViewMode="rowViewMode"
            :key="`wfm-${keyGraphicState}`">
          </wafer-mini>
        </v-col>
    </v-row>
</template>

<script>
import { mapGetters } from 'vuex';
import WaferMapMini from './WaferMapMini.vue';

export default {
  props: ['loading', 'graphicName', 'rowViewMode', 'keyGraphicState'],
  data() {
    return {

    };
  },
  components: {
    'wafer-mini': WaferMapMini,
  },

  computed: {
    ...mapGetters({
      selectedDies: 'wafermeas/selectedDies',
      avbSelectedDies: 'wafermeas/avbSelectedDies',
    }),

    dirtyCellsPercentage() {
      return Math.ceil((1.0 - (this.dirtyCellsSnapshot.badDies.length - ([...new Set([...this.selectedDies, ...this.dirtyCellsSnapshot.badDies])].length - this.selectedDies.length)) / this.selectedDies.length) * 100);
    },

    dirtyCellsSnapshot() {
      return this.$store.getters['wafermeas/getDirtyCellsSnapshotByKeyGraphicState'](this.keyGraphicState);
    },

    responsiveSettings() {
      if (this.rowViewMode === 'bigChart') {
        return {
          rowClass: 'd-flex flex-column',
          vToolbarFlexSize: 12,
          waferMiniFlexSize: 12,
        };
      }

      return {
        rowClass: '',
        vToolbarFlexSize: 8,
        waferMiniFlexSize: 4,
      };
    },
  },
};
</script>

<style>

</style>
