<template>
  <div v-if="loading" class="loading-container">
    <div class="loading"></div>
    <p>Đang tải...</p>
  </div>

  <div v-else-if="food" class="food-detail">
    <div class="food-detail-grid">
      <!-- Image Section -->
      <div class="food-image-section">
        <div class="main-image">
          <img :src="currentImage" :alt="food.name" />
          <span v-if="food.isFeatured" class="badge-featured">⭐ Nổi bật</span>
          <span v-if="!food.isAvailable" class="badge-unavailable">Hết hàng</span>
        </div>

        <!-- Thumbnail images (if multiple) -->
        <div v-if="food.images && food.images.length > 1" class="thumbnail-gallery">
          <img
              v-for="(img, index) in food.images"
              :key="index"
              :src="img"
              :class="{ active: currentImage === img }"
              @click="currentImage = img"
              class="thumbnail"
          />
        </div>
      </div>

      <!-- Info Section -->
      <div class="food-info-section">
        <h1 class="food-title">{{ food.name }}</h1>

        <!-- Rating -->
        <div class="food-rating-detail">
          <div class="stars">
            <span v-for="i in 5" :key="i" :class="{ filled: i <= Math.floor(food.rating || 4.5) }">⭐</span>
          </div>
          <span class="rating-number">{{ food.rating || 4.5 }}</span>
          <span class="rating-count">({{ food.reviewCount || 0 }} đánh giá)</span>
        </div>

        <!-- Price -->
        <div class="price-section">
          <span v-if="food.originalPrice" class="original-price">{{ food.originalPrice.toLocaleString() }}đ</span>
          <span class="current-price">{{ food.price.toLocaleString() }}đ</span>
          <span v-if="food.originalPrice" class="discount-badge">-{{ calculateDiscount(food.originalPrice, food.price) }}%</span>
        </div>

        <!-- Description -->
        <div class="description-section">
          <h3>Mô tả</h3>
          <p>{{ food.description }}</p>
        </div>

        <!-- Category -->
        <div class="category-section">
          <span class="label">Danh mục:</span>
          <span class="category-badge">{{ food.categoryName || 'Tất cả' }}</span>
        </div>

        <!-- Toppings (if available) -->
        <div v-if="availableToppings.length > 0" class="toppings-section">
          <h3>Topping (Tùy chọn)</h3>
          <div class="toppings-grid">
            <label v-for="topping in availableToppings" :key="topping.id" class="topping-item">
              <input
                  type="checkbox"
                  :value="topping.id"
                  v-model="selectedToppings"
              />
              <span>{{ topping.name }} (+{{ topping.price.toLocaleString() }}đ)</span>
            </label>
          </div>
        </div>

        <!-- Quantity -->
        <div class="quantity-section">
          <h3>Số lượng</h3>
          <div class="quantity-control">
            <button @click="decreaseQuantity" :disabled="quantity <= 1">-</button>
            <input type="number" v-model.number="quantity" min="1" />
            <button @click="increaseQuantity">+</button>
          </div>
        </div>

        <!-- Total Price -->
        <div class="total-section">
          <span class="total-label">Tổng cộng:</span>
          <span class="total-price">{{ totalPrice.toLocaleString() }}đ</span>
        </div>

        <!-- Actions -->
        <div class="action-buttons">
          <button
              @click="addToCart"
              :disabled="!food.isAvailable"
              class="btn btn-primary btn-large"
          >
            {{ food.isAvailable ? '🛒 Thêm vào giỏ hàng' : 'Hết hàng' }}
          </button>
          <button @click="buyNow" :disabled="!food.isAvailable" class="btn btn-secondary btn-large">
            ⚡ Mua ngay
          </button>
        </div>
      </div>
    </div>

    <!-- Reviews Section -->
    <div class="reviews-section">
      <h2>Đánh giá của khách hàng</h2>

      <div v-if="reviews.length === 0" class="no-reviews">
        <p>Chưa có đánh giá nào</p>
      </div>

      <div v-else class="reviews-list">
        <div v-for="review in reviews" :key="review.id" class="review-item">
          <div class="review-header">
            <div class="reviewer-info">
              <div class="avatar">{{ review.userName?.charAt(0) }}</div>
              <div>
                <p class="reviewer-name">{{ review.userName }}</p>
                <div class="review-stars">
                  <span v-for="i in review.rating" :key="i">⭐</span>
                </div>
              </div>
            </div>
            <span class="review-date">{{ formatDate(review.createdDate) }}</span>
          </div>
          <p class="review-comment">{{ review.comment }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useCartStore } from '../../store/cart'
