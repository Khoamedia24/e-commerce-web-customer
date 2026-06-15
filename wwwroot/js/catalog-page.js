(function () {
  'use strict';

  const page = document.querySelector('[data-catalog-page]');
  if (!page) return;

  const reducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  initializePromoCarousel();
  initializeHotSaleCarousel();
  initializeStickyFilter();
  initializeFilterDropdowns();
  initializeSectionNavigation();
  initializeSectionPills();
  initializeSectionReveals();
  initializeCountdowns();
  initializeLoadMore();
  initializeSeoContent();
  initializeQuestionAnswer();

  function initializePromoCarousel() {
    const element = page.querySelector('.catalog-promo-swiper');
    if (!element || typeof window.Swiper !== 'function') return;

    new window.Swiper(element, {
      slidesPerView: 1.08,
      spaceBetween: 8,
      speed: reducedMotion ? 0 : 350,
      grabCursor: true,
      watchOverflow: true,
      keyboard: {
        enabled: true,
        onlyInViewport: true
      },
      navigation: {
        prevEl: page.querySelector('[data-promo-prev]'),
        nextEl: page.querySelector('[data-promo-next]')
      },
      breakpoints: {
        720: {
          slidesPerView: 2,
          spaceBetween: 10
        }
      }
    });
  }

  function initializeHotSaleCarousel() {
    const element = page.querySelector('.catalog-hot-sale-swiper');
    if (!element || typeof window.Swiper !== 'function') return;

    new window.Swiper(element, {
      slidesPerView: 1.35,
      slidesPerGroup: 1,
      spaceBetween: 8,
      speed: reducedMotion ? 0 : 350,
      grabCursor: true,
      watchOverflow: true,
      keyboard: {
        enabled: true,
        onlyInViewport: true
      },
      navigation: {
        prevEl: page.querySelector('[data-hot-sale-prev]'),
        nextEl: page.querySelector('[data-hot-sale-next]')
      },
      breakpoints: {
        480: {
          slidesPerView: 2.15,
          slidesPerGroup: 2
        },
        768: {
          slidesPerView: 3,
          slidesPerGroup: 3
        },
        1024: {
          slidesPerView: 5,
          slidesPerGroup: 5
        }
      }
    });
  }

  function initializeStickyFilter() {
    const sentinel = page.querySelector('[data-filter-sentinel]');
    const filter = page.querySelector('[data-filter-shell]');
    if (!sentinel || !filter || !('IntersectionObserver' in window)) return;

    const headerHeight = setStickyOffset();
    const siteHeader = document.getElementById('site-header');

    if (siteHeader && 'ResizeObserver' in window) {
      new ResizeObserver(setStickyOffset).observe(siteHeader);
    }

    const observer = new IntersectionObserver((entries) => {
      const entry = entries[0];
      const isStuck = !entry.isIntersecting && entry.boundingClientRect.top < headerHeight;

      filter.classList.toggle('is-stuck', isStuck);
    }, {
      threshold: 0,
      rootMargin: `-${headerHeight}px 0px 0px 0px`
    });

    observer.observe(sentinel);
  }

  function initializeFilterDropdowns() {
    const form = page.querySelector('[data-catalog-filter-form]');
    if (!form) return;

    const groups = Array.from(form.querySelectorAll('[data-filter-group]'));
    const status = form.querySelector('[data-filter-status]');
    let openGroup = null;

    const updateGroupState = (group) => {
      const checkedCount = group.querySelectorAll(
        '.catalog-filter-option input:checked'
      ).length;
      const count = group.querySelector('[data-filter-count]');

      group.classList.toggle('has-selection', checkedCount > 0);
      if (count) {
        count.textContent = String(checkedCount);
        count.hidden = checkedCount === 0;
      }

      if (status) {
        const total = form.querySelectorAll(
          '.catalog-filter-option input:checked'
        ).length;
        status.textContent = total > 0
          ? `Đã chọn ${total} tiêu chí. Nhấn Xem kết quả để áp dụng.`
          : 'Chưa chọn tiêu chí lọc.';
      }
    };

    const restoreGroup = (group) => {
      group.querySelectorAll('.catalog-filter-option input').forEach((input) => {
        input.checked = input.dataset.initialChecked === 'true';
        input.closest('.catalog-filter-option')?.classList.toggle(
          'is-selected',
          input.checked
        );
      });
      updateGroupState(group);
    };

    const closeGroup = (group, restore = false) => {
      if (!group) return;

      if (restore) restoreGroup(group);

      const trigger = group.querySelector('[data-filter-trigger]');
      const panel = group.querySelector('[data-filter-panel]');

      group.classList.remove('is-open');
      trigger?.setAttribute('aria-expanded', 'false');
      if (panel) {
        panel.hidden = true;
        panel.style.removeProperty('top');
        panel.style.removeProperty('left');
        panel.style.removeProperty('max-height');
      }

      if (openGroup === group) openGroup = null;
    };

    const positionPanel = (group) => {
      const trigger = group.querySelector('[data-filter-trigger]');
      const panel = group.querySelector('[data-filter-panel]');
      if (!trigger || !panel || panel.hidden) return;

      const viewportPadding = 12;
      const gap = 8;
      const triggerRect = trigger.getBoundingClientRect();
      const panelWidth = panel.getBoundingClientRect().width;
      const left = Math.min(
        Math.max(viewportPadding, triggerRect.left),
        Math.max(viewportPadding, window.innerWidth - panelWidth - viewportPadding)
      );
      const spaceBelow = window.innerHeight - triggerRect.bottom - gap - viewportPadding;
      const spaceAbove = triggerRect.top - gap - viewportPadding;
      const opensAbove = spaceBelow < 190 && spaceAbove > spaceBelow;
      const maxHeight = Math.max(170, opensAbove ? spaceAbove : spaceBelow);

      panel.style.left = `${Math.round(left)}px`;
      panel.style.maxHeight = `${Math.floor(maxHeight)}px`;

      if (opensAbove) {
        const panelHeight = Math.min(panel.scrollHeight, maxHeight);
        panel.style.top = `${Math.max(
          viewportPadding,
          Math.round(triggerRect.top - panelHeight - gap)
        )}px`;
      } else {
        panel.style.top = `${Math.round(triggerRect.bottom + gap)}px`;
      }
    };

    const openFilterGroup = (group) => {
      if (openGroup && openGroup !== group) {
        closeGroup(openGroup);
      }

      const trigger = group.querySelector('[data-filter-trigger]');
      const panel = group.querySelector('[data-filter-panel]');
      if (!trigger || !panel) return;

      panel.hidden = false;
      group.classList.add('is-open');
      trigger.setAttribute('aria-expanded', 'true');
      openGroup = group;
      positionPanel(group);
    };

    groups.forEach((group) => {
      const trigger = group.querySelector('[data-filter-trigger]');

      trigger?.addEventListener('click', () => {
        if (group.classList.contains('is-open')) {
          closeGroup(group);
        } else {
          openFilterGroup(group);
        }
      });

      group.querySelector('[data-filter-close]')?.addEventListener('click', () => {
        closeGroup(group, true);
        trigger?.focus();
      });

      group.querySelectorAll('.catalog-filter-option input').forEach((input) => {
        input.addEventListener('change', () => {
          input.closest('.catalog-filter-option')?.classList.toggle(
            'is-selected',
            input.checked
          );
          updateGroupState(group);
        });
      });
    });

    document.addEventListener('click', (event) => {
      if (!openGroup || openGroup.contains(event.target)) return;
      closeGroup(openGroup, true);
    });

    document.addEventListener('keydown', (event) => {
      if (event.key !== 'Escape' || !openGroup) return;

      const trigger = openGroup.querySelector('[data-filter-trigger]');
      closeGroup(openGroup, true);
      trigger?.focus();
    });

    window.addEventListener('resize', () => {
      if (openGroup) positionPanel(openGroup);
    });

    window.addEventListener('scroll', () => {
      if (openGroup) closeGroup(openGroup);
    }, { passive: true });

    form.addEventListener('submit', () => {
      form.setAttribute('aria-busy', 'true');
      form.querySelectorAll('.catalog-filter-panel__apply').forEach((button) => {
        button.disabled = true;
        button.textContent = 'Đang lọc...';
      });
    });
  }

  function initializeSectionNavigation() {
    const sentinel = page.querySelector('[data-section-nav-sentinel]');
    const endSentinel = page.querySelector('[data-section-nav-end]');
    const nav = page.querySelector('[data-section-nav-shell]');
    const links = Array.from(page.querySelectorAll('[data-section-nav-link]'));
    const sections = Array.from(page.querySelectorAll('[data-section-target]'));

    if (!nav || links.length === 0) return;

    let headerHeight = setStickyOffset();
    let sectionNavigationHideTimer = 0;
    let sectionNavigationFrame = 0;
    const siteHeader = document.getElementById('site-header');

    if (siteHeader && 'ResizeObserver' in window) {
      new ResizeObserver(() => {
        headerHeight = setStickyOffset();
        updateNavByScrollPosition();
      }).observe(siteHeader);
    }

    const setNavVisible = (isVisible) => {
      nav.classList.toggle('is-stuck', isVisible);
      nav.setAttribute('aria-hidden', String(!isVisible));

      links.forEach((link) => {
        if (isVisible) {
          link.removeAttribute('tabindex');
        } else {
          link.setAttribute('tabindex', '-1');
        }
      });
    };

    const getSectionNavigationRange = () => {
      if (sections.length === 0) return null;

      const firstSection = sections[0];
      const lastSection = sections[sections.length - 1];
      const navHeight = Math.ceil(nav.getBoundingClientRect().height || 56);
      const scrollTop = window.scrollY || window.pageYOffset || 0;
      const start = scrollTop + firstSection.getBoundingClientRect().top - headerHeight - navHeight - 28;
      const end = scrollTop + lastSection.getBoundingClientRect().bottom - headerHeight - navHeight;

      return { start, end };
    };

    function updateNavByScrollPosition() {
      const range = getSectionNavigationRange();
      if (!range) return;

      const scrollTop = window.scrollY || window.pageYOffset || 0;
      setNavVisible(scrollTop >= range.start && scrollTop < range.end);
    }

    const requestNavVisibilityUpdate = () => {
      if (sectionNavigationFrame) return;

      sectionNavigationFrame = window.requestAnimationFrame(() => {
        sectionNavigationFrame = 0;
        updateNavByScrollPosition();
      });
    };

    const keepNavVisibleDuringScroll = () => {
      window.clearTimeout(sectionNavigationHideTimer);
      setNavVisible(true);
      window.requestAnimationFrame(updateNavByScrollPosition);
      sectionNavigationHideTimer = window.setTimeout(
        updateNavByScrollPosition,
        reducedMotion ? 40 : 420
      );
    };

    const setActive = (id) => {
      links.forEach((link) => {
        const isActive = link.dataset.sectionNavLink === id;

        link.classList.toggle('is-active', isActive);
        if (isActive) {
          link.setAttribute('aria-current', 'true');
        } else {
          link.removeAttribute('aria-current');
        }
      });
    };

    links.forEach((link) => {
      link.addEventListener('click', (event) => {
        const target = document.querySelector(link.hash);
        if (!target) return;

        event.preventDefault();
        setActive(link.dataset.sectionNavLink);
        keepNavVisibleDuringScroll();

        target.scrollIntoView({
          behavior: reducedMotion ? 'auto' : 'smooth',
          block: 'start'
        });

        if (history.pushState) {
          history.pushState(null, '', link.hash);
        }
      });
    });

    if (window.location.hash) {
      const initialTarget = document.querySelector(window.location.hash);
      if (initialTarget?.dataset.sectionTarget) {
        setActive(initialTarget.dataset.sectionTarget);
      }
    }

    setNavVisible(false);
    window.addEventListener('scroll', requestNavVisibilityUpdate, { passive: true });

    if (!('IntersectionObserver' in window)) {
      updateNavByScrollPosition();
      return;
    }

    if (sentinel) {
      const stickyObserver = new IntersectionObserver(updateNavByScrollPosition, {
        threshold: 0,
        rootMargin: `-${headerHeight}px 0px 0px 0px`
      });

      stickyObserver.observe(sentinel);
    }

    if (endSentinel) {
      const endObserver = new IntersectionObserver(updateNavByScrollPosition, {
        threshold: 0,
        rootMargin: `-${headerHeight}px 0px 0px 0px`
      });

      endObserver.observe(endSentinel);
    }

    window.requestAnimationFrame(updateNavByScrollPosition);

    if (sections.length === 0) return;

    const sectionObserver = new IntersectionObserver((entries) => {
      const visibleEntry = entries
        .filter((entry) => entry.isIntersecting)
        .sort((first, second) => first.boundingClientRect.top - second.boundingClientRect.top)[0];

      const id = visibleEntry?.target?.dataset.sectionTarget;
      if (id) setActive(id);
    }, {
      threshold: [0.12, 0.35, 0.6],
      rootMargin: `-${headerHeight + 70}px 0px -52% 0px`
    });

    sections.forEach((section) => sectionObserver.observe(section));
  }

  function initializeSectionPills() {
    const pills = Array.from(page.querySelectorAll('[data-section-pill-link]'));
    if (pills.length === 0) return;

    const setActivePill = (pill) => {
      const pillGroup = pill.closest('.catalog-section-pill-nav');
      if (!pillGroup) return;

      pillGroup.querySelectorAll('[data-section-pill-link]').forEach((item) => {
        const isActive = item === pill;

        item.classList.toggle('is-active', isActive);
        if (isActive) {
          item.setAttribute('aria-current', 'true');
        } else {
          item.removeAttribute('aria-current');
        }
      });
    };

    pills.forEach((pill) => {
      pill.addEventListener('click', (event) => {
        const section = pill.closest('[data-section-target]');
        if (!section) return;

        event.preventDefault();
        setActivePill(pill);

        const targetUrl = new URL(pill.href, window.location.origin);
        targetUrl.hash = section.id;

        if (history.replaceState) {
          history.replaceState(null, '', `${targetUrl.pathname}${targetUrl.search}${targetUrl.hash}`);
        }

        section.scrollIntoView({
          behavior: reducedMotion ? 'auto' : 'smooth',
          block: 'start'
        });
      });
    });
  }

  function initializeSectionReveals() {
    const items = Array.from(page.querySelectorAll('.catalog-section-product-grid__item'));
    if (items.length === 0) return;

    if (reducedMotion || !('IntersectionObserver' in window)) {
      items.forEach((item) => item.classList.add('is-revealed'));
      return;
    }

    const observer = new IntersectionObserver((entries) => {
      entries.forEach((entry) => {
        if (!entry.isIntersecting) return;

        const item = entry.target;
        const index = Number.parseInt(item.dataset.revealIndex || '0', 10);

        item.style.animationDelay = `${Math.min(index % 10, 9) * 28}ms`;
        item.classList.add('is-revealed');
        observer.unobserve(item);
      });
    }, {
      threshold: 0.08,
      rootMargin: '0px 0px -8% 0px'
    });

    items.forEach((item, index) => {
      item.dataset.revealIndex = String(index);
      observer.observe(item);
    });
  }

  function setStickyOffset() {
    const siteHeader = document.getElementById('site-header');
    const fallbackHeight = Number.parseInt(
      getComputedStyle(document.documentElement).getPropertyValue('--header-height'),
      10
    ) || 100;
    const headerHeight = Math.ceil(siteHeader?.getBoundingClientRect().height || fallbackHeight);

    page.style.setProperty('--catalog-sticky-offset', `${headerHeight}px`);

    return headerHeight;
  }

  function initializeCountdowns() {
    page.querySelectorAll('[data-hot-sale-countdown]').forEach((countdown) => {
      const target = Date.parse(countdown.dataset.endsAt || '');
      if (Number.isNaN(target)) return;

      const hours = countdown.querySelector('[data-countdown-hours]');
      const minutes = countdown.querySelector('[data-countdown-minutes]');
      const seconds = countdown.querySelector('[data-countdown-seconds]');

      const update = () => {
        const remaining = Math.max(0, target - Date.now());
        const totalSeconds = Math.floor(remaining / 1000);
        const hourValue = Math.floor(totalSeconds / 3600);
        const minuteValue = Math.floor((totalSeconds % 3600) / 60);
        const secondValue = totalSeconds % 60;

        if (hours) hours.textContent = String(hourValue).padStart(2, '0');
        if (minutes) minutes.textContent = String(minuteValue).padStart(2, '0');
        if (seconds) seconds.textContent = String(secondValue).padStart(2, '0');

        return remaining;
      };

      update();
      const timer = window.setInterval(() => {
        if (update() <= 0) {
          window.clearInterval(timer);
        }
      }, 1000);
    });
  }

  function initializeLoadMore() {
    const button = page.querySelector('[data-load-more]');
    const grid = page.querySelector('[data-product-grid]');
    if (!button || !grid) return;

    button.addEventListener('click', () => {
      if (button.getAttribute('aria-busy') === 'true') return;

      const hiddenItems = Array.from(
        grid.querySelectorAll('[data-product-grid-item][hidden]')
      );
      const batchSize = Number.parseInt(button.dataset.batchSize || '5', 10);
      const nextItems = hiddenItems.slice(0, batchSize);

      if (!nextItems.length) {
        button.closest('.catalog-load-more')?.setAttribute('hidden', '');
        return;
      }

      button.setAttribute('aria-busy', 'true');
      grid.setAttribute('aria-busy', 'true');
      const label = button.querySelector('span');
      if (label) label.textContent = 'Đang tải sản phẩm';

      window.setTimeout(() => {
        nextItems.forEach((item, index) => {
          item.hidden = false;
          item.style.animationDelay = reducedMotion ? '0ms' : `${index * 35}ms`;
          item.classList.add('is-revealed');
        });

        const remaining = grid.querySelectorAll('[data-product-grid-item][hidden]').length;
        button.setAttribute('aria-busy', 'false');
        grid.setAttribute('aria-busy', 'false');

        if (remaining === 0) {
          button.closest('.catalog-load-more')?.setAttribute('hidden', '');
        } else if (label) {
          label.textContent = `Xem thêm ${remaining} sản phẩm`;
        }
      }, reducedMotion ? 0 : 220);
    });
  }

  function initializeSeoContent() {
    const section = page.querySelector('[data-seo-content]');
    const toggle = section?.querySelector('[data-seo-toggle]');
    if (!section || !toggle) return;

    toggle.addEventListener('click', () => {
      const expanded = section.classList.toggle('is-expanded');
      const label = toggle.querySelector('span');

      toggle.setAttribute('aria-expanded', String(expanded));
      if (label) label.textContent = expanded ? 'Thu gọn' : 'Xem thêm';
    });
  }

  function initializeQuestionAnswer() {
    const section = page.querySelector('[data-question-answer]');
    if (!section) return;

    section.querySelector('[data-qa-show-more]')?.addEventListener('click', (event) => {
      section.querySelectorAll('[data-qa-thread][hidden]').forEach((thread) => {
        thread.hidden = false;
      });

      event.currentTarget.closest('.pd-qa-more')?.setAttribute('hidden', '');
    });

    const input = section.querySelector('[data-qa-input]');
    const submit = section.querySelector('[data-qa-submit]');
    const feedback = section.querySelector('[data-qa-feedback]');

    submit?.addEventListener('click', () => {
      const value = input?.value.trim() || '';

      if (value.length < 10) {
        if (feedback) {
          feedback.textContent = 'Vui lòng nhập câu hỏi có ít nhất 10 ký tự.';
          feedback.style.color = '#dc2626';
        }
        input?.focus();
        return;
      }

      if (feedback) {
        feedback.textContent = 'Câu hỏi đã được ghi nhận. TechStore sẽ phản hồi sớm.';
        feedback.style.color = '#15803d';
      }

      if (input) input.value = '';
    });
  }
})();
