<template>
    <v-container>
     <v-row>
      <v-col lg="4">
        <v-row class="mt-6">
           <v-select
                                    :items="['Статистика', 'Границы']"
                                    outlined
                                    v-model="russianModeName"
                                    no-data-text="Нет данных"
                                    label="Режим отсеивания"
                                ></v-select>
        </v-row>
        <v-row>
          <v-card class="flex" v-if="mode==='STAT'">
            <v-subheader>Коэффициент отсеивания:</v-subheader>
            <v-slider
                v-model="statisticKf"
                :min="0.5"
                :max="2"
                step="0.25"
                color="indigo"
                ticks
                thumb-label="always">
            </v-slider>
          </v-card>
        </v-row>
        <v-row>
           <v-text-field v-model="lowBorder" :readonly="mode=='STAT'" outlined label="Нижняя граница"></v-text-field>
        </v-row>
        <v-row>
            <v-text-field v-model="topBorder" :readonly="mode=='STAT'" outlined label="Верхняя граница"></v-text-field>
        </v-row>
        <v-row>
          <v-checkbox v-model="autoRefresh" label="Автоматическое отсеивание"></v-checkbox>
        </v-row>
        <v-row>
          <v-btn block @click="setStatisticKf">Применить</v-btn>
        </v-row>
      </v-col>
      <v-col lg="8">
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
                            <td class="text-center">{{ statParameter.minimum }}</td>
                            <td class="text-center">{{ statParameter.maximum }}</td>
                        </tr>
                    </tbody>
                    </template>
                </v-simple-table>
        </v-row>
        <v-row>
          <StatParameterWafer :keyGraphicState="keyGraphicState" :statParameter="statParameter"></StatParameterWafer>
        </v-row>
      </v-col>
      </v-row>
    </v-container>
</template>

<script>
import { mapGetters } from 'vuex';
import StatParameterWafer from './StatParameterWafer.vue';

export default {
  props: ['measurementId', 'keyGraphicState', 'statParameter'],

  components: {
    StatParameterWafer,
  },

  data() {
    return {
      statisticKf: '',
      lowBorder: '',
      topBorder: '',
      russianModeName: 'Статистика',
      autoRefresh: true,
    };
  },

  mounted() {
    this.statisticKf = this.currentDcProfile.k;
    this.russianModeName = this.currentDcProfile.type === 'FXD' ? 'Границы' : 'Статистика';
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
    mode() {
      if (this.russianModeName === 'Статистика') return 'STAT';
      if (this.russianModeName === 'Границы') return 'FXD';
      return '';
    },
  },

  watch: {
    currentDcProfile(currentDcProfile) {
      this.statisticKf = currentDcProfile.k;
      this.russianModeName = currentDcProfile.type === 'FXD' ? 'Границы' : 'Статистика';
      this.topBorder = currentDcProfile.topBorder;
      this.lowBorder = currentDcProfile.lowBorder;
    },
  },

  methods: {
    async setStatisticKf() {
      const dcProfiles = this.dcProfileVmBuilder(this.dcProfiles, this.mode);
      const path = `/api/statrwrk/dirtyCellsSnapshot/${this.measurementId}/${this.keyGraphicState}/withprofile?dcProfilesJSON`;
      const s = (await this.$http.get(`${path}=${JSON.stringify(dcProfiles)}`)).data;
      this.$store.dispatch('wafermeas/updateDirtyCellsSnapshot', { keyGraphicState: this.keyGraphicState, snapshotChunk: s.singleGraphicDirtyCells });
      this.$store.dispatch('wafermeas/updateDcProfiles', { keyGraphicState: this.keyGraphicState, keyGraphicStateDcProfile: s.dcProfiles });
      if (this.autoRefresh) this.refreshSelectedDies();
    },

    dcProfileVmBuilder(dcProfiles, mode) {
      if (mode === 'FXD') {
        return dcProfiles.map((dc) => (dc.statName === this.statParameter.statisticsName
          ? {
            ...dc, k: this.statisticKf, type: mode, lowBorder: this.lowBorder, topBorder: this.topBorder,
          }
          : dc));
      }
      if (mode === 'STAT') {
        return dcProfiles.map((dc) => (dc.statName === this.statParameter.statisticsName
          ? { ...dc, k: this.statisticKf, type: mode }
          : dc));
      }
      return [];
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
