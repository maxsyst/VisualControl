<template>   
     <v-skeleton-loader v-if="loading"
                          class="mx-auto"
                          type="date-picker-days">
    </v-skeleton-loader>
    <v-container v-else>
        <v-row>            
            <v-col lg="6">
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
                            <td><v-chip color="indigo" label v-html="statParameter.unit.trim() ? statParameter.shortStatisticsName + ', ' + statParameter.unit : statParameter.shortStatisticsName" dark></v-chip></td>
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
                    <v-tabs v-model="activeTab" color="primary" dark slider-color="indigo">
                        <v-tab href="#gradientMapTab">Карта градиента</v-tab>
                        <v-tab href="#histogramTab">Гистограмма распределения</v-tab>
                        <v-tab-item value="gradientMapTab">
                            <gradient-map :gradientSteps="gradientData.gradientSteps" :avbSelectedDies="avbSelectedDies"></gradient-map>
                        </v-tab-item>
                        <v-tab-item value="histogramTab">
                            <gradient-hstg :gradientSteps="gradientData.gradientSteps"></gradient-hstg>
                        </v-tab-item>
                    </v-tabs>
                </v-row>
            </v-col>
            <v-col lg="6">
                    <perfect-scrollbar>
                        <v-simple-table>
                            <template v-slot:default>
                            <thead>
                                <tr>
                                    <th class="text-center">Название</th>
                                    <th class="text-center">Интервал</th>
                                    <th class="text-center">Всего кристаллов</th>
                                    <th class="text-center">Цвет</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="step in gradientData.gradientSteps" :key="step.name">
                                    <td class="text-center"><v-chip color="indigo" label v-html="step.name" dark></v-chip></td>
                                    <td class="text-center border">{{ step.borderDescription }}</td>
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
        </v-row>
        <v-row>
            
        </v-row>
    </v-container>
</template>
<script>
    import GradientWafer from './gradient-wafer.vue' 
    import GradientHstg from './gradient-histogram.vue' 
    export default {
        props: ['measurementId', 'avbSelectedDies', 'keyGraphicState', 'statParameter', 'divider', 'statisticKf'],
        components: {
            "gradient-map": GradientWafer,
            "gradient-hstg": GradientHstg
        },
        data() {
            return {
               loading: false,
               activeTab: "gradientMapTab",
               gradientData: {},
               stepsQuantity: 32
        }
    },

    methods: {
        deleteByColor(dieList) {
            this.$store.dispatch("wafermeas/updateSelectedDies", this.selectedDies.filter(x => !dieList.includes(x)));
        }
    },

    watch: {
        selectedDies: async function(newVal) {
            this.gradientData = (await this.$http
                .get(`/api/gradient/statparameter?gradientViewModelJSON=${JSON.stringify({measurementRecordingId: this.measurementId,
                                                                                        stepsQuantity: this.stepsQuantity,
                                                                                        divider: this.divider, 
                                                                                        keyGraphicState: this.keyGraphicState, 
                                                                                        statParameter: this.statParameter.statisticsName,
                                                                                        k: this.statisticKf,
                                                                                        selectedDiesId: [...this.selectedDies]})}`)).data
        }
    },

    computed:
    {
      selectedDies() {
        return this.$store.getters['wafermeas/selectedDies']
      }
    },

    async mounted() {
        this.loading = true
        this.gradientData = (await this.$http
            .get(`/api/gradient/statparameter?gradientViewModelJSON=${JSON.stringify({measurementRecordingId: this.measurementId,
                                                                                      stepsQuantity: this.stepsQuantity,
                                                                                      divider: this.divider, 
                                                                                      keyGraphicState: this.keyGraphicState, 
                                                                                      statParameter: this.statParameter.statisticsName,
                                                                                      k: this.statisticKf,
                                                                                      selectedDiesId: [...this.selectedDies]})}`)).data
        this.loading = false
    }
}
</script>

<style scoped>
  .ps {
    height: 475px;
  }

  .border {
    font-size: x-small;
  }
</style>
