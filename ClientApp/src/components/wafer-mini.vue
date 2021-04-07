<template>
    <v-container>
      <v-row class="d-flex flex-row">
        <v-col class="d-flex flex-column align-end">
          <svg :style="svgRotation" :height="size.fieldHeight" :width="size.fieldWidth" :viewBox="fieldViewBox">
            <g v-for="(die, key) in dies" :key="die.id">
              <rect :dieIndex="key" :id="die.id" :x="die.x" :y="die.y" :width="die.width" :height="die.height" :fill="die.fill" :fill-opacity="die.fillOpacity" @mouseover="mouseOver" @click="selectDie"/>
            </g>
          </svg>
        </v-col>
        <v-col class="d-flex flex-column align-center">
            <v-btn :color="mode === 'initial' ? 'indigo' : 'grey darken-2'" class="mt-auto" fab x-small dark @click="goToInitial(selectedDies)">
              Стд
            </v-btn>
            <v-btn :color="mode === 'selected' ? 'indigo' : 'grey darken-2'" class="mt-auto" fab x-small dark @click="goToSelected(selectedDies)">
              Вбр
            </v-btn>
            <v-btn :color="mode === 'dirty' ? 'indigo' : 'grey darken-2'" class="mt-auto" fab x-small dark @click="goToDirty(selectedDies)">
              Гдн
            </v-btn>
            <v-btn :color="mode === 'color' ? 'indigo' : 'grey darken-2'" class="mt-auto" fab x-small dark @click="goToColor(selectedDies)">
              Цвт
            </v-btn>
        </v-col>
      </v-row>
    </v-container>
</template>

<script>
import Loading from 'vue-loading-overlay';
import { mapGetters } from 'vuex';

export default {
  props: ['keyGraphicState', 'viewMode', 'rowViewMode'],
  components: { Loading },
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
      fieldViewBox: '',
    };
  },

  mounted() {
    this.dies = _.cloneDeep(this.wafer.formedMapMini.dies);
    this.initialOrientation = +this.wafer.formedMapMini.orientation;
    this.currentOrientation = this.initialOrientation;
    this.initialize(this.dies);
    this.goToInitial(this.selectedDies);
  },

  methods: {

    initialize(dies) {
      dies.forEach((die) => {
        die.fill = '#A1887F';
        die.isActive = false;
        die.fillOpacity = 1.0;
      });
    },

    mouseOver(e) {
      if (this.selectedDies.includes(+e.target.id) && this.mode === 'color') {
        this.$store.dispatch('wafermeas/hoverWaferMini', { dieId: +e.target.id, keyGraphicState: this.keyGraphicState });
      }
    },

    mouseLeave() {
      if (this.mode === 'color') {
        this.$store.dispatch('wafermeas/unHoverWaferMini');
      }
    },

    selectDie(e) {
      e.preventDefault();
      const die = this.dies[+e.currentTarget.attributes.dieIndex.value];
      const dieId = die.id;
      if (die.isActive) {
        const position = this.selectedDies.indexOf(dieId);
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
      this.$store.dispatch('wafermeas/changeKeyGraphicStateMode', { keyGraphicState: this.keyGraphicState, mode: 'initial' });
      this.avbSelectedDies.forEach((avb) => {
        const die = this.dies.find((d) => d.id === avb);
        die.fill = this.dirtyCells.fullWafer.cells.includes(die.id)
          ? selectedDies.includes(die.id) ? '#F50057' : '#580000'
          : selectedDies.includes(die.id) ? '#00E676' : '#1B5E20';
        die.fillOpacity = 1.0;
        die.isActive = true;
      });
    },

    goToDirty(selectedDies) {
      this.$store.dispatch('wafermeas/changeKeyGraphicStateMode', { keyGraphicState: this.keyGraphicState, mode: 'dirty' });
      this.avbSelectedDies.forEach((avb) => {
        const die = this.dies.find((d) => d.id === avb);
        die.fill = this.dirtyCells.fullWafer.cells.includes(die.id)
          ? selectedDies.includes(die.id) ? '#F50057' : '#580000'
          : selectedDies.includes(die.id) ? '#00E676' : '#1B5E20';
        die.fillOpacity = 1.0;
        die.isActive = true;
      });
    },

    goToSelected(selectedDies) {
      this.$store.dispatch('wafermeas/changeKeyGraphicStateMode', { keyGraphicState: this.keyGraphicState, mode: 'selected' });
      this.avbSelectedDies.forEach((avb) => {
        const die = this.dies.find((d) => d.id === avb);
        die.fillOpacity = 1.0;
        die.fill = selectedDies.includes(die.id) ? '#3D5AFE' : '#8C9EFF';
        die.isActive = true;
      });
    },

    goToColor(selectedDies) {
      this.$store.dispatch('wafermeas/changeKeyGraphicStateMode', { keyGraphicState: this.keyGraphicState, mode: 'color' });
      this.avbSelectedDies.forEach((avb) => {
        const die = this.dies.find((d) => d.id === avb);
        const isSelected = selectedDies.includes(die.id);
        die.fillOpacity = 1.0;
        die.fill = isSelected ? this.dieColors.find((d) => d.dieId === die.id).hexColor : '#424242';
        die.isActive = true;
      });
    },

    refresh(selectedDies) {
      if (this.mode === 'initial') {
        this.goToInitial(selectedDies);
      }

      if (this.mode === 'dirty') {
        this.goToDirty(selectedDies);
      }

      if (this.mode === 'selected') {
        this.goToSelected(selectedDies);
      }

      if (this.mode === 'color') {
        this.goToColor(selectedDies);
      }
    },
  },

  watch: {
    fieldWidth: {
      immediate: true,
      handler(newVal, oldVal) {
        this.fieldViewBox = `0 0 ${this.size.fieldHeight} ${this.size.fieldWidth}`;
      },
    },

    viewMode() {
      this.refresh(this.selectedDies);
    },

    rowViewMode(rowViewMode) {
      if (rowViewMode === 'bigChart') {
        this.dies.forEach(function (die) {
          const gDie = this.wafer.formedMapGradient.find((d) => d.dieId === die.Id);
          die.x = gDie.x;
          die.y = gDie.y;
          die.width = gDie.width;
          die.height = gDie.height;
        });
      }
      if (rowViewMode === 'miniChart') {
        this.dies.forEach(function (die) {
          const mDie = this.wafer.formedMapMini.find((d) => d.dieId === die.Id);
          die.x = mDie.x;
          die.y = mDie.y;
          die.width = mDie.width;
          die.height = mDie.height;
        });
      }
    },

    selectedDies(selectedDies) {
      this.refresh(selectedDies);
    },
  },

  computed: {
    ...mapGetters({
      wafer: 'wafermeas/wafer',
      selectedDies: 'wafermeas/selectedDies',
      avbSelectedDies: 'wafermeas/avbSelectedDies',
      sizeGetter: 'wafermeas/size',
      dieColors: 'wafermeas/dieColors',
      modeGetter: 'wafermeas/getKeyGraphicStateMode',
      dirtyCellsGetter: 'wafermeas/getDirtyCellsByGraphic',
    }),

    dirtyCells() {
      return this.dirtyCellsGetter(this.keyGraphicState, this.viewMode);
    },

    mode() {
      return this.modeGetter(this.keyGraphicState);
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
