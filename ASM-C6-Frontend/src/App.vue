<template>
  <div id="app">
    <Navbar />
    <main>
      <router-view />
    </main>
    <Footer />
    <CartSidebar v-if="showCart" @close="showCart = false" />
  </div>
</template>

<script setup>
import { ref, provide, onMounted } from 'vue'
import { useAuthStore } from './store/auth'
import Navbar from './components/layout/Navbar.vue'
import Footer from './components/layout/Footer.vue'
import CartSidebar from './components/layout/CartSidebar.vue'

const authStore = useAuthStore()
const showCart = ref(false)

provide('toggleCart', () => {
  showCart.value = !showCart.value
})

onMounted(() => {
  authStore.fetchUser()
})
</script>
