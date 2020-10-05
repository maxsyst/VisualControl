<template>
    <div class="d-flex align-stretch">
        <v-card class="mt-2" color="#303030" dark v-if="avbSelectedDies.length > 0">      
            <v-card-text>
                <div class="d-flex justify-space-between">
                    <div>
                        <p>{{"Выбрано " + selectedDies.length + " из " + avbSelectedDies.length }}</p>
                    </div>
                    <div>
                        <v-progress-circular
                        :rotate="360"
                        :size="60"
                        :width="3"
                        :value="(selectedDies.length / avbSelectedDies.length)*100"
                        :color="viewMode === 'Мониторинг' ? 'primary' : '#80DEEA'">
                        {{ Math.ceil((selectedDies.length / avbSelectedDies.length)*100) + "%" }}</v-progress-circular>
                    </div>
                </div>
            </v-card-text>
            <v-card-actions>
              <v-btn block :color="viewMode === 'Мониторинг' ? 'primary' : '#80DEEA'" outlined @click="selectAllDies(avbSelectedDies)">Выбрать все кристаллы</v-btn>
            </v-card-actions>
        </v-card>
    </div>
</template>

<script>
import { mapGetters } from 'vuex';
export default {
    props: ["selectedMeasurementId", "viewMode"],
    data() {
        return {

        }
    },

    computed:{
        ...mapGetters({
            selectedDies: 'wafermeas/selectedDies',
            avbSelectedDies: 'wafermeas/avbSelectedDies'
        }),
    },

    methods: {
        selectAllDies: function(avbSelectedDies) {
            this.$store.dispatch("wafermeas/updateSelectedDies", [...avbSelectedDies])
        }
    }
}
</script>

<style>

</style>