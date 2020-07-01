<script>
import { Bar } from 'vue-chartjs'

export default {
  extends: Bar,
  props: {
    chartdata: {
      type: Object,
      default: null
    },
    options: {
      type: Object,
      default: null
    },
    keyGraphicState: {
      type: String,
      default: ""
    }
  },

  mounted () {
    this.renderChart(this.chartdata, this.options)
  },

  watch: {
    mode: function(newValue) {
      if(newValue === "dirty") {
        this.chartdata.datasets[0].dieIdList.forEach((d,index) => this.chartdata.datasets[0].backgroundColor[index] = this.dirtyCells.fullWafer.cells.includes(d) ? "#ff1744" : "#00e676")
      }
      if(newValue === "color") {
        this.chartdata.datasets[0].dieIdList.forEach((d,index) => this.chartdata.datasets[0].backgroundColor[index] = this.dieColors.find(dc => dc.dieId === d).hexColor)
      }
      if(newValue === "selected") {
        this.chartdata.datasets[0].backgroundColor = this.chartdata.datasets[0].backgroundColor.map(x => "#3D5AFE")
      }       
      this.renderChart(this.chartdata, this.options)
    }
  },

  computed: {

    mode() {
      return this.$store.getters['wafermeas/getKeyGraphicStateMode'](this.keyGraphicState)
    },

    dieColors() {
      return this.$store.getters['wafermeas/dieColors']
    },

    dirtyCells() {
      return this.$store.getters['wafermeas/getDirtyCellsByGraphic'](this.keyGraphicState)
    }
  },
}
</script>