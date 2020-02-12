<template>
     <v-container>
        <v-row>
            <v-col lg="8" offset-lg="1">
                <v-card>                
                    <v-toolbar color="indigo" dark>
                        <v-row>
                            <v-col lg="3" offset-lg="1">
                               {{'Таблица параметров'}}
                            </v-col>
                            <v-col lg="3">
                                <v-btn absolute dark fab small right color="pink" @click="openCreatingDialog">
                                    <v-icon>add</v-icon>
                                </v-btn>                                          
                            </v-col>
                        </v-row>
                    </v-toolbar>
                    <v-list class="align-lg-center">
                        <v-row v-for="parameter in parameterList" :key="parameter.id" dense>
                            <v-col lg="10" offset-lg="1">
                                <v-hover>
                                    <template v-slot="{ hover }">
                                        <v-card :elevation="hover ? 24 : 4">
                                            <v-list-item>                                   
                                                <v-list-item-content>
                                                    <v-list-item-title v-text="parameter.parameterName"></v-list-item-title>
                                                </v-list-item-content>                          
                                            </v-list-item>
                                        </v-card>
                                    </template>
                                </v-hover>
                            </v-col>
                            <v-col class="d-flex justify-end" lg="1">
                                <v-btn dark fab small color="indigo"  @click="openUpdateDialog(stage)">
                                    <v-icon color="white">edit</v-icon>
                                </v-btn>
                                <v-btn dark fab small color="indigo" @click="deleteParameter(parameter.id)">
                                    <v-icon color="pink">delete</v-icon>
                                </v-btn>
                            </v-col>
                        </v-row>
                    </v-list>
                </v-card>
            </v-col>
        </v-row>
        <!-- <v-row justify="center">
            <v-dialog v-model="editing.dialog" persistent max-width="450px">
                <v-card>
                    <v-card-title>Редактирование параметра</v-card-title>
                    <v-divider></v-divider>
                    <v-card-text style="height: 200px;">           
                        <v-text-field class="mt-8" outlined label="Старое название этапа" readonly v-model="editing.stage.stageName"></v-text-field>          
                        <v-text-field outlined label="Новое название этапа" v-model="editing.newName"></v-text-field>
                    </v-card-text>
                    <v-card-actions class="d-flex justify-lg-space-between">          
                        <v-btn color="indigo" @click="wipeEditing()">Закрыть</v-btn>
                        <v-btn v-if="editing.newName && editing.newName!==editing.stage.stageName" color="success" @click="updateStageName(editing.stage, editing.newName)">Обновить название</v-btn>
                        <v-chip v-else label color="pink">
                            <v-icon left>warning</v-icon>
                            {{!editing.newName ? "Введите название" : "Название совпадает с существующим"}}
                        </v-chip>
                    </v-card-actions>
                </v-card>
            </v-dialog>
        </v-row> -->
        <v-row justify="center">
            <v-dialog v-model="creating.dialog" persistent max-width="750px">
                <v-card v-if="creating.dialog">
                    <v-card-title>Создание стандартного параметра</v-card-title>
                    <v-divider></v-divider>
                    <v-card-text style="height: 400px;" class="mt-4">  
                        <v-text-field outlined label="Краткое название" v-model="creating.parameter.parameterName" :rules="[rules.required, isParameterNameExist]"></v-text-field>
                        <v-text-field outlined label="Развернутое название" v-model="creating.parameter.russianParameterName" :rules="[rules.required, isRussianParameterNameExist]"></v-text-field>
                        <v-text-field outlined label="Системное название" v-model="creating.parameter.parameterNameStat" :rules="[rules.required, isParameterNameStatExist]"></v-text-field>
                        <v-checkbox v-model="creating.parameter.dividerNeed" label="Нужно ли приводить к мм"></v-checkbox>
                        <v-checkbox v-model="creating.parameter.specialRon" label="Приведение как в Ron"></v-checkbox>                      
                    </v-card-text>
                    <v-card-actions class="d-flex justify-lg-space-between">          
                        <v-btn color="indigo" @click="wipeCreating()">Закрыть</v-btn>
                        <v-btn v-if="readyToCreate" color="success" @click="createParameter(creating.parameter)">Создать параметр</v-btn>
                        <v-chip v-else label color="pink">
                            <v-icon left>warning</v-icon>
                            {{"Заполните все поля корректно"}}
                        </v-chip>
                    </v-card-actions>
                </v-card>
            </v-dialog>
        </v-row>
        <v-snackbar v-model="snackbar.visible" top>
            {{ snackbar.text }}
            <v-btn color="pink" text @click="snackbar.visible = false">Закрыть</v-btn>
        </v-snackbar>
    </v-container>
