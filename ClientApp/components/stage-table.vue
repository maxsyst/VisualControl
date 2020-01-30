<template>
    <v-container>
        <v-row>
            <v-col lg="8" offset-lg="1">
                <v-card>                
                    <v-toolbar color="indigo" dark>
                        <v-btn absolute
                                dark
                                fab
                                small
                                right
                                color="pink"
                                @click="openAddDialog()">
                            <v-icon>add</v-icon>
                        </v-btn>
                        <v-toolbar-title>Этапы на процессе</v-toolbar-title>                    
                    </v-toolbar>
                    <v-list>
                        <v-divider></v-divider>
                        <v-list-item v-for="stage in stagesList" :key="stage.stageId">  
                            <v-list-item-content>
                                <v-list-item-title v-text="stage.stageName"></v-list-item-title>
                            </v-list-item-content>                           
                            <v-list-item-action>
                                <v-btn icon ripple @click="openUpdateDialog(stage)">
                                    <v-icon color="primary">edit</v-icon>
                                </v-btn>
                                <v-btn icon ripple @click="deleteStage(stage.stageId)">
                                    <v-icon color="primary">delete</v-icon>
                                </v-btn>
                            </v-list-item-action>
                        </v-list-item>
                        <v-divider></v-divider>
                    </v-list>
                </v-card>
            </v-col>
        </v-row>
        <v-row justify="center">
            <v-dialog v-model="editing.dialog" persistent max-width="450px">
                <v-card>
                    <v-card-title>Редактирование этапа</v-card-title>
                    <v-card-text style="height: 200px;">           
                        <v-text-field outlined label="Старое название этапа" readonly v-model="editing.stage.stageName"></v-text-field>          
                        <v-text-field outlined label="Новое название этапа" v-model="editing.newName"></v-text-field>
                    </v-card-text>
                    <v-card-actions class="d-flex justify-lg-space-between">          
                    <v-btn color="indigo" @click="wipeEditing()">Закрыть</v-btn>
                    <v-btn v-if="editing.newName && editing.newName!==editing.stage.stageName" color="success" @click="updateStageName(editing.stage, editing.newName)">Обновить название</v-btn>
                    </v-card-actions>
                </v-card>
            </v-dialog>
        </v-row>
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
            this.processesList = await getProcesses()
        },

        async getProcesses() {
            await this.$http
            .get(`/api/process/all`)
            .then(response => {return response.data})
            .catch(error => this.showSnackbar(error))

        },

        async getStagesByProcessId(processId) {
            await this.$http
            .get(`/api/stage/process/${processId}`)
            .then(response => {return response.data})
            .catch(error => this.showSnackBar(error));
        },

        async createStage(processId) {
            let stageViewModel = {processId: processId, name: creating.stage.name}
            await this.$http.put('/api/create', stageViewModel)
            .then(response => {
                this.showSnackbar(`Этап ${response.data.stageName} успешно добавлен`)
                this.stagesList.push(response.data)
                this.creating.dialog = false
            })
            .catch(error => error.status === 403 ? this.showSnackbar("Ошибка валидации") : this.showSnackbar("Ошибка сервера"))
        },

        async updateStageName(stage, newName) {
            let stageViewModel = {id: stage.id, name: newName}
            await this.$http.post('/api/update', stageViewModel)
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
            await this.$http.delete(`/api/delete/${stageId}`)
            .then(response => {
                this.showSnackbar("Успешно удалено")
                this.stagesList = this.stagesList.filter(x => x.stageId != stageId)
            })
            .catch(error => {
                error.status === 404 ? this.showSnackbar("Запрещено удалять этап привязанный к измерению") : this.showSnackbar("Ошибка при удалении")
            })
        }
    },

    watch: {    
        processId: async function(newVal, oldVal) {
            this.stagesList = await getStagesByProcessId(newVal)
        }
    },

    async mounted() {
        await this.initialize()
    }
}
</script>
