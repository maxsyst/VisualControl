<script>
import { Bar } from 'vue-chartjs';

export default {
  extends: Bar,
  props: {

    chartdata: {
      type: Object,
      default: null,
    },

    options: {
      type: Object,
      default: null,
    },

    height: {
      type: Number,
      default: 350,
    },

    width: {
      type: Number,
      default: 475,
    },
  },

  mounted() {
    this.renderChart(this.chartdata, this.options);
  },

  methods: {

  },

  watch: {
    chartdata(newVal) {
      this.renderChart(newVal, this.options);
    },
  },

  computed: {

  },
};
</script>
