<template>
    <v-container>
        <v-row v-for="digitMeasurement in digitMeasurementDictionary" :key="digitMeasurement.digitMeasurementName">
            <v-col lg="8" offset-lg="1">
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
                        <wp-card :measurementRecordingId="elementMeasurement.idmr" :waferId="waferId" :digit="digitMeasurement.digitMeasurementName" :elementName="elementMeasurement.virtualElement"></wp-card>
                    </v-col>
                </v-row>
            </v-col>
            <v-col lg="3">

            </v-col>
        </v-row>
    </v-container>   
</template>

<script>
import Card from "./waferpath-card.vue";
export default {
    data() {
        return {
            waferId: "",
            waferMap: {},
            initialArray: [],
            digitMeasurementDictionary: []
        }
    },

    components: {
        "wp-card": Card
    },

    async mounted() {
        this.waferId = this.$route.params.waferId
        this.waferMap = (await this.$http.get(`/api/wafermap/getformedwafermap?waferMapFieldViewModelJSON=${JSON.stringify({waferId: this.waferId, fieldHeight: 200, fieldWidth: 200, streetSize: 2})}`)).data
        await this.$http.get(`/api/measurementrecording/wafer/${this.waferId}/dietype/0`)
            .then(response => {
                if(response.status === 200) {
                    this.initialArray = response.data.reduce((p,c) => {
                        return [...p, ...c.measurementRecordingList.map(function(mr) { return {   
                            key: mr.name.split('.')[1].split('_')[0],
                            trueElement: mr.element,
                            virtualElement: mr.name.split('_').slice(1).reduce((p,c) => p + "_" + c, "").slice(1),
                            idmr: mr.id,
                            stage: {id: c.id, name: c.name}
                        }})]
                    }, [])
                    let initialArray = [...this.initialArray]
                    this.digitMeasurementDictionary = [...new Set(initialArray.map(x => x.key).filter(f => Number.isInteger(+f.charAt(0))).sort((a,b) => +a - +b))]
                                                      .map(function(digitName) {
                        let digitMeasurementArray = initialArray.filter(x => x.key === digitName)
                        return {
                            digitMeasurementName: digitName,
                            stageArray: [...new Map(digitMeasurementArray.map(x => x.stage).map(item => [item["id"], item])).values()],
                            elementMeasurementArray: digitMeasurementArray.map(function(x, index) {
                                return {
                                    trueElement: x.trueElement,
                                    virtualElement: x.virtualElement.length > 0 ? x.virtualElement : "EL_" + (index + 1),
                                    idmr: x.idmr,
                                    statDirtyCellsPercentage: 0.0
                                }
                            }) 
                        }
                    })
                }
                if(response.status === 204) {
                    this.showSnackbar("Ошибка при загрузке информации")
                }                            
            })
            .catch((error) => {
                this.showSnackbar(error)
            });
        
    },

    methods: {
        showSnackbar(text) {
            this.$store.dispatch("alert/success", text)
        }
    },

    computed: {

    }
}
</script>