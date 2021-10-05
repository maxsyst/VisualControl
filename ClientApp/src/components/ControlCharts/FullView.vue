<template>
    <v-container>
        <initial-dialog></initial-dialog>
        <v-row>
            <v-col lg="6">
                <stageandparameters-select v-if="!_.isEmpty(selectedProcess)"></stageandparameters-select>
            </v-col>
            <v-col lg="4" offset-lg="2">
                <wafer-select v-if="!_.isEmpty(selectedProcess)"></wafer-select>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
import { mapGetters } from 'vuex';
import InitialDialog from './ProcessSelectDialog.vue' 
import WaferSelect from './WaferSelect.vue'
import StageAndParametersSelects from './StagesAndParametersSelects.vue'
export default {
    data() {
        return {

        }
    },

    components: {
        "initial-dialog": InitialDialog,
        "wafer-select": WaferSelect,
        "stageandparameters-select": StageAndParametersSelects
    },

    mounted() {
        this.$store.dispatch("controlCharts/getProcessesFromDb", {ctx: this})
    },

    computed: {
        ...mapGetters({
            selectedProcess: 'controlCharts/selectedProcess',
        })
    },

    beforeDestroy() {
        this.$store.dispatch("controlCharts/reset")
  }
}
</script>

<style>

</style>