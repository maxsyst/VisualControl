<template>
  <v-card>
    <v-card-subtitle>
      <div class="d-flex justify-space-between">
        <v-chip label>
          <span>Полный период</span>
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
                    <v-text-field
                      :value="siftedK"
                      :error-messages="[rules.siftedK.required, rules.siftedK.counter, rules.siftedK.isNumber, rules.siftedK.positive]"
                      @change="siftedKChange($event)">
                      </v-text-field>
                  </v-list-item-action>
                  <v-list-item-title>Количество точек на графике(0-все точки)</v-list-item-title>
                </v-list-item>
                <v-list-item>
                  <v-list-item-action>
                  <v-switch
                    :input-value="withoutBadPoints"
                    @change="withoutBadPointsChange($event)">
                  </v-switch>
                  </v-list-item-action>
                  <v-list-item-title>Исключить выпадающие точки</v-list-item-title>
                </v-list-item>
                <v-list-item>
                  <v-list-item-action>
                    <v-swatches v-model="chartSettings.colors.chartColor" />
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
import StartTimeAmChart from '../AmCharts/StartTimeAmChart'
import VSwatches from 'vue-swatches'
export default {
  props: ['characteristic', 'chartData', 'axisX', 'loaded', 'siftedK', 'withoutBadPoints'],
  components: {
    StartTimeAmChart, VSwatches
  },
  data () {
    return {
      showSettings: false,
      rules: {
        siftedK: {
          required: value => !!value || 'Требуется значение',
          counter: value => value.length <= 500 || 'Макс 500 точек',
          isNumber: value => Number.isInteger(value) || 'Введите число',
          positive: value => +value > 0 || 'Введите положительное число'
        }
      },
      chartSettings: {
        type: 'singleFullPeriod',
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
        axisY: {
          strictMinMax: false,
          min: 0,
          max: 0
        },
        colors: {
          backgroundColor: '#303030',
          textColor: '#ffffff',
          gridColor: '#ffcc00',
          chartColor: '#ff0080'
        }
      }
    }
  },
  methods: {
    showSettingsContainer: function () {},
    withoutBadPointsChange: function (withoutBadPoints) {
      this.$emit('withoutbadpoint-change', withoutBadPoints)
    },

    siftedKChange: function (k) {
      this.$emit('siftedK-change', k)
    }
  },
  async created () {
    this.chartSettings.characteristic = { ...this.characteristic }
    this.chartSettings.axisX = Object.assign({}, this.axisX)
  }
}
</script>
