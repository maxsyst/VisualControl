<template>
    <v-container>
        <v-row>

        </v-row>
    </v-container>
</template>

<script>
export default {
    props: {
        key: String
    },

    data() {
       
    },

    computed: {
        currentSMP() {
            return this.$store.getters['smpstorage/currentSmp'](this.key)
        }
    }
}
</script>

