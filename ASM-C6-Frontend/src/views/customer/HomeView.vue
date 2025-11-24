<template>
  <div>
    <!-- Hero -->
    <section class="hero">
      <div class="container">
        <h1 class="fade-in">🍔 Chào mừng đến FastFood</h1>
        <p class="fade-in">Đặt đồ ăn nhanh chóng, giao hàng tận nơi</p>
        <router-link to="/menu" class="btn btn-primary btn-large">
          Xem thực đơn ngay
        </router-link>
      </div>
    </section>

    <!-- Featured Foods -->
    <section class="container" style="margin-top: 4rem;">
      <h2 class="text-center mb-4" style="font-size: 2.5rem;">Món ăn nổi bật</h2>

      <div v-if="loading" class="loading-container">
        <div class="loading"></div>
        <p>Đang tải...</p>
      </div>

      <div v-else-if="error" class="error-message">
        <p>❌ {{ error }}</p>
        <button @click="loadFoods" class="btn btn-primary">Thử lại</button>
      </div>

      <div v-else class="food-grid">
        <FoodCard v-for="food in featuredFoods" :key="food.id" :food="food" />
      </div>
    </section>

    <!-- Categories -->
    <section class="container" style="margin-top: 4rem;">
      <h2 class="text-center mb-4" style="font-size: 2.5rem;">Danh mục món ăn</h2>
      <div class="categories-grid">
        <div v-for="cat in categories" :key="cat.id" class="category-card" @click="goToCategory(cat.id)">
          <div class="category-icon">{{ cat.icon }}</div>
          <h3>{{ cat.name }}</h3>
        </div>
      </div>
    </section>

    <!-- Features -->
    <section class="features-section">
      <div class="container">
        <h2 class="text-center mb-4" style="font-size: 2.5rem;">Tại sao chọn chúng tôi?</h2>
        <div class="features-grid">
          <div class="feature-card">
            <div class="feature-icon">🚀</div>
            <h3>Giao hàng nhanh</h3>
            <p>Giao hàng trong vòng 30 phút</p>
          </div>
          <div class="feature-card">
            <div class="feature-icon">⭐</div>
            <h3>Chất lượng đảm bảo</h3>
            <p>Nguyên liệu tươi ngon, vệ sinh an toàn</p>
          </div>
          <div class="feature-card">
            <div class="feature-icon">💰</div>
            <h3>Giá cả hợp lý</h3>
            <p>Nhiều ưu đãi và khuyến mãi</p>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import FoodCard from '../../components/foods/FoodCard.vue'
import { getFoods } from '../../api/foods'

const router = useRouter()
const featuredFoods = ref([])
const loading = ref(false)
const error = ref(null)

const categories = ref([
  { id: 1, name: 'Burger', icon: '🍔' },
  { id: 2, name: 'Pizza', icon: '🍕' },
  { id: 3, name: 'Gà rán', icon: '🍗' },
  { id: 4, name: 'Đồ uống', icon: '🥤' }
])

const loadFoods = async () => {
  loading.value = true
  error.value = null

  try {
    const res = await getFoods({ isFeatured: true })
    console.log('API Response:', res.data) // Debug
    featuredFoods.value = res.data.slice(0, 8)
  } catch (err) {
    console.error('API Error:', err)
    error.value = 'Không thể kết nối tới server. Vui lòng kiểm tra backend đã chạy chưa.'

    // Fallback mock data để test
    featuredFoods.value = [
      { id: 1, name: 'Burger Phô Mai', price: 45000, imageUrl: 'https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=400', description: 'Burger thịt bò, phô mai', isAvailable: true, isFeatured: true },
      { id: 2, name: 'Pizza Hải Sản', price: 89000, imageUrl: 'https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=400', description: 'Pizza hải sản tươi ngon', isAvailable: true, isFeatured: true },
      { id: 3, name: 'Gà Rán Giòn', price: 65000, imageUrl: 'https://images.unsplash.com/photo-1626645738196-c2a7c87a8f58?w=400', description: 'Gà rán giòn rụm', isAvailable: true, isFeatured: true },
      { id: 4, name: 'Coca Cola', price: 15000, imageUrl: 'https://images.unsplash.com/photo-1554866585-cd94860890b7?w=400', description: 'Nước ngọt', isAvailable: true, isFeatured: true }
    ]
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadFoods()
})

const goToCategory = (id) => {
  router.push(`/menu?category=${id}`)
}
</script>

<style scoped>
.btn-large {
  padding: 1rem 2rem;
  font-size: 1.125rem;
}

.food-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.5rem;
}

.categories-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1.5rem;
}

.category-card {
  background: white;
  padding: 2rem;
  border-radius: 1rem;
  text-align: center;
  cursor: pointer;
  box-shadow: var(--shadow);
  transition: var(--transition);
}

.category-card:hover {
  transform: translateY(-8px);
  box-shadow: var(--shadow-lg);
}

.category-icon {
  font-size: 4rem;
  margin-bottom: 1rem;
}

.features-section {
  margin-top: 4rem;
  padding: 4rem 1rem;
  background: var(--light-gray);
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
}

.feature-card {
  text-align: center;
  padding: 2rem;
}

.feature-icon {
  font-size: 4rem;
  margin-bottom: 1rem;
}

.feature-card h3 {
  font-size: 1.5rem;
  margin-bottom: 0.5rem;
  color: var(--dark);
}

.feature-card p {
  color: var(--gray);
}

.error-message {
  text-align: center;
  padding: 3rem;
  color: #EF4444;
}

@media (max-width: 768px) {
  .food-grid {
    grid-template-columns: 1fr;
  }
}
</style>
