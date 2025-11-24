<template>
  <div class="food-list">
    <div v-if="loading" class="loading-container">
      <div class="loading"></div>
      <p>Đang tải...</p>
    </div>

    <div v-else-if="foods.length === 0" class="empty-state">
      <div class="empty-icon">🍽️</div>
      <p>Không tìm thấy món ăn nào</p>
    </div>

    <div v-else class="food-grid">
      <FoodCard v-for="food in foods" :key="food.id" :food="food" />
    </div>
  </div>
</template>

<script setup>
import FoodCard from './FoodCard.vue'

defineProps({
  foods: {
    type: Array,
    default: () => []
  },
  loading: {
    type: Boolean,
    default: false
  }
})
</script>

<style scoped>
.food-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.5rem;
}

.loading-container {
  text-align: center;
  padding: 3rem;
}

.empty-state {
  text-align: center;
  padding: 3rem;
}

.empty-icon {
  font-size: 5rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

@media (max-width: 768px) {
  .food-grid {
    grid-template-columns: 1fr;
  }
}
</style>
