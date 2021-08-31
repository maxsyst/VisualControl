<template>
    <v-card class="elevation-8" color="#303030">
        <v-row>
            <v-col lg="4" offset-lg="1" class="d-flex align-center justify-start">
                <v-chip :color="isGraphicSelected ? 'indigo' : 'pink'" label v-html="graphic.graphicName"
                        @click="$vuetify.goTo('#ss_' + keyGraphicState)" dark></v-chip>
            </v-col>
            <v-col lg="2" class="d-flex align-center justify-start">
                 <v-progress-circular v-if="dirtyCellsSnapshot"
                      :rotate="360"
                      :size="50"
                      :width="2"
                      :value="dirtyCellsSnapshot.goodDiesPercentage"
                      :color="this.$store.getters['wafermeas/calculateColor'](dirtyCellsSnapshot.goodDiesPercentage / 100)">
                    {{ dirtyCellsSnapshot.goodDiesPercentage + '%'}}
                    </v-progress-circular>
                    <v-progress-circular v-else
                        indeterminate
                        color='primary'
                    ></v-progress-circular>
            </v-col>
            <v-col lg="2" class="d-flex align-center justify-start">
                <v-progress-circular v-if="dirtyCellsSnapshot"
                    :rotate="360"
                    :size="50"
                    :width="2"
                    :value="dirtyCellsPercentage"
                    color='primary'>
                    {{ dirtyCellsPercentage + '%' }}
                  </v-progress-circular>
                   <v-progress-circular v-else
                        indeterminate
                        color='primary'
                    ></v-progress-circular>
            </v-col>
            <v-col lg="1" offset-lg="1" class="d-flex align-center">
                <v-checkbox
                    v-model="isGraphicSelected"
                    color='primary'
                    @change="changeGraphicSelection"
                ></v-checkbox>
            </v-col>
        </v-row>
    </v-card>
</template>

<script>
import { mapGetters } from 'vuex';

export default {

  props: ['keyGraphicState'],

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
      selectedDies: 'wafermeas/selectedDies',
      avbSelectedDies: 'wafermeas/avbSelectedDies',
    }),

    dirtyCellsSnapshot() {
      return this.$store.getters['wafermeas/getDirtyCellsSnapshotByKeyGraphicState'](this.keyGraphicState);
    },

    dirtyCellsPercentage() {
      return Math.ceil((1.0 - (this.dirtyCellsSnapshot.badDies.length - ([...new Set([...this.selectedDies, ...this.dirtyCellsSnapshot.badDies])].length - this.selectedDies.length)) / this.selectedDies.length) * 100);
    },

    isGraphicSelected() {
      return this.$store.getters['wafermeas/selectedGraphics'].includes(this.keyGraphicState);
    },

    graphic() {
      return this.$store.getters['wafermeas/getGraphicByGraphicState'](this.keyGraphicState);
    },
  },

};
</script>
