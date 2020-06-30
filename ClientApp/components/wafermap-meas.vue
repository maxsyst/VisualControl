<template>
  <v-container grid-list-lg>
    <v-btn
      v-scroll="onScroll"
      v-show="fabToTop"
      fab
      dark
      fixed
      bottom
      right
      color="indigo"
      @click="toTop">
      <v-icon>keyboard_arrow_up</v-icon>
    </v-btn>
    <loading
      :active.sync="loading"
      :can-cancel="false"
      color="#fc0"
      :width="100"
      :height="50"
      loader="bars"
      :is-full-page="true"
    ></loading>
    <v-row wrap>
      <v-col lg="5">
        <v-row justify-center column>
          <mini-report :waferId="selectedWafer" :selectedMeasurementId="selectedMeasurementId"></mini-report>
        </v-row>
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
                        v-if="measurementRecordings.length>0"
                        v-model="selectedMeasurementId"
                        :items="measurementRecordings"
                        no-data-text="Нет данных"
                        item-text="name"
                        item-value="id"
                        filled
                        outlined
                        label="Выберите измерение:">
                      </v-select>
                      <v-progress-circular class="ml-4"
                        v-else
                        :width="3"
                        indeterminate
                        color="primary">
                      </v-progress-circular>
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
                    </v-row>
                  </v-card-text>
                </v-card>
              </v-tab-item>
              <v-tab-item value="statistics">
                <v-row>
                  <v-col lg="8">
                    <v-card dark> 
                      <perfect-scrollbar>
                        <micro-row class="mt-2" v-for="graphic in availiableGraphics" :key="`kgs-${graphic.keyGraphicState}`" :avbSelectedDies="avbSelectedDies" :keyGraphicState="graphic.keyGraphicState"></micro-row>
                      </perfect-scrollbar>
                    </v-card>
                  </v-col>
                  <v-col lg="4">
                  <v-btn color="primary" outlined small @click="selectAllGraphics">Выбрать все графики</v-btn> 
                  <v-chip class="elevation-12 mt-4" color="#303030" dark>Годны по всей пластине</v-chip>
                  <v-card class="mr-2 mt-2 mb-4" color="#303030" dark>
                      <v-card-text>
                        <v-progress-circular
                          :rotate="360"
                          :size="90"
                          :width="3"
                          :value="dirtyCells.statPercentageFullWafer"
                          :color="$store.getters['wafermeas/calculateColor'](dirtyCells.statPercentageFullWafer / 100)">
                          {{ dirtyCells.statPercentageFullWafer + "%" }}
                        </v-progress-circular>
                        <v-tooltip class="ml-8" v-model="showUnSelectedGraphics" v-if="unSelectedGraphics.length>0" top>
                          <template v-slot:activator="{ on, attrs }">
                            <v-btn icon v-bind="attrs" v-on="on">
                              <v-icon color="primary">report</v-icon>
                            </v-btn>
                          </template>
                          <div>
                              <p>
                                <span>Некоторые графики не выбраны:</span>
                              </p>
                              <p v-for="g in unSelectedGraphics">
                                <span :key="'SPAN_'+g.keyGraphicState">{{g.graphicName}}</span>
                              </p>
                          </div>
                        
                      </v-tooltip>
                      </v-card-text>
                    </v-card>
                      <v-chip class="elevation-12 mt-4 ms-2" color="#303030" dark>Годны из выбранных</v-chip>
                      <v-card class="mr-2 mt-2" color="#303030" dark>
                        <v-card-text>
                          <v-progress-circular
                            :rotate="360"
                            :size="90"
                            :width="3"
                            :value="dirtyCells.statPercentageSelected"
                            color="primary">{{ dirtyCells.statPercentageSelected + "%" }}
                          </v-progress-circular>
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
          :avbSelectedDies="avbSelectedDies"
          :mapMode="mapMode">
        </wafermap-svg>
      </v-col>
      <v-col lg="2">
          <v-chip color="#303030" v-if="avbSelectedDies.length > 0" dark>{{"Выбрано " + selectedDies.length + " из " + avbSelectedDies.length + " кристаллов" }}</v-chip>
          <v-card class="mt-2" color="#303030" dark v-if="avbSelectedDies.length > 0">            
            <v-card-text class="d-flex justify-space-between">
              <div class="d-flex flex-column">
                  <v-btn :color="mapMode === 'selected' ? 'indigo' : 'grey darken-2'" class="mt-auto" fab small dark @click="mapMode='selected'">
                    Вбр
                  </v-btn>
                  <v-btn :color="mapMode === 'dirty' ? 'indigo' : 'grey darken-2'" class="mt-auto" fab small dark @click="mapMode='dirty'">
                    Гдн
                  </v-btn>
                </div>
                <v-progress-circular
                  :rotate="360"
                  :size="90"
                  :width="3"
                  :value="(selectedDies.length / avbSelectedDies.length)*100"
                  color="primary">
                  {{ Math.ceil((selectedDies.length / avbSelectedDies.length)*100) + "%" }}</v-progress-circular>
            </v-card-text>
            <v-card-actions>
              <v-btn color="primary" outlined @click="selectAllDies(avbSelectedDies)">Выбрать все кристаллы</v-btn>
            </v-card-actions>
          </v-card>
      </v-col>
    </v-row>
    <v-divider></v-divider>
    <v-row v-for="graphic in availiableGraphics.filter(x => selectedGraphics.includes(x.keyGraphicState))" :id="`kgs-${graphic.keyGraphicState}`" :key="`kgs-${graphic.keyGraphicState}`">
      <v-col lg="8" class="d-flex">
        <stat-single :id="'ss_' + graphic.keyGraphicState"
          :measurementId="selectedMeasurementId"
          :keyGraphicState="graphic.keyGraphicState"
          :avbSelectedDies="avbSelectedDies"
          :divider="selectedDivider"
          :statisticKf="statisticKf"
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
import MiniReport from "./wafermeas-report.vue"
import AmChart from "./chart-lnr.vue"
import ChartLNR from "./chart-lnr-cjs.vue";
import ChartHSTG from "./chart-bar-cjs.vue";
import StatSingle from "./stat-single.vue";
import Loading from "vue-loading-overlay";
import WaferMap from "./wafermap-svg.vue";
import MiniGraphicRow from "./wafermeas-minigraphicrow";

