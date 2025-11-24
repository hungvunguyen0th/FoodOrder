<template>
  <footer class="footer">
    <div class="container">
      <div class="footer-grid">
        <div>
          <h3 class="footer-title">🍔 FastFood</h3>
          <p class="footer-text">Chuỗi cửa hàng đồ ăn nhanh uy tín</p>
        </div>
        <div>
          <h4 class="footer-subtitle">Liên hệ</h4>
          <p class="footer-text">📞 1900 xxxx</p>
          <p class="footer-text">📧 contact@fastfood.vn</p>
        </div>
      </div>
      <div class="footer-bottom">
        <p>&copy; 2025 FastFood. All rights reserved.</p>
      </div>
    </div>
  </footer>
</template>

<style scoped>
.footer-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
  margin-bottom: 2rem;
}

.footer-title {
  color: var(--primary);
  margin-bottom: 1rem;
  font-size: 1.5rem;
}

.footer-subtitle {
  color: var(--primary);
  margin-bottom: 1rem;
}

.footer-text {
  color: #9CA3AF;
  margin-bottom: 0.5rem;
}

.footer-bottom {
  border-top: 1px solid #374151;
  padding-top: 2rem;
  text-align: center;
  color: #9CA3AF;
}
</style>
