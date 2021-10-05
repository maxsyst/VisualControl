<script>
import { Line } from 'vue-chartjs';

export default {
  name: 'TestTimeLinearChart',
  extends: Line,
  props: ['chartData', 'chartOptions'],
  data() {
    return {

    };
  },
  mounted() {
    this.renderChart(this.chartData, this.chartOptions);
  },
};

</script>

<style scoped>

</style>
