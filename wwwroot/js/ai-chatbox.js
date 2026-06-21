(function () {
  'use strict';

  const root = document.getElementById('ai-chat');
  if (!root) return;

  const storageKey = 'techstore-ai-chat-history';
  const panel = document.getElementById('ai-chat-panel');
  const toggle = document.getElementById('ai-chat-toggle');
  const close = document.getElementById('ai-chat-close');
  const form = document.getElementById('ai-chat-form');
  const input = document.getElementById('ai-chat-input');
  const messages = document.getElementById('ai-chat-messages');
  const loading = document.getElementById('ai-chat-loading');
  const submit = form.querySelector('button[type="submit"]');

  let history = loadHistory();
  if (history.length === 0) {
    history = [{
      role: 'assistant',
      message: 'Xin chào! Mình có thể giúp bạn tìm và so sánh sản phẩm tại TechStore.'
    }];
  }
  renderHistory();

  toggle.addEventListener('click', () => setOpen(!root.classList.contains('is-open')));
  close.addEventListener('click', () => setOpen(false));

  form.addEventListener('submit', async (event) => {
    event.preventDefault();
    const question = input.value.trim();
    if (!question || submit.disabled) return;

    const previousHistory = history.slice(-8).map((item) => ({
      role: item.role,
      message: item.message
    }));
    addMessage('user', question);
    input.value = '';
    setLoading(true);

    try {
      const response = await fetch('/api/chat', {
        method: 'POST',
        credentials: 'same-origin',
        headers: {
          Accept: 'application/json',
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ message: question, history: previousHistory })
      });
      const result = await response.json().catch(() => null);

      if (!response.ok || !result?.success) {
        throw new Error(result?.message || 'Không thể nhận phản hồi từ trợ lý.');
      }

      addMessage('assistant', result.reply, false, result.products);
    } catch (error) {
      addMessage('assistant', error.message || 'Chatbox đang gặp lỗi. Vui lòng thử lại sau.', true);
    } finally {
      setLoading(false);
      input.focus();
    }
  });

  input.addEventListener('keydown', (event) => {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      form.requestSubmit();
    }
  });

  function setOpen(open) {
    root.classList.toggle('is-open', open);
    panel.setAttribute('aria-hidden', String(!open));
    toggle.setAttribute('aria-expanded', String(open));
    if (open) {
      scrollToLatest();
      input.focus();
    }
  }

  function addMessage(role, message, isError = false, products = []) {
    history.push({
      role,
      message: String(message).trim(),
      isError,
      products: Array.isArray(products) ? products : []
    });
    history = history.slice(-20);
    saveHistory();
    renderHistory();
  }

  function renderHistory() {
    messages.replaceChildren();
    history.forEach((item) => {
      const bubble = document.createElement('div');
      bubble.className = `ai-chat__message ai-chat__message--${item.role}`;
      if (item.isError) bubble.classList.add('is-error');
      bubble.textContent = item.message;
      messages.appendChild(bubble);

      if (item.role === 'assistant' && Array.isArray(item.products) && item.products.length > 0) {
        messages.appendChild(createProductCards(item.products));
      }
    });
    scrollToLatest();
  }

  function createProductCards(products) {
    const list = document.createElement('div');
    list.className = 'ai-chat__products';
    list.setAttribute('aria-label', 'Sản phẩm được gợi ý');

    products.forEach((product) => {
      if (typeof product?.detailUrl !== 'string' || !product.detailUrl.startsWith('/product/')) {
        return;
      }

      const card = document.createElement('a');
      card.className = 'ai-chat__product-card';
      card.href = product.detailUrl;

      if (product.imageUrl) {
        const image = document.createElement('img');
        image.src = product.imageUrl;
        image.alt = product.name || 'Sản phẩm';
        image.loading = 'lazy';
        card.appendChild(image);
      }

      const content = document.createElement('span');
      content.className = 'ai-chat__product-content';

      if (product.categoryName) {
        const category = document.createElement('small');
        category.textContent = product.categoryName;
        content.appendChild(category);
      }

      const name = document.createElement('strong');
      name.textContent = product.name || 'Sản phẩm';
      content.appendChild(name);

      const footer = document.createElement('span');
      footer.className = 'ai-chat__product-footer';

      const price = document.createElement('b');
      price.textContent = formatPrice(product.price);
      footer.appendChild(price);

      const detail = document.createElement('span');
      detail.className = 'ai-chat__product-link';
      detail.textContent = 'Xem chi tiết';
      footer.appendChild(detail);

      content.appendChild(footer);
      card.appendChild(content);
      list.appendChild(card);
    });

    return list;
  }

  function formatPrice(value) {
    const price = Number(value);
    return Number.isFinite(price)
      ? `${new Intl.NumberFormat('vi-VN').format(price)}đ`
      : 'Liên hệ';
  }

  function setLoading(isLoading) {
    loading.hidden = !isLoading;
    submit.disabled = isLoading;
    input.disabled = isLoading;
    if (isLoading) scrollToLatest();
  }

  function scrollToLatest() {
    requestAnimationFrame(() => {
      messages.scrollTop = messages.scrollHeight;
    });
  }

  function loadHistory() {
    try {
      const value = JSON.parse(localStorage.getItem(storageKey));
      return Array.isArray(value)
        ? value.filter((item) =>
          ['user', 'assistant'].includes(item?.role) && typeof item?.message === 'string')
        : [];
    } catch {
      return [];
    }
  }

  function saveHistory() {
    try {
      localStorage.setItem(storageKey, JSON.stringify(history));
    } catch {
      // Chat vẫn hoạt động nếu trình duyệt chặn localStorage.
    }
  }
})();
