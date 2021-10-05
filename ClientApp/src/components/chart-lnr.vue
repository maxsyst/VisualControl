<template>
<v-card>

          <v-container  fluid>
              <v-row class="charts" ref="chartdiv">
              </v-row>

          </v-container>

    </v-card>
</template>

<script>
import * as am4core from '@amcharts/amcharts4/core';
import * as am4charts from '@amcharts/amcharts4/charts';
import am4themes_animated from '@amcharts/amcharts4/themes/animated';

export default {

  props: ['keyGraphicState', 'measurementId', 'divider'],
  data() {
    return {
      chartData: {},
    };
  },

  mounted() {
    const chart = am4core.create(this.$refs.chartdiv, am4charts.XYChart);
    this.getChartData();
  },

  beforeDestroy() {
    if (this.chart) {
      this.chart.dispose();
    }
  },

  computed:
    {
      selectedDies() {
        return this.$store.getters['wafermeas/selectedDies'];
      },
    },

  watch:
    {
      selectedDies() {
        this.getChartData();
      },

      divider() {
        this.getChartData();
      },
    },

  methods:
    {
      getChartData() {
        const singlestatModel = {};
        singlestatModel.divider = this.divider;
        singlestatModel.keyGraphicState = this.keyGraphicState;
        singlestatModel.measurementId = this.measurementId;
        singlestatModel.dieIdList = this.selectedDies;
        this.$http
          .get(`/api/amchart/GetLinearForMeasurement?statisticSingleGraphicViewModelJSON=${JSON.stringify(singlestatModel)}`)
          .then((response) => {
            this.chartData = response.data;

            const chart = am4core.create(this.$refs.chartdiv, am4charts.XYChart);

            const categoryAxis = this.chart.xAxes.push(new am4charts.ValueAxis());

            const valueAxis = this.chart.yAxes.push(new am4charts.ValueAxis());

            for (let j = 0; j < this.chartData.data.length; j++) {
              const data = [];
              for (let i = 0; i < this.chartData.data[j].pointList.length; i++) {
                data.push({ category: this.chartData.data[j].pointList[i].ctg, value: this.chartData.data[j].pointList[i].value });
              }
              const series = this.chart.series.push(new am4charts.LineSeries());
              series.data = data;
              series.dataFields.valueX = 'category';
              series.dataFields.valueY = 'value';
              series.strokeWidth = 2;
            }
            this.chart = chart;
          })
          .catch((error) => {});
      },
    },

};
</script>

<style scoped>
  .charts {
    width: 90%;
    height: 90%;
  }
</style>
