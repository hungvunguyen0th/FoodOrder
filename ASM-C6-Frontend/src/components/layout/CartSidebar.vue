<template>
  <div class="cart-overlay" @click.self="emit('close')">
    <div class="cart-sidebar">
      <div class="cart-header">
        <h2>Giỏ hàng</h2>
        <button @click="emit('close')" class="close-btn">✕</button>
      </div>

      <div v-if="cartStore.items.length === 0" class="cart-empty">
        <div class="empty-icon">🛒</div>
        <p>Giỏ hàng trống</p>
      </div>

      <div v-else class="cart-content">
        <div class="cart-items">
          <div v-for="item in cartStore.items" :key="item.id" class="cart-item">
            <img :src="item.imageUrl" />
            <div>
              <h3>{{ item.name }}</h3>
              <p>{{ item.price.toLocaleString() }}đ x {{ item.quantity }}</p>
            </div>
          </div>
        </div>
        <div class="cart-footer">
          <p><strong>Tổng:</strong> {{ cartStore.totalPrice.toLocaleString() }}đ</p>
          <router-link to="/cart" @click="emit('close')" class="btn btn-primary btn-full">
            Xem giỏ hàng
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useCartStore } from '../../store/cart'

const cartStore = useCartStore()
const emit = defineEmits(['close'])
</script>

<style scoped>
.cart-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  z-index: 1000;
}

.cart-sidebar {
  position: fixed;
  right: 0;
  top: 0;
  height: 100vh;
  width: 100%;
  max-width: 400px;
  background: white;
  display: flex;
  flex-direction: column;
}

.cart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 2px solid var(--light-gray);
}

.close-btn {
  background: none;
  border: none;
  font-size: 2rem;
  cursor: pointer;
}

.cart-empty {
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
}

.empty-icon {
  font-size: 5rem;
  opacity: 0.5;
}

.cart-content {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.cart-items {
  flex: 1;
  overflow-y: auto;
  padding: 1rem;
}

.cart-item {
  display: flex;
  gap: 1rem;
  padding: 1rem;
  border-bottom: 1px solid var(--light-gray);
}

.cart-item img {
  width: 60px;
  height: 60px;
  object-fit: cover;
  border-radius: 0.5rem;
}

.cart-footer {
  padding: 1.5rem;
  border-top: 2px solid var(--light-gray);
}

.btn-full {
  width: 100%;
  margin-top: 1rem;
}
</style>
