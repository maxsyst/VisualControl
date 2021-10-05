<script>
import { Bar } from 'vue-chartjs';
import { mapGetters } from 'vuex';

export default {
  extends: Bar,
  props: {
    divider: {
      type: String,
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
  },

  data() {
    return {
      chartdata: {},
    };
  },

  beforeDestroy() {
    this.chartdata = null;
  },

  mounted() {
    this.getChartDataFromStore(this.selectedDies);
    this.renderChart(this.chartdata, this.options);
  },

  methods: {
    getChartDataFromStore(selectedDies) {
      let datasets = [];
      const badDies = new Set([...this.$store.getters['wafermeas/getDirtyCellsSnapshotBadDiesByKeyGraphicState'](this.keyGraphicState)]);
      const dieColors = this.$store.getters['wafermeas/dieColors'];
      this.chartdata = _.cloneDeep(this.$store.getters['wafermeas/getChartsDataByKeyGraphicState'](this.keyGraphicState));
      selectedDies.forEach((dieId) => {
        const singleDataset = {
          dieId,
          backgroundColor: this.mode === 'dirty'
            ? badDies.has(dieId) ? '#ff1744' : '#00e676'
            : this.mode === 'color' ? dieColors.get(dieId) : '#3D5AFE',
          data: this.chartdata.datasets[1].data[dieId],
          label: this.wafer.formedMapMini.dies.find((d) => d.id === dieId).code,
        };
        datasets.push(singleDataset);
      });
      datasets = [..._.sortBy(datasets, [function f1(o) { return +o.label.split('-')[0]; }])];
      this.chartdata.labels = datasets.map((d) => d.label);
      this.chartdata.datasets = [];
      this.chartdata.datasets[0] = {
        backgroundColor: [...datasets.map((x) => x.backgroundColor)],
        dieIdList: [...datasets.map((x) => x.dieId)],
        data: [...datasets.map((x) => x.data)],
        fill: false,
        borderWidth: 1,
        pointHoverRadius: 0,
        pointRadius: 0,
      };
    },
  },

  watch: {
    mode(mode) {
      if (mode === 'dirty') {
        const badDies = new Set([...this.$store.getters['wafermeas/getDirtyCellsSnapshotBadDiesByKeyGraphicState'](this.keyGraphicState)]);
        this.chartdata.datasets[0].dieIdList.forEach((d, index) => this.chartdata.datasets[0].backgroundColor[index] = badDies.has(d) ? '#ff1744' : '#00e676');
      }
      if (mode === 'color') {
        const dieColors = this.$store.getters['wafermeas/dieColors'];
        this.chartdata.datasets[0].dieIdList.forEach((d, index) => this.chartdata.datasets[0].backgroundColor[index] = dieColors.get(d));
      }
      if (mode === 'gradient') {
        const gradientData = this.$store.getters['wafermeas/getGradientDataByKeyGraphicState'](this.keyGraphicState);
        this.chartdata.datasets[0].dieIdList.forEach((d, index) => this.chartdata.datasets[0].backgroundColor[index] = gradientData.gradientSteps.find((g) => g.dieList.includes(d)).color);
      }
      if (mode === 'selected' || mode === 'initial') {
        this.chartdata.datasets[0].backgroundColor = this.chartdata.datasets[0].backgroundColor.map((x) => '#3D5AFE');
      }
      this.renderChart(this.chartdata, this.options);
    },

    selectedDies(newValue) {
      this.getChartDataFromStore(newValue);
      this.renderChart(this.chartdata, this.options);
    },
  },

  computed: {

    ...mapGetters({
      wafer: 'wafermeas/wafer',
      selectedDies: 'wafermeas/selectedDies',
      modeGetter: 'wafermeas/getKeyGraphicStateMode',
    }),

    mode() {
      return this.modeGetter(this.keyGraphicState);
    },

  },
};
</script>
