<template>
    <v-container>
        <v-row dense>
            <v-col lg="12">
                <v-card class="elevation-8" color="#303030">
                    <v-row>
                        <v-col lg="3" offset-lg="1" class="d-flex align-center">
                            Номер пластины:
                        </v-col>
                        <v-col lg="2">
                            <v-chip color="indigo" large label v-html="waferId" dark></v-chip>
                        </v-col>
                        <v-col lg="2" offset-lg="1" class="d-flex align-center">
                            Код изделия:
                        </v-col>
                        <v-col lg="3" class="d-flex align-center justify-center">
                            <v-chip v-if="codeProduct.name==='Неизвестно'" color="pink darken-1" large label v-html="codeProduct.name" dark></v-chip>
                            <v-chip v-else color="indigo lighten-1" large label v-html="codeProduct.name" dark></v-chip>
                        </v-col>
                    </v-row>
                </v-card>
            </v-col>
        </v-row>
         <v-row dense> 
            <v-col lg="12">
                <v-card class="elevation-8" color="#303030">
                    <v-row>
                        <v-col lg="3" offset-lg="1" class="d-flex align-center">
                            Номер операции:
                        </v-col>
                        <v-col lg="4" class="d-flex align-center">
                            <v-chip v-if="measurement.name === 'Не выбрано'" color="pink darken-1" label v-html="measurement.name" dark></v-chip>
                            <v-chip v-else color="indigo" label v-html="measurement.name" dark></v-chip>
                        </v-col>
                        <v-col lg="2" offset-lg="2" class="d-flex align-center">
                            <v-speed-dial v-if="selectedMeasurementId>0 && !modes.measurement.edit && !modes.measurement.delete" v-model="fab.measurement" open-on-hover direction="right"  transition="slide-y-reverse-transition">
                                <template v-slot:activator>
                                    <v-btn v-model="fab.measurement"
                                        small
                                        color="indigo lighten-1"
                                        dark
                                        fab>
                                        <v-icon v-if="fab.measurement">close</v-icon>
                                        <v-icon v-else>settings</v-icon>
                                    </v-btn>
                                </template>
                                <v-btn 
                                    fab
                                    dark
                                    small
                                    color="green"
                                    @click="modes.measurement.edit=true">
                                    <v-icon>edit</v-icon>
                                </v-btn>
                                <v-btn
                                    fab
                                    dark
                                    small
                                    color="pink"
                                    @click="modes.measurement.delete=true">
                                    <v-icon>delete</v-icon>
                                </v-btn>
                            </v-speed-dial>
                        </v-col>
                    </v-row>
                </v-card>
            </v-col>
        </v-row>
        <v-row dense v-if="modes.measurement.edit || modes.measurement.delete"> 
                <v-col lg="12">
               
                <v-card class="elevation-8" color="#303030" v-if="modes.measurement.edit">
                    <v-row>                   
                        <v-col lg="4" offset-lg="1" class="mt-4">
                            <v-text-field                                
                                v-model="measurement.name"
                                outlined
                                readonly
                                label="Текущее название">
                            </v-text-field>
                        </v-col>
                        <v-col lg="4" class="mt-4">
                            <v-text-field                                
                                v-model="modes.measurement.newName"
                                :error-messages="validationNewMeasurementName"
                                outlined
                                label="Новое название">
                            </v-text-field>
                        </v-col>
                        <v-col lg="2">
                            <v-btn v-if="Array.isArray(validationNewMeasurementName)" outlined block color="green" class="d-flex mt-2" @click="editMeasurement">
                               Изменить
                            </v-btn>
                             <v-btn outlined block color="pink" class="d-flex mt-2" @click="cancelMeasurementEdit">
                               Отменить
                            </v-btn>
                        </v-col>                       
                    </v-row>
                </v-card>
                <v-card class="elevation-8" color="#303030" v-if="modes.measurement.delete">
                    <v-row>
                        <v-col lg="6" offset-lg="1" class="mt-4">
                             <v-text-field                                
                                v-model="modes.measurement.deletePassword"
                                type="password"
                                outlined
                                label="Пароль для удаления">
                            </v-text-field>
                        </v-col>
                        <v-col lg="2">
                            <v-btn v-if="modes.measurement.deletePassword" :error-messages="!modes.measurement.deletePassword
                                                                             ? 'Введите пароль' : []"  outlined block color="green" class="d-flex mt-2" @click="deleteMeasurement">
                               Удалить измерение
                            </v-btn>
                             <v-btn outlined block color="pink" class="d-flex mt-2" @click="cancelMeasurementEdit">
                               Отменить удаление
                            </v-btn>
                        </v-col>
                    </v-row>
                </v-card>
            </v-col>
        </v-row>
         <v-row dense>
            <v-col lg="12">
                <v-card class="elevation-8" color="#303030">
                    <v-row>
                        <v-col lg="3" offset-lg="1" class="d-flex align-center">
                            Текущий этап:
                        </v-col>
                        <v-col lg="4" class="d-flex align-center">
                             <v-chip v-if="stage.stageName === 'Неизвестно'" color="pink darken-1" label v-html="stage.stageName" dark></v-chip>
                            <v-chip v-else color="indigo" label v-html="stage.stageName" dark></v-chip>
                        </v-col>
                        <v-col lg="2" offset-lg="2" class="d-flex align-center">
                            <v-speed-dial v-if="selectedMeasurementId>0" v-model="fab.stage" open-on-hover direction="right"  transition="slide-y-reverse-transition">
                                <template v-slot:activator>
                                    <v-btn
                                        v-model="fab.stage"
                                        small
                                        color="indigo lighten-1"
                                        dark
                                        fab>
                                        <v-icon v-if="fab.stage">close</v-icon>
                                        <v-icon v-else>settings</v-icon>
                                    </v-btn>
                                </template>
                                <v-btn
                                    fab
                                    dark
                                    small
                                    color="green"
                                    @click="modes.stage.edit=true">
                                    <v-icon>edit</v-icon>
                                </v-btn>
                            </v-speed-dial>
                        </v-col>
                    </v-row>
                </v-card>
            </v-col>
        </v-row>
        <v-row dense v-if="modes.stage.edit"> 
            <v-col lg="12">
                <v-card class="elevation-8" color="#303030">
                    <v-row>  
                        <v-col lg="6" offset-lg="1" class="mt-4">
                            <v-select 
                                v-model="modes.stage.selected"
                                :items="modes.stage.avStages"
                                no-data-text="Нет данных"
                                item-text="stageName"
                                item-value="stageId"
                                outlined
                                label="Выберите новый этап">
                            </v-select>
                        </v-col>
                        <v-col lg="2">
                            <v-btn v-if="modes.stage.selected" outlined block color="green" class="d-flex mt-2" @click="changeStage">
                                Изменить этап
                            </v-btn>
                             <v-btn outlined block color="pink" class="d-flex mt-2" @click="cancelStageEdit">
                               Отменить изменение
                            </v-btn>
                        </v-col>
                    </v-row>
                </v-card>
            </v-col>
        </v-row>
        <v-row dense>
            <v-col lg="12">
                <v-card class="elevation-8" color="#303030">
                    <v-row>
                        <v-col lg="3" offset-lg="1" class="d-flex align-center">
                            Элемент:
                        </v-col>
                        <v-col lg="4" class="d-flex align-center">
                            <v-chip v-if="element.name === 'Неизвестно'" color="pink darken-1" label v-html="element.name" dark></v-chip>
                            <v-chip v-else color="indigo" label v-html="element.name" dark></v-chip>
                        </v-col>
                        <v-col lg="2" offset-lg="2" class="d-flex align-center">
                            <v-speed-dial v-if="selectedMeasurementId>0" v-model="fab.element" open-on-hover direction="right"  transition="slide-y-reverse-transition">
                                <template v-slot:activator>
                                    <v-btn
                                        v-model="fab.element"
                                        small
                                        color="indigo lighten-1"
                                        dark
                                        fab>
                                        <v-icon v-if="fab.element">close</v-icon>
                                        <v-icon v-else>settings</v-icon>
                                    </v-btn>
                                </template>
                                <v-btn 
                                    fab
                                    dark
                                    small
                                    color="green">
                                    <v-icon>edit</v-icon>
                                </v-btn>
                            </v-speed-dial>
                        </v-col>
                    </v-row>
                </v-card>
            </v-col>
        </v-row>
        <v-row dense v-if="modes.element.edit"> 
            <v-col lg="12">
                <v-card class="elevation-8" color="#303030">
                
                </v-card>
            </v-col>
        </v-row>
    </v-container>
