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
           <v-select
                                    :items="['STAT', 'FXD']"
                                    v-model="mode"
                                    no-data-text="Нет данных"
                                    label="Режим отсеивания"
                                ></v-select>
        </v-row>
        <v-row>
            <v-subheader>Коэффициент отсеивания:</v-subheader>
            <v-slider
                v-model="statisticKf"
                :tick-labels="['0.5', '0.75', '1', '1.25', '1.5', '1.75', '2']"
                :min="0.5"
                :max="2"
                step="0.25"
                ticks="always"
                tick-size="4">
            </v-slider>
            <v-btn block @click="setStatisticKf">Применить</v-btn>
        </v-row>
        <v-row>
           <v-text-field v-model="lowBorder" :readonly="mode=='STAT'" outlined label="lowBorder"></v-text-field>
        </v-row>
        <v-row>
            <v-text-field v-model="topBorder" :readonly="mode=='STAT'" outlined label="topBorder"></v-text-field>
        </v-row>
        <v-row>
          <v-checkbox v-model="autoRefresh" label="Автоматическое отсеивание"></v-checkbox>
        </v-row>
    </v-container>
</template>

<script>
import { mapGetters } from 'vuex';

export default {
  props: ['measurementId', 'keyGraphicState', 'statParameter'],
  data() {
    return {
      statisticKf: '',
      lowBorder: '',
      topBorder: '',
      mode: 'STAT',
      autoRefresh: true,
    };
  },

  mounted() {
    this.statisticKf = this.currentDcProfile.k;
    this.mode = this.currentDcProfile.type;
    this.topBorder = this.currentDcProfile.topBorder;
    this.lowBorder = this.currentDcProfile.lowBorder;
  },

  computed: {

    ...mapGetters({
      selectedDies: 'wafermeas/selectedDies',
    }),

    dcProfiles() {
      return this.$store.getters['wafermeas/getDcProfilesByKeyGraphicState'](this.keyGraphicState);
    },
    currentDcProfile() {
      return this.dcProfiles.find((dc) => dc.statName === this.statParameter.statisticsName);
    },
    dirtyCellsSnapshotBadDies() {
      return this.$store.getters['wafermeas/dirtyCellsSnapshotBadDies'];
    },
  },

  watch: {
    currentDcProfile(currentDcProfile) {
      this.statisticKf = currentDcProfile.k;
      this.mode = currentDcProfile.type;
      this.topBorder = currentDcProfile.topBorder;
      this.lowBorder = currentDcProfile.lowBorder;
    },
  },

  methods: {
    async setStatisticKf() {
      const dcProfiles = this.dcProfiles.map((dc) => (dc.statName === this.statParameter.statisticsName ? { ...dc, k: this.statisticKf } : dc));
      const path = `/api/statrwrk/dirtyCellsSnapshot/${this.measurementId}/${this.keyGraphicState}/withprofile?dcProfilesJSON`;
      const s = (await this.$http.get(`${path}=${JSON.stringify(dcProfiles)}`)).data;
      this.$store.dispatch('wafermeas/updateDirtyCellsSnapshot', { keyGraphicState: this.keyGraphicState, snapshotChunk: s.singleGraphicDirtyCells });
      this.$store.dispatch('wafermeas/updateDcProfiles', { keyGraphicState: this.keyGraphicState, keyGraphicStateDcProfile: s.dcProfiles });
      if (this.autoRefresh) this.refreshSelectedDies();
    },

    refreshSelectedDies() {
      const avbSelectedDies = this.$store.getters['wafermeas/avbSelectedDies'];
      const selectedDies = avbSelectedDies.filter((d) => !this.dirtyCellsSnapshotBadDies.includes(d));
      this.$store.dispatch('wafermeas/updateSelectedDies', selectedDies);
    },
  },
};
</script>

<style scoped>
  .ps {
    height: 475px;
  }

  .border {
    font-size: x-small;
  }
</style>
