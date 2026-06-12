(function () {
  'use strict';

  /* ================================================================
     checkout.js
     - Google Maps modal
     - Payment method toggle
     - Vietnamese address cascade via provinces.open-api.vn
  ================================================================ */

  /* ── Helpers ─────────────────────────────────────────────────── */

  /**
   * Populate a <select> element with an array of {code, name} items.
   * The first option is a placeholder that resets dependent dropdowns.
   */
  function populateSelect(selectEl, items, placeholder) {
    selectEl.innerHTML = `<option value="">${placeholder}</option>`;
    items.forEach(({ code, name }) => {
      const opt = document.createElement('option');
      opt.value = code;
      opt.textContent = name;
      selectEl.appendChild(opt);
    });
    selectEl.disabled = false;
  }

  function resetSelect(selectEl, placeholder) {
    selectEl.innerHTML = `<option value="">${placeholder}</option>`;
    selectEl.disabled = true;
    selectEl.value = '';
  }

  function setLoading(selectEl, loading) {
    if (loading) {
      selectEl.innerHTML = '<option value="">Đang tải...</option>';
      selectEl.disabled = true;
    }
  }

  /* ── Address cascade ─────────────────────────────────────────── */
  const provinceSelect = document.getElementById('co-province');
  const districtSelect = document.getElementById('co-district');
  const wardSelect     = document.getElementById('co-ward');

  const API_BASE = 'https://provinces.open-api.vn/api';

  // Load all provinces on page init
  if (provinceSelect) {
    fetch(`${API_BASE}/p/`)
      .then((r) => r.json())
      .then((provinces) => {
        populateSelect(
          provinceSelect,
          provinces.map((p) => ({ code: p.code, name: p.name })),
          '-- Chọn Tỉnh / Thành phố --'
        );

        // Restore server-side selected value after form validation failure
        const serverValue = provinceSelect.dataset.serverValue;
        if (serverValue) provinceSelect.value = serverValue;
      })
      .catch(() => {
        provinceSelect.innerHTML = '<option value="">Không tải được dữ liệu</option>';
      });
  }

  provinceSelect?.addEventListener('change', () => {
    const code = provinceSelect.value;
    resetSelect(districtSelect, '-- Chọn Quận / Huyện --');
    resetSelect(wardSelect, '-- Chọn Phường / Xã --');

    if (!code) return;

    setLoading(districtSelect, true);
    fetch(`${API_BASE}/p/${code}?depth=2`)
      .then((r) => r.json())
      .then((province) => {
        populateSelect(
          districtSelect,
          (province.districts || []).map((d) => ({ code: d.code, name: d.name })),
          '-- Chọn Quận / Huyện --'
        );
      })
      .catch(() => {
        districtSelect.innerHTML = '<option value="">Không tải được dữ liệu</option>';
        districtSelect.disabled = false;
      });
  });

  districtSelect?.addEventListener('change', () => {
    const code = districtSelect.value;
    resetSelect(wardSelect, '-- Chọn Phường / Xã --');

    if (!code) return;

    setLoading(wardSelect, true);
    fetch(`${API_BASE}/d/${code}?depth=2`)
      .then((r) => r.json())
      .then((district) => {
        populateSelect(
          wardSelect,
          (district.wards || []).map((w) => ({ code: w.code, name: w.name })),
          '-- Chọn Phường / Xã --'
        );
      })
      .catch(() => {
        wardSelect.innerHTML = '<option value="">Không tải được dữ liệu</option>';
        wardSelect.disabled = false;
      });
  });

  /* ── Google Map modal ────────────────────────────────────────── */
  const mapModal      = document.getElementById('co-map-modal');
  const mapOpenBtn    = document.getElementById('co-map-open');
  const mapCloseBtn   = document.getElementById('co-map-close');
  const mapIframe     = document.getElementById('co-map-iframe');
  const MAP_EMBED_SRC =
    'https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d125416.39705649617!2d106.62829!3d10.82302!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x317529292e8d3dd1%3A0xf15f5aad773c112b!2sHo+Chi+Minh+City!5e0!3m2!1sen!2s!4v1';

  function openMap() {
    if (!mapModal) return;
    if (mapIframe && !mapIframe.src.includes('google.com/maps')) {
      mapIframe.src = MAP_EMBED_SRC;
    }
    mapModal.classList.add('is-open');
    document.body.style.overflow = 'hidden';
    mapCloseBtn?.focus();
  }

  function closeMap() {
    if (!mapModal) return;
    mapModal.classList.remove('is-open');
    document.body.style.overflow = '';
    mapOpenBtn?.focus();
  }

  mapOpenBtn?.addEventListener('click', openMap);
  mapCloseBtn?.addEventListener('click', closeMap);
  mapModal?.addEventListener('click', (e) => { if (e.target === mapModal) closeMap(); });
  document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && mapModal?.classList.contains('is-open')) closeMap();
  });

  /* ── Payment method switching ────────────────────────────────── */
  const paymentRadios = document.querySelectorAll('[name="PaymentMethod"]');
  const codInfoBlock  = document.getElementById('co-cod-info');

  function syncPaymentInfo() {
    const selected = document.querySelector('[name="PaymentMethod"]:checked');
    if (!codInfoBlock) return;
    codInfoBlock.hidden = selected?.value !== '0';
  }

  paymentRadios.forEach((r) => r.addEventListener('change', syncPaymentInfo));
  syncPaymentInfo();
})();
