<template>
    <v-container>
      <v-row class="d-flex flex-row">
        <v-col class="d-flex flex-column align-end">
          <svg :style="svgRotation" :height="size.fieldHeight" :width="size.fieldWidth" :viewBox="fieldViewBox">
            <g v-for="(die, key) in dies" :key="die.id">
              <rect :dieIndex="key" :id="die.id"
                    :x="die.x" :y="die.y"
                    :width="die.width" :height="die.height"
                    :fill="die.fill" :fill-opacity="die.fillOpacity" @click="selectDieWithTimer"  @contextmenu="selectDie"/>
            </g>
          </svg>
        </v-col>
        <v-col>
            <v-progress-circular
              v-if="updateCircular.value > 0"
              :value="updateCircular.value"
              color="primary">
          </v-progress-circular>
        </v-col>
      </v-row>
    </v-container>
</template>

<script>
import { mapGetters } from 'vuex';

export default {
  props: ['keyGraphicState', 'statParameter'],
  data() {
    return {
      capSelectedDies: [],
      updateTimer: null,
      updateCircular: {
        value: 0,
        interval: {},
      },
      dies: [],
      isloading: false,
      activeBtn: 1,
      showNav: false,
      x: 0,
      y: 0,
      initialOrientation: -1,
      currentOrientation: -1,
      wafer: {},
    };
  },

  mounted() {
    const wafer = this.$store.getters['wafermeas/wafer'];
    this.dies = _.cloneDeep(wafer.formedMapGradient.dies);
    this.initialOrientation = +wafer.formedMapGradient.orientation;
    this.currentOrientation = this.initialOrientation;
    this.initialize(this.dies);
    this.goToInitial(this.selectedDies);
  },

  beforeDestroy() {
    this.dies = null;
    this.wafer = null;
  },

  methods: {

    initialize() {
      this.dies = this.dies.map((die) => ({
        ...die, fill: '#A1887F', isActive: false, fillOpacity: 1.6,
      }));
    },

    selectDieWithTimer(e) {
      e.preventDefault();
      const die = this.dies[+e.currentTarget.attributes.dieIndex.value];
      const dieId = die.id;
      if (die.isActive) {
        if (this.updateTimer) {
          clearInterval(this.updateCircular.interval);
          this.updateCircular.value = 0.1;
          clearTimeout(this.updateTimer);
        } else {
          this.capSelectedDies = [...this.selectedDies];
        }
        const position = this.capSelectedDies.indexOf(dieId);
        // eslint-disable-next-line no-bitwise
        if (~position) {
          this.capSelectedDies.splice(position, 1);
          die.fill = this.dieFillStrategy(die, this.mode, new Set([...this.capSelectedDies]), this.$store.getters['wafermeas/dieColors']);
          this.updateTimer = setTimeout(this.updateSelectedDies, 2000, this.capSelectedDies);
          this.updateCircular.interval = setInterval(() => {
            this.updateCircular.value += 25;
          }, 250);
        } else {
          this.capSelectedDies.push(dieId);
          die.fill = this.dieFillStrategy(die, this.mode, new Set([...this.capSelectedDies]), this.$store.getters['wafermeas/dieColors']);
          this.updateTimer = setTimeout(this.updateSelectedDies, 2000, this.capSelectedDies);
          this.updateCircular.interval = setInterval(() => {
            this.updateCircular.value += 25;
          }, 250);
        }
      }
    },

    selectDie(e) {
      e.preventDefault();
      const die = this.dies[+e.currentTarget.attributes.dieIndex.value];
      const dieId = die.id;
      if (die.isActive) {
        this.capSelectedDies = [...this.selectedDies];
        const position = this.capSelectedDies.indexOf(dieId);
        // eslint-disable-next-line no-bitwise
        if (~position) {
          this.capSelectedDies.splice(position, 1);
          die.fill = this.dieFillStrategy(die, this.mode, new Set([...this.capSelectedDies]), this.$store.getters['wafermeas/dieColors']);
          this.updateSelectedDies([...this.capSelectedDies]);
        } else {
          this.capSelectedDies.push(dieId);
          die.fill = this.dieFillStrategy(die, this.mode, new Set([...this.capSelectedDies]), this.$store.getters['wafermeas/dieColors']);
          this.updateSelectedDies([...this.capSelectedDies]);
        }
      }
    },

    updateSelectedDies(selectedDies) {
      clearInterval(this.updateCircular.interval);
      this.updateCircular.value = 100;
      this.$store.dispatch('wafermeas/updateSelectedDies', selectedDies);
      this.updateCircular.value = 0;
    },

    goToInitial(selectedDies) {
      const selectedDiesSet = new Set([...selectedDies]);
      this.$store.dispatch('wafermeas/changeKeyGraphicStateMode', { keyGraphicState: this.keyGraphicState, mode: 'initial' });
      const avbSelectedDies = this.$store.getters['wafermeas/avbSelectedDies'];
      avbSelectedDies.forEach((avb) => {
        const die = this.dies.find((d) => d.id === avb);
        die.fill = this.dieFillStrategy(die, 'initial', selectedDiesSet);
        die.fillOpacity = 1.0;
        die.isActive = true;
      });
    },

    dieFillStrategy(die, mode, selectedDiesSet) {
      if (mode === 'initial' || mode === 'dirty') {
        const isDirtyCellsSnapshotBadDiesHas = this.dirtyCellsSnapshotBadDies.has(die.id);
        const selectedDiesSetHas = selectedDiesSet.has(die.id);
        if (isDirtyCellsSnapshotBadDiesHas) {
          if (selectedDiesSetHas) {
            return '#F50057';
          }

          return '#580000';
        }
        if (selectedDiesSetHas) {
          return '#00E676';
        }
        return '#1B5E20';
      }
      return '#424242';
    },

  },

  watch: {

    dirtyCellsSnapshotBadDies() {
      this.goToInitial(this.selectedDies);
    },

    selectedDies(selectedDies) {
      this.goToInitial(selectedDies);
    },
  },

  computed: {
    ...mapGetters({
      selectedDies: 'wafermeas/selectedDies',
      sizeGetter: 'wafermeas/size',
    }),

    dirtyCellsSnapshotBadDies() {
      return new Set([...this.$store.getters['wafermeas/getDirtyCellsSnapshotByKeyGraphicState'](this.keyGraphicState)
        .statNameDirtyCellsDictionary[this.statParameter.statisticsName].badDirtyCells]);
    },

    size() {
      return this.sizeGetter('gradient');
    },

    fieldViewBox() {
      return `0 0 ${this.size.fieldHeight} ${this.size.fieldWidth}`;
    },

    svgRotation() {
      return {
        transform: `rotate(${this.currentOrientation - this.initialOrientation}deg)`,
      };
    },
  },
};
</script>

<style scoped>
  rect:hover {
    stroke: #fc0;
    stroke-width: 1.2;
  }
</style>
