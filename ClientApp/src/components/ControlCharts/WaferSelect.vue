<template>
    <v-card style="max-height: 500px" class="overflow-y-auto">
         <v-card-actions>
            <div v-if="wafersWithParcels.length>0" class="d-flex align-center">
                <v-btn color="green" class="mr-2" @click="selectAllWafers">Выбрать все пластины</v-btn>
                <v-btn color="pink" @click="clearSelectedWafers">Очистить выбор</v-btn>
            </div>
        </v-card-actions>
        <v-card-text>
            <div class="d-flex">
                <v-treeview
                    v-model="selectedWafers"
                    selectable
                    :items="wafersWithParcels"
                    @input="wafersArrayChanged">
                </v-treeview>
            </div>
        </v-card-text>

    </v-card>
</template>

<script>
import { mapGetters } from 'vuex';

export default {
  data() {
    return {
      selectedWafers: [],
    };
  },

  computed: {
    ...mapGetters({
      wafersWithParcels: 'controlCharts/wafersWithParcels',
    }),
  },

  methods: {
    wafersArrayChanged(selectedWafers) {
      this.$store.dispatch('controlCharts/changeSelectedWafers', selectedWafers);
    },

    selectAllWafers() {
      this.selectedWafers = this.wafersWithParcels.map((x) => x.children.map((c) => c.name)).flat();
      this.$store.dispatch('controlCharts/changeSelectedWafers', this.selectedWafers);
    },

    clearSelectedWafers() {
      this.selectedWafers = [];
      this.$store.dispatch('controlCharts/changeSelectedWafers', []);
    },
  },

};
</script>

<style>

</style>
