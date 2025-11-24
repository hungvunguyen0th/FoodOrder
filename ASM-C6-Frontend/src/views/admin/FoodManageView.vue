<template>
  <div class="container mx-auto p-6">
    <h1 class="text-3xl font-bold mb-6">Quản lý món ăn</h1>
    <button @click="showAddForm = true" class="btn btn-primary mb-4">Thêm món mới</button>

    <table class="w-full">
      <thead>
      <tr class="bg-gray-100">
        <th class="p-3 text-left">Tên món</th>
        <th class="p-3 text-left">Giá</th>
        <th class="p-3 text-left">Trạng thái</th>
        <th class="p-3">Hành động</th>
      </tr>
      </thead>
      <tbody>
      <tr v-for="food in foods" :key="food.id" class="border-b">
        <td class="p-3">{{ food.name }}</td>
        <td class="p-3">{{ food.price.toLocaleString() }}đ</td>
        <td class="p-3">{{ food.isAvailable ? 'Còn' : 'Hết' }}</td>
        <td class="p-3 text-center">
          <button @click="editFood(food)" class="btn-sm">Sửa</button>
          <button @click="deleteFood(food.id)" class="btn-sm btn-danger">Xóa</button>
        </td>
      </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { getFoods, deleteFood as apiDeleteFood } from '../../api/foods'

const foods = ref([])
const showAddForm = ref(false)

onMounted(async () => {
  const res = await getFoods()
  foods.value = res.data
})

const deleteFood = async (id) => {
  if (confirm('Xác nhận xóa?')) {
    await apiDeleteFood(id)
    foods.value = foods.value.filter(f => f.id !== id)
  }
}
</script>
