<template>
    <v-container>
        <v-row>
            <v-col lg="8" offset-lg="1">
                <v-card>
                    <v-card-title>
                    Измерения
                    <v-spacer></v-spacer>
                    <v-text-field
                        v-model="search"
                        append-icon="mdi-magnify"
                        label="Поиск"
                        single-line
                        hide-details
                    ></v-text-field>
                    </v-card-title>
                    <v-data-table
                        :headers="headers"
                        :items="pwaferData"
                        :search="search"
                        @click:row="go"
                    ></v-data-table>
                </v-card>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
    export default {
        data() {
            return {
                pwaferData: [],
                search: '',
                headers: [
                    {
                        text: 'WaferId',
                        align: 'center',
                        sortable: false,
                        value: 'waferId',
                    },
                    { text: 'CodeProductName',  sortable: false, value: 'codeProductName' },
                    { text: 'ProcessName',  sortable: false, value: 'processName' },
                    { text: 'MeasurementDate ',  sortable: false, value: 'measurementDate' },
                    { text: 'StartTime',  sortable: false, value: 'startTime' },
                ],
        }
    },

    methods: {
        go: async function(payload) {
            await this.$router.push({ name: 'wafermeasurement-onlywafer', params: { waferId: payload.waferId}})
        }
    },

    async created() {
        this.pwaferData = (await this.$http.get(`/api/wafer/pwafer`)).data
    }
}
</script>

