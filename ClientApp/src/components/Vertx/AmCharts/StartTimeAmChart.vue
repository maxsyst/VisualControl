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

  watch: {
    'settings.serieName': function (serieName) {
      Object.entries(this.data).forEach((m) => {
        const [measurementId, pointsData] = m
        const serie = this.chart.map.getKey(measurementId)
        serie.name = serieName === 'name' ? pointsData.name : new Date(pointsData.creationDate).toLocaleString('ru-RU', { year: 'numeric', month: 'long', day: 'numeric' })
      })
    },
    'settings.colors.backgroundColor': function (color) {
      this.chart.background.fill = color
    },
    'settings.colors.gridColor': function (color) {
      const dateAxis = this.chart.map.getKey('DAX_0001')
      const valueAxis = this.chart.map.getKey('VAX_0001')
      dateAxis.renderer.grid.template.stroke = color
      valueAxis.renderer.grid.template.stroke = color
    },
    'settings.colors.textColor': function (color) {
      const dateAxis = this.chart.map.getKey('DAX_0001')
      const valueAxis = this.chart.map.getKey('VAX_0001')
      dateAxis.renderer.labels.template.fill = color
      valueAxis.renderer.labels.template.fill = color
    },
    'settings.minutes': function (minutes) {
      const dateAxis = this.chart.map.getKey('DAX_0001')
      dateAxis.max = minutes === 0 ? undefined : minutes * 60000
    }
  },
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

    const dateAxis = chart.xAxes.push(new am4charts.DurationAxis())
    dateAxis.id = 'DAX_0001'
    dateAxis.baseUnit = 'millisecond'
    dateAxis.title.text = 'Время испытаний (чч:мм:cc)'

    chart.cursor = new am4charts.XYCursor()
    chart.cursor.behavior = 'zoomXY'
    chart.durationFormatter.durationFormat = "hh':'mm':'ss"
    const valueAxis = chart.yAxes.push(new am4charts.ValueAxis())
    valueAxis.id = 'VAX_0001'
    valueAxis.title.text = `${this.settings.characteristic.name}(${this.settings.characteristic.unit})`

    Object.entries(this.data).forEach((m) => {
      const [measurementId, pointsData] = m
      const series = chart.series.push(new am4charts.LineSeries())
      series.id = measurementId
      series.dataFields.valueX = 'duration'
      series.dataFields.valueY = 'value'
      series.strokeWidth = 2
      series.tensionX = 0.7
      series.tensionY = 0.7
      series.name = this.settings.serieName === 'name' ? pointsData.name : new Date(pointsData.creationDate)
        .toLocaleString('ru-RU', { year: 'numeric', month: 'long', day: 'numeric' })
      series.data = pointsData.points
        .map((p) => ({ duration: this.moment.duration(p.fromStartDate), value: p.value }))
    })
    chart.series.showOnInit = false

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

    if (this.settings.axisX.strictMinMax) {
      dateAxis.strictMinMax = true
      dateAxis.min = this.settings.axisX.min
    }
    dateAxis.max = this.settings.minutes === 0 ? undefined : this.settings.minutes * 60000
    chart.svgContainer.autoResize = false
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
