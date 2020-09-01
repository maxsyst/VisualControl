<template>
    <v-container fluid>
      <v-row>
        <v-col lg="8">
        <v-toolbar extended>          
            <v-container>
              <v-row>    
                <v-col lg="6" class="d-flex align-center justify-start" v-if="!loading">  
                <v-chip class="elevation-8" label x-large color="#303030">
                  {{graphicName}}
                </v-chip>
                </v-col>  
                <v-col lg="6" class="d-flex align-center justify-space-around" v-if="!loading">
                <div class="d-flex flex-column" >
                  <v-chip class="elevation-8" color="#303030">
                    Годны по всей пластине
                  </v-chip>
                  <div class="d-flex flex-row mt-4">   
                    <v-progress-circular
                      :rotate="360"
                      :size="60"
                      :width="2"
                      :value="dirtyCellsFullWafer.statPercentage"
                      :color="this.$store.getters['wafermeas/calculateColor'](dirtyCellsFullWafer.statPercentage / 100)">
                    {{ dirtyCellsFullWafer.statPercentage + '%'}}
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
                    :value="mode === `stat` ? dirtyCellsStatPercentage : dirtyCellsFixedPercentage"
                    :color="mode === `stat` ? 'primary' : 'indigo lighten-4'">
                    {{ mode === `stat` ? dirtyCellsStatPercentage + '%' : dirtyCellsFixedPercentage + '%' }}
                  </v-progress-circular>
                  <v-btn text icon :color="mode === `stat` ? 'primary' : 'indigo lighten-4'" @click="delDirtyCells(dirtyCells)">
                    <v-icon>cached</v-icon>
                  </v-btn>    
               </div>   
              </div>
                            
              <!-- <v-switch color="primary" v-model="switchMode" :label="mode"></v-switch> -->
               </v-col> 
               <v-col v-else>
                <v-skeleton-loader
                  class="mx-auto"
                  type="date-picker-options">
                </v-skeleton-loader>
               </v-col>
              </v-row>
            </v-container>
        </v-toolbar>
        </v-col>
        <v-col lg="4">
          <wafer-mini v-if="dirtyCellsFullWafer.statPercentage > 0"
            :avbSelectedDies="avbSelectedDies"
            :keyGraphicState="keyGraphicState"
            :key="`wfm-${keyGraphicState}`"
        ></wafer-mini>
        </v-col>
      </v-row>
      <v-row>
        <v-col lg="12">
          <v-tabs v-model="activeTab" color="primary" dark slider-color="indigo">
            <v-tab href="#commonTable">Сводная таблица</v-tab>
            <v-tab
              v-for="stat in statArray"
              :key="stat.shortStatisticsName"
              :href="'#' + stat.shortStatisticsName"
              v-html="stat.shortStatisticsName">
            </v-tab>
            <v-tab-item
              v-for="stat in statArray"
              :key="stat.shortStatisticsName"
              :value="stat.shortStatisticsName">
              <gradient-full :key="'GRF_' + stat.shortStatisticsName + keyGraphicState" 
                             :avbSelectedDies="avbSelectedDies"
                             :measurementId="measurementId" 
                             :keyGraphicState="keyGraphicState" 
                             :statParameter="stat" 
                             :divider="divider" 
                             :statisticKf="statisticKf">
              </gradient-full>
            </v-tab-item>
            <v-tab-item value="commonTable">
              <v-card flat>
                <v-card-text>
                  <v-row>
                    <v-col lg="12">
                      <v-data-table v-if="statArray.length > 0"
                        :headers="headers"
                        :items="statArray"
                        loading-text="Загрузка данных..."
                        no-data-text="Нет данных"
                        class="elevation-2 pa-0"  
                        :loading="loading"
                        hide-default-footer
                        dark
                      >
                        <template v-slot:item.shortStatisticsName="{ item }">
                          <v-tooltip bottom>
                            <template v-slot:activator="{ on }">
                                <v-chip color="indigo" v-on="on" label v-html="item.unit.trim() ? item.shortStatisticsName + ', ' + item.unit : item.shortStatisticsName" dark></v-chip>
                            </template>
                            <span v-html="item.statisticsName"></span>
                          </v-tooltip>
                       
                        </template>
                        <template v-slot:item.fwStatPercentage="{item}">
                        <td class="text-xs-center">
                          <v-progress-circular
                            :rotate="360"
                            :size="45"
                            :width="2"
                            :value = "item.fwStatPercentage"
                            :color= "$store.getters['wafermeas/calculateColor'](item.fwStatPercentage/100)"
                          >{{ item.fwStatPercentage + '%'}}</v-progress-circular>

                        </td>
                        </template>
                        <template v-slot:item.dirtyCells="{item}">
                        <td class="text-xs-center">
                          <v-progress-circular
                            :rotate="360"
                            :size="45"
                            :width="2"
                            :value = "mode === `stat` ? item.dirtyCells.statPercentageFullWafer : item.dirtyCells.fixedPercentageFullWafer"
                            :color= "mode === `stat` ? 'primary' : 'indigo lighten-4'"
                          >{{ mode === `stat` ? item.dirtyCells.statPercentageFullWafer + '%' : item.dirtyCells.fixedPercentageFullWafer + '%' }}</v-progress-circular>

                          <v-btn text icon :color="mode === `stat` ? 'primary' : 'indigo lighten-4'" @click="delDirtyCells(item.dirtyCells)">
                            <v-icon>cached</v-icon>
                          </v-btn>
                        </td>
                        </template>
                      </v-data-table>
                      <div v-else>
                       <v-skeleton-loader v-if="loading"
                          class="mx-auto"
                          type="table-tbody"
                        ></v-skeleton-loader>
                        <p v-else>Не найдено статистических параметров на данном графике</p>
                      </div>
                    </v-col>
                  </v-row>
                </v-card-text>
              </v-card>
            </v-tab-item>
          </v-tabs>
        </v-col>
      </v-row>
    </v-container>
