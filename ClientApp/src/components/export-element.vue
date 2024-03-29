<template>
  <v-container>
    <v-row>
      <v-col lg="3">
        <v-text-field
          v-model="operation.number"
          :error-messages="
            $v.$dirty && !$v.operation.number.required
              ? [operation.errorMessage]
              : []
          "
          @change="validateElement()"
          label="Номер операции:"
          outlined
        >
        </v-text-field>
      </v-col>
      <v-col lg="3">
        <v-text-field
          v-model="element.name"
          :error-messages="
            $v.$dirty && !$v.element.name.required ? [element.errorMessage] : []
          "
          @change="validateElement()"
          label="Название элемента:"
          outlined
        >
        </v-text-field>
      </v-col>
      <v-col lg="3">
        <v-btn v-if="!isElementReady" large block outlined color="pink"
          >Элемент заполнен некорректно</v-btn
        >
        <v-btn v-else large block outlined color="green"
          >Элемент заполнен корректно</v-btn
        >
      </v-col>
      <v-col lg="1" offset-lg="1">
        <v-menu
          v-model="menu"
          :close-on-content-click="false"
          :nudge-width="300"
          offset-x
        >
          <template v-slot:activator="{ on }">
            <v-btn outlined fab dark small color="primary" v-on="on">
              <v-icon dark color="primary">perm_data_setting</v-icon>
            </v-btn>
          </template>
          <v-card>
            <v-card-text>
              <v-row>
                <v-switch
                  v-model="element.isAddedToCommonWorksheet"
                  color="primary"
                  :label="
                    element.isAddedToCommonWorksheet
                      ? `Включить в сводную таблицу`
                      : `Не включать в сводную таблицу`
                  "
                >
                </v-switch>
              </v-row>
              <v-row>
                <v-col lg="11" offset-lg="1">
                  <v-text-field
                    v-model="operation.waferId"
                    label="Номер пластины:"
                    readonly
                  >
                  </v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-col lg="11" offset-lg="1">
                  <v-textarea
                    v-if="operation.avStages.length === 0"
                    v-model="operation.stageName"
                    label="Название этапа:"
                    readonly
                    no-resize
                  >
                  </v-textarea>
                  <v-select
                    v-else
                    v-model="operation.stageName"
                    :items="operation.avStages"
                    no-data-text="Нет данных"
                    item-value="stageName"
                    item-text="stageName"
                    outlined
                    label="Выберите параметр:"
                  >
                  </v-select>
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>
        </v-menu>
      </v-col>
      <v-col lg="1">
        <v-tooltip bottom>
          <template v-slot:activator="{ on }">
            <v-btn
              outlined
              fab
              dark
              small
              color="primary"
              v-on="on"
              @click="getAutoIdmrSingle()"
            >
              <v-icon dark color="primary">brightness_auto</v-icon>
            </v-btn>
          </template>
          <span>Автозаполнение</span>
        </v-tooltip>
      </v-col>
    </v-row>
    <v-row>
      <v-col lg="12">
        <v-stepper v-model="e1">
          <v-stepper-header>
            <template v-for="(parameter, index) in parameters">
              <v-stepper-step
                :key="`${index}-step`"
                :step="index + 1"
                color="indigo"
                editable
              >
                {{ parameter.parameterName.value }}
              </v-stepper-step>
              <v-divider :key="index + 1"></v-divider>
            </template>
          </v-stepper-header>
          <v-stepper-items>
            <v-stepper-content
              v-for="(parameter, index) in parameters"
              :key="`${index}-content`"
              :step="index + 1"
            >
              <div>
                <v-row>
                  <v-col lg="3">
                    <v-text-field
                      v-model="parameter.parameterName.value"
                      :error-messages="
                        parameter.parameterName.isValidDirty &&
                        !parameter.parameterName.isValid
                          ? defaultRequiredMessage
                          : []
                      "
                      @change="validateParameter(parameter)"
                      label="Буквенное обозначение:"
                    >
                    </v-text-field>
                  </v-col>
                  <v-col lg="6" offset-lg="1">
                    <v-text-field
                      v-model="parameter.russianParameterName.value"
                      :error-messages="
                        parameter.russianParameterName.isValidDirty &&
                        !parameter.russianParameterName.isValid
                          ? defaultRequiredMessage
                          : []
                      "
                      @change="validateParameter(parameter)"
                      label="Наименование:"
                    >
                    </v-text-field>
                  </v-col>
                  <v-col lg="1" offset-lg="1">
                    <v-tooltip bottom>
                      <template v-slot:activator="{ on }">
                        <v-btn
                          outlined
                          fab
                          dark
                          small
                          color="primary"
                          v-on="on"
                          @click="cleanParameter(parameter)"
                        >
                          <v-icon dark color="primary">autorenew</v-icon>
                        </v-btn>
                      </template>
                      <span>Очистить параметр</span>
                    </v-tooltip>
                  </v-col>
                </v-row>
                <v-row v-if="!parameter.shortLink.success">
                  <v-col lg="3">
                    <v-text-field
                      v-model="parameter.shortLink.value"
                      :error-messages="
                        $v.$dirty && !parameter.shortLink.success
                          ? [parameter.shortLink.errorMessage]
                          : []
                      "
                      outlined
                      label="Короткая ссылка:"
                      @change="shortLinkHandler($event, index)"
                    >
                    </v-text-field>
                  </v-col>
                </v-row>
                <v-row v-else>
                  <v-col lg="3">
                    <v-text-field
                      v-model="parameter.waferId"
                      readonly
                      label="Номер пластины:"
                    >
                    </v-text-field>
                  </v-col>
                  <v-col lg="8" offset-lg="1">
                    <v-select
                      v-model="parameter.selectedStatParameter"
                      :items="parameter.statParameterArray"
                      no-data-text="Нет данных"
                      outlined
                      v-on:change="changeDividerId(parameter)"
                      label="Выберите параметр:"
                    >
                    </v-select>
                  </v-col>
                </v-row>
                <v-row v-if="parameter.shortLink.success">
                  <v-col lg="3">
                    <v-select
                      v-if="measurementRecordings.length > 1"
                      v-model="parameter.measurementRecording"
                      :items="measurementRecordings"
                      no-data-text="Нет данных"
                      item-value="id"
                      item-text="name"
                      outlined
                      v-on:change="changeMeasurementRecording(parameter)"
                      label="Выберите номер операции:"
                    >
                    </v-select>

                    <v-text-field
                      v-else
                      :value="
                        measurementRecordings.length
                          ? measurementRecordings.find(
                              x => x.id === parameter.measurementRecording
                            ).name
                          : ''
                      "
                      readonly
                      label="Номер операции:"
                    >
                    </v-text-field>
                    <p>ID операции: {{ parameter.measurementRecording }}</p>
                  </v-col>
                  <v-col lg="3" offset-lg="1">
                    <v-select
                      v-model="parameter.dividerId"
                      :items="dividers"
                      no-data-text="Нет данных"
                      item-value="id"
                      item-text="name"
                      outlined
                      v-on:change="changeDividerId(parameter)"
                      label="Выберите приведение:"
                    >
                    </v-select>
                    <p v-if="parameter.dividerId">
                      Коэффициент приведения: {{ parameter.divider }}
                    </p>
                  </v-col>
                  <v-col lg="2" offset-lg="1">
                    <v-text-field
                      v-model="parameter.bounds.lower.value"
                      :error-messages="
                        parameter.bounds.lower.isValidDirty &&
                        !parameter.bounds.lower.isValid
                          ? parameter.bounds.lower.errorMessages[0]
                          : []
                      "
                      @change="validateParameter(parameter)"
                      outlined
                      label="Нижняя граница:"
                    >
                    </v-text-field>
                  </v-col>
                  <v-col lg="2">
                    <v-text-field
                      v-model="parameter.bounds.upper.value"
                      :error-messages="
                        parameter.bounds.upper.isValidDirty &&
                        !parameter.bounds.upper.isValid
                          ? parameter.bounds.upper.errorMessages[0]
                          : []
                      "
                      @change="validateParameter(parameter)"
                      outlined
                      label="Верхняя граница:"
                    >
                    </v-text-field>
                  </v-col>
                </v-row>

                <v-tooltip v-if="index > 0" bottom>
                  <template v-slot:activator="{ on }">
                    <v-btn
                      fab
                      dark
                      small
                      color="indigo"
                      v-on="on"
                      @click="prevStep(index + 1)"
                    >
                      <v-icon dark color="primary">skip_previous</v-icon>
                    </v-btn>
                  </template>
                  <span>Предыдущий параметр</span>
                </v-tooltip>
                <v-tooltip v-if="index < parameters.length - 1" bottom>
                  <template v-slot:activator="{ on }">
                    <v-btn
                      fab
                      dark
                      small
                      color="indigo"
                      v-on="on"
                      @click="nextStep(index + 1)"
                    >
                      <v-icon dark color="primary">skip_next</v-icon>
                    </v-btn>
                  </template>
                  <span>Следующий параметр</span>
                </v-tooltip>
              </div>
            </v-stepper-content>
          </v-stepper-items>
          <div class="parameter-actions">
            <v-tooltip bottom>
              <template v-slot:activator="{ on }">
                <v-btn
                  fab
                  dark
                  small
                  color="indigo"
                  v-on="on"
                  @click="createParameter()"
                >
                  <v-icon dark color="primary">add</v-icon>
                </v-btn>
              </template>
              <span>Добавить параметр</span>
            </v-tooltip>
            <v-tooltip bottom>
              <template v-slot:activator="{ on }">
                <v-btn
                  v-if="parameters.length > 0"
                  fab
                  dark
                  small
                  color="indigo"
                  v-on="on"
                  @click="deleteParameterDialog = true"
                >
                  <v-icon dark color="primary">delete</v-icon>
                </v-btn>
              </template>
              <span>Удалить параметр</span>
            </v-tooltip>
          </div>
        </v-stepper>
      </v-col>
    </v-row>
    <v-row>
      <v-dialog v-model="deleteParameterDialog" persistent max-width="400">
        <v-card>
          <v-card-title class="headline">Удаление</v-card-title>
          <v-card-text
            >Вы действительно хотите удалить текущий параметр?</v-card-text
          >
          <v-spacer></v-spacer>
          <v-card-actions>
            <v-btn color="pink" text @click="deleteParameter()">Удалить</v-btn>
            <v-btn color="indigo" text @click="deleteParameterDialog = false"
              >Отмена</v-btn
            >
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-row>
  </v-container>