export default {
  data() {
    return {
      mapMode: "selected",
      fabToTop: false,
      fabToNext: false,
      fabToPrev: false,
      loading: false,
      showUnSelectedGraphics: false,
      activeTab: "wafer",
      wafers: [],
      dividers: [],
      avbSelectedDies: [],
      selectedWafer: "",
      selectedDivider: "1.0",
      selectedMeasurementId: 0,
      statisticKf: 1.5
    };
  },

  components: {
    "mini-report": MiniReport,
    "micro-row": MiniGraphicRow,
    "stat-single": StatSingle,
    "wafermap-svg": WaferMap,
    "amchart": AmChart,
    "chart-lnr": ChartLNR,
    "chart-hstg": ChartHSTG,
    Loading
  },

  async created() {
    this.wafers = (await this.$http.get(`/api/wafer/all`)).data
    this.dividers = (await this.$http.get(`/api/divider/all`)).data
    this.routeHandler(this.$route.name)
  },

  computed: {

    dirtyCells() {
      return this.$store.getters['wafermeas/dirtyCells']
    },

    selectedDies() {
      return this.$store.getters['wafermeas/selectedDies']
    },

    selectedGraphics() {
      return this.$store.getters['wafermeas/selectedGraphics']      
    },

    unSelectedGraphics() {
      return this.$store.getters['wafermeas/unSelectedGraphics']  
    },

    availiableGraphics() {
      return this.$store.getters['wafermeas/avbGraphics']
    },
    measurementRecordings() {
      return this.$store.getters['wafermeas/measurements']
    },

    selectedGraphicsIcon() {
      if (this.availiableGraphics.length === this.selectedGraphics.length)
        return "check_box";
      if (this.selectedGraphics.length > 0) return "indeterminate_check_box";
        return "check_box_outline_blank";
    }
  },

  methods: {

    onScroll(e) {
      if (typeof window === 'undefined') return
      const top = window.pageYOffset || e.target.scrollTop || 0
      this.fabToTop = top > 20
    },

    toTop () {
      this.$vuetify.goTo(0)
    },

    routeHandler: function(routeName) {
      if(routeName === "wafermeasurement-onlywafer") {
        this.selectedWafer = this.$route.params.waferId
      }
      if(routeName === "wafermeasurement-fullselected") {
        this.selectedWafer = this.$route.params.waferId
        this.selectedMeasurementId = this.measurementRecordings.find(x => x.name === "оп." + this.$route.params.measurementName).id
      }
    },

    delDirtyCells: function(dirtyCellsList, selectedDies) {
      this.$store.dispatch("wafermeas/updateSelectedDies", selectedDies.filter(die => !dirtyCellsList.includes(die)))
    },

    selectAllDies: function(avbSelectedDies) {
      this.$store.dispatch("wafermeas/updateSelectedDies", [...avbSelectedDies])
    },

    selectAllGraphics: function() {
      this.$nextTick(() => {
        if (this.selectedGraphics.length !== this.availiableGraphics.length) {
          this.$store.dispatch("wafermeas/updateSelectedGraphics", [...this.availiableGraphics.map(g => g.keyGraphicState)])
          if(this.unSelectedGraphics.length > 0) {
            this.$store.dispatch("wafermeas/addToDirtyCellsStat", {keyGraphicState: this.unSelectedGraphics.map(g => g.keyGraphicState), avbSelectedDies: this.avbSelectedDies})
          }
        }
      });
    }
  },

  watch: {

    selectedWafer: async function(newValue) {
      this.$store.dispatch("wafermeas/updateAvbGraphics", [])
      this.$store.dispatch("wafermeas/updateDieColors", {ctx: this, waferId: newValue})
      this.$store.dispatch("wafermeas/updateSelectedWaferId", {ctx: this, waferId: newValue})
    },

    selectedMeasurementId: async function(newValue) {
      this.loading = true
      this.$store.dispatch("wafermeas/updateSelectedDies", [])
      this.$store.dispatch("wafermeas/clearSelectedGraphics")
      let dieValues = (await this.$http.get(`/api/dievalue/GetByMeasurementRecordingId?measurementRecordingId=${newValue}`)).data
      let keyGraphicStateJSON = JSON.stringify(Object.keys(dieValues))
      let availiableGraphics = (await this.$http.get(`/api/graphicsrv6/GetAvailiableGraphicsByKeyGraphicStateList?keyGraphicStateJSON=${keyGraphicStateJSON}`)).data
      this.$store.dispatch("wafermeas/updateAvbGraphics", availiableGraphics)
      let diesList = (await this.$http.get(`/api/dievalue/GetSelectedDiesByMeasurementRecordingId?measurementRecordingId=${newValue}`)).data
      this.avbSelectedDies = [...diesList]
      this.$store.dispatch("wafermeas/updateDirtyCells", (await this.$http.get(`/api/statistic/GetDirtyCellsByMeasurementRecording?measurementRecordingId=${newValue}&&diesCount=${this.avbSelectedDies.length}&&k=${this.statisticKf}`)).data)
      this.$store.dispatch("wafermeas/updateSelectedDies", diesList)
      this.selectAllGraphics() 
      this.delDirtyCells(this.dirtyCells.statList, this.avbSelectedDies)
      this.loading = false
      this.activeTab = "statistics"
      await this.$router.push({ name: 'wafermeasurement-fullselected', params: { waferId: this.selectedWafer, measurementName: this.measurementRecordings.find(x => x.id === newValue).name}});
    },

    statisticKf: async function(k) {
      this.loading = true
      this.$store.dispatch("wafermeas/updateDirtyCells", (await this.$http.get(`/api/statistic/GetDirtyCellsByMeasurementRecording?measurementRecordingId=${this.selectedMeasurementId}&&diesCount=${this.avbSelectedDies.length}&&k=${k}`)).data)   
      this.delDirtyCells(this.dirtyCells.statList, this.avbSelectedDies)
      this.loading = false
    },

    availiableGraphics: function() {
      if (this.availiableGraphics.length === 0) {
        this.$store.dispatch("wafermeas/clearSelectedGraphics")
      }
    },

    selectedDies: function(newValue) {
      if(newValue.length > 0) {
        let {statList, fixedList} = this.dirtyCells
        let statPercentageSelected = Math.ceil((1.0 - newValue.filter(value => statList.includes(value)).length / newValue.length) * 100)
        let fixedPercentageSelected = Math.ceil((1.0 - newValue.filter(value => fixedList.includes(value)).length / newValue.length) * 100)
        this.$store.dispatch("wafermeas/updateDirtyCellsPercentageSelected", {statPercentageSelected, fixedPercentageSelected})
      }      
    }
  },
  
  beforeDestroy() {
    this.$store.dispatch("wafermeas/reset")
  }
};
</script>

<style>
  .ps {
    height: 400px;
  }
</style>
