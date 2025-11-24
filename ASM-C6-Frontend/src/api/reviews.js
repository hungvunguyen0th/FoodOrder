import api from './index'

// READ
export const getReviewsByFood = (foodId) => api.get(`/reviews/food/${foodId}`)
export const getUserReviews = (userId) => api.get(`/reviews/user/${userId}`)

// CREATE
export const createReview = (data) => api.post('/reviews', data)

// UPDATE
export const approveReview = (id) => api.put(`/reviews/${id}/approve`)

// DELETE
export const deleteReview = (id) => api.delete(`/reviews/${id}`)