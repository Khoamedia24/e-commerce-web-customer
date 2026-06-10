/**
 * site.js - Global site scripts
 * Handles: back-to-top, toast notifications and cart badge sync
 */

(function () {
  'use strict';

  // ============================================================
  // BACK TO TOP
  // ============================================================
  const backToTopBtn = document.getElementById('back-to-top');

  if (backToTopBtn) {
    window.addEventListener('scroll', () => {
      if (window.scrollY > 400) {
        backToTopBtn.classList.add('visible');
      } else {
        backToTopBtn.classList.remove('visible');
      }
    }, { passive: true });

    backToTopBtn.addEventListener('click', () => {
      window.scrollTo({ top: 0, behavior: 'smooth' });
    });
  }

  // ============================================================
  // CART BADGE UPDATE
  // Backend should call window.updateCartCount(n) after cart mutations
  // ============================================================
  window.updateCartCount = function (count) {
    const badges = document.querySelectorAll('[id^="cart-item-count"]');
    badges.forEach((badge) => {
      if (count > 0) {
        badge.textContent = count > 99 ? '99+' : count;
        badge.hidden = false;
        badge.setAttribute('aria-label', `${count} sản phẩm trong giỏ`);
      } else {
        badge.hidden = true;
      }
    });
  };

  // ============================================================
  // TOAST NOTIFICATIONS
  // Usage: window.showToast('Đã thêm vào giỏ hàng', 'success')
  // ============================================================
  window.showToast = function (message, type = 'default', duration = 3000) {
    const container = document.getElementById('toast-container');
    if (!container) return;

    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.innerHTML = `
      <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="none" viewBox="0 0 24 24" aria-hidden="true">
        <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 1 1-18 0 9 9 0 0 1 18 0z"/>
      </svg>
      ${escapeHtml(message)}
    `;
    container.appendChild(toast);

    setTimeout(() => {
      toast.style.opacity = '0';
      toast.style.transform = 'translateY(8px)';
      toast.style.transition = 'all 0.3s ease';
      setTimeout(() => toast.remove(), 300);
    }, duration);
  };

  // ============================================================
  // HELPER: Escape HTML for user input
  // ============================================================
  function escapeHtml(str) {
    const div = document.createElement('div');
    div.appendChild(document.createTextNode(str));
    return div.innerHTML;
  }

})();
