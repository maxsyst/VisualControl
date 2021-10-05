<template>
  <v-card>
    <v-card-text>
        <div class="d-flex flex-column">
            <div class="d-flex">
                <v-select v-if="stagesList.length > 0"
                    v-model="selectedStage"
                    :items="stagesList"
                    no-data-text="Нет данных"
                    item-text="stageName"
                    return-object
                    outlined
                    label="Выберите этап:">
                </v-select>
            </div>
            <div class="d-flex">
                <v-select v-if="!_.isEmpty(selectedStage) && statParametersList.length > 0"
                    v-model="selectedStatParameter"
                    :items="statParametersList"
                    no-data-text="Нет данных"
                    item-text="name"
                    return-object
                    outlined
                    label="Выберите параметр:">
                </v-select>
                <v-chip v-else-if="statParametersList.length===0" label color="pink">
                    Параметры на этом этапе не найдены
                </v-chip>
            </div>
            <div class="d-flex">
            
            </div>
        </div>
    </v-card-text>
  </v-card>
</template>

<script>
import { mapGetters } from 'vuex';
export default {
    data() {
        return {
           selectedStage: {},
           selectedStatParameter: {}
        }
    },

    watch: {
        selectedStage: function(selectedStage) {
            this.$store.dispatch("controlCharts/changeSelectedStage", selectedStage)
            this.$store.dispatch("controlCharts/getStatParametersByStage", {ctx: this, stageId: selectedStage.stageId})
        },

        selectedStatParameter: function(selectedStatParameter) {
            this.$store.dispatch("controlCharts/changeSelectedStatParameter", selectedStatParameter)
        }
    },

    computed: {
        ...mapGetters({
            stagesList: 'controlCharts/stagesList',
            statParametersList: 'controlCharts/statParametersList'
        })
    }
}
</script>