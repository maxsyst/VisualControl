export const dividers = {
    namespaced: true,
    state: {
       dividers: []
    },

    actions: {
        async getAllDividers({commit}, ctx) {
            await ctx.$http
            .get(`/api/divider/all`)
            .then(response => commit('updateDividers', response.data), error => error )
        }
    },
  
    getters: {
        getAll: state => {
            return state.dividers
        },
        getById: state => id => {
            return state.dividers.find(x => x.id === id)
        }
    },

    mutations: {
        updateDividers(state, dividers) {
            state.dividers = [...dividers]
        }
    }
  }
  