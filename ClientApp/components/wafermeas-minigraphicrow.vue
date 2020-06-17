<template>
    <v-card class="elevation-8" color="#303030">
        <v-row>
            <v-col lg="4" offset-lg="1" class="d-flex align-center justify-start">
                <v-chip color="indigo" label v-html="graphic.graphicName" @click="$vuetify.goTo('#ss_' + keyGraphicState)" dark></v-chip>
            </v-col>
            <v-col lg="2" class="d-flex align-center justify-start">
                 <v-progress-circular v-if="dirtyCells && dirtyCells.fullWafer.percentage >= 0"
                      :rotate="360"
                      :size="50"
                      :width="2"
                      :value="dirtyCells.fullWafer.percentage"
                      :color="this.$store.getters['wafermeas/calculateColor'](dirtyCells.fullWafer.percentage/ 100)">
                    {{ dirtyCells.fullWafer.percentage + '%'}}
                    </v-progress-circular>
                    <v-progress-circular v-else
                        indeterminate
                        color="primary"
                    ></v-progress-circular>
            </v-col>
            <v-col lg="2" class="d-flex align-center justify-start">
                <v-progress-circular v-if="dirtyCells && dirtyCells.selectedNow.percentage >= 0"
                    :rotate="360"
                    :size="50"
                    :width="2"
                    :value="dirtyCells.selectedNow.percentage"
                    color="primary">
                    {{ dirtyCells.selectedNow.percentage + '%' }}
                  </v-progress-circular>
                   <v-progress-circular v-else
                        indeterminate
                        color="primary"
                    ></v-progress-circular>
            </v-col>
            <v-col lg="1" offset-lg="1" class="d-flex align-center justify-end"> 
                <v-checkbox
                    v-model="checkbox"
                    dark
                    color="indigo lighten-2"
                ></v-checkbox>
            </v-col>
        </v-row>
    </v-card>
</template>

<script>
export default {

    props: ["keyGraphicState"],

    data() {
        return {
            checkbox: true
        }
    },

    computed: {
        graphic() {
            return this.$store.getters['wafermeas/getGraphicByGraphicState'](this.keyGraphicState)
        },
        dirtyCells() {
            return this.$store.getters['wafermeas/getDirtyCellsByGraphic'](this.keyGraphicState)
        }
    },

}
</script>