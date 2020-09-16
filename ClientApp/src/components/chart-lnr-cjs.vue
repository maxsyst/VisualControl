<template>
  <div class="d-flex flex-column" v-if="loaded">
    <div class="d-flex">
      <v-col class="d-flex">
        <v-card>
          <v-container>
            <line-chart
              :id="'LCHART_' + keyGraphicState"
              :keyGraphicState="keyGraphicState"
              :chartdata="chartdata"
              :settings="settings"
              :viewMode="viewMode"
              :options="options"/>
          </v-container>
        </v-card>
      </v-col>
      <div class="d-flex flex-column justify-space-between">
        <div class="d-flex">
          <v-col class="d-flex flex-column justify-start">
            <v-btn :color="log === false ? 'indigo' : 'grey darken-2'" fab x-small dark @click="changeLogMode(false)">
              Лин
            </v-btn>
            <v-btn :color="log === true ? 'indigo' : 'grey darken-2'" fab x-small dark @click="changeLogMode(true)">
              Лог
            </v-btn>      
          </v-col>
        </div>
        <div class="d-flex">
          <v-col class="d-flex">
            <v-btn :color="showSettings === true ? 'indigo' : 'grey darken-2'" fab x-small dark @click="showSettingsContainer(showSettings)">
              <v-icon>settings</v-icon>
            </v-btn>
          </v-col>
        </div>
      </div>
    </div>
    <div class="d-flex"> 
      <settings v-if="showSettings" :keyGraphicState="keyGraphicState" @settings-changed="settingsChanged"></settings>
    </div>
  </div>
  <v-progress-circular v-else :size="50" color="primary" indeterminate></v-progress-circular>
</template>

