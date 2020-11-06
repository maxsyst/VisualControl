<template>
    <div class="d-flex flex-column">
        <div class="d-flex">
            <v-btn color="green" @click="selectAllWafers">Выбрать все пластины</v-btn>
            <v-btn color="pink" @click="clearSelectedWafers">Очистить выбор</v-btn>
        </div>
        <div class="d-flex">
            <v-treeview
                v-model="selectedWafers"
                selectable
                :items="wafersWithParcels" 
                @input="wafersArrayChanged">
            </v-treeview>
        </div>
    </div>
</template>

<script>
import { mapGetters } from 'vuex';
export default {
    data() {
        return {
            selectedWafers: []
        }
    },

    computed: {
        ...mapGetters({
            wafersWithParcels: 'controlCharts/wafersWithParcels',
        })
    },

    methods: {
        wafersArrayChanged: function(selectedWafers) {
            this.$store.dispatch("controlCharts/changeSelectedWafers", selectedWafers)
        },

        selectAllWafers: function() {
            this.selectedWafers = this.wafersWithParcels.map(x => x.children.map(c => c.name)).flat()
            this.$store.dispatch("controlCharts/changeSelectedWafers", this.selectedWafers)
        },

        clearSelectedWafers: function() {
            this.selectedWafers = []
            this.$store.dispatch("controlCharts/changeSelectedWafers", [])
        }
    }

}
</script>

<style>

</style>