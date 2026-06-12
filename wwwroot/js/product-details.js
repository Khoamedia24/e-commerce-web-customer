(function () {
  'use strict';

  const page = document.querySelector('[data-product-detail]');
  if (!page) return;

  const mainImage = page.querySelector('[data-product-main-image]');
  const stickyOrder = page.querySelector('[data-sticky-order]');
  const stickySentinel = page.querySelector('[data-sticky-sentinel]');
  const stickyImage = page.querySelector('[data-sticky-product-image]');
  const stickyProductName = page.querySelector('[data-sticky-product-name]');
  const stickyProductPrice = page.querySelector('[data-sticky-product-price]');
  const stickyPrice = page.querySelector('[data-sticky-price]');
  const specModal = page.querySelector('[data-spec-modal]');
  const specModalContent = specModal?.querySelector('[data-spec-modal-content]');
  const reviewModal = page.querySelector('[data-review-modal]');
  const extraVersions = Array.from(page.querySelectorAll('[data-extra-version="true"]'));
  const thumbs = Array.from(page.querySelectorAll('[data-gallery-thumb]'));
  const cartActions = Array.from(page.querySelectorAll('[data-add-to-cart], [data-buy-now]'));
  const specSections = specModalContent
    ? Array.from(specModalContent.querySelectorAll('[data-spec-section]'))
    : [];
  let modalLockCount = 0;
  let activeGalleryIndex = thumbs.findIndex((thumb) => thumb.classList.contains('is-active'));
  let specScrollFrame = 0;

  if (activeGalleryIndex < 0) {
    activeGalleryIndex = 0;
  }

  page.addEventListener('click', async (event) => {
    if (event.target === specModal) {
      closeModal(specModal);
      return;
    }

    if (event.target === reviewModal) {
      closeModal(reviewModal);
      return;
    }

    const sectionLink = event.target.closest('a[href^="#"]');
    if (sectionLink && sectionLink.hash.length > 1) {
      const target = page.querySelector(sectionLink.hash);

      if (target) {
        event.preventDefault();
        target.scrollIntoView({ behavior: 'smooth', block: 'start' });
        window.history.replaceState(null, '', sectionLink.hash);
        return;
      }
    }

    if (event.target.closest('[data-spec-open]')) {
      openSpecModal();
      return;
    }

    if (event.target.closest('[data-spec-close]')) {
      closeModal(specModal);
      return;
    }

    const specTab = event.target.closest('[data-spec-tab]');
    if (specTab) {
      scrollToSpecSection(specTab.dataset.target);
      return;
    }

    const relatedTab = event.target.closest('[data-related-tab]');
    if (relatedTab) {
      setRelatedGroup(relatedTab);
      return;
    }

    const reviewShowAll = event.target.closest('[data-review-show-all]');
    if (reviewShowAll) {
      revealItems('[data-review-item]', reviewShowAll);
      return;
    }

    const qaShowMore = event.target.closest('[data-qa-show-more]');
    if (qaShowMore) {
      revealItems('[data-qa-thread]', qaShowMore);
      return;
    }

    if (event.target.closest('[data-review-open]')) {
      openModal(reviewModal);
      return;
    }

    if (event.target.closest('[data-review-close]')) {
      closeModal(reviewModal);
      return;
    }

    const versionToggle = event.target.closest('[data-version-toggle]');
    if (versionToggle) {
      toggleExtraVersions(versionToggle);
      return;
    }

    const thumb = event.target.closest('[data-gallery-thumb]');
    if (thumb) {
      showGalleryItem(Number(thumb.dataset.galleryIndex) || 0);
      return;
    }

    if (event.target.closest('[data-gallery-prev]')) {
      showGalleryItem(activeGalleryIndex - 1);
      return;
    }

    if (event.target.closest('[data-gallery-next]')) {
      showGalleryItem(activeGalleryIndex + 1);
      return;
    }

    const colorOption = event.target.closest('[data-color-option]');
    if (colorOption) {
      setActiveButton('[data-color-option]', colorOption);
      updateMainImage(colorOption.dataset.image, colorOption.dataset.alt);
      updateStickyProduct(colorOption.dataset.image, colorOption.dataset.colorName, colorOption.dataset.priceText);
      syncCartActionsFromColor(colorOption);
      return;
    }

    const buyNowButton = event.target.closest('[data-buy-now]');
    if (buyNowButton) {
      event.preventDefault();
      await submitCartAction(buyNowButton, '/Cart/BuyNow', { redirect: true });
      return;
    }

    const addToCartButton = event.target.closest('[data-add-to-cart]');
    if (addToCartButton) {
      await submitCartAction(addToCartButton, '/Cart/AddItem');
    }
  });

  document.addEventListener('keydown', (event) => {
    if (event.key !== 'Escape') return;

    closeModal(specModal);
    closeModal(reviewModal);
  });

  specModalContent?.addEventListener('scroll', () => {
    if (specScrollFrame) return;

    specScrollFrame = window.requestAnimationFrame(() => {
      specScrollFrame = 0;
      updateActiveSpecTabFromScroll();
    });
  }, { passive: true });

  setupStickyOrder();

  function setActiveButton(selector, activeButton) {
    page.querySelectorAll(selector).forEach((button) => {
      const isActive = button === activeButton;
      button.classList.toggle('is-active', isActive);
      button.setAttribute('aria-pressed', isActive ? 'true' : 'false');
    });
  }

  function showGalleryItem(index) {
    if (thumbs.length === 0) return;

    activeGalleryIndex = (index + thumbs.length) % thumbs.length;
    const thumb = thumbs[activeGalleryIndex];

    setActiveButton('[data-gallery-thumb]', thumb);
    updateMainImage(thumb.dataset.image, thumb.dataset.alt);
    thumb.scrollIntoView({ behavior: 'smooth', block: 'nearest', inline: 'center' });
  }

  function toggleExtraVersions(button) {
    const isExpanded = button.getAttribute('aria-expanded') === 'true';
    const nextExpanded = !isExpanded;
    const text = button.querySelector('[data-version-toggle-text]');

    extraVersions.forEach((version) => {
      version.hidden = !nextExpanded;
    });

    button.classList.toggle('is-expanded', nextExpanded);
    button.setAttribute('aria-expanded', nextExpanded ? 'true' : 'false');

    if (text) {
      text.textContent = nextExpanded ? 'Thu gọn phiên bản' : 'Xem thêm phiên bản khác';
    }
  }

  function openSpecModal() {
    openModal(specModal);

    if (specModalContent) {
      specModalContent.scrollTop = 0;
      setActiveSpecTab(specSections[0]?.id);
    }
  }

  function openModal(modal) {
    if (!modal || modal.classList.contains('is-open')) return;

    modal.hidden = false;
    lockBodyScroll();

    window.requestAnimationFrame(() => {
      modal.classList.add('is-open');
    });
  }

  function closeModal(modal) {
    if (!modal || modal.hidden || !modal.classList.contains('is-open')) return;

    modal.classList.remove('is-open');
    unlockBodyScroll();

    window.setTimeout(() => {
      if (!modal.classList.contains('is-open')) {
        modal.hidden = true;
      }
    }, 300);
  }

  function lockBodyScroll() {
    modalLockCount += 1;

    if (modalLockCount === 1) {
      document.body.dataset.productDetailPreviousOverflow = document.body.style.overflow;
      document.body.style.overflow = 'hidden';
    }
  }

  function unlockBodyScroll() {
    modalLockCount = Math.max(0, modalLockCount - 1);

    if (modalLockCount === 0) {
      document.body.style.overflow = document.body.dataset.productDetailPreviousOverflow || '';
      delete document.body.dataset.productDetailPreviousOverflow;
    }
  }

  function scrollToSpecSection(targetId) {
    if (!targetId) return;

    const section = specModal?.querySelector(`#${targetId}`);
    if (!section) return;

    setActiveSpecTab(targetId);
    section.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }

  function updateActiveSpecTabFromScroll() {
    if (!specModalContent || specSections.length === 0) return;

    const rootTop = specModalContent.getBoundingClientRect().top;
    let activeId = specSections[0].id;

    specSections.forEach((section) => {
      const offset = section.getBoundingClientRect().top - rootTop;

      if (offset <= 90) {
        activeId = section.id;
      }
    });

    setActiveSpecTab(activeId);
  }

  function setActiveSpecTab(activeId) {
    if (!activeId) return;

    specModal?.querySelectorAll('[data-spec-tab]').forEach((tab) => {
      const isActive = tab.dataset.target === activeId;
      tab.classList.toggle('is-active', isActive);
      tab.setAttribute('aria-current', isActive ? 'true' : 'false');

      if (isActive) {
        tab.scrollIntoView({ behavior: 'smooth', block: 'nearest', inline: 'center' });
      }
    });
  }

  function setRelatedGroup(activeTab) {
    const activeGroup = activeTab.dataset.relatedTab;
    if (!activeGroup) return;

    page.querySelectorAll('[data-related-tab]').forEach((tab) => {
      const isActive = tab === activeTab;
      tab.classList.toggle('is-active', isActive);
      tab.setAttribute('aria-selected', isActive ? 'true' : 'false');
    });

    page.querySelectorAll('[data-related-panel]').forEach((panel) => {
      panel.hidden = panel.dataset.relatedPanel !== activeGroup;
    });
  }

  function revealItems(selector, trigger) {
    page.querySelectorAll(selector).forEach((item) => {
      item.hidden = false;
    });

    trigger.closest('.pd-review-more, .pd-qa-more')?.setAttribute('hidden', '');
  }

  function setupStickyOrder() {
    if (!stickyOrder || !stickySentinel || !('IntersectionObserver' in window)) return;

    const observer = new IntersectionObserver((entries) => {
      const entry = entries[0];
      const shouldShow = !entry.isIntersecting && entry.boundingClientRect.top < 0;
      setStickyVisible(shouldShow);
    }, {
      threshold: 0,
      rootMargin: '0px 0px -8px 0px'
    });

    observer.observe(stickySentinel);
  }

  function setStickyVisible(isVisible) {
    stickyOrder.classList.toggle('show', isVisible);
    stickyOrder.setAttribute('aria-hidden', isVisible ? 'false' : 'true');
  }

  function syncCartActionsFromColor(colorOption) {
    cartActions.forEach((action) => {
      action.dataset.cartImage = colorOption.dataset.image || action.dataset.cartImage || '';
      action.dataset.cartAlt = colorOption.dataset.alt || action.dataset.cartAlt || '';
      action.dataset.cartVariant = colorOption.dataset.colorName || action.dataset.cartVariant || '';
      action.dataset.cartPrice = colorOption.dataset.price || action.dataset.cartPrice || '';
    });
  }

  async function submitCartAction(action, endpoint, options = {}) {
    if (action.dataset.pending === 'true') return;

    const payload = buildCartPayload(action);
    setActionBusy(action, true);

    try {
      const response = await fetch(endpoint, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'RequestVerificationToken': readCsrfToken(),
        },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        throw new Error(`Cart request failed with status ${response.status}`);
      }

      const result = await response.json().catch(() => ({}));

      if (Number.isFinite(Number(result.count))) {
        window.updateCartCount?.(Number(result.count));
      }

      if (options.redirect && result.redirectUrl) {
        window.location.href = result.redirectUrl;
        return;
      }

      window.showToast?.('Đã thêm sản phẩm vào giỏ hàng', 'success');
    } catch (error) {
      console.error(error);
      window.showToast?.('Không thể cập nhật giỏ hàng. Vui lòng thử lại.', 'error');
    } finally {
      setActionBusy(action, false);
    }
  }

  function buildCartPayload(action) {
    const price = Number(action.dataset.cartPrice);

    return {
      id: action.dataset.cartId || '',
      name: action.dataset.cartName || '',
      productUrl: action.dataset.cartUrl || window.location.pathname,
      imageUrl: action.dataset.cartImage || '',
      imageAlt: action.dataset.cartAlt || '',
      variant: action.dataset.cartVariant || '',
      unitPrice: Number.isFinite(price) ? price : 0,
      quantity: 1,
    };
  }

  function readCsrfToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
  }

  function setActionBusy(action, isBusy) {
    action.dataset.pending = isBusy ? 'true' : 'false';
    action.setAttribute('aria-busy', isBusy ? 'true' : 'false');

    if ('disabled' in action) {
      action.disabled = isBusy;
    }
  }

  function updateStickyProduct(src, colorName, priceText) {
    if (stickyImage && src) {
      stickyImage.src = src;
    }

    if (stickyProductName && colorName) {
      stickyProductName.textContent = `${stickyProductName.dataset.baseName} - ${colorName}`;
    }

    if (priceText) {
      if (stickyProductPrice) stickyProductPrice.textContent = priceText;
      if (stickyPrice) stickyPrice.textContent = priceText;
    }
  }

  function updateMainImage(src, alt) {
    if (!mainImage || !src) return;

    mainImage.src = src;
    updateStickyProduct(src);

    if (alt) {
      mainImage.alt = alt;
    }
  }
})();
