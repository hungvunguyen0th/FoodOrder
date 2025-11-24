<template>
  <nav class="navbar">
    <div class="container navbar-content">
      <router-link to="/" class="logo">
        <span style="font-size: 2rem">🍔</span>
        <span>FastFood</span>
      </router-link>

      <ul class="nav-links">
        <li><router-link to="/" class="nav-link">Trang chủ</router-link></li>
        <li><router-link to="/menu" class="nav-link">Thực đơn</router-link></li>
      </ul>

      <div class="flex gap-2" style="align-items: center;">
        <button @click="toggleCart" class="cart-btn">
          🛒
          <span v-if="cartStore.totalItems > 0" class="badge">{{ cartStore.totalItems }}</span>
        </button>

        <router-link to="/login" class="btn btn-secondary">Đăng nhập</router-link>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { inject } from 'vue'
import { useCartStore } from '../../store/cart'

const cartStore = useCartStore()
const toggleCart = inject('toggleCart', () => {})
</script>

<style scoped>
.cart-btn {
  position: relative;
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
}
</style>
