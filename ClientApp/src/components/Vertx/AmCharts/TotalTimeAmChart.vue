<template>
  <div
    ref="chartdiv"
    class="hello"
  />
</template>

<script>
import Gradient from 'javascript-color-gradient'
export default {
  props: ['data', 'settings'],
  mounted () {
    const { am4core, am4charts, am4lang } = this.$am4core()
    const chart = am4core.create(this.$refs.chartdiv, am4charts.XYChart)
    chart.language.locale = am4lang
    const { gridColor } = this.settings.colors
    const { textColor } = this.settings.colors
    function am4themes_myTheme (target) {
      if (target instanceof am4core.InterfaceColorSet) {
        target.setFor('grid', am4core.color(gridColor))
        target.setFor('text', am4core.color(textColor))
      }
    }

    if (this.settings.colors.chartColor === undefined) {
      const colorGradient = new Gradient()
      colorGradient.setMidpoint(Object.entries(this.data).length)
      colorGradient.setGradient(this.settings.colors.chartColors.first, this.settings.colors.chartColors.middle, this.settings.colors.chartColors.last)
      chart.colors.list = [...colorGradient.getArray().map((x) => am4core.color(x))]
    } else {
      chart.colors.list = [am4core.color(this.settings.colors.chartColor)]
    }

    const dateAxis = chart.xAxes.push(new am4charts.DateAxis())
    dateAxis.title.text = 'Дата испытаний'

    chart.cursor = new am4charts.XYCursor()
    chart.cursor.behavior = 'zoomXY'

    const valueAxis = chart.yAxes.push(new am4charts.ValueAxis())
    valueAxis.title.text = `${this.settings.characteristic.name}(${this.settings.characteristic.unit})`

    Object.entries(this.data).forEach((m) => {
      const [, pointsData] = m
      const series = chart.series.push(new am4charts.LineSeries())
      series.dataFields.dateX = 'date'
      series.dataFields.valueY = 'value'
      series.strokeWidth = 2
      series.tensionX = 0.7
      series.tensionY = 0.7
      series.name = pointsData.name
      series.data = pointsData.points.map((p) => ({ date: new Date(p.trueDate), value: p.value }))
    })
    am4core.useTheme(am4themes_myTheme)
    chart.legend = new am4charts.Legend()
    chart.legend.useDefaultMarker = true
    const markerTemplate = chart.legend.markers.template
    markerTemplate.width = 40
    markerTemplate.height = 40

    valueAxis.strictMinMax = false
    if (this.settings.axisY.strictMinMax) {
      valueAxis.strictMinMax = true
      valueAxis.min = this.settings.axisY.min
      valueAxis.max = this.settings.axisY.max
    }
    this.chart = chart
  },

  beforeDestroy () {
    if (this.chart) {
      this.chart.dispose()
    }
  }
}
</script>

<style>
.hello {
  width: 100%;
  height: 500px;
}
</style>
