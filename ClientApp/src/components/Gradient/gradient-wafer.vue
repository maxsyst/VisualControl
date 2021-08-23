<template>
    <v-container>
      <v-row>
        <v-col class="d-flex flex-column align-center">
          <svg :style="svgRotation" :height="size.fieldHeight" :width="size.fieldWidth" :viewBox="fieldViewBox">
            <g v-for="(die, key) in dies" :key="die.id">
              <rect :dieIndex="key" :id="die.id" :x="die.x" :y="die.y" :width="die.width" :height="die.height" :fill="die.fill"/>
            </g>
          </svg>
        </v-col>
      </v-row>
    </v-container>
</template>

<script>
import { mapGetters } from 'vuex';

export default {
  props: ['gradientSteps'],
  data() {
    return {
      dies: [],
      activeBtn: 1,
      showNav: false,
      x: 0,
      y: 0,
      initialOrientation: -1,
      currentOrientation: -1,
    };
  },

  mounted() {
    this.dies = _.cloneDeep(this.wafer.formedMapGradient.dies);
    this.initialOrientation = +this.wafer.formedMapGradient.orientation;
    this.currentOrientation = this.initialOrientation;
    this.initialize(this.dies);
    if (this.gradientSteps) this.goToInitial(this.selectedDies, this.gradientSteps);
  },

  methods: {

    initialize(dies) {
      dies.forEach((die) => {
        die.fill = '#A1887F';
        die.isActive = false;
        die.fillOpacity = 1.0;
      });
    },

    goToInitial(selectedDies, gradientSteps) {
      this.avbSelectedDies.forEach((avb) => {
        const die = this.dies.find((d) => d.id === avb);
        const step = gradientSteps.find((g) => g.dieList.includes(die.id));
        const isSelected = selectedDies.includes(die.id);
        die.fill = isSelected ? step.color : '#303030';
        die.fillOpacity = 1.0;
        die.isActive = false;
      });
    },
  },

  watch:
    {
      fieldWidth: {
        immediate: true,
        handler(newValue) {
          this.fieldViewBox;
        },
      },

      gradientSteps(newVal) {
        this.goToInitial(this.selectedDies, newVal);
      },

      selectedDies(selectedDies) {
        this.goToInitial(selectedDies, this.gradientSteps);
      },
    },

  computed:
    {
      ...mapGetters({
        selectedDies: 'wafermeas/selectedDies',
        wafer: 'wafermeas/wafer',
        sizeGetter: 'wafermeas/size',
        avbSelectedDies: 'wafermeas/avbSelectedDies',
      }),

      fieldViewBox() {
        return `0 0 ${this.size.fieldHeight} ${this.size.fieldWidth}`;
      },

      size() {
        return this.sizeGetter('gradient');
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
