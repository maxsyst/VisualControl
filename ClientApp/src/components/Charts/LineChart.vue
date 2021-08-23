<script>
import { Line } from 'vue-chartjs';

export default {
  extends: Line,
  data() {
    return {
      oldHovered: { color: '', dieId: 0 },
    };
  },
  props: {
    chartdata: {
      type: Object,
      default: null,
    },
    options: {
      type: Object,
      default: null,
    },
    keyGraphicState: {
      type: String,
      default: '',
    },
    settings: {
      type: Object,
      default: null,
    },
  },

  mounted() {
    this.renderChart(this.chartdata, this.options);
  },

  methods: {
    getChartDataFromStore(selectedDies) {
      const datasets = [];
      selectedDies.forEach((dieId) => {
        const singleDataset = {
          dieId,
          borderColor: this.mode === 'dirty' ? this.dirtyCells.fullWafer.cells.includes(dieId) ? '#ff1744' : '#00e676'
            : this.dieColors.find((dc) => dc.dieId === dieId).hexColor,
          data: [...this.dieValues.find((dv) => dv.d === dieId).y.map((x) => +x)],
          fill: false,
          borderWidth: 1,
          pointHoverRadius: 0,
          pointRadius: 0,
        };
        datasets.push(singleDataset);
      });
      this.chartdata.datasets = [...datasets];
    },

    recalculateXAxisOptions(xAxis) {
      if (xAxis.current.min === 'Авто') {
        delete this.options.scales.xAxes[0].ticks.min;
      } else {
        this.options.scales.xAxes[0].ticks.min = +xAxis.current.min;
      }

      if (xAxis.current.max === 'Авто') {
        delete this.options.scales.xAxes[0].ticks.max;
      } else {
        this.options.scales.xAxes[0].ticks.max = +xAxis.current.max;
      }
      this.options.scales.xAxes[0].ticks.maxTicksLimit = +xAxis.current.maxTicksLimit;
    },

    recalculateYAxisOptions(yAxis) {
      if (yAxis.current.min === 'Авто') {
        delete this.options.scales.yAxes[0].ticks.min;
      } else {
        this.options.scales.yAxes[0].ticks.min = +yAxis.current.min;
      }

      if (yAxis.current.max === 'Авто') {
        delete this.options.scales.yAxes[0].ticks.max;
      } else {
        this.options.scales.yAxes[0].ticks.max = +yAxis.current.max;
      }
      this.options.scales.yAxes[0].ticks.maxTicksLimit = +yAxis.current.maxTicksLimit;
    },

    resetZoom() {
      this.$data._chart.resetZoom();
    },

    resetHighligted() {
      if (this.oldHovered.dieId > 0) {
        const oldHovered = this.chartdata.datasets.find((x) => x.dieId === this.oldHovered.dieId);
        oldHovered.borderWidth = 1;
      }
    },
  },

  watch: {

    settings(newValue) {
      this.recalculateXAxisOptions(newValue.xAxis);
      this.recalculateYAxisOptions(newValue.yAxis);
      this.renderChart(this.chartdata, this.options);
    },

    hovered(newValue) {
      if (newValue.keyGraphicState === this.keyGraphicState) {
        if (this.oldHovered.dieId > 0) {
          const oldHovered = this.chartdata.datasets.find((x) => x.dieId === this.oldHovered.dieId);
          oldHovered.borderWidth = 1;
        }
        const highligted = this.chartdata.datasets.find((x) => x.dieId === newValue.dieId);
        this.oldHovered.dieId = newValue.dieId;
        this.oldHovered.color = highligted.borderColor;
        highligted.borderWidth = 7;
        this.renderChart(this.chartdata, this.options);
      }
    },

    mode(newValue) {
      if (newValue === 'dirty') {
        this.chartdata.datasets.forEach((d) => d.borderColor = this.dirtyCells.fullWafer.cells.includes(d.dieId) ? '#ff1744' : '#00e676');
      } else {
        this.chartdata.datasets.forEach((d) => d.borderColor = this.dieColors.find((dc) => dc.dieId === d.dieId).hexColor);
      }
      this.renderChart(this.chartdata, this.options);
    },

    log(newValue) {
      if (newValue) {
        this.options.scales.yAxes[0].type = 'logarithmic';
      } else {
        this.options.scales.yAxes[0].type = 'linear';
      }
      this.renderChart(this.chartdata, this.options);
    },

    selectedDies(newValue) {
      this.getChartDataFromStore(newValue);
      this.renderChart(this.chartdata, this.options);
    },

  },

  computed: {

    selectedDies() {
      return this.$store.getters['wafermeas/selectedDies'];
    },

    dieValues() {
      return this.$store.getters['wafermeas/getDieValuesByKeyGraphicState'](this.keyGraphicState);
    },

    hovered() {
      return this.$store.getters['wafermeas/hoveredDieId'];
    },

    mode() {
      return this.$store.getters['wafermeas/getKeyGraphicStateMode'](this.keyGraphicState);
    },

    log() {
      return this.$store.getters['wafermeas/getKeyGraphicStateLog'](this.keyGraphicState);
    },

    dieColors() {
      return this.$store.getters['wafermeas/dieColors'];
    },

    dirtyCells() {
      return this.$store.getters['wafermeas/getDirtyCellsByGraphic'](this.keyGraphicState);
    },
  },
};
</script>
