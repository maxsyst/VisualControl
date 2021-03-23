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
            <td class="text-center font-weight-black border">{{characteristic.name}}</td>
            <td class="text-center border">{{characteristic.unit}}</td>
            <td class="text-center border">{{statistic.firstValue.toFixed(3)}}</td>
            <td class="text-center border">{{statistic.averageValue.toFixed(3)}}</td>
            <td class="text-center border">{{statistic.lastValue.toFixed(3)}}</td>
            <td class="text-center border">{{statistic.periodOscillation.absolute.toFixed(3)}}</td>
            <td class="text-center border">{{statistic.periodOscillation.percent.toFixed(3)}}%</td>
            <td class="text-center border">{{Math.floor(startPeriod)}}</td>
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
  mounted () {
    this.pointsData = this.chartData[this.measurementId].points.map(x => x.value)
    this.statistic.firstValue = _.first(this.pointsData)
    this.statistic.lastValue = _.last(this.pointsData)
    this.statistic.averageValue = _.meanBy(this.pointsData)
    this.statistic.periodOscillation.absolute = this.statistic.lastValue - this.statistic.firstValue
    this.statistic.periodOscillation.percent = (this.statistic.periodOscillation.absolute / this.statistic.firstValue) * 100
  }
}
</script>
