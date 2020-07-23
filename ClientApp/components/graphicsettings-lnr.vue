<template>
    <v-container>
         <v-row>
            <v-col lg="6">
                <v-chip label color="indigo">
                    Ось X
                </v-chip>
            </v-col>
            <v-col lg="6">
                <v-chip label color="indigo">
                    Ось Y
                </v-chip>
            </v-col>
        </v-row>
        <v-row>
            <v-col lg="6">
                <v-text-field                  
                  v-model="settings.XAxis.min"
                  label="Минимум">
                </v-text-field>
            </v-col>
            <v-col lg="6">
                <v-text-field                  
                  v-model="settings.YAxis.min"
                  label="Минимум">
                </v-text-field>
            </v-col>
        </v-row>
         <v-row>
            <v-col lg="6">
                <v-text-field                  
                  v-model="settings.XAxis.max"
                  label="Максимум">
                </v-text-field>
            </v-col>
            <v-col lg="6">
                <v-text-field                  
                  v-model="settings.YAxis.max"
                  label="Максимум">
                </v-text-field>
            </v-col>
        </v-row>
         <v-row>
            <v-col lg="6">
                <v-text-field                  
                  v-model="settings.XAxis.step"
                  label="Шаг">
                </v-text-field>
            </v-col>
            <v-col lg="6">
                <v-text-field                  
                  v-model="settings.YAxis.step"
                  label="Шаг">
                </v-text-field>
            </v-col>
        </v-row>
        <v-row>
            <v-col lg="6">
            </v-col>
             <v-col lg="6">
                <v-btn block @click="applySettings">Применить настройки</v-btn>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>

export default {
    props: ["keyGraphicState"],
    data() {
        return {

        }
    },

    methods: {
        
    },

    computed: {
        settings() {

        }
    },
}
</script>