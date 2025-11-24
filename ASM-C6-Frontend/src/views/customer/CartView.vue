<template>
  <div class="container cart-page">
    <h1 class="page-title">Giỏ hàng của bạn</h1>

    <div v-if="cartStore.items.length === 0" class="empty-cart">
      <div class="empty-icon">🛒</div>
      <p>Giỏ hàng trống</p>
      <router-link to="/menu" class="btn btn-primary">Tiếp tục mua sắm</router-link>
    </div>

    <div v-else class="cart-content">
      <div class="cart-items">
        <div v-for="item in cartStore.items" :key="item.id" class="cart-item-row">
          <img :src="item.imageUrl" :alt="item.name" />
          <div class="item-info">
            <h3>{{ item.name }}</h3>
            <p>{{ item.price.toLocaleString() }}đ</p>
          </div>
          <div class="quantity-control">
            <button @click="cartStore.updateQuantity(item.id, item.quantity - 1)">-</button>
            <span>{{ item.quantity }}</span>
            <button @click="cartStore.updateQuantity(item.id, item.quantity + 1)">+</button>
          </div>
          <p class="item-total">{{ (item.price * item.quantity).toLocaleString() }}đ</p>
          <button @click="cartStore.removeItem(item.id)" class="remove-btn">✕</button>
        </div>
      </div>

      <div class="cart-summary">
        <h2>Tổng đơn hàng</h2>
        <div class="summary-row">
          <span>Tạm tính:</span>
          <span>{{ cartStore.totalPrice.toLocaleString() }}đ</span>
        </div>
        <div class="summary-row">
          <span>Phí giao hàng:</span>
          <span>15,000đ</span>
        </div>
        <div class="summary-total">
          <span>Tổng cộng:</span>
          <span>{{ (cartStore.totalPrice + 15000).toLocaleString() }}đ</span>
        </div>
        <button class="btn btn-primary btn-full">Thanh toán</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useCartStore } from '../../store/cart'

const cartStore = useCartStore()
</script>

<style scoped>
.cart-page {
  padding: 2rem 1rem;
  min-height: 60vh;
}

.page-title {
  font-size: 2.5rem;
  margin-bottom: 2rem;
}

.empty-cart {
  text-align: center;
  padding: 4rem 2rem;
}

.empty-icon {
  font-size: 6rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

.cart-content {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 2rem;
}

.cart-items {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.cart-item-row {
  display: grid;
  grid-template-columns: 100px 1fr auto auto auto;
  gap: 1rem;
  align-items: center;
  background: white;
  padding: 1rem;
  border-radius: 1rem;
  box-shadow: var(--shadow);
}

.cart-item-row img {
  width: 100px;
  height: 100px;
  object-fit: cover;
  border-radius: 0.5rem;
}

.quantity-control {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.quantity-control button {
  width: 2rem;
  height: 2rem;
  border: 1px solid var(--light-gray);
  background: white;
  border-radius: 0.25rem;
  cursor: pointer;
}

.item-total {
  font-weight: bold;
  color: var(--primary);
  font-size: 1.25rem;
}

.remove-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #EF4444;
}

.cart-summary {
  background: white;
  padding: 2rem;
  border-radius: 1rem;
  box-shadow: var(--shadow);
  height: fit-content;
  position: sticky;
  top: 6rem;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.summary-total {
  display: flex;
  justify-content: space-between;
  padding-top: 1rem;
  border-top: 2px solid var(--light-gray);
  margin: 1rem 0;
  font-size: 1.25rem;
  font-weight: bold;
}

.summary-total span:last-child {
  color: var(--primary);
}

.btn-full {
  width: 100%;
}

@media (max-width: 968px) {
  .cart-content {
    grid-template-columns: 1fr;
  }

  .cart-item-row {
    grid-template-columns: 80px 1fr;
  }
}
</style>
