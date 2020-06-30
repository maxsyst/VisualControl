<script>
import { Line } from 'vue-chartjs'

export default {
  extends: Line,
  data() {
     return {
       oldHovered: {color: "", dieId: 0}
     }
  },
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

  mounted() {
    this.renderChart(this.chartdata, this.options)
  },

  watch: {
    hovered: function(newValue) {
      if(newValue.keyGraphicState === this.keyGraphicState) {
        if(this.oldHovered.dieId > 0) {
          let oldHovered = this.chartdata.datasets.find(x => x.dieId === this.oldHovered.dieId)
          oldHovered.borderWidth = 1
        }
        let highligted = this.chartdata.datasets.find(x => x.dieId === newValue.dieId)
        this.oldHovered.dieId = newValue.dieId
        this.oldHovered.color = highligted.borderColor
        highligted.borderWidth = 7       
        this.renderChart(this.chartdata, this.options)
      }        
    },

    mode: function(newValue) {
      if(newValue === "dirty") {
        this.chartdata.datasets.forEach(d =>  d.borderColor = this.dirtyCells.fullWafer.cells.includes(d.dieId) ? "#ff1744" : "#00e676")
      } else {
        this.chartdata.datasets.forEach(d => d.borderColor = this.dieColors.find(dc => dc.dieId === d.dieId).hexColor)
      }
      this.renderChart(this.chartdata, this.options)
    }
  },

  computed: {

    hovered() {
      return this.$store.getters['wafermeas/hoveredDieId']
    },

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