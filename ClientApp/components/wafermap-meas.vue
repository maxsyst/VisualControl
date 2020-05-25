<template>
  <v-container grid-list-lg>
    <loading
      :active.sync="loading"
      :can-cancel="false"
      color="#fc0"
      :width="64"
      :height="64"
      loader="bars"
      :is-full-page="true"
    ></loading>
    <v-row wrap>
      <v-col lg="4">
        <v-row justify-center column>
          <v-col>
            <v-tabs v-model="activeTab" background-color="indigo" dark slider-color="primary" icons-and-text>
              <v-tab href="#wafer">
                Выбор пластины
                <v-icon>table_chart</v-icon>
              </v-tab>

              <v-tab href="#extra">
                Доп.настройки
                <v-icon>timeline</v-icon>
              </v-tab>

              <v-tab href="#statistics">
                Статистика по годным
                <v-icon>equalizer</v-icon>
              </v-tab>

              <v-tab-item value="wafer">
                <v-card color="#303030" dark>
                  <v-card-text>
                    <v-layout justify-center column>
                      <v-autocomplete
                        v-model="selectedWafer"
                        :items="wafers"
                        no-data-text="Нет данных"
                        item-text="waferId"
                        item-value="waferId"
                        filled
                        outlined
                        label="Номер пластины"
                      ></v-autocomplete>
                      <v-select
                        v-model="selectedMeasurementId"
                        :items="measurementRecordings"
                        no-data-text="Нет данных"
                        item-text="name"
                        item-value="id"
                        filled
                        outlined
                        label="Выберите измерение:"
                      ></v-select>
                    </v-layout>
                  </v-card-text>
                </v-card>
              </v-tab-item>
              <v-tab-item value="extra">
                <v-card color="#303030" dark>
                  <v-card-text>
                    <v-row justify-center column>
                      <v-col lg="6">
                        <v-select
                          v-model="selectedGraphics"
                          :items="availiableGraphics"
                          chips
                          no-data-text="Нет данных"
                          item-text="graphicName"
                          item-value="keyGraphicState"
                          item-disabled="disabled"
                          label="Выберите графики:"
                          multiple>
                          <template v-slot:prepend-item>
                            <v-list-item ripple @click="selectAllGraphics">
                              <v-list-item-action>
                                <v-icon :color="selectedGraphics.length > 0 ? 'primary' : ''">{{ selectedGraphicsIcon }}</v-icon>
                              </v-list-item-action>
                              <v-list-item-content>
                                <v-list-item-title>Выбрать все</v-list-item-title>
                              </v-list-item-content>
                            </v-list-item>
                            <v-divider class="mt-2"></v-divider>
                          </template>
                          <template slot="selection" slot-scope="{ item, index }">
                            <v-chip v-if="index < 4">
                              <span>{{ item.graphicName }}</span>
                            </v-chip>
                          </template>
                        </v-select>
                      </v-col>
                      <v-col lg="6">
                        <v-select
                          v-model="selectedDivider"
                          :items="dividers"
                          no-data-text="Нет данных"
                          item-text="name"
                          item-value="dividerK"
                          filled
                          outlined
                          label="Выберите приведение к мм"
                        ></v-select>
                      </v-col>
                    </v-row>
                  </v-card-text>
                </v-card>
              </v-tab-item>
              <v-tab-item value="statistics">
                <v-row>
                  <v-col>
                    <v-card color="#303030" dark>
                      <v-card-title primary-title>
                       <v-chip color="indigo" label dark>Годные по статистике</v-chip>
                      </v-card-title>
                      <v-card-text>
                         <v-progress-circular
                          :rotate="360"
                          :size="90"
                          :width="7"
                          :value="dirtyCells.statPercentageFullWafer"
                          color="primary"
                        >{{ dirtyCells.statPercentageFullWafer + "%" }}</v-progress-circular>
                      </v-card-text>
                    </v-card>
                  </v-col>
                  <v-col>
                    <v-card color="#303030" dark>
                      <v-card-title primary-title>
                        <v-chip color="indigo" label dark>Годные по границам</v-chip>
                      </v-card-title>
                      <v-card-text>
                        <v-progress-circular
                          :rotate="360"
                          :size="90"
                          :width="7"
                          :value="dirtyCells.statPercentageSelected"
                          color="primary"
                        >{{ dirtyCells.statPercentageSelected + "%" }}</v-progress-circular>
                        <v-btn outlined color="primary" @click="delDirtyCells(dirtyCells.statList, selectedDies)">
                          <v-icon>cached</v-icon>
                        </v-btn>
                      </v-card-text>
                    </v-card>
                  </v-col>
                </v-row>
              </v-tab-item>
            </v-tabs>
          </v-col>
        </v-row>
      </v-col>
      <v-col lg="4" offset-lg="1">
        <wafermap-svg
          :waferId="selectedWafer"
          :avbSelectedDies="avbSelectedDies"
          :streetSize="streetSize"
          :fieldHeight="fieldHeight"
          :fieldWidth="fieldWidth"
        ></wafermap-svg>
      </v-col>
      <v-col lg="2">
          <v-card color="#303030" dark v-if="avbSelectedDies.length > 0">
            <v-chip class="elevation-12" color="#303030" dark>{{"Выбрано " + selectedDies.length + " из " + avbSelectedDies.length + " кристаллов" }}</v-chip>
            <v-card-text>
              <v-progress-circular
                :rotate="360"
                :size="90"
                :width="7"
                :value="(selectedDies.length / avbSelectedDies.length)*100"
                color="primary"
              >{{ Math.ceil((selectedDies.length / avbSelectedDies.length)*100) + "%" }}</v-progress-circular>
            </v-card-text>
            <v-card-actions>
              <v-btn outlined color="primary" @click="selectAllDies(avbSelectedDies)">Выбрать все кристаллы</v-btn>
            </v-card-actions>
          </v-card>
      </v-col>
    </v-row>

    <v-row v-for="graphic in availiableGraphics.filter(x => selectedGraphics.includes(x.keyGraphicState))" :key="`kgs-${graphic.keyGraphicState}`">
      <v-col lg="8" class="d-flex">
        <stat-single 
          :measurementId="selectedMeasurementId"
          :keyGraphicState="graphic.keyGraphicState"
          :avbSelectedDies="avbSelectedDies"
          :divider="selectedDivider"
        ></stat-single>
        <v-divider light></v-divider>
      </v-col>
      <v-col lg="4" class="d-flex align-self-center">
        <chart-lnr
          v-if="graphic.keyGraphicState.includes(`LNR`)"
          :measurementId="selectedMeasurementId"
          :keyGraphicState="graphic.keyGraphicState"
          :chartOptions="chartOptions"
          :divider="selectedDivider"
        ></chart-lnr>
        <chart-hstg
          v-else
          :measurementId="selectedMeasurementId"
          :keyGraphicState="graphic.keyGraphicState"
          :divider="selectedDivider"
        ></chart-hstg>
        <v-divider light></v-divider>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import ChartLNR from "./chart-lnr-cjs.vue";
