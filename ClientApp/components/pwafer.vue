<template>
    <v-container>
        <v-row>
            <v-col lg="10" offset-lg="1">
                <v-card class="elevation-8">
                    <v-card-title v-if="!loading">
                    Выбор пластины
                    <v-spacer></v-spacer>
                    <v-text-field 
                        v-model="search"
                        append-icon="search"
                        label="Поиск"
                        single-line
                        hide-details
                    ></v-text-field>
                    </v-card-title>
                     <v-skeleton-loader v-if="loading"
                          class="mx-auto"
                          type="table"
                        ></v-skeleton-loader>
                    <v-data-table v-else
                        :headers="headers"
                        :items="pwaferData"
                        :search="search"
                        :footer-props= "{
                            itemsPerPageText: 'Строк на странице:',
                            pageText: ''
                        }"
                        @click:row="go">
                        <template v-slot:item.waferId="{ item }">
                            <v-chip color="indigo" v-on="on" label v-html="item.waferId" dark></v-chip>
                        </template>
                        <template v-slot:item.measurementDate="{ item }">
                            <span>{{new Date(item.measurementDate).toLocaleString("ru-RU", { year: 'numeric', month: 'long', day: 'numeric' })}}</span>
                        </template>
                         <template v-slot:item.startTime="{ item }">
                            <span>{{new Date(item.startTime).toLocaleString("ru-RU", { year: 'numeric', month: 'long', day: 'numeric' })}}</span>
                        </template>
                    </v-data-table>
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
                loading: true,
                search: '',
                headers: [
                    { text: 'Номер пластины', align: 'center', sortable: false, value: 'waferId'},
                    { text: 'Код изделия',  sortable: false, align: 'center', value: 'codeProductName' },
                    { text: 'Процесс',  sortable: false, align: 'center', value: 'processName' },
                    { text: 'Дата последнего измерения',  sortable: false, align: 'center', value: 'measurementDate' },
                    { text: 'Номер последней операции',  sortable: false, align: 'center', value: 'measurementRecordingName' },
                    { text: 'Дата добавления пластины',  sortable: false, align: 'center', value: 'startTime' },
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
        this.loading = false
    }
}
</script>

