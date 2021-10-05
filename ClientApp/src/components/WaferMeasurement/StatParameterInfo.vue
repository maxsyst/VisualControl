<template>
    <v-container>
        <v-tabs v-model="activeTab" color="primary" dark slider-color="indigo">
            <v-tab href="#statExtendedInfoTab">Подробная информация</v-tab>
            <v-tab href="#gradientTab">Градиент</v-tab>
            <v-tab-item value="statExtendedInfoTab">
                <statparameter-view :key="'STV_' + statParameter.shortStatisticsName + keyGraphicState"
                                    :measurementId="measurementId"
                                    :keyGraphicState="keyGraphicState"
                                    :statParameter="statParameter">
                </statparameter-view>
            </v-tab-item>
            <v-tab-item value="gradientTab">
                <gradient-full  :key="'GRF_' + statParameter.shortStatisticsName + keyGraphicState"
                                :measurementId="measurementId"
                                :keyGraphicState="keyGraphicState"
                                :statParameter="statParameter"
                                :divider="divider">
                </gradient-full>
            </v-tab-item>
        </v-tabs>
    </v-container>
</template>

<script>

import GradientFull from '../Gradient/gradient-full.vue';
import StatParameterView from './StatParameterView.vue';

export default {
  props: ['measurementId', 'keyGraphicState', 'statParameter', 'divider'],

  components: {
    'gradient-full': GradientFull,
    'statparameter-view': StatParameterView,
  },

  data() {
    return {
      activeTab: 'statExtendedInfoTab',
    };
  },

  computed: {
  },

  methods: {
  },
};
</script>

<style>

</style>
