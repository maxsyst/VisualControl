<template>
    <v-container>
      <v-row class="d-flex flex-row">
        <v-col class="d-flex flex-column align-end">
          <svg :style="svgRotation" :height="size.fieldHeight" :width="size.fieldWidth" :viewBox="fieldViewBox">
            <g v-for="(die, key) in dies" :key="die.id">
              <rect :dieIndex="key" :id="die.id"
                    :x="die.x" :y="die.y"
                    :width="die.width" :height="die.height"
                    :fill="die.fill" :fill-opacity="die.fillOpacity" @click="selectDie"/>
            </g>
          </svg>
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

    selectDie(e) {
      e.preventDefault();
      const die = this.dies[+e.currentTarget.attributes.dieIndex.value];
      const dieId = die.id;
      if (die.isActive) {
        const position = this.selectedDies.indexOf(dieId);
        // eslint-disable-next-line no-bitwise
        if (~position) {
          this.selectedDies.splice(position, 1);
          this.$store.dispatch('wafermeas/updateSelectedDies', this.selectedDies);
        } else {
          this.selectedDies.push(dieId);
          this.$store.dispatch('wafermeas/updateSelectedDies', this.selectedDies);
        }
      }
    },

    goToInitial(selectedDies) {
      const selectedDiesSet = new Set([...selectedDies]);
      this.$store.dispatch('wafermeas/changeKeyGraphicStateMode', { keyGraphicState: this.keyGraphicState, mode: 'initial' });
      const avbSelectedDies = this.$store.getters['wafermeas/avbSelectedDies'];
      avbSelectedDies.forEach((avb) => {
        const die = this.dies.find((d) => d.id === avb);
        // eslint-disable-next-line no-nested-ternary
        die.fill = this.dirtyCellsSnapshotBadDies.has(die.id)
          ? selectedDiesSet.has(die.id) ? '#F50057' : '#580000'
          : selectedDiesSet.has(die.id) ? '#00E676' : '#1B5E20';
        die.fillOpacity = 1.0;
        die.isActive = true;
      });
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
