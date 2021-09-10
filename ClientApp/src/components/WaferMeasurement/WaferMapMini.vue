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
        <v-col class="d-flex flex-column align-center">
            <v-btn :color="mode === 'initial' ? 'indigo' : 'grey darken-2'" class="mt-auto" fab x-small dark @click="goToInitial()">
              Стд
            </v-btn>
            <v-btn :color="mode === 'selected' ? 'indigo' : 'grey darken-2'" class="mt-auto" fab x-small dark @click="goToSelected()">
              Вбр
            </v-btn>
            <v-btn :color="mode === 'dirty' ? 'indigo' : 'grey darken-2'" class="mt-auto" fab x-small dark @click="goToDirty()">
              Гдн
            </v-btn>
            <v-btn :color="mode === 'color' ? 'indigo' : 'grey darken-2'" class="mt-auto" fab x-small dark @click="goToColor()">
              Цвт
            </v-btn>
            <v-btn v-if="mode === 'gradient'" :color="mode === 'gradient' ? 'indigo' : 'grey darken-2'"  class="mt-auto" fab x-small dark>
              Грд
            </v-btn>
        </v-col>
      </v-row>
    </v-container>
</template>

<script>
import { mapGetters } from 'vuex';

export default {
  props: ['keyGraphicState', 'rowViewMode'],
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
    this.wafer = this.$store.getters['wafermeas/wafer'];
    this.dies = _.cloneDeep(this.wafer.formedMapMini.dies);
    this.initialOrientation = +this.wafer.formedMapMini.orientation;
    this.currentOrientation = this.initialOrientation;
    this.initialize();
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

    goToInitial() {
      const selectedDiesSet = new Set([...this.selectedDies]);
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

    goToDirty() {
      const selectedDiesSet = new Set([...this.selectedDies]);
      this.$store.dispatch('wafermeas/changeKeyGraphicStateMode', { keyGraphicState: this.keyGraphicState, mode: 'dirty' });
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

    goToSelected() {
      const selectedDiesSet = new Set([...this.selectedDies]);
      this.$store.dispatch('wafermeas/changeKeyGraphicStateMode', { keyGraphicState: this.keyGraphicState, mode: 'selected' });
      const avbSelectedDies = this.$store.getters['wafermeas/avbSelectedDies'];
      avbSelectedDies.forEach((avb) => {
        const die = this.dies.find((d) => d.id === avb);
        die.fillOpacity = 1.0;
        die.fill = selectedDiesSet.has(die.id) ? '#3D5AFE' : '#8C9EFF';
        die.isActive = true;
      });
    },

    goToColor() {
      const selectedDiesSet = new Set([...this.selectedDies]);
      this.$store.dispatch('wafermeas/changeKeyGraphicStateMode', { keyGraphicState: this.keyGraphicState, mode: 'color' });
      const dieColors = this.$store.getters['wafermeas/dieColors'];
      const avbSelectedDies = this.$store.getters['wafermeas/avbSelectedDies'];
      avbSelectedDies.forEach((avb) => {
        const die = this.dies.find((d) => d.id === avb);
        const isSelected = selectedDiesSet.has(die.id);
        die.fillOpacity = 1.0;
        die.fill = isSelected ? dieColors.get(die.id) : '#424242';
        die.isActive = true;
      });
    },

    refresh() {
      if (this.mode === 'initial') {
        this.goToInitial();
      }

      if (this.mode === 'dirty') {
        this.goToDirty();
      }

      if (this.mode === 'selected') {
        this.goToSelected();
      }

      if (this.mode === 'color') {
        this.goToColor();
      }
    },
  },

  watch: {

    dirtyCellsSnapshotBadDies() {
      this.refresh();
    },

    rowViewMode(rowViewMode) {
      if (rowViewMode === 'bigChart') {
        this.dies = this.dies.map(function f(die) {
          const gDie = this.wafer.formedMapGradient.find((d) => d.dieId === die.Id);
          return ({
            ...die, x: gDie.x, y: gDie.y, width: gDie.width, height: gDie.height,
          });
        });
      }
      if (rowViewMode === 'miniChart') {
        this.dies = this.dies.map(function f(die) {
          const gDie = this.wafer.formedMapGradient.find((d) => d.dieId === die.Id);
          return ({
            ...die, x: gDie.x, y: gDie.y, width: gDie.width, height: gDie.height,
          });
        });
      }
    },

    selectedDies() {
      this.refresh();
    },
  },

  computed: {
    ...mapGetters({
      selectedDies: 'wafermeas/selectedDies',
      sizeGetter: 'wafermeas/size',
      modeGetter: 'wafermeas/getKeyGraphicStateMode',
    }),

    dirtyCellsSnapshotBadDies() {
      return new Set([...this.$store.getters['wafermeas/getDirtyCellsSnapshotBadDiesByKeyGraphicState'](this.keyGraphicState)]);
    },

    mode() {
      return this.modeGetter(this.keyGraphicState);
    },

    fieldViewBox() {
      return `0 0 ${this.size.fieldHeight} ${this.size.fieldWidth}`;
    },

    size() {
      return this.rowViewMode === 'bigChart' ? this.sizeGetter('gradient') : this.sizeGetter('mini');
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
