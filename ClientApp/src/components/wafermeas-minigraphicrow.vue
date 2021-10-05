<template>
    <v-card class="elevation-8" color="#303030">
        <v-row>
            <v-col lg="4" offset-lg="1" class="d-flex align-center justify-start">
                <v-chip :color="isGraphicSelected ? 'indigo' : 'pink'" label v-html="graphic.graphicName"
                        @click="$vuetify.goTo('#ss_' + keyGraphicState)" dark></v-chip>
            </v-col>
            <v-col lg="2" class="d-flex align-center justify-start">
                 <v-progress-circular v-if="dirtyCells && dirtyCells.fullWafer.percentage >= 0"
                      :rotate="360"
                      :size="50"
                      :width="2"
                      :value="dirtyCells.fullWafer.percentage"
                      :color="this.$store.getters['wafermeas/calculateColor'](dirtyCells.fullWafer.percentage / 100)">
                    {{ dirtyCells.fullWafer.percentage + '%'}}
                    </v-progress-circular>
                    <v-progress-circular v-else
                        indeterminate
                        :color="viewMode === 'Мониторинг' ? 'primary' : '#80DEEA'"
                    ></v-progress-circular>
            </v-col>
            <v-col lg="2" class="d-flex align-center justify-start">
                <v-progress-circular v-if="dirtyCells && dirtyCells.selectedNow.percentage >= 0"
                    :rotate="360"
                    :size="50"
                    :width="2"
                    :value="dirtyCells.selectedNow.percentage"
                    :color="viewMode === 'Мониторинг' ? 'primary' : '#80DEEA'">
                    {{ dirtyCells.selectedNow.percentage + '%' }}
                  </v-progress-circular>
                   <v-progress-circular v-else
                        indeterminate
                       :color="viewMode === 'Мониторинг' ? 'primary' : '#80DEEA'"
                    ></v-progress-circular>
            </v-col>
            <v-col lg="1" offset-lg="1" class="d-flex align-center">
                <v-checkbox
                    v-model="isGraphicSelected"
                    :color="viewMode === 'Мониторинг' ? 'primary' : '#80DEEA'"
                    @change="changeGraphicSelection"
                ></v-checkbox>
            </v-col>
        </v-row>
    </v-card>
</template>

<script>
import { mapGetters } from 'vuex';

export default {

  props: ['keyGraphicState', 'viewMode'],

  data() {
    return {

    };
  },

  methods: {
    changeGraphicSelection() {
      if (this.isGraphicSelected) {
        this.$store.dispatch('wafermeas/deleteSelectedGraphic', this.keyGraphicState);
        this.$store.dispatch('wafermeas/deleteFromDirtyCells', { keyGraphicState: this.keyGraphicState, avbSelectedDies: this.avbSelectedDies });
      } else {
        this.$store.dispatch('wafermeas/addSelectedGraphic', this.keyGraphicState);
        this.$store.dispatch('wafermeas/addToDirtyCells', { keyGraphicState: this.keyGraphicState, avbSelectedDies: this.avbSelectedDies });
      }
    },
  },

  computed: {

    ...mapGetters({
      avbSelectedDies: 'wafermeas/avbSelectedDies',
    }),

    isGraphicSelected() {
      return this.$store.getters['wafermeas/selectedGraphics'].includes(this.keyGraphicState);
    },

    graphic() {
      return this.$store.getters['wafermeas/getGraphicByGraphicState'](this.keyGraphicState);
    },

    dirtyCells() {
      return this.$store.getters['wafermeas/getDirtyCellsByGraphic'](this.keyGraphicState, this.viewMode);
    },
  },

};
</script>
