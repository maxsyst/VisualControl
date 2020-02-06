<template>
    <v-container>
        <v-row>
            <v-col lg="6" offset-lg="1">
                <v-select   v-model="selectedElementType" 
                            :items="elementTypes"
                            no-data-text="Нет данных"
                            outlined
                            filled
                            item-text="name"
                            item-value="id"
                            label="Тип элемента">
                </v-select>
            </v-col>
            <v-col lg="2">
                <v-btn color="indigo" @click="updateElementType(selectedElementType)">Изменить тип</v-btn>
            </v-col>
            <v-col lg="2">
                <v-btn color="indigo" @click="deleteElementType(selectedElementType)">Удалить тип</v-btn>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
export default {
    data() {
        return {
            snackbar: {visible: false, text: ""},
            elementTypes: [],
            selectedElementType: {},
            specificElementTypes: []
        }
    },

    methods: {

        showSnackbar(text) {
            this.snackbar.visible = true
            this.snackbar.text = text
        },

        async initialize() {
            await getElementTypes()
        },

        async getElementTypes() {
            await this.$http.get(`/api/elementtype/all`)
            .then(response => {
                this.elementTypes = response.data
            })
            .catch((error) => {
                this.showSnackbar("Ошибка сервера")
            });
        },

        async updateElementType(selectedElementType) {
            
        },

        async deleteElementType(selectedElementType) {

        }
        
    },

    watch: {
        selectedElementType: async function(newVal, oldVal) {

        }
    },

    async mounted() {
        await initialize()
    }
}
</script>

<style>

</style>