import { getToppings } from '../../api/toppings'

const props = defineProps({
  food: {
    type: Object,
    required: true
  },
  loading: {
    type: Boolean,
    default: false
  }
})

const router = useRouter()
const cartStore = useCartStore()

const currentImage = ref(props.food?.imageUrl || '/placeholder.jpg')
const quantity = ref(1)
const selectedToppings = ref([])
const reviews = ref([])

// Mock toppings - thay bằng API call thực tế
const availableToppings = ref([
  { id: 1, name: 'Phô mai', price: 10000 },
  { id: 2, name: 'Thịt xông khói', price: 15000 },
  { id: 3, name: 'Trứng', price: 8000 }
])

const totalPrice = computed(() => {
  let price = props.food.price * quantity.value
  selectedToppings.value.forEach(toppingId => {
    const topping = availableToppings.value.find(t => t.id === toppingId)
    if (topping) {
      price += topping.price * quantity.value
    }
  })
  return price
})

const increaseQuantity = () => {
  quantity.value++
}

const decreaseQuantity = () => {
  if (quantity.value > 1) {
    quantity.value--
  }
}

const calculateDiscount = (original, current) => {
  return Math.round(((original - current) / original) * 100)
}

const addToCart = () => {
  const item = {
    ...props.food,
    quantity: quantity.value,
    toppings: selectedToppings.value.map(id =>
        availableToppings.value.find(t => t.id === id)
    )
  }
  cartStore.addItem(item, quantity.value)
  alert(`Đã thêm ${quantity.value} "${props.food.name}" vào giỏ hàng!`)
}

const buyNow = () => {
  addToCart()
  router.push('/cart')
}

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('vi-VN')
}

//Compilot
// Trong onMounted
onMounted(async () => {
  try {
    const res = await getToppings()
    availableToppings.value = res.data
  } catch (error) {
    console.error('Lỗi load toppings:', error)
  }
})
</script>

<style scoped>
.loading-container {
  text-align: center;
  padding: 4rem;
}

.food-detail {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem 1rem;
}

.food-detail-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 3rem;
  margin-bottom: 3rem;
}

@media (max-width: 968px) {
  .food-detail-grid {
    grid-template-columns: 1fr;
  }
}

/* Image Section */
.food-image-section {
  position: sticky;
  top: 5rem;
  height: fit-content;
}

.main-image {
  position: relative;
  border-radius: 1rem;
  overflow: hidden;
  box-shadow: var(--shadow-lg);
  margin-bottom: 1rem;
}

.main-image img {
  width: 100%;
  height: 500px;
  object-fit: cover;
}

.badge-featured, .badge-unavailable {
  position: absolute;
  top: 1rem;
  right: 1rem;
  padding: 0.5rem 1rem;
  border-radius: 999px;
  font-weight: bold;
  color: white;
  font-size: 0.875rem;
}