</template>

<script>
import { required } from 'vuelidate/lib/validators';

export default {
  data() {
    return {
      menu: false,
      freakDividerParameters: [
        'r<sub>DS(on)</sub> (сопротивление открытого канала при Uси = 0.02В)',
        'R<sub>ds(on)</sub> (сопротивление открытого канала)',
      ],
      defaultRequiredMessage: 'Введите значение',
      deleteParameterDialog: false,
      measurementRecordings: [],
      e1: 0,
      done: 'loading',
    };
  },

  props: ['id', 'parameters', 'dividers', 'operation', 'element'],

  computed: {
    isElementReady() {
      return this.parameters.length > 0 && this.validateElement();
    },
  },

  methods: {
    async getAutoIdmr(waferId, stageName) {
      await this.$http
        .get(
          `api/measurementrecording/getbyelement?waferId=${waferId}&elementName=${this.element.name}&stageName=${stageName}`,
        )
        .then((response) => {
          const { data } = response;
          if (response.status === 200) {
            this.measurementRecordings = data;
            this.parameters.forEach((parameter) => {
              const currentMr = this.measurementRecordings.find((x) => x.avStatisticParameters.includes(
                parameter.selectedStatParameter,
              ));
              if (currentMr) {
                parameter.statParameterArray = currentMr.avStatisticParameters;
                if (!parameter.selectedStatParameter) {
                  parameter.selectedStatParameter = parameter.statParameterArray[0];
                }
                parameter.waferId = waferId;
                parameter.measurementRecording = currentMr.id;
                parameter.shortLink.success = true;
                parameter.shortLink.errorMessage = '';
              }
            });

            this.$store.commit('exportkurb/updateElementAutoIdmr', {
              key: this.id,
              operation: this.operation.number,
              element: this.element.name,
              done: 'success',
            });
          }
        })
        .catch(() => {
          this.$store.commit('exportkurb/updateElementAutoIdmr', {
            key: this.id,
            operation: this.operation.number,
            element: this.element.name,
            done: 'fail',
          });
        });
    },

    cleanParameter(parameter) {
      parameter.waferId = '';
      parameter.measurementRecording = 0;
      parameter.statParameterArray = [];
      parameter.shortLink = { value: '', success: '', errorMessage: '' };
    },

    createParameter() {
      const parameter = {
        parameterName: { value: '', isValidDirty: false, isValid: true },
        russianParameterName: { value: '', isValidDirty: false, isValid: true },
        waferId: '',
        measurementRecording: { id: '0', name: 'Неизвестно' },
        selectedStatParameter: '',
        bounds: {
          lower: {
            value: '',
            isValidDirty: false,
            isValid: true,
            errorMessages: [],
          },
          upper: {
            value: '',
            isValidDirty: false,
            isValid: true,
            errorMessages: [],
          },
        },
        dividerId: 1,
        divider: '1.0',
        statParameterArray: [],
        shortLink: { value: '', success: '', errorMessage: '' },
      };
      this.parameters.push(parameter);
      this.e1 += 1;
    },
    deleteParameter() {
      this.parameters.splice(this.e1 - 1, 1);
      this.deleteParameterDialog = false;
      this.e1 -= 1;
    },

    validateElement() {
      this.$v.$touch();
      if (
        !this.$v.operation.number.required
        || !this.$v.element.name.required
      ) {
        return false;
      }
      if (this.parameters.length === 0) {
        return true;
      }
      for (let i = 0; i < this.parameters.length; i += 1) {
        const currentParameter = this.parameters[i];
        if (!this.validateParameter(currentParameter)) {
          return false;
        }
      }
      return true;
    },
    validateParameter(currentParameter) {
      currentParameter.parameterName.isValidDirty = true;
      currentParameter.russianParameterName.isValidDirty = true;
      currentParameter.parameterName.isValid = true;
      currentParameter.russianParameterName.isValid = true;

      if (!currentParameter.parameterName.value) {
        currentParameter.parameterName.isValid = false;
      }
      if (!currentParameter.russianParameterName.value) {
        currentParameter.russianParameterName.isValid = false;
      }

      if (currentParameter.shortLink.success) {
        const isNumber = (n) => !Number.isNaN(parseFloat(n)) && !Number.isNaN(n - 0);
        const lowerBound = currentParameter.bounds.lower;
        const upperBound = currentParameter.bounds.upper;
        lowerBound.errorMessages = [];
        upperBound.errorMessages = [];
        lowerBound.isValidDirty = true;
        upperBound.isValidDirty = true;
        lowerBound.isValid = true;
        upperBound.isValid = true;
        if (lowerBound.value && !isNumber(lowerBound.value)) {
          lowerBound.isValid = false;
          lowerBound.errorMessages.push(
            'Введите значение в правильном формате',
          );
        }
        if (upperBound.value && !isNumber(upperBound.value)) {
          upperBound.isValid = false;
          upperBound.errorMessages.push(
            'Введите значение в правильном формате',
          );
        }
        if (!lowerBound.value && !upperBound.value) {
          lowerBound.isValid = false;
          upperBound.isValid = false;
          lowerBound.errorMessages.push(
            'Должна быть установлена хотя бы одна граница',
          );
          upperBound.errorMessages.push(
            'Должна быть установлена хотя бы одна граница',
          );
        }
        ('');
        if (
          lowerBound.value
          && upperBound.value
          && lowerBound.isValid
          && upperBound.isValid
          && parseFloat(lowerBound.value) > parseFloat(upperBound.value)
        ) {
          upperBound.isValid = false;
          upperBound.errorMessages.push(
            'Верхняя граница должна быть больше нижней',
          );
        }
      }

      if (
        currentParameter.russianParameterName.isValid
        && currentParameter.parameterName.isValid
        && currentParameter.shortLink.success
        && currentParameter.bounds.lower.isValid
        && currentParameter.bounds.upper.isValid
      ) {
        return true;
      }
      return false;
    },
    async shortLinkHandler(shortLink, index) {
      const generatedId = shortLink.split('=')[1];
      const parameter = this.parameters[index];
      await this.$http
        .get(`api/shortlink/${generatedId}/element-export`)
        .then((response) => {
          const { data } = response;
          if (response.status === 200) {
            this.measurementRecordings = data;
            const currentMeasurementRecording = this.measurementRecordings[0];
            parameter.statParameterArray = currentMeasurementRecording.avStatisticParameters;
            if (!parameter.selectedStatParameter) {
              parameter.selectedStatParameter = parameter.statParameterArray[0];
            }
            parameter.waferId = currentMeasurementRecording.waferId;
            parameter.measurementRecording = currentMeasurementRecording.id;
            parameter.shortLink.success = true;
            parameter.shortLink.errorMessage = '';
          } else {
            parameter.shortLink.success = false;
            parameter.shortLink.errorMessage = data.reduce(
              (r, c) => `${r}/n${c.message}`,
            );
          }
        })
        .catch(() => {
          parameter.shortLink.success = false;
          parameter.shortLink.errorMessage = 'Не удалось обработать ссылку';
        });
    },
    changeDividerId(parameter) {
      parameter.divider = this.freakDividerParameters.includes(
        parameter.selectedStatParameter,
      )
        ? (
          1 / this.dividers.find((_) => _.id === parameter.dividerId).dividerK
        ).toFixed(3)
        : (+this.dividers.find((_) => _.id === parameter.dividerId)
          .dividerK).toFixed(3);
    },

    changeMeasurementRecording(parameter) {
      parameter.statParameterArray = this.measurementRecordings.filter(
        (x) => x.id === parameter.measurementRecording,
      )[0].avStatisticParameters;
    },

    nextStep(n) {
      if (n === this.parameters.length) {
        this.e1 = 1;
      } else {
        this.e1 = n + 1;
      }
    },
    prevStep(n) {
      if (n === 1) {
        this.e1 = this.parameters.length;
      } else {
        this.e1 = n - 1;
      }
    },
  },
  watch: {
    isElementReady: {
      handler(newValue) {
        this.$store.commit('exportkurb/updateElementsReady', {
          key: this.id,
          ready: newValue,
        });
      },
      immediate: true,
    },
  },
  validations: {
    operation: {
      number: {
        required,
      },
    },
    element: {
      name: {
        required,
      },
    },
  },
};
</script>

<style>
.parameter-actions {
  margin-top: 10px;
  border-width: 0.5px;
  border-style: outset;
  border-color: #ffcc00;
}
</style>
