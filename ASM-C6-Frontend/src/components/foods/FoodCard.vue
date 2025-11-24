<template>
  <div class="food-card">
    <div class="food-image-wrapper">
      <img :src="food.imageUrl || '/placeholder.jpg'" :alt="food.name" />
      <span v-if="food.isFeatured" class="badge-hot">HOT</span>
      <span v-if="!food.isAvailable" class="badge-sold">Hết hàng</span>
    </div>

    <div class="food-content">
      <h3 class="food-name">{{ food.name }}</h3>
      <p class="food-description">{{ food.description }}</p>

      <div class="food-rating">
        <span class="stars">⭐</span>
        <span class="rating-value">{{ food.rating || 4.5 }}</span>
        <span class="rating-count">({{ food.reviewCount || 0 }})</span>
      </div>

      <div class="food-footer">
        <span class="food-price">{{ food.price.toLocaleString() }}đ</span>
        <button
            @click="addToCart"
            :disabled="!food.isAvailable"
            class="btn btn-primary btn-sm"
        >
          {{ food.isAvailable ? 'Thêm vào giỏ' : 'Hết hàng' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useCartStore } from '../../store/cart'

const props = defineProps({
  food: {
    type: Object,
    required: true
  }
})

const cartStore = useCartStore()

const addToCart = () => {
  cartStore.addItem(props.food)
  alert(`Đã thêm "${props.food.name}" vào giỏ hàng!`)
}
</script>

<style scoped>
.food-card {
  background: white;
  border-radius: 1rem;
  overflow: hidden;
  box-shadow: var(--shadow);
  transition: var(--transition);
  animation: fadeInUp 0.5s ease;
}

.food-card:hover {
  transform: translateY(-8px) scale(1.02);
  box-shadow: var(--shadow-lg);
}

.food-image-wrapper {
  position: relative;
  overflow: hidden;
}

.food-image-wrapper img {
  width: 100%;
  height: 200px;
  object-fit: cover;
  transition: var(--transition);
}

.food-card:hover img {
  transform: scale(1.1);
}

.badge-hot, .badge-sold {
  position: absolute;
  top: 0.75rem;
  right: 0.75rem;
  padding: 0.25rem 0.75rem;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: bold;
  color: white;
}

.badge-hot {
  background: linear-gradient(135deg, #EF4444, #DC2626);
}

.badge-sold {
  background: #6B7280;
}

.food-content {
  padding: 1.25rem;
}

.food-name {
  font-size: 1.125rem;
  font-weight: bold;
  color: var(--dark);
  margin-bottom: 0.5rem;
}

.food-description {
  color: var(--gray);
  font-size: 0.875rem;
  margin-bottom: 0.75rem;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.food-rating {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  margin-bottom: 1rem;
  font-size: 0.875rem;
}

.stars {
  color: #FBBF24;
}

.rating-value {
  font-weight: 600;
  color: var(--dark);
}

.rating-count {
  color: var(--gray);
}

.food-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.food-price {
  font-size: 1.5rem;
  font-weight: bold;
  color: var(--primary);
}

.btn-sm {
  padding: 0.5rem 1rem;
  font-size: 0.875rem;
}
</style>
