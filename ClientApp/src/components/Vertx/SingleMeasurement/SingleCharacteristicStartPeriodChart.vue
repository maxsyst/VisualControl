<template>
    <v-card>
        <v-card-subtitle>
          <div class="d-flex justify-space-between">
            <v-chip label>
              <span>Период стабилизации</span>
            </v-chip>
            <v-btn color="indigo" fab x-small dark @click="showSettingsContainer(showSettings)">
              <v-icon>settings</v-icon>
            </v-btn>
          </div>          
        </v-card-subtitle>
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
  props: ['characteristic', 'chartData', 'axisX', 'loaded'],
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
        axisX: {
          strictMinMax: false,
          min: 0
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
              gridColor: '#ffcc00',
              chartColor: '#8E2DE2'
            }
      }
    }
  },
  async created () {
    this.chartSettings.characteristic = { ...this.characteristic }
    this.chartSettings.axisX = Object.assign({}, this.axisX)
  }
}
</script>