<script>  
import LineChart from './linechart-cjs.vue'
import Settings from './graphicsettings-lnr.vue'
export default {
  
  props: ["keyGraphicState", "measurementId", "divider", "viewMode"],
  components: { LineChart, Settings },
  data: () => ({
    loaded: false,
    showSettings: false,
    settings: {},
    chartdata: {},
    options: {}
  }),

  async mounted() {
    await this.getChartData(this.selectedDies);
  },

   computed: {
      selectedDies() {
        return this.$store.getters['wafermeas/selectedDies']
      },
      log() {
        return this.$store.getters['wafermeas/getKeyGraphicStateLog'](this.keyGraphicState)
      },

    },

    watch: {
      divider: async function() {
        await this.getChartData(this.selectedDies);
      }
    },

    methods: {

      async getChartData(selectedDies) {
        this.loaded = false     
        let singlestatModel = {};
        singlestatModel.divider = this.divider;
        singlestatModel.keyGraphicState = this.keyGraphicState;
        singlestatModel.measurementId = this.measurementId;
        singlestatModel.dieIdList = selectedDies;
        await this.$http
          .get(`/api/chartjs/GetLinearForMeasurement?statisticSingleGraphicViewModelJSON=${JSON.stringify(singlestatModel)}`)
          .then(response => {
            let chart = response.data
            chart.chartData.labels = [...chart.chartData.labels.map(x => +x)]
            console.log(chart.options)
            this.calculateOptions(chart.options, 
                                  {min: chart.chartData.labels[0], max: chart.chartData.labels[chart.chartData.labels.length -1], maxTicksLimit: 11}, 
                                  {min: 0, max: 0, maxTicksLimit: 11})
            this.$store.dispatch("wafermeas/changeGraphicInitialSettings", {keyGraphicState: this.keyGraphicState, axisType: "xAxis", settings: {min: chart.chartData.labels[0], max: chart.chartData.labels[chart.chartData.labels.length -1], maxTicksLimit: 11}})
            this.$store.dispatch("wafermeas/changeGraphicInitialSettings", {keyGraphicState: this.keyGraphicState, axisType: "yAxis", settings: {min: "Авто", max: "Авто", maxTicksLimit: 11}})
            // let ds = []
            // chart.chartData.datasets.forEach((dataset,index) => {
            //   let downsampled = this.downsample(dataset.data.map(function (d,index) { return {x: d, y: chart.chartData.labels[index]}}), 201)
            //   dataset.data = downsampled.map(x => x.x)
            //   ds.push({x: dataset.data, y: downsampled.map(x => x.y)})
            //   if(index === 0)
            //     chart.chartData.labels = downsampled.map(x => x.y)
            // });
            this.chartdata = chart.chartData
            this.loaded = true       
        })
        .catch(error => {console.log(error)});
      },

      downsample(data, threshold) {
        var floor = Math.floor
        var abs = Math.abs
        var dataLength = data.length;
        if (threshold >= dataLength || threshold <= 0) {
            return data; // nothing to do
        }
        var sampled = [],
            sampledIndex = 0;

        // bucket size, leave room for start and end data points
        var every = (dataLength - 2) / (threshold - 2);

        var a = 0,  // initially a is the first point in the triangle
            maxAreaPoint,
            maxArea,
            area,
            nextA;

        // always add the first point
        sampled[sampledIndex++] = data[a];

        for (var i = 0; i < threshold - 2; i++) {
            // Calculate point average for next bucket (containing c)
            var avgX = 0,
                avgY = 0,
                avgRangeStart = floor(( i + 1 ) * every) + 1,
                avgRangeEnd = floor(( i + 2 ) * every) + 1;
            avgRangeEnd = avgRangeEnd < dataLength ? avgRangeEnd : dataLength;

            var avgRangeLength = avgRangeEnd - avgRangeStart;
            for (; avgRangeStart < avgRangeEnd; avgRangeStart++) {
                avgX += data[avgRangeStart].x * 1; // * 1 enforces Number (value may be Date)
                avgY += data[avgRangeStart].y * 1;
            }
            avgX /= avgRangeLength;
            avgY /= avgRangeLength;

            // Get the range for this bucket
            var rangeOffs = floor((i + 0) * every) + 1,
                rangeTo = floor((i + 1) * every) + 1;

            // Point a
            var pointAX = data[a].x * 1, // enforce Number (value may be Date)
                pointAY = data[a].y * 1;

            maxArea = area = -1;
           
            for (; rangeOffs < rangeTo; rangeOffs++) {
                // Calculate triangle area over three buckets
                area = abs(( pointAX - avgX ) * ( data[rangeOffs].y - pointAY ) -
                        ( pointAX - data[rangeOffs].x ) * ( avgY - pointAY )
                    ) * 0.5;
                if (area > maxArea) {
                    maxArea = area;
                    maxAreaPoint = data[rangeOffs];
                    nextA = rangeOffs; // Next a is this b
                }
            }

            sampled[sampledIndex++] = maxAreaPoint; // Pick this point from the bucket
            a = nextA; // This a is the next a (chosen b)
        }

        sampled[sampledIndex] = data[dataLength - 1]; // Always add last

        return sampled;
      },

      settingsChanged(e) {
        this.settings = _.cloneDeep(this.$store.getters['wafermeas/getGraphicSettingsKeyGraphicState'](this.keyGraphicState))
      },

      showSettingsContainer(showSettings) {
        this.showSettings = !showSettings
      },

      changeLogMode(log) {
        this.$store.dispatch("wafermeas/changeKeyGraphicStateLog", {keyGraphicState: this.keyGraphicState, log: log});
      },

        calculateOptions(chartOptions, xAxisSettings, yAxisSettings) {
          let log = this.log === true ? 'logarithmic' : 'linear'
          this.options = {
            animation: chartOptions.animation,
            hover: chartOptions.hover,
            tooltips: chartOptions.tooltips,
            legend: {
              display: chartOptions.legend.display
            },
            responsive: chartOptions.responsive,
            responsiveAnimationDuration: chartOptions.responsiveAnimationDuration,
            scales: {
              xAxes: [{
                scaleLabel: {
                  display: chartOptions.xAxis.display,
                  labelString: chartOptions.xAxis.label,
                  fontColor: '#BDBDBD'
                },
                gridLines: {
                  display: true,
                  color: '#303030'
                },
                ticks: {
                  fontColor: '#BDBDBD',
                  min: xAxisSettings.min,
                  max: xAxisSettings.max,
                  maxTicksLimit: xAxisSettings.maxTicksLimit
                
                }
              }],
              yAxes: [{
                type: log,
                scaleLabel: {
                  display: chartOptions.yAxis.display,
                  labelString: chartOptions.yAxis.label,
                  fontColor: '#BDBDBD'
                },
                gridLines: {
                  display: true,
                  color: '#303030'
                },
                ticks: {
                  fontColor: '#BDBDBD',
                  maxTicksLimit: yAxisSettings.maxTicksLimit
                }
              }]
            }
          }   
        }
    }

    
}
</script>
