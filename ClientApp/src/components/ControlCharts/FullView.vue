<template>
    <v-container>
        <initial-dialog></initial-dialog>
        <v-row>

        </v-row>
    </v-container>
</template>

<script>
import { mapGetters } from 'vuex';
import InitialDialog from './ProcessSelectDialog.vue' 

export default {
    data() {
        return {

        }
    },

    components: {
        "initial-dialog": InitialDialog
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