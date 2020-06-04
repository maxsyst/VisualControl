<template>
    <v-container>
        <v-row>
            <v-col lg="6" offset-lg="1">
                {{graphic.graphicName}}
            </v-col>
            <v-col lg="2">
            </v-col>
            <v-col lg="2">
            </v-col>
            <v-col lg="1">
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
export default {

    props: ["keyGraphicState"],

    data() {
        return {

        }
    },

    computed: {
        graphic() {
            return this.$store.getters['wafermeas/getGraphicByGraphicState'](this.keyGraphicState)
        }
    },

}
</script>