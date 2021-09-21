<template>
    <v-container>
        <v-row>
            <v-col class="d-flex">
                <v-autocomplete
                        v-model="selectedWafer"
                        :items="wafers"
                        no-data-text="Нет данных"
                        item-text="waferId"
                        item-value="waferId"
                        filled
                        readonly
                        outlined
                        label="Номер пластины"
                        color='primary'
                ></v-autocomplete>
            </v-col>
        </v-row>
        <v-row>
            <v-col class="d-flex">
                <v-text-field   v-model="measurementRecordingName"
                                outlined label="Номер операции:">
                </v-text-field>
            </v-col>
        </v-row>
        <v-row>
            <v-col class="d-flex">
                 <v-select
                    v-model="uploadingType"
                    :items="uploadingTypes"
                    no-data-text="Нет данных"
                    item-value="type"
                    item-text="type"
                    outlined
                    label="Выберите тип загрузки:">
                  </v-select>
            </v-col>
        </v-row>
        <v-row>
            <v-col class="d-flex">
                 <v-btn color="indigo" block @click="createStandartMeasurementPattern">
                    Загрузить измерения
                </v-btn>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
export default {
  data() {
    return {
      wafers: [],
      selectedWafer: '',
      measurementRecordingName: '',
      uploadingTypes: [],
      uploadingType: '',
    };
  },

  async created() {
    await this.getWafers();
  },

  methods: {
    async getWafers() {
      const response = await this.$http.get('/api/wafer/all');
      this.wafers = response.data;
    },
    async getUploadingTypes() {
      const response = await this.$http.get('');
      this.uploadingTypes = response.data;
    },
  },
};
</script>
