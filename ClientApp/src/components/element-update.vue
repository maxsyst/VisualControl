<template>
  <v-container>
    <v-row>
      <v-col lg="12">
        <v-menu v-model="menu" :close-on-content-click="false" :nudge-width="300">
          <template v-slot:activator="{ on }">
            <v-icon v-on="on" color="primary">edit</v-icon>
          </template>
          <v-card>
            <v-row>
              <v-col lg="5" class="pl-8">
                <v-text-field
                  v-model="editedElement.name"
                  :error-messages="validationErrors"
                  label="Название элемента"
                ></v-text-field>
              </v-col>
              <v-col lg="7" class="px-8">
                <v-select
                  :items="avElementTypes"
                  v-model="editedElement.typeId"
                  no-data-text="Нет данных"
                  item-text="name"
                  item-value="id"
                  label="Тип элемента"
                ></v-select>
              </v-col>
            </v-row>
            <v-row>
                <v-col lg="12" class="px-8">
                  <v-text-field v-model="editedElement.docName" label="Название в документации"></v-text-field>
                </v-col>
            </v-row>
            <v-row>
            <v-col lg="12" class="px-8">
                <v-text-field v-model="editedElement.comment" label="Описание элемента"></v-text-field>
              </v-col>
            </v-row>
            <v-row>
              <v-col lg="6" offset-lg="6" class="pr-8">
                <v-btn v-if="validationErrors.length === 0" block color="success" @click="updateElement">Изменить</v-btn>
              </v-col>
            </v-row>
          </v-card>
        </v-menu>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>

export default {
  data() {
    return {
      validationErrors: [],
      menu: false,
    };
  },
  props: ['editedElement', 'avElementTypes'],

  methods: {

    updateElement() {
      this.menu = false;
      this.$emit('update-element', this.editedElement);
    },
  },

  watch: {
    elementName(newValue) {
      this.validationErrors = [];
      if (!newValue) {
        this.validationErrors = 'Введите название элемента';
      }
      if (this.elements.filter((x) => (x.name === newValue && x.elementId !== this.editedElement.elementId)).length > 0) {
        this.validationErrors = 'Элемент с таким названием уже существует';
      }
    },
  },

  computed: {

    elementName() {
      return this.editedElement.name;
    },

    elements() {
      return this.$store.state.elements.elements;
    },
  },

  async mounted() {
    this.initElementTypes();
  },

};
</script>

<style>

</style>
