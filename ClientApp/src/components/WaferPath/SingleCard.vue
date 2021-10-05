<template>
  <div>
    <v-skeleton-loader v-if="_.isEmpty(dirtyCellsInfo)" class="mx-auto" type="card-avatar">
    </v-skeleton-loader>
    <v-card v-else color="#303030" class="mx-auto">
      <v-container>
        <v-row>
          <v-col lg="12">
            <v-btn
              block
              color="primary"
              @click="
                $router.push({
                  name: 'wafermeasurement-fullselected',
                  params: { waferId: waferId, measurementName: digit + '_' + elementName }
                })
              "
              outlined
              >{{ elementName }}</v-btn
            >
          </v-col>
        </v-row>
        <v-row>
          <v-col lg="6">
            <v-card color="#303030" class="ma-1">
              <v-progress-circular
                :rotate="360"
                :size="50"
                :width="2"
                :value="dirtyCellsInfo.goodCellsPercentage"
                :color="cardColor"
              >
                {{ dirtyCellsInfo.goodCellsPercentage + "%" }}
              </v-progress-circular>
            </v-card>
          </v-col>
          <v-col lg="6">
            <v-card color="#303030" class="ma-1">
              {{ dirtyCellsInfo.diesCount - dirtyCellsInfo.dirtyCellsArray.length }}/{{
                dirtyCellsInfo.diesCount
              }}
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </v-card>
  </div>
</template>

<script>
export default {
  props: ['waferId', 'digit', 'elementName', 'dirtyCellsInfo'],
  data() {
    return {};
  },

  async mounted() {
    // this.dirtyCellsInfo = (await this.$http.get(`/api/statistic/GetDirtyCellsByMeasurementRecordingOnly?measurementRecordingId=${this.measurementRecordingId}`)).data
  },

  computed: {
    cardColor() {
      return this.$store.getters['wafermeas/calculateColor'](
        this.dirtyCellsInfo.goodCellsPercentage / 100,
      );
    },
  },
};
</script>
