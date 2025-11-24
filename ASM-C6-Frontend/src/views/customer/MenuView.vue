<template>
  <div class="container menu-page">
    <h1 class="page-title">Thực đơn</h1>

    <!-- Filters -->
    <div class="filters">
      <button
          @click="selectedCategory = null"
          :class="{ active: selectedCategory === null }"
          class="filter-btn"
      >
        Tất cả
      </button>
      <button
          v-for="cat in categories"
          :key="cat.id"
          @click="selectedCategory = cat.id"
          :class="{ active: selectedCategory === cat.id }"
          class="filter-btn"
      >
        {{ cat.icon }} {{ cat.name }}
      </button>
    </div>

    <!-- Search -->
    <div class="search-box">
      <input
          v-model="searchQuery"
          type="text"
          placeholder="🔍 Tìm kiếm món ăn..."
          class="search-input"
      />
    </div>

    <!-- Food List -->
    <FoodList :foods="filteredFoods" :loading="loading" />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import FoodList from '../../components/foods/FoodList.vue'
import { getFoods } from '../../api/foods'

const foods = ref([])
const loading = ref(false)
const selectedCategory = ref(null)
const searchQuery = ref('')

const categories = ref([
  { id: 1, name: 'Burger', icon: '🍔' },
  { id: 2, name: 'Pizza', icon: '🍕' },
  { id: 3, name: 'Gà rán', icon: '🍗' },
  { id: 4, name: 'Đồ uống', icon: '🥤' }
])

const filteredFoods = computed(() => {
  let result = foods.value

  if (selectedCategory.value) {
    result = result.filter(f => f.categoryId === selectedCategory.value)
  }

  if (searchQuery.value) {
    result = result.filter(f =>
        f.name.toLowerCase().includes(searchQuery.value.toLowerCase())
    )
  }

  return result
})

onMounted(async () => {
  loading.value = true
  try {
    const res = await getFoods()
    foods.value = res.data
  } catch (error) {
    // Mock data
    foods.value = [
      { id: 1, name: 'Burger Phô Mai', price: 45000, imageUrl: 'https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=400', categoryId: 1, isAvailable: true },
      { id: 2, name: 'Pizza Hải Sản', price: 89000, imageUrl: 'https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=400', categoryId: 2, isAvailable: true },
      { id: 3, name: 'Gà Rán Giòn', price: 65000, imageUrl: 'https://images.unsplash.com/photo-1626645738196-c2a7c87a8f58?w=400', categoryId: 3, isAvailable: true },
      { id: 4, name: 'Coca Cola', price: 15000, imageUrl: 'https://images.unsplash.com/photo-1554866585-cd94860890b7?w=400', categoryId: 4, isAvailable: true }
    ]
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.menu-page {
  padding: 2rem 1rem;
}

.page-title {
  font-size: 2.5rem;
  margin-bottom: 2rem;
  text-align: center;
}

.filters {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  flex-wrap: wrap;
  justify-content: center;
}

.filter-btn {
  padding: 0.75rem 1.5rem;
  border: 2px solid var(--light-gray);
  background: white;
  border-radius: 999px;
  cursor: pointer;
  transition: var(--transition);
  font-weight: 600;
}

.filter-btn:hover {
  border-color: var(--primary);
}

.filter-btn.active {
  background: var(--primary);
  color: white;
  border-color: var(--primary);
}

.search-box {
  max-width: 600px;
  margin: 0 auto 2rem;
}

.search-input {
  width: 100%;
  padding: 1rem 1.5rem;
  border: 2px solid var(--light-gray);
  border-radius: 999px;
  font-size: 1rem;
}

.search-input:focus {
  outline: none;
  border-color: var(--primary);
}
</style>
