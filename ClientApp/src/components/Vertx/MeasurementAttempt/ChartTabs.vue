<template>
  <v-tabs
    v-model="activeTab"
    color="primary"
    fixed-tabs
    slider-color="indigo"
  >
    <v-tab href="#testTimeTab">
      Test Time
    </v-tab>
    <v-tab href="#totalTimeTab">
      Total Time
    </v-tab>
    <v-tab href="#startTimeTab">
      Starting Time
    </v-tab>

    <v-tab-item value="testTimeTab">
      <TestTimeTab
        :characteristic="characteristic"
        :source-id="sourceId"
        :is-single-measurement="isSingleMeasurement"
      />
    </v-tab-item>
    <v-tab-item value="totalTimeTab">
      <TotalTimeTab
        :characteristic="characteristic"
        :source-id="sourceId"
        :is-single-measurement="isSingleMeasurement"
      />
    </v-tab-item>
    <v-tab-item value="startTimeTab">
      <StartTimeTab
        :characteristic="characteristic"
        :source-id="sourceId"
        :is-single-measurement="isSingleMeasurement"
      />
    </v-tab-item>
  </v-tabs>
</template>

<script>

import TestTimeChartTab from '../MeasurementAttempt/ChartTabs/TestTimeChartTab'
import TotalTimeChartTab from '../MeasurementAttempt/ChartTabs/TotalTimeChartTab'
import StartTimeChartTab from '../MeasurementAttempt/ChartTabs/StartTimeChartTab'

export default {
  name: 'ChartTabs',
  components: {
    TestTimeTab: TestTimeChartTab,
    TotalTimeTab: TotalTimeChartTab,
    StartTimeTab: StartTimeChartTab
  },
  props: ['characteristic', 'sourceId', 'isSingleMeasurement'],
  data () {
    return {
      activeTab: 'testTimeTab'
    }
  }
}
</script>

<style scoped>

</style>
