(function () {
  'use strict';

  const page = document.querySelector('[data-cart-page]');

  if (!page) {
    return;
  }

  /* ─── Formatters ─────────────────────────────────────────────── */

  const formatPrice = new Intl.NumberFormat('vi-VN', { maximumFractionDigits: 0 });
  const money = (value) => `${formatPrice.format(value)}đ`;

  /* ─── Element refs ───────────────────────────────────────────── */

  const itemsContainer    = page.querySelector('[data-cart-items]');
  const toolbar           = page.querySelector('[data-cart-toolbar]');
  const emptyState        = page.querySelector('[data-cart-empty]');
  const selectAllCheckbox = page.querySelector('[data-cart-select-all]');
  const selectedCountEl   = page.querySelector('[data-cart-selected-count]');
  const deleteSelectedBtn = page.querySelector('[data-cart-delete-selected]');
  const clearButton       = page.querySelector('[data-cart-clear]');
  const titleCount        = page.querySelector('[data-cart-title-count]');
  const subtotalOutput    = page.querySelector('[data-cart-subtotal]');
  const shippingOutput    = page.querySelector('[data-cart-shipping]');
  const taxOutput         = page.querySelector('[data-cart-tax]');
  const totalOutput       = page.querySelector('[data-cart-total]');
  const checkoutButton    = page.querySelector('[data-cart-checkout]');

  /* ─── Helpers ────────────────────────────────────────────────── */

  function getItems() {
    return Array.from(page.querySelectorAll('[data-cart-item]'));
  }

  function getQuantity(item) {
    return Number(item.querySelector('[data-cart-quantity]').textContent) || 0;
  }

  function readCsrfToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
  }

  function readCartItemPayload(item) {
    return {
      id: item.dataset.productId || '',
      name: item.querySelector('h2 a')?.textContent?.trim() || '',
      productUrl: item.querySelector('h2 a')?.getAttribute('href') || '',
      imageUrl: item.querySelector('img')?.getAttribute('src') || '',
      imageAlt: item.querySelector('img')?.getAttribute('alt') || '',
      variant: (() => {
        const btn = item.querySelector('.cart-item-variant');
        if (!btn) return '';

        return Array.from(btn.childNodes)
          .filter((node) => node.nodeType === Node.TEXT_NODE)
          .map((node) => node.textContent.trim())
          .join(' ')
          .trim();
      })(),
      unitPrice: parseFloat(item.dataset.unitPrice) || 0,
      quantity: getQuantity(item),
    };
  }

  function collectCartItems(options = {}) {
    return getItems()
      .filter((item) => !options.selectedOnly || isSelected(item))
      .map(readCartItemPayload)
      .filter((item) => item.quantity > 0);
  }

  function isSelected(item) {
    const checkbox = item.querySelector('[data-cart-select]');
    return !checkbox || checkbox.checked;
  }

  function updateItem(item, nextQuantity) {
    const quantity       = Math.max(1, nextQuantity);
    const quantityOutput = item.querySelector('[data-cart-quantity]');
    const decreaseButton = item.querySelector('[data-cart-decrease]');

    quantityOutput.textContent = quantity;
    decreaseButton.disabled    = quantity <= 1;
  }

  let persistTimer = 0;

  function scheduleCartPersist() {
    window.clearTimeout(persistTimer);
    persistTimer = window.setTimeout(() => {
      persistCartState();
    }, 180);
  }

  async function persistCartState() {
    try {
      const res = await fetch('/Cart/SaveSession', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'RequestVerificationToken': readCsrfToken(),
        },
        body: JSON.stringify(collectCartItems()),
      });

      if (!res.ok) return;

      const result = await res.json().catch(() => ({}));
      if (Number.isFinite(Number(result.count))) {
        window.updateCartCount?.(Number(result.count));
      }
    } catch (error) {
      console.warn('Cart session update failed.', error);
    }
  }

  /* ─── Master summary update ──────────────────────────────────── */

  function updateSummary() {
    const items         = getItems();
    const selectedItems = items.filter(isSelected);
    const selectedCount = selectedItems.length;
    const itemCount     = items.reduce((t, i) => t + getQuantity(i), 0);
    const subtotal      = selectedItems.reduce(
      (t, i) => t + (Number(i.dataset.unitPrice) || 0) * getQuantity(i), 0
    );
    const shipping      = Number(shippingOutput?.dataset.value) || 0;
    const tax           = Number(taxOutput?.dataset.value) || 0;
    const total         = subtotal + shipping + tax;

    /* title badge */
    if (titleCount) titleCount.textContent = `(${itemCount})`;

    /* totals */
    if (subtotalOutput) subtotalOutput.textContent = money(subtotal);
    if (totalOutput)    totalOutput.textContent    = money(total);

    /* empty / visible */
    itemsContainer.hidden = items.length === 0;
    if (toolbar)    toolbar.hidden    = items.length === 0;
    emptyState.hidden     = items.length !== 0;

    /* checkout state */
    if (checkoutButton) {
      checkoutButton.classList.toggle('is-disabled', selectedCount === 0);
      checkoutButton.setAttribute('aria-disabled', String(selectedCount === 0));
    }

    /* legacy clear button (if any) */
    if (clearButton) clearButton.disabled = items.length === 0;

    /* delete-selected button */
    if (deleteSelectedBtn) {
      deleteSelectedBtn.disabled = selectedCount === 0;
    }

    /* global nav badge */
    window.updateCartCount?.(itemCount);

    /* select-all checkbox state */
    syncSelectAll(items, selectedCount);

    /* selected count label */
    if (selectedCountEl) {
      selectedCountEl.textContent = selectedCount > 0 ? `(${selectedCount})` : '';
    }
  }

  /* ─── Keep select-all checkbox in sync ──────────────────────── */

  function syncSelectAll(items, selectedCount) {
    if (!selectAllCheckbox) return;

    if (items.length === 0) {
      selectAllCheckbox.checked       = false;
      selectAllCheckbox.indeterminate = false;
    } else if (selectedCount === 0) {
      selectAllCheckbox.checked       = false;
      selectAllCheckbox.indeterminate = false;
    } else if (selectedCount === items.length) {
      selectAllCheckbox.checked       = true;
      selectAllCheckbox.indeterminate = false;
    } else {
      selectAllCheckbox.checked       = false;
      selectAllCheckbox.indeterminate = true;
    }
  }

  /* ─── Event delegation (click) ──────────────────────────────── */

  page.addEventListener('click', (event) => {
    const increaseButton   = event.target.closest('[data-cart-increase]');
    const decreaseButton   = event.target.closest('[data-cart-decrease]');
    const removeButton     = event.target.closest('[data-cart-remove]');

    if (increaseButton) {
      const item        = increaseButton.closest('[data-cart-item]');
      const maxQuantity = Number(increaseButton.dataset.maxQuantity) || 10;
      updateItem(item, Math.min(getQuantity(item) + 1, maxQuantity));
      updateSummary();
      scheduleCartPersist();
      return;
    }

    if (decreaseButton) {
      const item = decreaseButton.closest('[data-cart-item]');
      updateItem(item, getQuantity(item) - 1);
      updateSummary();
      scheduleCartPersist();
      return;
    }

    if (removeButton) {
      const item = removeButton.closest('[data-cart-item]');
      item.remove();
      updateSummary();
      scheduleCartPersist();
      return;
    }
  });

  /* ─── Per-item checkbox change ──────────────────────────────── */

  page.addEventListener('change', (event) => {
    if (event.target.matches('[data-cart-select]')) {
      updateSummary();
    }
  });

  /* ─── Select All ─────────────────────────────────────────────── */

  if (selectAllCheckbox) {
    selectAllCheckbox.addEventListener('change', () => {
      const checked = selectAllCheckbox.checked;
      getItems().forEach((item) => {
        const checkbox = item.querySelector('[data-cart-select]');
        if (checkbox) checkbox.checked = checked;
      });
      updateSummary();
    });
  }

  /* ─── Delete Selected ────────────────────────────────────────── */

  if (deleteSelectedBtn) {
    deleteSelectedBtn.addEventListener('click', () => {
      getItems().forEach((item) => {
        if (isSelected(item)) item.remove();
      });
      updateSummary();
      scheduleCartPersist();
    });
  }

  /* ─── Legacy clear-all button (if present in markup) ─────────── */

  if (clearButton) {
    clearButton.addEventListener('click', () => {
      getItems().forEach((item) => item.remove());
      updateSummary();
      scheduleCartPersist();
    });
  }

  /* ─── Checkout intercept: save cart to server session first ──── */

  if (checkoutButton) {
    checkoutButton.addEventListener('click', async (event) => {
      // Only intercept real navigation (not disabled state)
      if (checkoutButton.classList.contains('is-disabled')) return;

      event.preventDefault();

      const selectedItems = collectCartItems({ selectedOnly: true });

      if (selectedItems.length === 0) return;

      try {
        const res = await fetch('/Cart/SaveSession', {
          method:  'POST',
          headers: {
            'Content-Type':                'application/json',
            'RequestVerificationToken':    readCsrfToken(),
          },
          body: JSON.stringify(selectedItems),
        });

        if (!res.ok) {
          console.warn('Cart session save failed, navigating anyway.');
        }
      } catch {
        // Network error - navigate anyway; checkout redirects back to cart if the session is empty.
      }

      // Navigate to checkout
      window.location.href = checkoutButton.href;
    });
  }

  /* ─── Init ───────────────────────────────────────────────────── */

  updateSummary();
})();
