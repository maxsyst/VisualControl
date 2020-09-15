export const loading = {
    namespaced: true,
    state: {
      text: "",
      visible: false
    },
    actions: {
      show ({ commit }, text) {
        commit('show', text)
      },
      cloak ({ commit }) {
        commit('cloak')
      }
    },
    mutations: {
      show (state, text) {
        state.text = text
        state.visible = true
      },
      cloak (state) {
        state.text = ""
        state.visible = false
      }
    }
  }
  