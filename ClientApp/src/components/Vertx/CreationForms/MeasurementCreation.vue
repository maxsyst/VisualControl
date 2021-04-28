<template>
    <v-container>
      <v-row>
        <v-col>
          <v-select
          v-model="waferId"
          :items="wafers"
          no-data-text="Нет данных"
          item-text="waferId"
          item-value="waferId"
          outlined
          label="Номер пластины"
        >
        </v-select>
        </v-col>
        <v-col>
           <v-select
          v-model="code"
          :items="mdvs"
          no-data-text="Кристаллы не найдены"
          item-text="code"
          item-value="code"
          outlined
          label="Код кристалла"
        >
        </v-select>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <v-text-field v-model="name" :error-messages="nameValidator" outlined label="Название измерения"></v-text-field>
        </v-col>
        <v-col>
          <v-text-field v-model="measurementChannel" :error-messages="measurementChannelValidator" outlined label="Измерительный канал"></v-text-field>
        </v-col>
      </v-row>
      <v-row>
         <v-col>
          <v-text-field v-model="vgate" :error-messages="vgateValidator" outlined label="Vgate"></v-text-field>
        </v-col>
        <v-col>
           <v-text-field v-model="vpower" :error-messages="vpowerValidator" outlined label="Vpower"></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <v-text-field v-model="goal" :error-messages="goalValidator" outlined label="Цель"></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
            <v-text-field v-model="note" :error-messages="noteValidator" outlined label="Доп.комментарии"></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <v-text-field v-model="tempSensor" :error-messages="tempSensorValidator" outlined label="Темп.датчик"></v-text-field>
        </v-col>
        <v-col>
          <v-text-field v-model="matching" :error-messages="matchingValidator" outlined label="Согласование"></v-text-field>
        </v-col>
        <v-col>
          <v-text-field v-model="matchingBoard" :error-messages="matchingBoardValidator" outlined label="Плата согласования"></v-text-field>
        </v-col>
      </v-row>
    </v-container>
</template>

<script>
export default {
  data() {
    return {
      waferId: '',
      wafers: [],
      code: '',
      mdvs: [],
      name: '',
      measurementChannel: '',
      vgate: '',
      vpower: '',
      goal: '',
      note: '',
      tempSensor: '',
      matching: '',
      matchingBoard: '',
    };
  },

  async created() {
    this.showLoading('Загрузка списка пластин');
    this.$http.get('/api/wafer/all').then((response) => {
      this.wafers = response.data;
      this.closeLoading();
    });
  },

  watch: {
    async waferId(newValue) {
      this.$http.get(`/api/vertx/mdv/waferId/${newValue}`).then((response) => {
        this.mdvs = response.data;
        if (this.mdvs.length === 0) {
          this.showSnackbar('Кристаллы не найдены');
        }
      });
    },
  },

  methods: {
    showSnackbar(text) {
      this.$store.dispatch('alert/success', text);
    },
    showLoading(text) {
      this.$store.dispatch('loading/show', text);
    },

    closeLoading() {
      this.$store.dispatch('loading/cloak');
    },
    wipeEditing() {
      this.waferId = '';
      this.description = '';
      this.code = '';
    },
  },
};
</script>
