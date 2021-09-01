<template>
    <v-container>
        <v-row>
             <v-simple-table>
                    <template v-slot:default>
                    <thead>
                        <tr>
                            <th class="text-center">Название</th>
                            <th class="text-center">μ</th>
                            <th class="text-center">σ</th>
                            <th class="text-center">Минимум</th>
                            <th class="text-center">Максимум</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td><v-chip color="indigo"
                                        label
                                        v-html="statParameter.unit.trim()
                                            ? statParameter.shortStatisticsName + ', ' + statParameter.unit : statParameter.shortStatisticsName"
                                        dark>
                                </v-chip>
                            </td>
                            <td>{{ statParameter.expectedValue }}</td>
                            <td>{{ statParameter.standartDeviation }}</td>
                            <td>{{ statParameter.minimum }}</td>
                            <td>{{ statParameter.maximum }}</td>
                        </tr>
                    </tbody>
                    </template>
                </v-simple-table>
        </v-row>
        <v-row>
            <v-subheader>Коэффициент отсеивания:</v-subheader>
            <v-slider
                v-model="statisticKf"
                :tick-labels="['0.5', '1', '1.5', '2']"
                :min="0.5"
                :max="2"
                step="0.5"
                ticks="always"
                tick-size="4">
            </v-slider>
            <v-btn block @click="setStatisticKf">Применить</v-btn>
        </v-row>
        <v-row>
            <v-chip color="indigo"
                                        label

                                        dark>
                                         <span>LowBorder</span>
            </v-chip>
            <v-chip color="success"
                                        label

                                        dark>
                                         <span>{{dcProfiles.find((dc) => (dc.statName === statParameter.statisticsName)).lowBorder }}</span>
            </v-chip>
        </v-row>
        <v-row>
            <v-chip color="indigo"
                                        label

                                        dark>
                                         <span>TopBorder</span>
            </v-chip>
            <v-chip color="success"
                                        label

                                        dark>
                                         <span>{{dcProfiles.find((dc) => (dc.statName === statParameter.statisticsName)).topBorder }}</span>
            </v-chip>
        </v-row>
    </v-container>
</template>

<script>

export default {
  props: ['measurementId', 'keyGraphicState', 'statParameter'],
  data() {
    return {
      statisticKf: '1.5',
    };
  },

  computed: {
    dcProfiles() {
      return this.$store.getters['wafermeas/getDcProfilesByKeyGraphicState'](this.keyGraphicState);
    },
  },

  methods: {
    async setStatisticKf() {
      const dcProfiles = this.dcProfiles.map((dc) => (dc.statName === this.statParameter.statisticsName ? { ...dc, k: this.statisticKf } : dc));
      const s = (await this.$http.get(`/api/statrwrk/dirtyCellsSnapshot/${this.measurementId}/${this.keyGraphicState}/withprofile?dcProfilesJSON=${JSON.stringify(dcProfiles)}`)).data;
      this.$store.dispatch('wafermeas/updateDirtyCellsSnapshot', { keyGraphicState: this.keyGraphicState, snapshotChunk: s.singleGraphicDirtyCells });
      this.$store.dispatch('wafermeas/updateDcProfiles', { keyGraphicState: this.keyGraphicState, keyGraphicStateDcProfile: s.dcProfiles });
    },
  },
};
</script>

<style>

</style>
