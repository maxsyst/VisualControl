<template>
    <v-container fluid>
      <v-row align-start justify-space-between>
        <v-toolbar>
          <v-toolbar-title>{{graphicName}}</v-toolbar-title>
          <v-spacer></v-spacer>
          <v-toolbar-items class="hidden-sm-and-down">
            <v-progress-circular
              :rotate="360"
              :size="60"
              :width="6"
              :value="mode === `stat` ? dirtyCellsStatPercentage : dirtyCellsFixedPercentage"
              :color="mode === `stat` ? 'primary' : 'indigo lighten-4'"
            >{{ mode === `stat` ? dirtyCellsStatPercentage + '%' : dirtyCellsFixedPercentage + '%' }}</v-progress-circular>
            <v-btn
              text
              icon
              :color="mode === `stat` ? 'primary' : 'indigo lighten-4'"
              @click="delDirtyCells(dirtyCells)"
            >
              <v-icon>cached</v-icon>
            </v-btn>
            <v-switch color="primary" v-model="switchMode" :label="mode"></v-switch>
          </v-toolbar-items>
        </v-toolbar>
      </v-row>
      <v-row>
        <v-col lg="12">
          <v-tabs v-model="activeTab" color="primary" dark slider-color="indigo">
            <v-tab href="#commonTable">Сводная таблица</v-tab>

            <v-tab
              v-for="stat in statArray"
              :key="stat.statisticsName"
              :href="'#' + stat.statisticsName"
              v-html="stat.statisticsName">
            </v-tab>

            <v-tab-item
              v-for="stat in statArray"
              :key="stat.statisticsName"
              :value="stat.statisticsName"
            >
              <v-card flat>
                <v-card-text>{{ stat.statisticsName }}</v-card-text>
              </v-card>
            </v-tab-item>
            <v-tab-item value="commonTable">
              <v-card flat>
                <v-card-text>
                  <v-row>
                    <v-col lg="12">
                      <v-data-table 
                        :headers="headers"
                        :items="statArray"
                        no-data-text="Нет данных"
                        class="elevation-2 pa-0"  
                        :loading="loading"
                        hide-default-footer
                        dark
                      >
                        <template v-slot:item.statisticsName="{ item }">
                          <v-chip color="indigo" label v-html="item.statisticsName" dark></v-chip>
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
export default {
  props: ["keyGraphicState", "measurementId", "divider"],

  data() {
    return {
      showPopover: false,
      PopoverX: 0,
      PopoverY: 0,
      switchMode: true,
      statArray: [],
      graphicName: "",
      activeTab: "commonTable",
      loading: false,
      headers: [
        {
          text: "Название",
          align: "center",
          sortable: false,
          value: "statisticsName"
        },
        {
          text: "μ",
          align: "center",
          sortable: false,
          value: "expectedValue"
        },
        {
          text: "σ",
          align: "center",
          sortable: false,
          value: "standartDeviation"
        },
        {
          text: "Min",
          align: "center",
          sortable: false,
          value: "minimum"
        },
        {
          text: "Max",
          align: "center",
          sortable: false,
          value: "maximum"
        },
        {
          text: "Med",
          align: "center",
          sortable: false,
          value: "median"
        },
        {
          text: "Correct,%",
          align: "center",
          sortable: false,
          value: "dirtyCells"
        }
      ]
    };
  },

  created() {
    this.$http
      .get(
        `api/graphicsrv6/GetGraphicNameByKeyGraphicState?=${this.keyGraphicState}`
      )
      .then(response => {
        this.graphicName = response.data;
        this.getStatArray();
      })
      .catch(error => {});
  },

  methods: {
    delDirtyCells: function(dirtyCells) {
      let deletedDies = [];
      if (this.mode === "stat") {
        deletedDies = dirtyCells.statList;
      } else {
        deletedDies = dirtyCells.fixedList;
      }

      let selectedDies = this.selectedDies.filter(
        el => !deletedDies.includes(el)
      );
      this.$store.commit("wafermeas/updateSelectedDies", selectedDies);
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

    getStatArray: function() {
      if (this.measurementId != 0 && this.selectedDies.length > 0) {
        this.loading = true;
        var singlestatModel = {};
        singlestatModel.divider = this.divider;
        singlestatModel.keyGraphicState = this.keyGraphicState;
        singlestatModel.measurementId = this.measurementId;
        singlestatModel.dieIdList = this.selectedDies;
        this.$http
          .get(
            `api/statistic/GetStatisticSingleGraphic?statisticSingleGraphicViewModelJSON=${JSON.stringify(
              singlestatModel
            )}`
          )
          .then(response => {
            var singleStat = response.data;
            this.loading = false;
            this.statArray = singleStat;
          })
          .catch(error => {});
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
      if (this.switchMode) {
        return "stat";
      } else {
        return "fixed";
      }
    },

    selectedDies() {
      return this.$store.state.wafermeas.selectedDies;
    },

    dirtyCells() {
      var statArray = [];
      var fixedArray = [];
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
      var percentage = Math.ceil(
        (1.0 - this.dirtyCells.statList.length / this.selectedDies.length) * 100
      );
      if (isNaN(percentage)) {
        return 0;
      } else {
        return percentage;
      }
    },

    dirtyCellsFixedPercentage() {
      var percentage = Math.ceil(
        (1.0 - this.dirtyCells.fixedList.length / this.selectedDies.length) *
          100
      );
      if (isNaN(percentage)) {
        return 0;
      } else {
        return percentage;
      }
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
