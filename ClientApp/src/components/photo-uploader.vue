<template>
  <div id="photouploader">
        <file-pond ref="pond"
               class-name="my-pond"
               label-idle="Загрузите фото..."
               labelFileProcessing = "Загрузка"
               labelFileProcessingError = "Ошибка при загрузке"
               labelTapToRetry = "Нажмите для повтора"
               labelFileProcessingComplete = "Загрузка завершена"
               labelTapToCancel ="Нажмите для отмены"
               labelTapToUndo ="Нажмите для удаления"
               allow-multiple="true"
               accepted-file-types="image/jpeg, image/png"
               v-on:processfile ="handleProcessFile"
               v-bind:server="server"/>

  </div>
</template>

<script>

import vueFilePond from 'vue-filepond';

// Import plugins
import FilePondPluginImagePreview from 'filepond-plugin-image-preview/dist/filepond-plugin-image-preview.esm.js';

// Import styles
import 'filepond/dist/filepond.min.css';
import 'filepond-plugin-image-preview/dist/filepond-plugin-image-preview.min.css';

// Create FilePond component
const FilePond = vueFilePond(FilePondPluginImagePreview);

export default {

  props: ['reset'],
  data() {
    return {

      server: {
        process: {
          url: './api/photouploading/saveimage',
          method: 'POST',
          withCredentials: false,
          onload(response) {
            return response;
          },
          onerror(response) {
            return response;
          },

        },

        fetch: null,
        revert: null,
      },
    };
  },
  methods: {

    handleProcessFile() {
      this.$emit('fileLoaded', this.$refs.pond.getFile().serverId);
    },

  },
  watch:
    {
      reset() {
        if (this.reset === 'reset') {
          this.$refs.pond.removeFiles();
        }
      },
    },
  components: {
    FilePond,
  },

};
</script>
