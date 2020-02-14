<template>
    <v-container>
        <v-row>
            <v-flex lg11 offset-lg1>
               <v-stepper v-if="elements.length>0" v-model="step" vertical>               
                    <template v-for="(kp, index) in kpArray">
                        <v-stepper-step 
                            :key="`${index+1}-step`"
                            :step="index + 1"
                            complete
                            editable>
                            {{"Операция: " + element.operation.number + " Элемент: " + element.element.name}}
                        </v-stepper-step>
                        <v-stepper-content
                            :key="`${index+1}-content`"
                            :step="index + 1">
                            <v-card>
                                <v-card-text>
                                <single-kp></single-kp>
                                </v-card-text>
                            </v-card>                                        
                        </v-stepper-content>
                    </template>                          
            </v-stepper>
           </v-flex>
        </v-row>
         <v-row>
            <v-dialog v-model="initialDialog" persistent max-width="400">
               <v-card class="mx-auto">
                    <v-card-text>
                        <v-row>
                            <v-flex lg="11" offset-lg="1">
                                <v-select class="pt-8" v-model="selectedDieType"
                                    :items="dieTypes"
                                    no-data-text="Нет данных"
                                    outlined
                                    label="Выберите тип кристалла:">
                                </v-select>
                            </v-flex>
                        </v-row>
                        <v-row v-if="selectedDieType">
                            <v-flex lg="11" offset-lg="1">
                                <v-select v-if="mode==='updating'" class="pt-8" v-model="selectedPattern"
                                    :items="patterns"
                                    no-data-text="Нет данных"
                                    outlined
                                    label="Выберите шаблон:">
                                </v-select>
                                <v-btn v-else outlined block @click="goToCreatingMode" color="indigo">Создать новый шаблон</v-btn>
                            </v-flex>
                        </v-row>
                        <v-row v-if="selectedDieType">
                            <v-flex lg="11" offset-lg="1">
                                <v-btn v-if="mode==='updating'" block @click="goToUpdatingMode" color="indigo">Подтвердить выбор</v-btn>
                                <v-btn v-else block @click="goToUpdatingMode(selectedDieType)" color="indigo">Редактировать шаблон</v-btn>
                            </v-flex>
                        </v-row>
                     
                    </v-card-text>
                </v-card>
            </v-dialog>
        </v-row>
    </v-container>
</template>

<script>
import singleKp from './kurbatovparametersingle-view.vue';
export default {
    data() {
        return {
            initialDialog: true,
            selectedDieType: {},
            dieTypes: [],
            selectedPattern: {},
            patterns: [],
            step: 0,
            kpArray: [],           
            mode: ""
        }
    },

    components:
    {
        "single-kp": singleKp,
    },

    methods: {
        async initialize() {
            await getAllDieTypes()
        },

        goToCreatingMode() {
            this.initialDialog = false
            this.mode = 'creating'
        },

        async goToUpdatingMode(selectedDieType) {
            this.mode = 'updating'
            await this.$http
            .get(`/api/standartpattern/dietype/${selectedDieType.id}`)
            .then(response => {this.patterns = response.data; this.selectedPattern = response.data[0] || {}})
            .catch(error => this.showSnackbar("Шаблоны не найдены в БД"))
        },

        async getAllDieTypes() {
            await this.$http
            .get(`/api/dietype/all`)
            .then(response => {this.dieTypes = response.data})
            .catch(error => this.showSnackbar("Типы кристаллов не найдены в БД"))
        }
    },

    async mounted() {
        await this.initialize()
    }
}
</script>

<style>

</style>