.badge-featured {
  background: linear-gradient(135deg, #F59E0B, #D97706);
}

.badge-unavailable {
  background: #6B7280;
}

.thumbnail-gallery {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 0.75rem;
}

.thumbnail {
  width: 100%;
  height: 80px;
  object-fit: cover;
  border-radius: 0.5rem;
  cursor: pointer;
  opacity: 0.6;
  transition: var(--transition);
  border: 2px solid transparent;
}

.thumbnail:hover {
  opacity: 1;
}

.thumbnail.active {
  opacity: 1;
  border-color: var(--primary);
}

/* Info Section */
.food-title {
  font-size: 2rem;
  color: var(--dark);
  margin-bottom: 1rem;
}

.food-rating-detail {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 1.5rem;
}

.stars span {
  font-size: 1.25rem;
  filter: grayscale(100%);
}

.stars span.filled {
  filter: grayscale(0%);
}

.rating-number {
  font-weight: bold;
  font-size: 1.125rem;
}

.rating-count {
  color: var(--gray);
}

.price-section {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.original-price {
  text-decoration: line-through;
  color: var(--gray);
  font-size: 1.25rem;
}

.current-price {
  font-size: 2rem;
  font-weight: bold;
  color: var(--primary);
}

.discount-badge {
  background: #EF4444;
  color: white;
  padding: 0.25rem 0.75rem;
  border-radius: 999px;
  font-size: 0.875rem;
  font-weight: bold;
}

.description-section {
  margin-bottom: 1.5rem;
}

.description-section h3 {
  margin-bottom: 0.5rem;
  color: var(--dark);
}

.description-section p {
  color: var(--gray);
  line-height: 1.8;
}

.category-section {
  margin-bottom: 1.5rem;
}

.label {
  color: var(--gray);
  margin-right: 0.5rem;
}

.category-badge {
  background: var(--light-gray);
  padding: 0.5rem 1rem;
  border-radius: 999px;
  font-weight: 600;
  color: var(--dark);
}

.toppings-section {
  margin-bottom: 1.5rem;
}

.toppings-section h3 {
  margin-bottom: 1rem;
}

.toppings-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 0.75rem;
}

.topping-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem;
  border: 2px solid var(--light-gray);
  border-radius: 0.5rem;
  cursor: pointer;
  transition: var(--transition);
}

.topping-item:hover {
  border-color: var(--primary);
}

.topping-item input[type="checkbox"] {
  width: 1.25rem;
  height: 1.25rem;
  cursor: pointer;
}

.quantity-section {
  margin-bottom: 1.5rem;
}

.quantity-section h3 {
  margin-bottom: 1rem;
}

.quantity-control {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.quantity-control button {
  width: 3rem;
  height: 3rem;
  border: 2px solid var(--light-gray);
  background: white;
  border-radius: 0.5rem;
  font-size: 1.5rem;
  cursor: pointer;
  transition: var(--transition);
}

.quantity-control button:hover:not(:disabled) {
  background: var(--primary);
  color: white;
  border-color: var(--primary);
}

.quantity-control button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.quantity-control input {
  width: 5rem;
  height: 3rem;
  text-align: center;
  font-size: 1.25rem;
  font-weight: bold;
  border: 2px solid var(--light-gray);
  border-radius: 0.5rem;
}

.total-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  background: var(--light-gray);
  border-radius: 0.5rem;
  margin-bottom: 1.5rem;
}

.total-label {
  font-size: 1.25rem;
  color: var(--dark);
}

.total-price {
  font-size: 2rem;
  font-weight: bold;
  color: var(--primary);
}

.action-buttons {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.btn-large {
  padding: 1rem;
  font-size: 1.125rem;
  font-weight: bold;
}

/* Reviews Section */
.reviews-section {
  margin-top: 3rem;
  padding-top: 3rem;
  border-top: 2px solid var(--light-gray);
}

.reviews-section h2 {
  margin-bottom: 2rem;
  font-size: 1.75rem;
}

.no-reviews {
  text-align: center;
  padding: 3rem;
  color: var(--gray);
}

.reviews-list {
  display: grid;
  gap: 1.5rem;
}

.review-item {
  padding: 1.5rem;
  background: white;
  border-radius: 1rem;
  box-shadow: var(--shadow);
}

.review-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.reviewer-info {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.avatar {
  width: 3rem;
  height: 3rem;
  border-radius: 50%;
  background: var(--primary);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  font-size: 1.25rem;
}

.reviewer-name {
  font-weight: 600;
  margin-bottom: 0.25rem;
}

.review-stars {
  font-size: 0.875rem;
}

.review-date {
  color: var(--gray);
  font-size: 0.875rem;
}

.review-comment {
  color: var(--dark);
  line-height: 1.6;
}
</style>