</template>

<script>

export default {
    
    data() {
        return {
           parameterList: [],
           snackbar: {visible: false, text: ""},
           creating: {dialog: false, parameter: {parameterName: "", russianParameterName: "", parameterNameStat: "", dividerNeed: false, specialRon: false}},
           editing: {dialog: false, stage: {}, newName: ""},
           rules: {
               required: value => !!value || 'Заполните поле'               
           }
        }
    }, 

    
  

    methods: {

        isParameterNameExist(value) { 
            return this.parameterList.every(x => x.parameterName !== value) || 'Параметр с таким значением уже существует'
        },

        isRussianParameterNameExist(value) {
            return this.parameterList.every(x => x.russianParameterName !== value) || 'Параметр с таким значением уже существует' 
        },

        isParameterNameStatExist(value) {
            return this.parameterList.every(x => x.parameterNameStat !== value) || 'Параметр с таким значением уже существует'
        },

        showSnackbar(text) {
            this.snackbar.visible = true
            this.snackbar.text = text
        },


        openCreatingDialog() {
            this.creating.dialog = true
        },

        openUpdateDialog(stage) {
            this.editing.dialog = true
            this.editing.stage = Object.assign({}, stage)
        },

        wipeCreating() {
            this.creating = {dialog: false, parameter: {parameterName: "", russianParameterName: "", parameterNameStat: "", dividerNeed: false, specialRon: false}}
        },

        wipeEditing() {
            this.editing.dialog = false
            this.editing.stage = {}
            this.editing.newName = ""
        },

        async initialize() {
            await this.getParameters().then(data => this.parameterList = data)
        },

        async getParameters() {
            return await this.$http
            .get(`/api/standartparameter/all`)
            .then(response => {return response.data ? response.data : []})
            .catch(error => this.showSnackbar("Параметры не найдены в БД"))

        },

        async createParameter() {
            let parameterViewModel = {...this.creating.parameter}
            await this.$http.put('/api/standartparameter/create', parameterViewModel)
            .then(response => {
                this.showSnackbar(`Этап ${response.data.parameterName} успешно добавлен`)
                this.parameterList.push(response.data)
                this.creating.dialog = false
            })
            .catch(error => error.response.status === 403 ? this.showSnackbar("Ошибка валидации") : this.showSnackbar("Ошибка сервера"))
        },

        async updateParameter() {
            let stageViewModel = {id: stage.stageId, name: newName, processId: this.processId}
            await this.$http.post('/api/stage/update', stageViewModel)
            .then((response) => {
                this.showSnackbar("Название изменено")
                this.stagesList.find(x => x.id == response.data.id).stageName = response.data.stageName
                this.wipeEditing()
            })
            .catch((error) => {
                this.showSnackbar("Ошибка при изменении названия")
            });
        },

        async deleteParameter(parameterId) {
            await this.$http.delete(`/api/standartparameter/delete/${parameterId}`)
            .then(response => {
                this.showSnackbar("Параметр успешно удален")
                this.parameterList = this.parameterList.filter(x => x.id !== parameterId)
            })
            .catch(error => {
                error.response.status === 403 ? this.showSnackbar("Запрещено удалять этап") : this.showSnackbar("Ошибка при удалении")
            })
        }
    },

    computed: {
        readyToCreate() {
            return this.creating.parameter.parameterName 
            && this.creating.parameter.russianParameterName 
            && this.creating.parameter.parameterNameStat 
            && this.isParameterNameExist(this.creating.parameter.parameterName)
            && this.isRussianParameterNameExist(this.creating.parameter.russianParameterName)
            && this.isParameterNameStatExist(this.creating.parameter.parameterNameStat )
        }
    },

    watch: {    
        // processId: {
        //     immediate: true,
        //     handler: 
        //         async function(newVal, oldVal) {
        //             this.$router.push({ name: 'stagetable', params: {processId: newVal}})
        //             await this.getStagesByProcessId(newVal).then(data => this.stagesList = data )
        //     }
        // } 
    },

    async mounted() {
        await this.initialize()
    }
}
</script>
