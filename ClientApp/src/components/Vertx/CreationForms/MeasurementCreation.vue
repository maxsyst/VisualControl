<template>
    <v-container>
    <v-row>
      <v-col lg="6">
        <v-chip class="elevation-8" label x-large color="#303030">
          Добавление измерения
        </v-chip>
      </v-col>
    </v-row>
      <v-row>
        <v-col>
          <v-select
          v-model="waferId"
          :items="wafers"
          no-data-text="Нет данных"
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
              label="Код кристалла">
        </v-select>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <v-text-field v-model="name" readonly outlined label="Название измерения"></v-text-field>
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
      <v-row>
        <v-col>
          <v-btn v-if="createButton" color="success" block @click="createMeasurement"
          >Добавить новое измерение</v-btn
          >
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
    this.$http.get('/api/vertx/mdv/wafers/all').then((response) => {
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
        } else {
          this.code = this.mdvs[0].code;
        }
      });
    },

    async code(newValue) {
      this.$http.get(`/api/vertx/measurement/generate/name/waferId/${this.waferId}/code/${newValue}`).then((response) => {
        this.name = response.data;
        if (this.name === '') {
          this.showSnackbar('Не удалось сгенерировать название');
        }
      });
    },
  },

  computed: {
    nameValidator() {
      if (this.name.length === 0) {
        return 'Введите название';
      }
      if (this.name.length > 30) {
        return 'Макс. 30 символов';
      }
      return '';
    },

    measurementChannelValidator() {
      if (this.measurementChannel.length === 0) {
        return 'Введите номер канала';
      }
      if (this.measurementChannel % 1 === 0) {
        if (+this.measurementChannel < 1 || +this.measurementChannel > 10) {
          return 'Введите номер от 1 до 10';
        }
        return '';
      }
      return 'Введите корректный номер канала';
    },

    vgateValidator() {
      if (this.vgate.length === 0) {
        return 'Введите напряжение';
      }
      if (isNaN(this.vgate)) {
        return 'Введите напряжение в корректном формате';
      }
      return '';
    },

    vpowerValidator() {
      if (this.vpower.length === 0) {
        return 'Введите напряжение';
      }
      if (isNaN(this.vpower)) {
        return 'Введите напряжение в корректном формате';
      }
      return '';
    },

    goalValidator() {
      if (this.goal.length > 150) {
        return 'Макс. 150 символов';
      }
      return '';
    },

    noteValidator() {
      if (this.note.length > 150) {
        return 'Макс. 150 символов';
      }
      return '';
    },

    tempSensorValidator() {
      if (this.tempSensor.length === 0) {
        return 'Введите наличие сенсора';
      }
      if (this.tempSensor.length > 20) {
        return 'Макс. 20 символов';
      }
      return '';
    },

    matchingValidator() {
      if (this.matching.length > 20) {
        return 'Макс. 20 символов';
      }
      return '';
    },

    matchingBoardValidator() {
      if (this.matchingBoard.length > 20) {
        return 'Макс. 20 символов';
      }
      return '';
    },

    createButton() {
      return this.code && this.measurementChannelValidator === ''
                       && this.vgateValidator === ''
                       && this.vpowerValidator === ''
                       && this.goalValidator === ''
                       && this.noteValidator === ''
                       && this.tempSensorValidator === ''
                       && this.matchingValidator === ''
                       && this.matchingBoardValidator === '';
    },
  },

  methods: {
    async createMeasurement() {
      const measurementViewModel = {
        waferId: this.waferId,
        code: this.code,
        measurementInputModel: {
          name: this.name,
          measurementChannel: this.measurementChannel,
          vgate: this.vgate,
          vpower: this.vpower,
          goal: [this.goal],
          comments: [],
          notes: [this.note],
          temperatureSensor: this.temperatureSensor,
          matching: this.matching,
          matchingBoard: this.matchingBoard,
        },
      };
      await this.$http
        .post('/api/vertx/measurement/create/withmdv', measurementViewModel)
        .then(() => {
          this.showSnackbar('Измерение добавлен в систему');
          this.wipeEditing();
        })
        .catch(() => {
          this.showSnackbar('Ошибка при создании измерения');
        });
    },
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
      this.code = '';
      this.name = '';
      this.measurementChannel = '';
      this.vgate = '';
      this.vpower = '';
      this.goal = '';
      this.note = '';
      this.tempSensor = '';
      this.matching = '';
      this.matchingBoard = '';
    },
  },
};
</script>
