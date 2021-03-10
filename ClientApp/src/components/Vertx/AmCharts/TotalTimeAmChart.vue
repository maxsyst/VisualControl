<template>
  <div
    ref="chartdiv"
    class="hello"
  />
</template>

<script>
import * as am4core from '@amcharts/amcharts4/core'
import * as am4charts from '@amcharts/amcharts4/charts'
import am4lang from '@amcharts/amcharts4/lang/ru_RU'
import Gradient from 'javascript-color-gradient'

export default {
  props: ['data', 'settings'],
  mounted () {
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

    const colorGradient = new Gradient()
    const color1 = '#40e0d0'
    const color2 = '#ff8c00'
    const color3 = '#ff0080'
    colorGradient.setMidpoint(Object.entries(this.data).length)
    colorGradient.setGradient(color1, color2, color3)
    chart.colors.list = [...colorGradient.getArray().map((x) => am4core.color(x))]

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

<style scoped>
.hello {
  width: 100%;
  height: 500px;
}
</style>
