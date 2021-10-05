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
      />
    </v-tab-item>
    <v-tab-item value="totalTimeTab">
      <TotalTimeTab
        :characteristic="characteristic"
        :source-id="sourceId"
      />
    </v-tab-item>
    <v-tab-item value="startTimeTab">
      <StartTimeTab
        :characteristic="characteristic"
        :source-id="sourceId"
      />
    </v-tab-item>
  </v-tabs>
</template>

<script>

import TestTimeChartTab from './ChartTabs/TestTimeChartTab.vue';
import TotalTimeChartTab from './ChartTabs/TotalTimeChartTab.vue';
import StartTimeChartTab from './ChartTabs/StartTimeChartTab.vue';

export default {
  name: 'ChartTabs',
  components: {
    TestTimeTab: TestTimeChartTab,
    TotalTimeTab: TotalTimeChartTab,
    StartTimeTab: StartTimeChartTab,
  },
  props: ['characteristic', 'sourceId'],
  data() {
    return {
      activeTab: 'testTimeTab',
    };
  },
};
</script>

<style scoped>

</style>
