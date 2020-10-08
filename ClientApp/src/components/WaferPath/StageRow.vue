<template>
    <div>
        <v-row>
            <v-col lg="4">
                <v-chip label x-large color="indigo">
                    {{digitMeasurement.digitMeasurementName}}
                </v-chip>
            </v-col>
            <v-col lg="8">
                <v-chip label x-large color="indigo">
                    {{digitMeasurement.stageArray.length === 1 ?  digitMeasurement.stageArray[0].name : digitMeasurement.stageArray.reduce((p,c) => p.name + "/" + c.name) }}
                </v-chip>
            </v-col>
        </v-row>
        <v-row>
            <v-col lg="2" v-for="elementMeasurement in digitMeasurement.elementMeasurementArray" :key="elementMeasurement.idmr">
                <SingleCard :dirtyCellsInfo="dirtyCellsCardsInfo.find(x => x.measurementId === elementMeasurement.idmr)" :waferId="waferId" :digit="digitMeasurement.digitMeasurementName" :elementName="elementMeasurement.virtualElement"></SingleCard>
            </v-col>
        </v-row>
    </div>
</template>

<script>
const SingleCard = () => import("./SingleCard.vue")
export default {
    props: ["digitMeasurement", "waferId"],
    data() {
        return {
            dirtyCellsCardsInfo : []
        }
    },
    components: {
        "SingleCard": SingleCard
    },

    async mounted() {
       this.dirtyCellsCardsInfo = await this.getDirtyCellsCardsInfo(this.digitMeasurement.elementMeasurementArray.map(x => x.idmr))
    },

    methods: {
        async getDirtyCellsCardsInfo(elementMeasurementIdArray) {
            return (await this.$http.get(`/api/statistic/GetDirtyCellsByMeasurementRecordingArray`, {
                                        params: {
                                            measurementRecordingIdArray: [...elementMeasurementIdArray]
                                        },
                                        paramsSerializer: params => {
                                            return this.$qs.stringify(params)
                                        }
            })).data
        }
    }
}
</script>

<style>

</style>