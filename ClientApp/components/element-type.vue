<template>
    <v-container>
        <v-row>
            <v-col lg="2" offset-lg="1">
                <v-select class="mt-3"  v-model="selectedElementType" 
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
                <v-btn color="indigo" block @click="updateElementType(selectedElementType)">Изменить тип</v-btn>
                <v-btn color="indigo" block class="mt-2" @click="deleteElementType(selectedElementType)">Удалить тип</v-btn>
            </v-col>
           
        </v-row>
    </v-container>
</template>

<script>
export default {
    data() {
        return {
            elementTypes: [],
            selectedElementType: {},
            specificElementTypes: []
        }
    },

    methods: {

        showSnackbar(text) {
            this.$store.dispatch("alert/success", text)
        },

        async initialize() {
            await this.getElementTypes()
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
        await this.initialize();
    }
}
</script>

<style>

</style>