import ChartHSTG from "./chart-bar-cjs.vue";
import StatSingle from "./stat-single.vue";
import Loading from "vue-loading-overlay";
import WaferMap from "./wafermap-svg.vue";
export default {
  data() {
    return {
      loading: false,
      activeTab: "wafer",
      wafers: [],
      dividers: [],
      avbSelectedDies: [],
      measurementRecordings: [],
      availiableGraphics: [],
      selectedWafer: "",
      selectedDivider: "1.0",
      selectedMeasurementId: 0,
      selectedGraphics: [],
      dirtyCells: [],
      streetSize: 3,
      fieldHeight: 320,
      fieldWidth: 320      
    };
  },

  components: {
    "stat-single": StatSingle,
    "wafermap-svg": WaferMap,
    "chart-lnr": ChartLNR,
    "chart-hstg": ChartHSTG,
    Loading
  },

  async created() {
    this.wafers = (await this.$http.get(`/api/wafer/all`)).data
    this.dividers = (await this.$http.get(`/api/divider/all`)).data
  },

  computed: {
    selectedDies() {
      return this.$store.getters['wafermeas/selectedDies']
    },
    selectedGraphicsIcon() {
      if (this.availiableGraphics.length === this.selectedGraphics.length)
        return "check_box";
      if (this.selectedGraphics.length > 0) return "indeterminate_check_box";
      return "check_box_outline_blank";
    }
  },

  methods: {
    delDirtyCells: function(dirtyCellsList, selectedDies) {
      this.$store.commit("wafermeas/updateSelectedDies", selectedDies.filter(die => !dirtyCellsList.includes(die)))
    },

    selectAllDies: function(avbSelectedDies) {
      this.$store.commit("wafermeas/updateSelectedDies", [...avbSelectedDies])
    },

    selectAllGraphics: function() {
      this.$nextTick(() => {
        if (this.selectedGraphics.length === this.availiableGraphics.length) {
          this.selectedFruits = [];
        } else {
          var kgsArray = this.availiableGraphics.map(function(g) {
            return g.keyGraphicState;
          });
          this.selectedGraphics = kgsArray;
        }
      });
    }
  },

  watch: {
    selectedWafer: async function(newValue) {
      this.availiableGraphics = []
      this.measurementRecordings = (await this.$http.get(`/api/measurementrecording?waferId=${newValue}`)).data     
    },

    selectedMeasurementId: async function(newValue) {
      this.availiableGraphics = []
      this.loading = true
      let dieValues = (await this.$http.get(`api/dievalue/GetByMeasurementRecordingId?measurementRecordingId=${newValue}`)).data
      let keyGraphicStateJSON = JSON.stringify(Object.keys(dieValues))
      this.availiableGraphics = (await this.$http.get(`api/graphicsrv6/GetAvailiableGraphicsByKeyGraphicStateList?keyGraphicStateJSON=${keyGraphicStateJSON}`)).data
      let diesList = (await this.$http.get(`api/dievalue/GetSelectedDiesByMeasurementRecordingId?measurementRecordingId=${newValue}`)).data
      this.avbSelectedDies = [...diesList]
      this.dirtyCells = (await this.$http.get(`api/statistic/GetDirtyCellsByMeasurementRecording?measurementRecordingId=${newValue}&&diesCount=${this.avbSelectedDies.length}`)).data    
      this.$store.commit("wafermeas/updateSelectedDies", diesList)
      this.selectAllGraphics() 
      this.delDirtyCells(this.dirtyCells.statList, this.selectedDies)
      this.loading = false
      this.activeTab = "statistics"
    },

    availiableGraphics: function() {
      if (this.availiableGraphics.length == 0) {
        this.selectedGraphics = [];
      }
    },

    selectedDies: function(newValue) {
      let {statList, fixedList} = this.dirtyCells;
      this.dirtyCells.statPercentageSelected = Math.ceil((1.0 - newValue.filter(value => statList.includes(value)).length / newValue.length) * 100)
      this.dirtyCells.fixedPercentageSelected = Math.ceil((1.0 - newValue.filter(value => fixedList.includes(value)).length / newValue.length) * 100)
    }
  }
};
</script>
