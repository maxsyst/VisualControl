<template>
  
</template>

<script>
import { mapGetters } from 'vuex';
export default {
    data() {

    },

    computed: {
        ...mapGetters({
            wafersWithParcels: 'controlCharts/wafersWithParcels',
        })
    },


}
</script>

<style>

</style>