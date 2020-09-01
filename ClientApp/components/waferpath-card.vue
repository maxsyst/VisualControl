<template>
   <v-card color="#303030" class="mx-auto">
       <v-container>
           <v-row>
               <v-col lg="12">
                   <v-btn block color="primary" outlined>{{elementName}}</v-btn> 
               </v-col>
           </v-row>
            <v-row>
               <v-col lg="6">
                   <v-card color="#303030" class="ma-1">
                       {{dirtyCellsInfo.diesCount - dirtyCellsInfo.dirtyCellsArray.length}}/{{dirtyCellsInfo.diesCount}}
                   </v-card>
               </v-col>
               <v-col lg="6">
                   <v-card color="#303030" class="ma-1">
                    <v-progress-circular
                      :rotate="360"
                      :size="50"
                      :width="2"
                      :value="dirtyCellsInfo.goodCellsPercentage"
                      :color="cardColor">
                      {{dirtyCellsInfo.goodCellsPercentage + '%'}}
                    </v-progress-circular>
                   </v-card>
               </v-col>
           </v-row>
       </v-container>
   </v-card>
</template>

<script>
export default {
    props: ["measurementRecordingId", "elementName"],
    data() {
        return {
            dirtyCellsInfo: {}
        }
    },

    async mounted() {
        this.dirtyCellsInfo = (await this.$http.get(`/api/statistic/GetDirtyCellsByMeasurementRecordingOnly?measurementRecordingId=${this.measurementRecordingId}`)).data
    },

    computed: {
        cardColor() {
            return this.$store.getters['wafermeas/calculateColor'](this.dirtyCellsInfo.goodCellsPercentage / 100)
        }
    }
}
</script>