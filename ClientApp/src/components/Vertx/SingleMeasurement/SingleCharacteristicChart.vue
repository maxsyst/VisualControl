<template>
    <v-card>
        <v-card-text>
            <StartTimeAmChart
                v-if="loaded"
                :data="chartData"
                :settings="chartSettings"
            />
            <v-skeleton-loader
                v-else
                class="mx-auto"
                type="image"
            />
        </v-card-text>
    </v-card>
</template>

<script>
import StartTimeAmChart from '../AmCharts/StartTimeAmChart'
export default {
  props: ['characteristic', 'chartData', 'loaded'],
  components: {
    StartTimeAmChart
  },
  data () {
    return {
      chartSettings: {
        characteristic: {},
        serieName: 'name',
        smoothing: {
          require: false,
          power: 8
        },
        axisY:
            {
              strictMinMax: false,
              min: 0,
              max: 0
            },
        colors:
            {
              backgroundColor: '#303030',
              textColor: '#ffffff',
              gridColor: '#ffcc00'
            }
      }
    }
  },
  async created () {
    this.chartSettings.characteristic = { ...this.characteristic }
  }
}
</script>
