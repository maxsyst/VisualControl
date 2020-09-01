<script>
import { Bar } from 'vue-chartjs'
import { mapGetters } from 'vuex';

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

  methods: {
    getChartDataFromStore(selectedDies) {
      let datasets = []
      selectedDies.forEach(dieId => {
        let singleDataset = {
          dieId: dieId,
          backgroundColor: this.mode === 'dirty' 
                                         ? this.dirtyCells.fullWafer.cells.includes(dieId) ? "#ff1744" : "#00e676" 
                                         : this.mode === 'color' ? this.dieColors.find(dc => dc.dieId === dieId).hexColor : "#3D5AFE",
          data: +this.dieValues.find(dv => dv.d === dieId).y[0],
          label: this.wafer.formedMapMini.dies.find(d => d.id === dieId).code
        }
        datasets.push(singleDataset)
      })
      datasets = [..._.sortBy(datasets, [function(o) { return +o.label.split('-')[0] }])]
      this.chartdata.labels = datasets.map(d => d.label)
      this.chartdata.datasets[0] = {
        backgroundColor: [...datasets.map(x => x.backgroundColor)],
        dieIdList: [...datasets.map(x => x.dieId)],
        data: [...datasets.map(x => x.data)],
        fill: false,
        borderWidth: 1,
        pointHoverRadius: 0,
        pointRadius: 0
      }
    }
  },

  watch: {
    mode: function(newValue) {
      if(newValue === "dirty") {
        this.chartdata.datasets[0].dieIdList.forEach((d,index) => this.chartdata.datasets[0].backgroundColor[index] = this.dirtyCells.fullWafer.cells.includes(d) ? "#ff1744" : "#00e676")
      }
      if(newValue === "color") {
        this.chartdata.datasets[0].dieIdList.forEach((d,index) => this.chartdata.datasets[0].backgroundColor[index] = this.dieColors.find(dc => dc.dieId === d).hexColor)
      }
      if(newValue === "selected" || newValue === "initial") {
        this.chartdata.datasets[0].backgroundColor = this.chartdata.datasets[0].backgroundColor.map(x => "#3D5AFE")
      }       
      this.renderChart(this.chartdata, this.options)
    },

    selectedDies: function(newValue) {
      this.getChartDataFromStore(newValue)
      this.renderChart(this.chartdata, this.options)
    }
  },

  computed: {

     ...mapGetters({
      wafer: 'wafermeas/wafer',
      selectedDies: 'wafermeas/selectedDies',
      dieColors: 'wafermeas/dieColors',
      dieValuesGetter: 'wafermeas/getDieValuesByKeyGraphicState',
      modeGetter: 'wafermeas/getKeyGraphicStateMode',
      dirtyCellsGetter: 'wafermeas/getDirtyCellsByGraphic'
    }),

    dieValues() {
      return this.dieValuesGetter(this.keyGraphicState)
    },

    mode() {
      return this.modeGetter(this.keyGraphicState)
    },

    dirtyCells() {
      return this.dirtyCellsGetter(this.keyGraphicState)
    }
  }
}
</script>