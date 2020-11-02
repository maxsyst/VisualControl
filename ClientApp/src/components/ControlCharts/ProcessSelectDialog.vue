<template>
    <v-dialog v-model="isProcessSelected" persistent max-width="400">
               <v-card class="mx-auto">
                    <v-card-text>
                       <v-select v-if="processesList.length > 0"
                            class="pt-8" 
                            v-model="selectedProcess"
                            :items="processesList"
                            no-data-text="Нет данных"
                            item-text="processName"
                            return-object
                            outlined
                            label="Выберите процесс:">
                        </v-select>
                        <v-skeleton-loader v-else
                            type="article">
                        </v-skeleton-loader>
                    </v-card-text>
                    <v-spacer></v-spacer>
                    <v-card-actions>
                        <v-btn color="indigo" to="/">Вернуться назад</v-btn>
                        <v-spacer></v-spacer>
                        <v-btn color="indigo" @click="changeProcess(selectedProcess)">Выбрать процесс</v-btn>
                    </v-card-actions>
                </v-card>
    </v-dialog>
</template>

<script>
    import { mapGetters } from 'vuex';
    export default {
        data() {
            return {
                selectedProcess: {}
            }
        },

        computed: {
            ...mapGetters({
                processesList: 'controlCharts/processesList',
                isProcessSelected: 'controlCharts/isProcessSelected'
            })
        },

        methods: {
            changeProcess: function (selectedProcess) {
                this.$store.dispatch("controlCharts/changeProcess", selectedProcess)
                this.$store.dispatch("controlCharts/getWafersWithParcels", {ctx: this, selectedProcess})
            } 
        }

    }
</script>