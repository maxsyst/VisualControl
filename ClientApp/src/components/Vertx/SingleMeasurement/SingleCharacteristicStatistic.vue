<template>
  <v-card class="elevation-8">
    <v-card-text>
      <v-simple-table>
        <template v-slot:default>
          <thead>
            <tr>
              <th class="text-center">Характеристика</th>
              <th class="text-center">Единица измерения</th>
              <th class="text-center">Начальное значение</th>
              <th class="text-center">Среднее значение</th>
              <th class="text-center">Последнее значение</th>
              <th class="text-center">Изменение за период(абсолютное)</th>
              <th class="text-center">Изменение за период(в процентах)</th>
              <th class="text-center">Период стабилизации(минут)</th>
            </tr>
          </thead>
          <tbody>
            <td class="text-center border">
              <v-chip label>
                <span>{{ characteristic.name }}</span>
              </v-chip>
            </td>
            <td class="text-center border">
              <v-chip label v-if="characteristic.unit">
                <span>{{ characteristic.unit }}</span>
              </v-chip>
              <v-btn color="pink" v-if="!unitEditor" outlined x-small @click.stop="showUnitEditor">
                Не установлено
              </v-btn>
              <div class="d-flex flex-row" v-if="unitEditor">
                 <v-text-field
                    dense
                    v-model="characteristicUnit"
                    @click.stop=""
                  ></v-text-field>
                  <div class="d-flex flex-column">
                    <v-btn icon fab x-small color="success" @click.stop="updateCharacteristicUnit"><v-icon>done</v-icon></v-btn>
                    <v-btn icon fab x-small color="pink" @click.stop="closeUnitEditor"><v-icon>close</v-icon></v-btn>
                  </div>
              </div>
            </td>
            <td class="text-center border">
              <v-btn color="primary" outlined small @click.stop="">
                {{ statistic.firstValue.toFixed(3) }}
              </v-btn>
            </td>
            <td class="text-center border">
              {{ statistic.averageValue.toFixed(3) }}
            </td>
            <td class="text-center border">
              <v-btn color="primary" outlined small @click.stop="">
                {{ statistic.lastValue.toFixed(3) }}
              </v-btn>

            </td>
            <td class="text-center border">
              {{ statistic.periodOscillation.absolute.toFixed(3) }}
            </td>
            <td class="text-center border">
              <v-btn color="success" outlined small @click.stop="">
                  {{ statistic.periodOscillation.percent.toFixed(3) }}%
              </v-btn>

            </td>
            <td class="text-center border">{{ Math.floor(startPeriod) }}</td>
          </tbody>
        </template>
      </v-simple-table>
    </v-card-text>
  </v-card>
</template>

<script>
export default {
  props: ['chartData', 'measurementId', 'startPeriod', 'characteristic'],
  data () {
    return {
      unitEditor: false,
      characteristicUnit: '',
      pointsData: [],
      statistic: {
        firstValue: 0,
        averageValue: 0,
        lastValue: 0,
        periodOscillation: {
          percent: 0,
          absolute: 0
        }
      }
    }
  },
  methods: {
    showUnitEditor: function () {
      this.unitEditor = true
    },

    closeUnitEditor: function () {
      this.unitEditor = false
    },

    updateCharacteristicUnit: async function () {
      const characteristicViewModel = {
        characteristicName: this.characteristic.name,
        characteristicUnit: this.characteristicUnit,
        measurementId: this.measurementId
      }
      try {
        await this.$http.post('/api/vertx/measurementsetplusunit/updateCharacteristicUnit', characteristicViewModel)
        this.showSnackBar('Единица измерения успешно изменена')
      } catch (ex) {
        this.showSnackBar('Ошибка при изменении единицы измерения')
      }
    }
  },
  mounted () {
    this.pointsData = this.chartData[this.measurementId].points.map(
      x => x.value
    )
    this.statistic.firstValue = _.first(this.pointsData)
    this.statistic.lastValue = _.last(this.pointsData)
    this.statistic.averageValue = _.meanBy(this.pointsData)
    this.statistic.periodOscillation.absolute =
      this.statistic.lastValue - this.statistic.firstValue
    this.statistic.periodOscillation.percent =
      (this.statistic.periodOscillation.absolute / this.statistic.firstValue) *
      100
  }
}
</script>
