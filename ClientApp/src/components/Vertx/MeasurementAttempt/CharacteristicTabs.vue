<template>
  <v-tabs
    v-model="activeTab"
    color="primary"
    background-color="indigo"
    right
    slider-color="primary"
  >
    <v-tab
      v-for="characteristic in characteristicData"
      :key="'tab' + characteristic.name"
      :href="'#' + characteristic.name + 'Tab'"
    >
      {{ characteristic.name }}
    </v-tab>
    <v-tab-item
      v-for="characteristic in characteristicData"
      :key="'tab-item' + characteristic.name"
      :value="characteristic.name + 'Tab'"
    >
      <ChartTabs
        :characteristic="characteristic"
        :source-id="sourceId"
        :is-single-measurement="isSingleMeasurement"
      />
    </v-tab-item>
  </v-tabs>
</template>

<script>
import ChartTabs from './ChartTabs.vue';

export default {
  name: 'CharacteristicTabs',
  components: {
    ChartTabs,
  },
  props: ['characteristicData', 'sourceId', 'isSingleMeasurement'],
  data() {
    return {

    };
  },
  computed: {
    activeTab() {
      return this.characteristicData.length === 0 ? '' : `${this.characteristicData[0].name}Tab`;
    },
  },
};
</script>

<style scoped>

</style>
