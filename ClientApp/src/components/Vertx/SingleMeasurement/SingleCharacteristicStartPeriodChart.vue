<template>
  <v-card>
    <v-card-subtitle>
      <div class="d-flex justify-space-between">
        <v-chip label>
          <span>Период стабилизации</span>
        </v-chip>
        <v-menu
          v-model="showSettings"
          absolute
          :close-on-content-click="false"
          offset-y
          style="max-width: 600px"
        >
          <template v-slot:activator="{ on }">
            <v-btn color="indigo" fab x-small dark v-on="on">
              <v-icon>settings</v-icon>
            </v-btn>
          </template>
          <v-card>
            <v-card-title>
              <v-chip label>
                <span>Настройки графика</span>
              </v-chip>
            </v-card-title>
            <v-card-text>
              <v-list>
                <v-list-item>
                  <v-list-item-action>

                  </v-list-item-action>
                  <v-list-item-title>Период измерения</v-list-item-title>
                </v-list-item>
                <v-list-item>
                  <v-list-item-action>
                    <v-swatches
                      v-model="chartSettings.colors.chartColor"
                    />
                  </v-list-item-action>
                  <v-list-item-title>Цвет графика</v-list-item-title>
                </v-list-item>
              </v-list>
            </v-card-text>
          </v-card>
        </v-menu>
      </div>
    </v-card-subtitle>
    <v-card-text>
      <StartTimeAmChart
        v-if="loaded"
        :data="chartData"
        :settings="chartSettings"
      />
      <v-skeleton-loader v-else class="mx-auto hello" type="image" />
    </v-card-text>
  </v-card>
</template>

<script>
import VSwatches from 'vue-swatches';
import StartTimeAmChart from '../AmCharts/StartTimeAmChart';

export default {
  props: ['characteristic', 'chartData', 'axisX', 'loaded'],
  components: {
    StartTimeAmChart, VSwatches,
  },
  data() {
    return {
      showSettings: false,
      chartSettings: {
        type: 'singleStartPeriod',
        characteristic: {},
        serieName: 'name',
        smoothing: {
          require: false,
          power: 8,
        },
        axisX: {
          strictMinMax: true,
          min: 0,
          max: 10,
        },
        axisY: {
          strictMinMax: false,
          min: 0,
          max: 0,
        },
        colors: {
          backgroundColor: '#303030',
          textColor: '#ffffff',
          gridColor: '#ffcc00',
          chartColor: '#8E2DE2',
        },
      },
    };
  },
  async created() {
    this.chartSettings.characteristic = { ...this.characteristic };
    this.chartSettings.axisX = { ...this.axisX };
  },
};
</script>
