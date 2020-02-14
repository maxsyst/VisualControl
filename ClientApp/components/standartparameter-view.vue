<template>
     <v-container>
        <v-row>
            <v-col lg="8" offset-lg="1">
                <v-card>                
                    <v-toolbar color="indigo" dark>
                        <v-row class="d-flex justify-space-between"> 
                            <v-col lg="3" offset-lg="1">
                               <h3>Таблица параметров</h3>
                            </v-col>
                            <v-col lg="3">
                                <v-btn dark fab small color="pink" @click="openCreatingDialog">
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
                                <v-btn dark fab small color="indigo"  @click="openUpdateDialog(parameter)">
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
        <v-row justify="center">
            <v-dialog v-model="editing.dialog" persistent max-width="750px">
                <v-card>
                    <v-card-title>Редактирование параметра</v-card-title>
                    <v-divider></v-divider>
                    <v-card-text style="height: 400px;" class="mt-4">            
                        <v-text-field outlined label="Краткое название" v-model="editing.parameter.parameterName" :rules="[rules.required, isParameterNameExist]"></v-text-field>
                        <v-text-field outlined label="Развернутое название" v-model="editing.parameter.russianParameterName" :rules="[rules.required, isRussianParameterNameExist]"></v-text-field>
                        <v-text-field outlined label="Системное название" v-model="editing.parameter.parameterNameStat" :rules="[rules.required, isParameterNameStatExist]"></v-text-field>
                        <v-checkbox v-model="editing.parameter.dividerNeed" label="Нужно ли приводить к мм"></v-checkbox>
                        <v-checkbox v-model="editing.parameter.specialRon" label="Приведение как в Ron"></v-checkbox> 
                    </v-card-text>
                    <v-card-actions class="d-flex justify-lg-space-between">          
                        <v-btn color="indigo" @click="wipe(editing)">Закрыть</v-btn>
                        <v-btn v-if="readyToUpdate" color="success" @click="updateParameter(editing.parameter)">Обновить параметр</v-btn>
                        <v-chip v-else label color="pink">
                            <v-icon left>warning</v-icon>
                             {{"Заполните все поля корректно"}}
                        </v-chip>
                    </v-card-actions>
                </v-card>
            </v-dialog>
        </v-row>
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
                        <v-btn color="indigo" @click="wipe(creating)">Закрыть</v-btn>
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
           creating: {dialog: false, parameter: {}},
           editing: {dialog: false, parameter: {}},
           rules: {
               required: value => !!value || 'Заполните поле'               
           }
        }
    }, 

    methods: {

        isParameterNameExist(value) { 
            return this.parameterList.every(x => (x.id === this.editing.parameter.id || x.parameterName !== value)) || 'Параметр с таким значением уже существует'
        },

        isRussianParameterNameExist(value) {
            return this.parameterList.every(x => (x.id === this.editing.parameter.id || x.russianParameterName !== value)) || 'Параметр с таким значением уже существует' 
        },

        isParameterNameStatExist(value) {
            return this.parameterList.every(x => (x.id === this.editing.parameter.id || x.parameterNameStat !== value)) || 'Параметр с таким значением уже существует'
        },

        showSnackbar(text) {
            this.snackbar.visible = true
            this.snackbar.text = text
        },


        openCreatingDialog() {
            this.creating.dialog = true
        },

        openUpdateDialog(parameter) {
            this.editing.dialog = true
            this.editing.parameter = {...parameter}
        },

        wipe(action) {
            action.dialog = false
            action.parameter = {...action.parameter, ...{id: 0, parameterName: "", russianParameterName: "", parameterNameStat: "", dividerNeed: false, specialRon: false}}
        },

        ready(action) {
            return action.dialog && action.parameter.parameterName 
            && action.parameter.russianParameterName 
            && action.parameter.parameterNameStat 
            && typeof(this.isParameterNameExist(action.parameter.parameterName)) === 'boolean'
            && typeof(this.isRussianParameterNameExist(action.parameter.russianParameterName)) === 'boolean'
            && typeof(this.isParameterNameStatExist(action.parameter.parameterNameStat)) === 'boolean'
        },

        async initialize() {
            this.wipe(this.creating)
            this.wipe(this.editing)
            await this.getParameters().then(data => this.parameterList = data || [])
        },

        async getParameters() {
            return await this.$http
            .get(`/api/standartparameter/all`)
            .then(response => response.data)
            .catch(error => this.showSnackbar("Параметры не найдены в БД"))

        },

        async createParameter() {
            let parameterViewModel = {...this.creating.parameter}
            await this.$http.put('/api/standartparameter/create', parameterViewModel)
            .then(response => {
                this.showSnackbar(`Параметр ${response.data.parameterName} успешно добавлен`)
                this.parameterList.push(response.data)
                this.wipe(this.creating) 
            })
            .catch(error => error.response.status === 403 ? this.showSnackbar("Ошибка валидации") : this.showSnackbar("Ошибка сервера"))
        },

        async updateParameter() {
            let parameterViewModel = {...this.editing.parameter}
            await this.$http.patch('/api/standartparameter/update', parameterViewModel)
            .then(response => {
                this.showSnackbar(`Параметр ${response.data.parameterName} успешно обновлен`)
                Object.assign(this.parameterList[this.parameterList.findIndex(x => x.id === response.data.id)], response.data)
                this.wipe(this.editing) 
            })
            .catch(error => error.response.status === 403 ? this.showSnackbar("Ошибка валидации") : this.showSnackbar("Ошибка сервера"))
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
            return this.ready(this.creating)
        },
        readyToUpdate() {
            return this.ready(this.editing)
        }

    },   

    async mounted() {
        await this.initialize()
    }
}
</script>
