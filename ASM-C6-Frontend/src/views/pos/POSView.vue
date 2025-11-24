<template>
  <div class="flex h-screen bg-gray-50">
    <!-- Phần chọn món -->
    <div class="flex-1 p-6 overflow-y-auto">
      <h2 class="text-2xl font-bold mb-6">Chọn món</h2>

      <!-- Loading -->
      <div v-if="loading" class="text-center py-8">
        <p class="text-gray-500">Đang tải...</p>
      </div>

      <!-- Danh sách món -->
      <div v-else class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
        <button
            v-for="food in foods"
            :key="food.id"
            @click="addToPOSCart(food)"
            class="bg-white p-4 rounded-lg shadow hover:shadow-lg transition-shadow border-2 border-transparent hover:border-orange-400"
        >
          <img :src="food.imageUrl || '/placeholder.jpg'" class="w-full h-32 object-cover rounded mb-2" />
          <p class="font-semibold text-sm">{{ food.name }}</p>
          <p class="text-orange-600 font-bold">{{ food.price.toLocaleString() }}đ</p>
        </button>
      </div>
    </div>

    <!-- Giỏ hàng POS -->
    <div class="w-96 bg-white shadow-xl p-6 flex flex-col">
      <h2 class="text-xl font-bold mb-4 pb-4 border-b">Đơn hàng hiện tại</h2>

      <!-- Danh sách món trong giỏ -->
      <div class="flex-1 overflow-y-auto">
        <div v-if="posCart.length === 0" class="text-center text-gray-400 py-8">
          Chưa có món nào
        </div>

        <div v-else>
          <div v-for="item in posCart" :key="item.id" class="mb-3 pb-3 border-b">
            <div class="flex justify-between items-start">
              <div class="flex-1">
                <p class="font-semibold">{{ item.name }}</p>
                <p class="text-sm text-gray-600">{{ item.price.toLocaleString() }}đ</p>
              </div>
              <button @click="removeFromPOSCart(item.id)" class="text-red-500 hover:text-red-700">
                ✕
              </button>
            </div>

            <div class="flex items-center gap-2 mt-2">
              <button @click="decreaseQuantity(item.id)" class="px-2 py-1 bg-gray-200 rounded">-</button>
              <span class="px-3">{{ item.quantity }}</span>
              <button @click="increaseQuantity(item.id)" class="px-2 py-1 bg-gray-200 rounded">+</button>
              <span class="ml-auto font-bold">{{ (item.price * item.quantity).toLocaleString() }}đ</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Thông tin khách hàng -->
      <div class="mt-4 pt-4 border-t">
        <input
            v-model="customerName"
            placeholder="Tên khách hàng"
            class="w-full p-2 border rounded mb-2"
        />
        <input
            v-model="customerPhone"
            placeholder="Số điện thoại"
            class="w-full p-2 border rounded"
        />
      </div>

      <!-- Tổng tiền và thanh toán -->
      <div class="mt-4 pt-4 border-t">
        <div class="flex justify-between mb-2">
          <span class="text-gray-600">Tổng cộng:</span>
          <span class="text-2xl font-bold text-orange-600">{{ totalPrice.toLocaleString() }}đ</span>
        </div>

        <button
            @click="submitOrder"
            :disabled="posCart.length === 0 || submitting"
            class="w-full bg-orange-600 text-white py-3 rounded-lg font-bold hover:bg-orange-700 disabled:bg-gray-300 disabled:cursor-not-allowed"
        >
          {{ submitting ? 'Đang xử lý...' : 'Tạo đơn hàng' }}
        </button>

        <button
            @click="clearPOSCart"
            class="w-full mt-2 bg-gray-200 text-gray-700 py-2 rounded-lg hover:bg-gray-300"
        >
          Hủy đơn
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { getFoods } from '../../api/foods'
import { createOrder as apiCreateOrder } from '../../api/orders' // Đổi tên để tránh trùng

const foods = ref([])
const posCart = ref([])
const loading = ref(false)
const submitting = ref(false)
const customerName = ref('')
const customerPhone = ref('')

const totalPrice = computed(() =>
    posCart.value.reduce((sum, item) => sum + item.price * item.quantity, 0)
)

onMounted(async () => {
  loading.value = true
  try {
    const res = await getFoods()
    foods.value = res.data
  } catch (error) {
    alert('Lỗi khi tải danh sách món')
  } finally {
    loading.value = false
  }
})

const addToPOSCart = (food) => {
  const existing = posCart.value.find(i => i.id === food.id)
  if (existing) {
    existing.quantity++
  } else {
    posCart.value.push({ ...food, quantity: 1 })
  }
}

const removeFromPOSCart = (id) => {
  posCart.value = posCart.value.filter(i => i.id !== id)
}

const increaseQuantity = (id) => {
  const item = posCart.value.find(i => i.id === id)
  if (item) item.quantity++
}

const decreaseQuantity = (id) => {
  const item = posCart.value.find(i => i.id === id)
  if (item && item.quantity > 1) {
    item.quantity--
  }
}

const clearPOSCart = () => {
  if (confirm('Xác nhận hủy đơn hàng?')) {
    posCart.value = []
    customerName.value = ''
    customerPhone.value = ''
  }
}

const submitOrder = async () => {
  if (!customerName.value || !customerPhone.value) {
    alert('Vui lòng nhập tên và số điện thoại khách hàng')
    return
  }

  submitting.value = true
  try {
    const orderData = {
      customerName: customerName.value,
      phoneNumber: customerPhone.value,
      orderType: 1, // TakeAway
      paymentMethod: 0, // Cash
      items: posCart.value.map(item => ({
        foodItemId: item.id,
        itemName: item.name,
        quantity: item.quantity,
        unitPrice: item.price
      }))
    }

    await apiCreateOrder(orderData)
    alert('Tạo đơn hàng thành công!')

    // Reset form
    posCart.value = []
    customerName.value = ''
    customerPhone.value = ''
  } catch (error) {
    alert('Lỗi khi tạo đơn hàng: ' + (error.response?.data?.message || error.message))
  } finally {
    submitting.value = false
  }
}
</script>
