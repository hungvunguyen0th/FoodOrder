import api from './index'

// READ
export const getFoods = (params) => api.get('/foods', { params })
export const getFoodById = (id) => api.get(`/foods/${id}`)
export const getFoodToppings = (foodId) => api.get(`/foods/${foodId}/toppings`)

// CREATE
export const createFood = (data) => api.post('/foods', data)

// UPDATE
export const updateFood = (id, data) => api.put(`/foods/${id}`, data)

// DELETE
export const deleteFood = (id) => api.delete(`/foods/${id}`)

// STOCK
export const updateFoodStock = (id, quantity) => api.patch(`/foods/${id}/stock`, null, { params: { quantity } })

// CATEGORIES
export const getCategories = () => api.get('/categories')
export const getCategoryById = (id) => api.get(`/categories/${id}`)
export const createCategory = (data) => api.post('/categories', data)
export const updateCategory = (id, data) => api.put(`/categories/${id}`, data)
export const deleteCategory = (id) => api.delete(`/categories/${id}`)