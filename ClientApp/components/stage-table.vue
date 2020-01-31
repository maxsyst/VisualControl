<template>
    <v-container>
        <v-row>
            <v-col lg="8" offset-lg="1">
                <v-card>                
                    <v-toolbar color="indigo" dark>
                        <v-row>
                            <v-col lg="3" offset-lg="1">
                                <v-select class="mt-8" style="right:16px;"
                                    :items="processesList"
                                    v-model="processId"
                                    outlined
                                    no-data-text="Нет данных"
                                    item-text="processName"
                                    item-value="processId"
                                    label="Название процесса">
                                </v-select>
                            </v-col>
                            <v-col lg="3">
                                <v-btn class="mt-8" absolute dark fab small right color="pink" @click="openCreatingDialog()">
                                    <v-icon>add</v-icon>
                                </v-btn>                                          
                            </v-col>
                        </v-row>
                    </v-toolbar>
                    <v-list class="align-lg-center">
                        <v-row v-for="stage in stagesList" :key="stage.stageId" dense>
                            <v-col lg="10" offset-lg="1">
                                <v-hover>
                                    <template v-slot="{ hover }">
                                        <v-card :elevation="hover ? 24 : 4">
                                            <v-list-item>                                   
                                                <v-list-item-content>
                                                    <v-list-item-title v-text="stage.stageName"></v-list-item-title>
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
                                <v-btn dark fab small color="indigo" @click="deleteStage(stage.stageId)">
                                    <v-icon color="pink">delete</v-icon>
                                </v-btn>
                            </v-col>
                        </v-row>
                    </v-list>
                </v-card>
            </v-col>
        </v-row>
        <v-row justify="center">
            <v-dialog v-model="editing.dialog" persistent max-width="450px">
                <v-card>
                    <v-card-title>Редактирование этапа</v-card-title>
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
        </v-row>
        <v-row justify="center">
            <v-dialog v-model="creating.dialog" persistent max-width="450px">
                <v-card v-if="creating.dialog">
                    <v-card-title>Создание этапа</v-card-title>
                    <v-divider></v-divider>
                    <v-card-text style="height: 200px;">  
                        <v-text-field class="mt-8" outlined label="Название процесса" readonly v-model="processesList.find(x => x.processId == processId).processName"></v-text-field>          
                        <v-text-field outlined label="Название этапа" v-model="creating.stage.name"></v-text-field>
                    </v-card-text>
                    <v-card-actions class="d-flex justify-lg-space-between">          
                        <v-btn color="indigo" @click="wipeCreating()">Закрыть</v-btn>
                        <v-btn v-if="creating.stage.name && stagesList.every(x=>x.stageName!==creating.stage.name)" color="success" @click="createStage(processId, creating.stage.name)">Создать этап</v-btn>
                        <v-chip v-else label color="pink">
                            <v-icon left>warning</v-icon>
                            {{!creating.stage.name ? "Введите название" : "Название совпадает с существующим"}}
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
    props: {
        processId: Number
    },

    data() {
        return {
           stagesList: [],
           processesList: [],
           snackbar: {visible: false, text: ""},
           creating: {dialog: false, stage: {name: ""}},
           editing: {dialog: false, stage: {}, newName: ""}
        }
    },    

    methods: {

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
            this.creating.dialog = false
            this.creating.stage.name = ""            
        },

        wipeEditing() {
            this.editing.dialog = false
            this.editing.stage = {}
            this.editing.newName = ""
        },

        async initialize() {
            await this.getProcesses().then(data => this.processesList = data)
        },

        async getProcesses() {
            return await this.$http
            .get(`/api/process/all`)
            .then(response => {return response.data})
            .catch(error => this.showSnackbar(error))

        },

        async getStagesByProcessId(processId) {
            return await this.$http
            .get(`/api/stage/process/${processId}`)
            .then(response => {return response.data})
            .catch(error => {
                this.stagesList = [] 
                error.response.status === 404 ? this.showSnackbar("Этапов не найдено") : this.showSnackbar(error)
            });
        },

        async createStage(processId, stageName) {
            let stageViewModel = {processId: processId, name: stageName}
            await this.$http.put('/api/stage/create', stageViewModel)
            .then(response => {
                this.showSnackbar(`Этап ${response.data.stageName} успешно добавлен`)
                this.stagesList.push(response.data)
                this.creating.dialog = false
            })
            .catch(error => error.response.status === 403 ? this.showSnackbar("Ошибка валидации") : this.showSnackbar("Ошибка сервера"))
        },

        async updateStageName(stage, newName) {
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

        async deleteStage(stageId) {
            await this.$http.delete(`/api/stage/delete/${stageId}`)
            .then(response => {
                this.showSnackbar("Успешно удалено")
                this.stagesList = this.stagesList.filter(x => x.stageId != stageId)
            })
            .catch(error => {
                error.response.status === 403 ? this.showSnackbar("Запрещено удалять этап привязанный к измерению") : this.showSnackbar("Ошибка при удалении")
            })
        }
    },

    watch: {    
        processId: async function(newVal, oldVal) {
            await this.getStagesByProcessId(newVal).then(data => this.stagesList = data )
        }
    },

    async mounted() {
        await this.initialize()
    }
}
</script>