</template>

<script>
import WaferMap from "./wafer-mini.vue";
import GradientFull from './Gradient/gradient-full.vue' 
export default {
  props: ["keyGraphicState", "measurementId", "divider", "statisticKf", "avbSelectedDies"],
  components: {
    "wafer-mini": WaferMap,
    "gradient-full": GradientFull
  },
  data() {
    return {
      showPopover: false,
      PopoverX: 0,
      PopoverY: 0,
      switchMode: true,
      statArray: [],
      fullWaferStatArray: [],
      dirtyCellsFullWafer: {cellsId: [], statPercentage: -1},
      graphicName: "",
      activeTab: "commonTable",
      loading: true,
      headers: [
        {
          text: "Название",
          align: "center",
          sortable: false,
          value: "shortStatisticsName",
          width: '20%'
        },
        {
          text: "μ",
          align: "center",
          sortable: false,
          value: "expectedValue",
          width: '10%'
        },
        {
          text: "σ",
          align: "center",
          sortable: false,
          value: "standartDeviation",
          width: '10%'
        },
        {
          text: "Мин",
          align: "center",
          sortable: false,
          value: "minimum",
          width: '10%'
        },
        {
          text: "Макс",
          align: "center",
          sortable: false,
          value: "maximum",
          width: '10%'
        },
        {
          text: "Медиана",
          align: "center",
          sortable: false,
          value: "median",
          width: '10%'
        },
        {

          text: "Годны по всей пластине, %",
          align: "start",
          sortable: false,
          value: "fwStatPercentage",
          width: '15%'
        },
        {
          text: "Годны из выбранных, %",
          align: "center",
          sortable: false,
          value: "dirtyCells",
          width: '15%'
        }
      ]
    };
  },

  async created() {
    this.graphicName = this.$store.getters['wafermeas/getGraphicByGraphicState'](this.keyGraphicState).graphicName
    this.fullWaferStatArray = (await this.$http
      .get(`/api/statistic/GetStatisticSingleGraphicFullWafer?measurementRecordingId=${this.measurementId}&keyGraphicState=${this.keyGraphicState}&k=${this.statisticKf}`)).data
    this.calculateFullWaferDirtyCells(this.fullWaferStatArray)
    await this.getStatArray()
  },

  methods: {
    delDirtyCells: function(dirtyCells) {
      let deletedDies = this.mode === "stat" ? dirtyCells.statList : dirtyCells.fixedList
      let selectedDies = this.selectedDies.filter(el => !deletedDies.includes(el))
      this.$store.dispatch("wafermeas/updateSelectedDies", selectedDies)
    },

    showStatTab(statisticsName) {
      this.activeTab = statisticsName
    },

    changeTab(e) {
      console.log(this.keyGraphicState + ' ' + e)
    },

    showPopoverClick(e) {
      e.preventDefault();
      this.showPopover = false;
      this.PopoverX = e.clientX;
      this.PopoverY = e.clientY;
      this.$nextTick(() => {
        this.showPopover = true;
      });
    },

    getStatArray: async function() {
      if (this.measurementId != 0 && this.selectedDies.length > 0) {
        this.loading = true
        let singlestatModel = {};
        singlestatModel.k = this.statisticKf
        singlestatModel.divider = this.divider;
        singlestatModel.keyGraphicState = this.keyGraphicState;
        singlestatModel.measurementId = this.measurementId;
        singlestatModel.dieIdList = this.selectedDies;
        this.statArray = (await this.$http
          .get(`/api/statistic/GetStatisticSingleGraphic?statisticSingleGraphicViewModelJSON=${JSON.stringify(singlestatModel)}`)).data
        this.statArray = this.statArray.map(s => ({...s, fwStatPercentage: this.fullWaferStatArray.find(f => f.parameterID === s.parameterID).dirtyCells.statPercentageFullWafer}))
        this.loading = false
      }
    },

    calculateFullWaferDirtyCells(fullWaferStatArray) {
      this.dirtyCellsFullWafer.cellsId = [...new Set(fullWaferStatArray.reduce((p,c) => [...p, ...c.dirtyCells.statList], []))]
      this.dirtyCellsFullWafer.statPercentage = Math.ceil((1.0 - (this.dirtyCellsFullWafer.cellsId.length / this.avbSelectedDies.length)) * 100)
      this.$store.dispatch("wafermeas/updateDirtyCellsFullWaferSingleGraphic", { 
        keyGraphicState: this.keyGraphicState, 
        dirtyCells: {cellsId: [...this.dirtyCellsFullWafer.cellsId], statPercentage: this.dirtyCellsFullWafer.statPercentage}
      })
    }
  },

  watch: {
    divider: function() {
      this.getStatArray();
    },

    statisticKf: async function(newValue) {
      this.fullWaferStatArray = (await this.$http
          .get(`/api/statistic/GetStatisticSingleGraphicFullWafer?measurementRecordingId=${this.measurementId}&keyGraphicState=${this.keyGraphicState}&k=${newValue}`)).data
      this.calculateFullWaferDirtyCells(this.fullWaferStatArray)
      await this.getStatArray()
    },

    selectedDies: function() {
      this.getStatArray();
    }
  },

  computed: {
    mode() {
      return this.switchMode ? "stat" : "fixed"
    },
  
    selectedDies() {
      return this.$store.getters['wafermeas/selectedDies']
    },

    dirtyCells() {
      let statArray = [];
      let fixedArray = [];
      this.statArray.forEach(s => {
        (statArray = statArray.concat(s.dirtyCells.statList)),
          (fixedArray = fixedArray.concat(s.dirtyCells.fixedList));
      });
      return {
        statList: [...new Set(statArray)],
        fixedList: [...new Set(fixedArray)]
      };
    },

    dirtyCellsStatPercentage() {
      let percentage = Math.ceil((1.0 - this.dirtyCells.statList.length / this.selectedDies.length) * 100)
      this.$store.dispatch("wafermeas/updateDirtyCellsSelectedNowSingleGraphic", { 
        keyGraphicState: this.keyGraphicState, 
        dirtyCells: {cellsId: [...this.dirtyCells.statList], statPercentage: percentage}
      })
      return isNaN(percentage) ? 0 : percentage
    },

    dirtyCellsFixedPercentage() {
      let percentage = Math.ceil((1.0 - this.dirtyCells.fixedList.length / this.selectedDies.length) * 100)
      return isNaN(percentage) ? 0 : percentage
    }
  }
};
</script>

<style>
.card-shadow {
  --box-shadow-color: palegoldenrod;
  box-shadow: 1px 2px 3px var(--box-shadow-color);
}
</style>
