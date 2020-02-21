<template>
    <v-container>
        <v-row>
            <v-col lg="2">
                <v-select :value="smp.element"
                    :items="this.$store.getters['smpstorage/elements']"
                    item-text="name"
                    no-data-text="Нет данных"
                    return-object
                    outlined
                    @change="updateElement($event)"
                    label="Выберите элемент:">
                </v-select>
            </v-col>
            <v-col lg="4">
                <v-select :value="smp.stage"
                    :items="this.$store.getters['smpstorage/stages']"
                    item-text="stageName"
                    no-data-text="Нет данных"
                    return-object
                    outlined
                    @change="updateStage($event)"
                    label="Выберите этап:">
                </v-select>
            </v-col>
            <v-col lg="2">
                <v-select :value="smp.divider"
                    :items="$store.getters['dividers/getAll']"
                    item-text="name"
                    no-data-text="Нет данных"
                    return-object
                    outlined
                    @change="updateDivider($event)"
                    label="Выберите периферию:">
                </v-select>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
export default {
    props: {
        guid: String
    },

    data() {
       return {
           
       }
    },

    methods: {
        updateElement(element) {
            this.$store.dispatch("smpstorage/updateElementSmp", {guid: this.guid, element})        
        },

        updateStage(stage) {
            this.$store.dispatch("smpstorage/updateStageSmp", {guid: this.guid, stage})        
        },

        updateDivider(divider) {
            this.$store.dispatch("smpstorage/updateDividerSmp", {guid: this.guid, divider})
        }
    },

    computed: {
        smp() {
            return this.$store.getters['smpstorage/currentSmp'](this.guid)
        }
    }
}
</script>

