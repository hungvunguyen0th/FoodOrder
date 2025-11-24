import { defineStore } from 'pinia'

export const useCartStore = defineStore('cart', {
    state: () => ({
        items: JSON.parse(localStorage.getItem('cart') || '[]')
    }),

    getters: {
        totalItems: (state) => state.items.reduce((sum, item) => sum + item.quantity, 0),
        totalPrice: (state) => state.items.reduce((sum, item) => sum + item.price * item.quantity, 0),
        isEmpty: (state) => state.items.length === 0
    },

    actions: {
        addItem(food, quantity = 1) {
            const existing = this.items.find(i => i.id === food.id)
            if (existing) {
                existing.quantity += quantity
            } else {
                this.items.push({ ...food, quantity })
            }
            this.saveCart()
        },

        updateQuantity(id, quantity) {
            const item = this.items.find(i => i.id === id)
            if (item) {
                if (quantity <= 0) {
                    this.removeItem(id)
                } else {
                    item.quantity = quantity
                    this.saveCart()
                }
            }
        },

        removeItem(id) {
            this.items = this.items.filter(i => i.id !== id)
            this.saveCart()
        },

        clearCart() {
            this.items = []
            this.saveCart()
        },

        saveCart() {
            localStorage.setItem('cart', JSON.stringify(this.items))
        }
    }
})
// code cũ ở trên
// code compilot ở dưới
// src/store/cart.js
// import { defineStore } from 'pinia'
// import { ref, computed } from 'vue'
//
// export const useCartStore = defineStore('cart', () => {
//   const items = ref([])
//
//   const addItem = (food, quantity) => {
//     const existing = items.value.find(i => i.id === food.id)
//     if (existing) {
//       existing.quantity += quantity
//     } else {
//       items.value.push({ ...food, quantity })
//     }
//   }
//
//   const removeItem = (foodId) => {
//     items.value = items.value.filter(i => i.id !== foodId)
//   }
//
//   const clearCart = () => {
//     items.value = []
//   }
//
//   const totalItems = computed(() =>
//     items.value.reduce((sum, item) => sum + item.quantity, 0)
//   )
//
//   const totalPrice = computed(() =>
//     items.value.reduce((sum, item) => sum + (item.price * item.quantity), 0)
//   )
//
//   return { items, addItem, removeItem, clearCart, totalItems, totalPrice }
// })