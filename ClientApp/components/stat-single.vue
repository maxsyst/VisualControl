<template>
    <v-container fluid>
      <v-row>
        <v-col lg="6">
        <v-toolbar extended>          
            <v-container>
              <v-row>    
                <v-col lg="6" class="d-flex align-center justify-start" v-if="this.fullWaferStatArray.length > 0">  
                <v-chip class="elevation-8" label x-large color="#303030">
                  {{graphicName}}
                </v-chip>
                </v-col>  
                <v-col lg="6" class="d-flex align-center justify-space-around" v-if="this.fullWaferStatArray.length > 0">
                <div class="d-flex flex-column" >
                  <v-chip class="elevation-8" color="#303030">
                    Годны по всей пластине
                  </v-chip>
                  <div class="d-flex flex-row mt-4">   
                    <v-progress-circular
                      :rotate="360"
                      :size="60"
                      :width="4"
                      :value="dirtyCellsFullWafer.statPercentage"
                      :color="calculateColor(dirtyCellsFullWafer.statPercentage / 100)">
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
                    :width="4"
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
              :value="stat.shortStatisticsName"
            >
              <v-card flat>
                <v-card-text>{{ stat.shortStatisticsName }}</v-card-text>
              </v-card>
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
                            :width="4"
                            :value = "Math.ceil((1.0 - item.fwStatPercentage) * 100)"
                            :color= "calculateColor(1.0 - item.fwStatPercentage)"
                          >{{ Math.ceil((1.0 - item.fwStatPercentage) * 100) + '%'}}</v-progress-circular>

                        </td>
                        </template>
                        <template v-slot:item.dirtyCells="{item}">
                        <td class="text-xs-center">
                          <v-progress-circular
                            :rotate="360"
                            :size="45"
                            :width="4"
                            :value = "mode === `stat` ? Math.ceil((1.0 - item.dirtyCells.statPercentage) * 100) : Math.ceil((1.0 - item.dirtyCells.fixedPercentage) * 100)"
                            :color= "mode === `stat` ? 'primary' : 'indigo lighten-4'"
                          >{{ mode === `stat` ? Math.ceil((1.0 - item.dirtyCells.statPercentage) * 100) + '%' : Math.ceil((1.0 - item.dirtyCells.fixedPercentage) * 100) + '%' }}</v-progress-circular>

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
import { width } from '@amcharts/amcharts4/.internal/core/utils/Utils';
export default {
  props: ["keyGraphicState", "measurementId", "divider", "avbSelectedDies", "colors"],

  data() {
    return {
      colors: {green: 0.8, orange: 0.6, red: 0.1, indigo: 0},
      showPopover: false,
      PopoverX: 0,
      PopoverY: 0,
      switchMode: true,
      statArray: [],
      fullWaferStatArray: [],
      dirtyCellsFullWafer: {cellsId: [], statPercentage: 0},
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
          text: "Min",
          align: "center",
          sortable: false,
          value: "minimum",
          width: '10%'
        },
        {
          text: "Max",
          align: "center",
          sortable: false,
          value: "maximum",
           width: '10%'
        },
        {
          text: "Median",
          align: "center",
          sortable: false,
          value: "median",
          width: '10%'
        },
        {
          text: "CorrectFullWafer,%",
          align: "center",
          sortable: false,
          value: "fwStatPercentage",
          width: '20%'
        },
        {
          text: "CorrectSelected,%",
          align: "center",
          sortable: false,
          value: "dirtyCells",
          width: '20%'
        }
      ]
    };
  },

  async created() {
    this.graphicName = (await this.$http
      .get(`api/graphicsrv6/GetGraphicNameByKeyGraphicState?=${this.keyGraphicState}`)).data
    this.fullWaferStatArray = (await this.$http
      .get(`api/statistic/GetStatisticSingleGraphicFullWafer?measurementRecordingId=${this.measurementId}&&keyGraphicState=${this.keyGraphicState}`)).data
    this.calculateFullWaferDirtyCells(this.fullWaferStatArray)
    await this.getStatArray()
  },

  methods: {
    delDirtyCells: function(dirtyCells) {
      let deletedDies = this.mode === "stat" ? dirtyCells.statList : dirtyCells.fixedList
      let selectedDies = this.selectedDies.filter(el => !deletedDies.includes(el))
      this.$store.commit("wafermeas/updateSelectedDies", selectedDies)
    },

    showStatTab(statisticsName) {
      this.activeTab = statisticsName;
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
        singlestatModel.divider = this.divider;
        singlestatModel.keyGraphicState = this.keyGraphicState;
        singlestatModel.measurementId = this.measurementId;
        singlestatModel.dieIdList = this.selectedDies;
        this.statArray = (await this.$http
          .get(`api/statistic/GetStatisticSingleGraphic?statisticSingleGraphicViewModelJSON=${JSON.stringify(singlestatModel)}`)).data
        this.statArray = this.statArray.map(s => ({...s, fwStatPercentage: this.fullWaferStatArray.find(f => f.parameterID === s.parameterID).dirtyCells.statPercentage}))
        this.loading = false
      }
    },

    calculateFullWaferDirtyCells(fullWaferStatArray) {
      this.dirtyCellsFullWafer.cellsId = [...new Set(fullWaferStatArray.reduce((p,c) => [...p, ...c.dirtyCells.statList], []))]
      this.dirtyCellsFullWafer.statPercentage = Math.ceil((1.0 - (this.dirtyCellsFullWafer.cellsId.length / this.avbSelectedDies.length)) * 100)
    },

    calculateColor(statPercentage) {     
      if(statPercentage >= this.colors.green) {
        return "green"
      }
      else {
        if(statPercentage >= this.colors.orange) {
          return "orange"
        }
        else {
          if(statPercentage >= this.colors.red) {
            return "pink"
          }
          else {
            return "indigo"
          }
        }
      }
    }
  },

  watch: {
    divider: function() {
      this.getStatArray();
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
