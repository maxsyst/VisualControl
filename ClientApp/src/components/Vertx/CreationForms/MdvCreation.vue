<template>
  <v-container>
    <v-row>
      <v-col lg="6">
        <v-chip class="elevation-8" label x-large color="#303030">
          Добавление кристалла
        </v-chip>
      </v-col>
    </v-row>
    <v-row>
      <v-col lg="6" offset-lg="3">
        <v-autocomplete
          v-model="waferId"
          :items="wafers"
          no-data-text="Нет данных"
          item-text="waferId"
          item-value="waferId"
          filled
          outlined
          label="Номер пластины"
        >
        </v-autocomplete>
      </v-col>
    </v-row>
    <v-row>
      <v-col lg="6" offset-lg="3">
        <v-text-field
          v-model="code"
          :error-messages="codeValidator"
          outlined
          label="Код кристалла"
        ></v-text-field>
      </v-col>
    </v-row>
    <v-row>
      <v-col lg="8" offset-lg="2">
        <v-text-field v-model="description" outlined label="Описание кристалла"></v-text-field>
      </v-col>
    </v-row>
    <v-row>
      <v-col lg="4" offset-lg="4">
        <v-btn v-if="createMdvButton" color="success" block @click="createMdv"
          >Добавить кристалл</v-btn
        >
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
export default {
  data() {
    return {
      wafers: [],
      waferId: '',
      code: '',
      description: '',
    };
  },

  async created() {
    this.showLoading('Загрузка списка пластин');
    this.$http.get('/api/wafer/all').then((response) => {
      this.wafers = response.data;
      this.closeLoading();
    });
  },

  computed: {
    codeValidator() {
      if (this.code.length === 0) {
        return 'Введите номер кристалла';
      }
      if (Number.isInteger(this.code)) {
        if (+this.code < 1 || +this.code > 999) {
          return 'Введите номер от 1 до 999';
        }
        return '';
      }

      if (this.code.match(/\d{2}-\d{2}$/)) {
        return '';
      }
      return 'Введите код в правильном формате';
    },
    descriptionValidator() {
      if (this.description.length > 50) {
        return 'Введите максимум 50 символов';
      }
      return '';
    },
    createMdvButton() {
      return this.waferId && this.codeValidator === '' && this.descriptionValidator === '';
    },
  },

  methods: {
    async createMdv() {
      const mdvViewModel = {
        waferId: this.waferId,
        code: this.code,
        description: this.description,
      };
      await this.$http
        .post('/api/vertx/mdv/create', mdvViewModel)
        .then(() => {
          this.showSnackbar('Кристалл добавлен в систему');
          this.wipeEditing();
        })
        .catch(() => {
          this.showSnackbar('Ошибка при создании кристалла');
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
      this.description = '';
      this.code = '';
    },
  },
};
</script>
