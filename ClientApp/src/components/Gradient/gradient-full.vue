<template>
    <v-container>
        <v-row>
             <v-col lg="6">
                    <perfect-scrollbar>
                        <v-simple-table>
                            <template v-slot:default>
                            <thead>
                                <tr>
                                    <th class="text-center">Название</th>
                                    <th class="text-center">LB</th>
                                    <th class="text-center">TB</th>
                                    <th class="text-center">К-во</th>
                                    <th class="text-center">Цвет</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="step in gradientTableData" :key="step.name">
                                    <td class="text-center"><v-chip color="indigo" label v-html="step.name" dark></v-chip></td>
                                    <td class="text-center border">{{ step.name == 'Low' ? '<' + step.lowBorder : step.lowBorder }}</td>
                                    <td class="text-center border">{{ step.name == 'High' ? '>' + step.topBorder : step.topBorder }}</td>
                                    <td class="text-center">{{ step.dieList.length }}</td>
                                    <td class="text-center"><v-chip :color="step.color" label dark></v-chip></td>
                                    <td class="text-center"> <v-icon v-if="step.dieList.length>0"
                                                                    color="primary"
                                                                    @click="deleteByColor(step.dieList)">
                                                                    delete_outline</v-icon>
                                    </td>
                                </tr>
                            </tbody>
                            </template>
                        </v-simple-table>
                    </perfect-scrollbar>
            </v-col>
            <v-col lg="6">
                <v-row>
                    <v-tabs right v-model="activeTab" color="primary" dark slider-color="indigo">
                        <v-tab href="#gradientMapTab">Карта</v-tab>
                        <v-tab href="#histogramTab">Гистограмма</v-tab>
                        <v-tab-item value="gradientMapTab">
                            <gradient-map :gradientSteps="gradientData.gradientSteps"></gradient-map>
                        </v-tab-item>
                        <v-tab-item value="histogramTab">
                            <gradient-hstg :gradientSteps="gradientData.gradientSteps"></gradient-hstg>
                        </v-tab-item>
                    </v-tabs>
                </v-row>
                <v-row>
                  <v-btn block @click="goToGradientChart">Построить график с градиентом</v-btn>
                </v-row>
            </v-col>
        </v-row>
        <v-row>

        </v-row>
    </v-container>
</template>
<script>
import GradientWafer from './gradient-wafer.vue';
import GradientHstg from './gradient-histogram.vue';

export default {
  props: ['measurementId', 'keyGraphicState', 'statParameter', 'divider'],
  components: {
    'gradient-map': GradientWafer,
    'gradient-hstg': GradientHstg,
  },
  data() {
    return {
      activeTab: 'gradientMapTab',
      gradientData: {},
      gradientTableData: [],
      stepsQuantity: 32,
    };
  },

  methods: {
    deleteByColor(dieList) {
      this.$store.dispatch('wafermeas/updateSelectedDies', this.selectedDies.filter((x) => !dieList.includes(x)));
    },

    goToGradientChart() {
      this.$store.dispatch('wafermeas/changeKeyGraphicStateMode', { keyGraphicState: this.keyGraphicState, mode: 'gradient' });
    },

    async refresh() {
      const dcProfile = this.dcProfiles.find((dc) => dc.statName === this.statParameter.statisticsName);
      this.gradientData = (await this.$http
        .get(`/api/gradient/statparameter?gradientViewModelJSON=${JSON.stringify({
          measurementRecordingId: this.measurementId,
          stepsQuantity: this.stepsQuantity,
          divider: this.divider,
          keyGraphicState: this.keyGraphicState,
          statParameter: this.statParameter.statisticsName,
          lowBorder: dcProfile.lowBorder,
          topBorder: dcProfile.topBorder,
        })}`)).data;
      this.$store.dispatch('wafermeas/updateGradientData', { keyGraphicState: this.keyGraphicState, gradientData: this.gradientData });
      this.rerenderTable(this.selectedDies);
    },

    rerenderTable(selectedDies) {
      this.gradientTableData = this.gradientData.gradientSteps.map((g) => ({ ...g, dieList: g.dieList.filter((d) => selectedDies.includes(d)) }));
    },
  },

  watch: {
    selectedDies(selectedDies) {
      this.rerenderTable(selectedDies);
    },
    async divider() {
      await this.refresh();
    },
    async dcProfiles() {
      await this.refresh();
    },
  },

  computed:
    {
      selectedDies() {
        return this.$store.getters['wafermeas/selectedDies'];
      },

      dcProfiles() {
        return this.$store.getters['wafermeas/getDcProfilesByKeyGraphicState'](this.keyGraphicState);
      },
    },

  async mounted() {
    await this.refresh();
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