</template>

<<script>
export default {

    props: ["waferId", "selectedMeasurementId"],

    data() {
        return {
            modes: {measurement: {edit: false, newName: "", deletePassword: "", delete: false}, stage: {edit: false, avStages: [], selected: {}}, element: {edit: false}},
            fab: {measurement: false, element: false, stage: false},
            measurement: {name: "Не выбрано"},
            codeProduct: {name: "Неизвестно"},
            element: {name: "Неизвестно"},
            stage: {stageName: "Неизвестно"}
        }
    },   

    methods: {
        editMeasurement: async function() {
            let measurementViewModel = {id: this.selectedMeasurementId, name: this.modes.measurement.newName}
            try {
                await this.$http.post(`/api/measurementrecording/edit/name`, measurementViewModel)
                this.$store.dispatch("wafermeas/updateMeasurementName", measurementViewModel)
                this.showSnackBar("Название успешно изменено")
                this.cancelMeasurementEdit()
            } 
            catch(ex) {
                this.showSnackBar("Ошибка при изменении названия")
            }
        },

        editStage: async function() {
            let measurementViewModel = {id: this.selectedMeasurementId, stageId: this.modes.stage.selected}
            try {
                await this.$http.post(`/api/measurementrecording/update-stage`, measurementViewModel)
                this.$store.dispatch("wafermeas/updateMeasurementStage", measurementViewModel)
                this.showSnackBar("Этап успешно изменен")
                Object.assign(this.stage, this.modes.stage.avStages.find(x => x.id === this.modes.stage.selected))
                this.cancelStageEdit()
            } 
            catch(ex) {
                this.showSnackBar("Ошибка при изменении этапа")
            }
        },

        deleteMeasurement: async function() {
            await this.$http.delete(`/api/measurementrecording/delete/${this.selectedMeasurementId}?superuser=${this.modes.measurement.deletePassword}`)
            .then(response => {
                this.$store.dispatch("wafermeas/deleteMeasurement", this.selectedMeasurementId)
                this.$router.push({ name: 'wafermeasurement-onlywafer', params: { waferId: this.waferId}})
                this.showSnackbar("Операция успешно удалена")
            })
            .catch(error => {
                error.response.status === 403 ? this.showSnackbar("Запрещено удаление") : this.showSnackbar("Ошибка при удалении")
            })    
        },

        cancelMeasurementEdit: function() {
            this.modes.measurement.edit = false
            this.modes.measurement.delete = false
            this.modes.measurement.newName = ""
            this.modes.measurement.deletePassword = ""
        },

        cancelStageEdit: function() {
            this.modes.stage.edit = false
            this.modes.stage.selected = Object.assign(this.modes.stage.selected, this.stage)
        },

        getCodeProduct: async function(waferId) {
            let response = await this.$http.get(`/api/codeproduct/getbywaferid?waferId=${waferId}`)
            if(response.status === 200) {
                return response.data
            }
            if(response.status === 404) {
                return {codeProductName: "Неизвестно"}
            }
            else {
                this.showSnackbar(response.data)
                return;
            }
        },

        getElement: async function(measurementRecordingId) {
            try {
                let response = await this.$http.get(`/api/element/getbyidmr?idmr=${measurementRecordingId}`)
                return response.data[0].elementId === 0 ? {name: "Неизвестно"} : response.data[0]
            }
            catch (error) {
                this.showSnackbar(error)
            }         
        },

        getStage: async function(stageId) {
            try {
                let response = await this.$http.get(`/api/stage/id/${stageId}`)
                return response.data.stageId === 0 ? {stageName: "Неизвестно"} : response.data
            }
            catch (error) {
               this.showSnackbar(error)
            }           
        },

        showSnackBar(text)
        {
            this.$store.dispatch("alert/success", text)
        }
    },

    watch: {
        waferId: async function(newValue) {
            this.codeProduct = await this.getCodeProduct(newValue)
            this.modes.stage.avStages = (await this.$http.get(`/api/stage/wafer/${newValue}`)).data
        },

        selectedMeasurementId: async function(newValue) {
            this.measurement = this.$store.getters['wafermeas/measurements'].find(x => x.id === this.selectedMeasurementId)
            this.element = await this.getElement(this.measurement.id)
            this.stage = await this.getStage(this.measurement.stageId)
            Object.assign(this.modes.stage.selected, this.stage)
        }
    },

    computed: {
        validationNewMeasurementName() {
            if(this.modes.measurement.edit === false) {
                return []
            }
            if(this.modes.measurement.newName.length === 0) {
                return "Введите название измерения"
            }
            if(this.measurementRecordings.some(x => x.name === this.modes.measurement.newName)) {
                return "Измерение с таким названием уже существует"
            }
            return []
        },

        measurementRecordings() {
            return this.$store.getters['wafermeas/measurements']
        }
    },

}
</script>