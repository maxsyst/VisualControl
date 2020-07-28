<template>
    <v-container>
        <v-card>
         <v-row>
            <v-col lg="4" offset-lg="5">
                <v-btn outlined color="primary" @click="resetSettings">Сбросить настройки</v-btn>
            </v-col>
        </v-row>
         <v-row>
            <v-col lg="3" offset-lg="2">
                <v-chip label color="indigo">
                    Ось X
                </v-chip>
            </v-col>
            <v-col lg="3" offset-lg="2">
                <v-chip label color="indigo">
                    Ось Y
                </v-chip>
            </v-col>
        </v-row>
        <v-row>
            <v-col class="d-flex flex-row" lg="4" offset-lg="2">
                <v-text-field                  
                  v-model="settings.xAxis.current.min"
                  :readonly="settings.xAxis.current.min === 'Авто'"
                  :error-messages=" (settings.xAxis.current.min).toString().trim() ? [] : 'Введите значение'" 
                  label="Минимум"
                  @change="validateMinMaxField(settings.xAxis.current, 'min')">
                </v-text-field>
                <v-switch
                    v-model="settings.xAxis.current.min === 'Авто'"
                    label="Авто"
                    color="primary"
                    @change="changeAuto(settings.xAxis.current, 'min')"
                ></v-switch>
            </v-col>
            <v-col class="d-flex flex-row" lg="4" offset-lg="1">
                <v-text-field 
                  v-model="settings.yAxis.current.min"
                  :readonly="settings.yAxis.current.min === 'Авто'"
                  :error-messages="settings.yAxis.current.min ? [] : 'Введите значение'" 
                  label="Минимум"
                  @change="validateMinMaxField(settings.yAxis.current, 'min')">
                </v-text-field>
                <v-switch
                    v-model="settings.yAxis.current.min === 'Авто'"
                    label="Авто"
                    color="primary"
                    @change="changeAuto(settings.yAxis.current, 'min')"
                ></v-switch>
            </v-col>
        </v-row>
         <v-row>
            <v-col class="d-flex flex-row" lg="4" offset-lg="2">
                <v-text-field                 
                  v-model="settings.xAxis.current.max"
                  :readonly="settings.xAxis.current.max === 'Авто'"
                  :error-messages="settings.xAxis.current.max ? [] : 'Введите значение'" 
                  label="Максимум"
                  @change="validateMinMaxField(settings.xAxis.current, 'max')">
                </v-text-field>
                <v-switch 
                    v-model="settings.xAxis.current.max === 'Авто'"
                    label="Авто"
                    color="primary"
                    @change="changeAuto(settings.xAxis.current, 'max')"
                ></v-switch>
            </v-col>
             <v-col class="d-flex flex-row" lg="4" offset-lg="1">
                <v-text-field                  
                  v-model="settings.yAxis.current.max"
                  :readonly="settings.yAxis.current.max === 'Авто'"
                  :error-messages="settings.yAxis.current.max ? [] : 'Введите значение'" 
                  label="Максимум"
                  @change="validateMinMaxField(settings.yAxis.current, 'max')">
                </v-text-field>
                <v-switch
                    v-model="settings.yAxis.current.max === 'Авто'"
                    label="Авто"
                    color="primary"
                    @change="changeAuto(settings.yAxis.current, 'max')"
                ></v-switch>
            </v-col>
        </v-row>
         <v-row>
            <v-col lg="4" offset-lg="2">
                <v-text-field dense                
                  v-model="settings.xAxis.current.maxTicksLimit"
                  :error-messages="settings.xAxis.current.maxTicksLimit ? [] : 'Введите значение'" 
                  @change="validateTicksField(settings.xAxis.current)"
                  label="Количество шагов">
                </v-text-field>
            </v-col>
            <v-col lg="4" offset-lg="1">
                <v-text-field dense                
                  v-model="settings.yAxis.current.maxTicksLimit"
                  :error-messages="settings.yAxis.current.maxTicksLimit ? [] : 'Введите значение'" 
                  @change="validateTicksField(settings.yAxis.current)"
                  label="Количество шагов">
                </v-text-field>
            </v-col>
        </v-row>
       
        <v-row>
            <v-col lg="4" offset-lg="5">
                <v-btn v-if="validation" color="green" @click="applySettings">Применить настройки</v-btn>
                <v-btn v-else outlined color="pink" @click="applySettings">Заполните все поля</v-btn>
            </v-col>
        </v-row>
        </v-card>
    </v-container>
</template>
<script>


export default {
    props: ["keyGraphicState"],
    data() {
        return {
            settings: {}
        }
    },

    created() {
        this.settings = _.cloneDeep(this.$store.getters['wafermeas/getGraphicSettingsKeyGraphicState'](this.keyGraphicState))
    },

    methods: {
        applySettings() {
            this.$store.dispatch("wafermeas/changeGraphicCurrentSettings", {keyGraphicState: this.keyGraphicState, axisType: "xAxis", settings: this.settings.xAxis.current})
            this.$store.dispatch("wafermeas/changeGraphicCurrentSettings", {keyGraphicState: this.keyGraphicState, axisType: "yAxis", settings: this.settings.yAxis.current})
            this.$emit('settings-changed', this.keyGraphicState)
        },

        resetSettings() {
            this.settings.xAxis.current = _.cloneDeep(this.settings.xAxis.initial)
            this.settings.yAxis.current = _.cloneDeep(this.settings.yAxis.initial)
            this.$store.dispatch("wafermeas/changeGraphicCurrentSettings", {keyGraphicState: this.keyGraphicState, axisType: "xAxis", settings: this.settings.xAxis.current})
            this.$store.dispatch("wafermeas/changeGraphicCurrentSettings", {keyGraphicState: this.keyGraphicState, axisType: "yAxis", settings: this.settings.yAxis.current})
            this.$emit('settings-changed', this.keyGraphicState)
        },

        validateMinMaxField(axis, value) {
            if(isNaN(axis[value]) || (axis['max'] < axis[value] && axis['max'] !== 'Авто') || (axis['min'] > axis[value] && axis['min'] !== 'Авто'))
                axis[value] = "Авто"
        },

        validateTicksField(axis) {
            if(isNaN(axis['maxTicksLimit']) || axis['maxTicksLimit'] < 4 || !Number.isInteger(+axis['maxTicksLimit']))
                axis['maxTicksLimit'] = 11
        },

        changeAuto(axis, value) {
            if(axis[value] === "Авто") {
                axis[value] = ''
            } else {
                axis[value] = "Авто"
            }
        }
    },

    computed: {
        validation() {
            if((this.settings.xAxis.current.min).toString().trim() && (this.settings.xAxis.current.max).toString().trim() && (this.settings.yAxis.current.min).toString().trim() && (this.settings.yAxis.current.max).toString().trim() && this.settings.xAxis.current.maxTicksLimit && this.settings.yAxis.current.maxTicksLimit) {
                return true
            }
            return false
        }
    },
}
</script>