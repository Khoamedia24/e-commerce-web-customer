(function () {
  'use strict';

  document.querySelectorAll('[data-password-toggle]').forEach((button) => {
    button.addEventListener('click', () => {
      const inputId = button.getAttribute('data-password-toggle');
      const input = inputId ? document.getElementById(inputId) : null;

      if (!input) {
        return;
      }

      const shouldShow = input.type === 'password';
      input.type = shouldShow ? 'text' : 'password';
      button.textContent = shouldShow ? 'Ẩn' : 'Hiện';
      button.setAttribute('aria-label', shouldShow ? 'Ẩn mật khẩu' : 'Hiện mật khẩu');
    });
  });
})();
