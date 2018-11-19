<template>
  <div class="hello" ref="chartdiv">
  </div>
</template>

<script>
  import * as am4core from "@amcharts/amcharts4/core";
  import * as am4charts from "@amcharts/amcharts4/charts";
  import am4themes_animated from "@amcharts/amcharts4/themes/animated";

  am4core.useTheme(am4themes_animated);
  export default {
    props: ['points', 'graphic', 'devices'],
   
    mounted() {
      let chart = am4core.create(this.$refs.chartdiv, am4charts.XYChart);

      

     
      let dateAxis = chart.xAxes.push(new am4charts.DurationAxis());
      dateAxis.baseUnit = "millisecond";
      dateAxis.title.text = "Время испытаний (чч:мм:cc)";
      

      chart.cursor = new am4charts.XYCursor();

      

      chart.durationFormatter.durationFormat = "hh':'mm':'ss";


      let valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
      valueAxis.title.text = this.graphic.russianName + " (" + this.graphic.unit + ") ";  

      for (var prop in this.points) {

        let data = [];
        let durationSpaces = [];
        
        var entryDatepoint = Date.parse(this.points[prop][1].time);
        var max = 0;
        for (let i = 1; i < this.points[prop].length; i++) {

          if (Date.parse(this.points[prop][i].time) - Date.parse(this.points[prop][i - 1].time) > 60000 * 10) {
            var spaceduration = Date.parse(this.points[prop][i].time) - Date.parse(this.points[prop][i - 1].time);
            for (let j = i; j < this.points[prop].length; j++) {
              this.points[prop][j].time = new Date(Date.parse(this.points[prop][j].time) - spaceduration);
            }
          }
          data.push({ "duration": Date.parse(this.points[prop][i].time) - entryDatepoint, "value": this.points[prop][i].value });
          max = Date.parse(this.points[prop][this.points[prop].length - 1].time) - entryDatepoint;
        }
        let series = chart.series.push(new am4charts.LineSeries());
        series.data = data;
        series.dataFields.valueX = "duration";
        series.dataFields.valueY = "value";
        series.strokeWidth = 2;
        series.tensionX = 0.7;
        series.tensionY = 0.7;
        let device = this.devices.find(x => x.deviceId === this.points[prop][0].deviceId);
        series.name = "Порт:" + this.points[prop][0].portNumber + " Устройство:" + device.address;
      }

      chart.legend = new am4charts.Legend();
      chart.legend.useDefaultMarker = true;
      var markerTemplate = chart.legend.markers.template;
      markerTemplate.width = 40;
      markerTemplate.height = 40;

            
      dateAxis.strictMinMax = true;
      dateAxis.min = 0;
      dateAxis.max = max;

      chart.colors.step = 2;
      this.chart = chart;
     
    },

    beforeDestroy() {
      if (this.chart) {
        this.chart.dispose();
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
  .hello {
    width: 90%;
    height: 500px;
  }
</style>
