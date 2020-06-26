<script>
import { Line } from 'vue-chartjs'

export default {
  extends: Line,
  data() {
     return {
       hoveredDieId: 0,
       hoveredColor: ""
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

  mounted () {
    this.renderChart(this.chartdata, this.options)
  },

  watch: {
    hovered: function(newValue) {
      if(_.isEmpty(newValue)) {
        if(this.hoveredDieId > 0) { 
          let highligted = this.chartdata.datasets.find(x => x.dieId === this.hoveredDieId)
          highligted.borderWidth = 1;
          this.hoveredDieId = 0
        }        
      }
      else {
        if(newValue.keyGraphicState === this.keyGraphicState) {
          this.hoveredDieId = newValue.dieId
          let highligted = this.chartdata.datasets.find(x => x.dieId === this.hoveredDieId)
          highligted.borderWidth = 7;          
          this.renderChart(this.chartdata, this.options)
        }        
      }
      
    }
  },

  computed: {
    hovered() {
      return this.$store.getters['wafermeas/hoveredDieId']
    }
  },
}
</script>