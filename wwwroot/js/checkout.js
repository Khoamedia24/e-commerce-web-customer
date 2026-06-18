(function () {
  'use strict';

  /* ================================================================
     checkout.js
     - Vietnamese address cascade via provinces.open-api.vn
     - Google Maps location picker with reverse geocoding
     - Payment method toggle
  ================================================================ */

  const provinceSelect = document.getElementById('co-province');
  const districtSelect = document.getElementById('co-district');
  const wardSelect = document.getElementById('co-ward');
  const addressDetailInput = document.querySelector('[name="AddressDetail"]');

  const API_BASE = 'https://provinces.open-api.vn/api';
  const DEFAULT_MAP_CENTER = { lat: 10.8231, lng: 106.6297 };

  let provincesCache = [];
  const districtCache = new Map();
  const wardCache = new Map();
  let provincesPromise = null;

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

  function setLoading(selectEl) {
    selectEl.innerHTML = '<option value="">Đang tải...</option>';
    selectEl.disabled = true;
  }

  function normalizeAddressName(value) {
    return (value || '')
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .replace(/đ/g, 'd')
      .replace(/Đ/g, 'd')
      .toLowerCase()
      .replace(/\b(thanh pho|tp|tinh|quan|q|huyen|thi xa|tx|phuong|p|xa|thi tran|tt|city|district|ward|province)\b/g, '')
      .replace(/[^a-z0-9]/g, '');
  }

  function findByName(items, query) {
    const normalizedQuery = normalizeAddressName(query);
    if (!normalizedQuery) return null;

    return items.find((item) => normalizeAddressName(item.name) === normalizedQuery)
      || items.find((item) => {
        const normalizedName = normalizeAddressName(item.name);
        return normalizedName.includes(normalizedQuery)
          || normalizedQuery.includes(normalizedName);
      })
      || null;
  }

  function setFieldValue(field, value) {
    if (!field || !value) return;
    field.value = value;
    field.dispatchEvent(new Event('input', { bubbles: true }));
    field.dispatchEvent(new Event('change', { bubbles: true }));
  }

  async function loadProvinces() {
    if (provincesCache.length > 0) return provincesCache;
    if (provincesPromise) return provincesPromise;

    provincesPromise = fetch(`${API_BASE}/p/`)
      .then((response) => response.json())
      .then((provinces) => {
        provincesCache = provinces.map((province) => ({
          code: String(province.code),
          name: province.name
        }));

        if (provinceSelect) {
          populateSelect(
            provinceSelect,
            provincesCache,
            '-- Chọn Tỉnh / Thành phố --'
          );
        }

        return provincesCache;
      })
      .catch(() => {
        if (provinceSelect) {
          provinceSelect.innerHTML = '<option value="">Không tải được dữ liệu</option>';
          provinceSelect.disabled = false;
        }

        return [];
      });

    return provincesPromise;
  }

  async function loadDistricts(provinceCode) {
    const key = String(provinceCode || '');
    if (!key) return [];
    if (districtCache.has(key)) {
      const cached = districtCache.get(key);
      populateSelect(districtSelect, cached, '-- Chọn Quận / Huyện --');
      return cached;
    }

    setLoading(districtSelect);
    const province = await fetch(`${API_BASE}/p/${key}?depth=2`)
      .then((response) => response.json());

    const districts = (province.districts || []).map((district) => ({
      code: String(district.code),
      name: district.name
    }));

    districtCache.set(key, districts);
    populateSelect(districtSelect, districts, '-- Chọn Quận / Huyện --');
    return districts;
  }

  async function loadWards(districtCode) {
    const key = String(districtCode || '');
    if (!key) return [];
    if (wardCache.has(key)) {
      const cached = wardCache.get(key);
      populateSelect(wardSelect, cached, '-- Chọn Phường / Xã --');
      return cached;
    }

    setLoading(wardSelect);
    const district = await fetch(`${API_BASE}/d/${key}?depth=2`)
      .then((response) => response.json());

    const wards = (district.wards || []).map((ward) => ({
      code: String(ward.code),
      name: ward.name
    }));

    wardCache.set(key, wards);
    populateSelect(wardSelect, wards, '-- Chọn Phường / Xã --');
    return wards;
  }

  async function restoreServerAddress() {
    if (!provinceSelect || !districtSelect || !wardSelect) return;

    const provinceCode = provinceSelect.dataset.serverValue;
    const districtCode = districtSelect.dataset.serverValue;
    const wardCode = wardSelect.dataset.serverValue;

    if (!provinceCode) return;
    provinceSelect.value = provinceCode;

    if (!districtCode) return;
    const districts = await loadDistricts(provinceCode);
    if (districts.some((district) => district.code === districtCode)) {
      districtSelect.value = districtCode;
    }

    if (!wardCode) return;
    const wards = await loadWards(districtCode);
    if (wards.some((ward) => ward.code === wardCode)) {
      wardSelect.value = wardCode;
    }
  }

  if (provinceSelect && districtSelect && wardSelect) {
    loadProvinces().then(restoreServerAddress);

    provinceSelect.addEventListener('change', async () => {
      const code = provinceSelect.value;
      resetSelect(districtSelect, '-- Chọn Quận / Huyện --');
      resetSelect(wardSelect, '-- Chọn Phường / Xã --');

      if (!code) return;

      try {
        await loadDistricts(code);
      } catch {
        districtSelect.innerHTML = '<option value="">Không tải được dữ liệu</option>';
        districtSelect.disabled = false;
      }
    });

    districtSelect.addEventListener('change', async () => {
      const code = districtSelect.value;
      resetSelect(wardSelect, '-- Chọn Phường / Xã --');

      if (!code) return;

      try {
        await loadWards(code);
      } catch {
        wardSelect.innerHTML = '<option value="">Không tải được dữ liệu</option>';
        wardSelect.disabled = false;
      }
    });
  }

  /* ===========================================================================
     ĐOẠN CODE GOOGLE MAPS CŨ (ĐÃ ĐƯỢC COMMENT LẠI ĐỂ DÙNG LÀM THAM KHẢO / FALLBACK)
     ===========================================================================
  
  // const googleMapsConfigUrl = mapModal?.dataset.googleMapsConfigUrl || '/api/integrations/google-maps/config';
  // const googleMapsReverseGeocodeUrl = mapModal?.dataset.googleMapsReverseGeocodeUrl || '/api/integrations/google-maps/reverse-geocode';
  //
  // let googleMapsClientConfigPromise = null;
  // let googleMapsPromise = null;
  // let googleMapsAuthFailed = false;
  // let googleMapsAuthReject = null;
  // let googleMapsAuthHandlerInstalled = false;
  //
  // function getGoogleMapsErrorMessage(error) {
  //   const currentOrigin = window.location.origin;
  //   if (error?.message === 'missing-api-key') {
  //     return 'Chưa cấu hình Google Maps API key. Vui lòng thêm key vào appsettings.GoogleMaps.json.';
  //   }
  //   if (error?.message === 'google-maps-auth-failure') {
  //     return `Google Maps đang từ chối API key. Hãy kiểm tra Maps JavaScript API, Billing và thêm đúng domain hiện tại: ${currentOrigin}/*`;
  //   }
  //   return 'Không thể tải Google Maps. Vui lòng kiểm tra API key và kết nối mạng.';
  // }
  //
  // function handleGoogleMapsAuthFailure() {
  //   googleMapsAuthFailed = true;
  //   showMapCanvasError(
  //     'Google Maps chưa chấp nhận API key',
  //     `Kiểm tra Maps JavaScript API, Billing và thêm referrer: ${window.location.origin}/*`
  //   );
  //   setMapStatus(getGoogleMapsErrorMessage(new Error('google-maps-auth-failure')), 'error');
  //   if (mapUseBtn) mapUseBtn.disabled = true;
  //   if (googleMapsAuthReject) {
  //     googleMapsAuthReject(new Error('google-maps-auth-failure'));
  //     googleMapsAuthReject = null;
  //   }
  // }
  //
  // function installGoogleMapsAuthFailureHandler() {
  //   if (googleMapsAuthHandlerInstalled) return;
  //   const previousHandler = window.gm_authFailure;
  //   window.gm_authFailure = () => {
  //     if (typeof previousHandler === 'function') {
  //       previousHandler();
  //     }
  //     handleGoogleMapsAuthFailure();
  //   };
  //   googleMapsAuthHandlerInstalled = true;
  // }
  //
  // async function loadGoogleMapsClientConfig() {
  //   if (googleMapsClientConfigPromise) return googleMapsClientConfigPromise;
  //   googleMapsClientConfigPromise = fetch(googleMapsConfigUrl, {
  //     headers: { Accept: 'application/json' }
  //   })
  //     .then(async (response) => {
  //       const payload = await response.json().catch(() => null);
  //       if (!response.ok || !payload?.isConfigured || !payload?.apiKey) {
  //         throw new Error(payload?.message || 'missing-api-key');
  //       }
  //       return payload;
  //     });
  //   return googleMapsClientConfigPromise;
  // }
  //
  // async function loadGoogleMapsApi() {
  //   if (googleMapsAuthFailed) throw new Error('google-maps-auth-failure');
  //   if (window.google?.maps) return Promise.resolve(window.google.maps);
  //   const clientConfig = await loadGoogleMapsClientConfig();
  //   if (googleMapsPromise) return googleMapsPromise;
  //   googleMapsAuthFailed = false;
  //   installGoogleMapsAuthFailureHandler();
  //   googleMapsPromise = new Promise((resolve, reject) => {
  //     const callbackName = 'techStoreCheckoutGoogleMapsReady';
  //     const existingScript = document.getElementById('google-maps-js-api');
  //     googleMapsAuthReject = reject;
  //     window[callbackName] = () => {
  //       window.setTimeout(() => {
  //         if (googleMapsAuthFailed) {
  //           reject(new Error('google-maps-auth-failure'));
  //           return;
  //         }
  //         googleMapsAuthReject = null;
  //         resolve(window.google.maps);
  //       }, 0);
  //     };
  //     if (existingScript) {
  //       existingScript.addEventListener('error', reject, { once: true });
  //       return;
  //     }
  //     const script = document.createElement('script');
  //     const params = new URLSearchParams({
  //       key: clientConfig.apiKey,
  //       callback: callbackName,
  //       language: 'vi',
  //       region: 'VN',
  //       libraries: 'places'
  //     });
  //     script.id = 'google-maps-js-api';
  //     script.async = true;
  //     script.defer = true;
  //     script.src = 'https://maps.googleapis.com/maps/api/js?' + params.toString();
  //     script.onerror = () => reject(new Error('google-maps-load-failed'));
  //     document.head.appendChild(script);
  //   }).catch((error) => {
  //     googleMapsPromise = null;
  //     throw error;
  //   });
  //   return googleMapsPromise;
  // }
  //
  // async function initCheckoutMap() {
  //   clearMapCanvasError();
  //   await loadGoogleMapsApi();
  //   if (googleMapsAuthFailed) throw new Error('google-maps-auth-failure');
  //   if (checkoutMap || !mapCanvas) return;
  //   checkoutMap = new google.maps.Map(mapCanvas, {
  //     center: DEFAULT_MAP_CENTER,
  //     zoom: 13,
  //     clickableIcons: false,
  //     fullscreenControl: true,
  //     mapTypeControl: false,
  //     streetViewControl: false
  //   });
  //   checkoutMarker = new google.maps.Marker({
  //     map: checkoutMap,
  //     draggable: true,
  //     visible: false
  //   });
  //   checkoutMap.addListener('click', (event) => {
  //     selectMapLocation(event.latLng);
  //   });
  //   checkoutMarker.addListener('dragend', (event) => {
  //     selectMapLocation(event.latLng);
  //   });
  //   window.setTimeout(() => {
  //     if (googleMapsAuthFailed) {
  //       handleGoogleMapsAuthFailure();
  //     }
  //   }, 400);
  // }
  //
  // function getComponent(components, typeGroups) {
  //   for (const types of typeGroups) {
  //     const match = components.find((component) =>
  //       types.every((type) => (component.types || []).includes(type))
  //     );
  //     if (match) return match.longName || match.long_name || '';
  //   }
  //   return '';
  // }
  //
  // function parseAddressComponents(result) {
  //   const components = result.addressComponents || result.address_components || [];
  //   const province = getComponent(components, [['administrative_area_level_1'], ['locality']]);
  //   const district = getComponent(components, [['administrative_area_level_2'], ['sublocality_level_1'], ['locality']]);
  //   const ward = getComponent(components, [['administrative_area_level_3'], ['administrative_area_level_4'], ['sublocality_level_2'], ['sublocality_level_3'], ['neighborhood']]);
  //   const streetNumber = getComponent(components, [['street_number']]);
  //   const route = getComponent(components, [['route']]);
  //   const premise = getComponent(components, [['premise'], ['point_of_interest'], ['establishment']]);
  //   return { province, district: district && normalizeAddressName(district) !== normalizeAddressName(province) ? district : '', ward, streetNumber, route, premise };
  // }
  //
  // function buildAddressDetail(result, parsedAddress) {
  //   const street = [parsedAddress.streetNumber, parsedAddress.route].filter(Boolean).join(' ').trim();
  //   if (street) return street;
  //   if (parsedAddress.premise) return parsedAddress.premise;
  //   const adminNames = [parsedAddress.ward, parsedAddress.district, parsedAddress.province, 'Việt Nam', 'Vietnam'].map(normalizeAddressName).filter(Boolean);
  //   return (result.formattedAddress || result.formatted_address || '').split(',').map((part) => part.trim()).filter((part) => {
  //     const normalizedPart = normalizeAddressName(part);
  //     return normalizedPart && !adminNames.some((name) => normalizedPart === name || normalizedPart.includes(name) || name.includes(normalizedPart));
  //   }).join(', ');
  // }
  //
  // async function fillAddressForm(result) {
  //   if (!provinceSelect || !districtSelect || !wardSelect) return;
  //   const parsedAddress = parseAddressComponents(result);
  //   await loadProvinces();
  //   const province = findByName(provincesCache, parsedAddress.province);
  //   if (province) {
  //     provinceSelect.value = province.code;
  //     resetSelect(districtSelect, '-- Chọn Quận / Huyện --');
  //     resetSelect(wardSelect, '-- Chọn Phường / Xã --');
  //     const districts = await loadDistricts(province.code);
  //     const district = findByName(districts, parsedAddress.district);
  //     if (district) {
  //       districtSelect.value = district.code;
  //       const wards = await loadWards(district.code);
  //       const ward = findByName(wards, parsedAddress.ward);
  //       if (ward) {
  //         wardSelect.value = ward.code;
  //       }
  //     }
  //   }
  //   const addressDetail = buildAddressDetail(result, parsedAddress);
  //   setFieldValue(addressDetailInput, addressDetail || result.formattedAddress || result.formatted_address || '');
  // }
  //
  // function toLatLngLiteral(latLng) {
  //   return {
  //     lat: typeof latLng.lat === 'function' ? latLng.lat() : latLng.lat,
  //     lng: typeof latLng.lng === 'function' ? latLng.lng() : latLng.lng
  //   };
  // }
  //
  // async function reverseGeocodeLocation(latLng) {
  //   if (!window.google?.maps?.Geocoder) {
  //     throw new Error('google-maps-not-loaded');
  //   }
  //   const geocoder = new google.maps.Geocoder();
  //   const coordinates = toLatLngLiteral(latLng);
  //   return new Promise((resolve, reject) => {
  //     geocoder.geocode({ location: coordinates }, (results, status) => {
  //       if (status === 'OK' && results && results[0]) {
  //         resolve(results[0]);
  //       } else {
  //         reject(new Error('reverse-geocode-failed'));
  //       }
  //     });
  //   });
  // }
  //
  // async function selectMapLocation(latLng) {
  //   if (!checkoutMarker) return;
  //   selectedLocation = latLng;
  //   checkoutMarker.setPosition(latLng);
  //   checkoutMarker.setVisible(true);
  //   checkoutMap.panTo(latLng);
  //   if (mapUseBtn) mapUseBtn.disabled = true;
  //   setMapStatus('Đang xác định địa chỉ từ vị trí đã chọn...', null);
  //   try {
  //     const result = await reverseGeocodeLocation(latLng);
  //     await fillAddressForm(result);
  //     if (mapUseBtn) mapUseBtn.disabled = false;
  //     setMapStatus('Đã tự động điền địa chỉ. Bạn có thể dùng vị trí này hoặc chọn lại trên bản đồ.', 'success');
  //   } catch (error) {
  //     setMapStatus(error?.message && error.message !== 'reverse-geocode-failed' ? error.message : 'Không thể xác định địa chỉ từ Google Maps. Vui lòng thử lại hoặc nhập thủ công.', 'error');
  //   }
  // }
  
     =========================================================================== */

  /* ── Mapbox location picker ─────────────────────────────── */
  const mapModal = document.getElementById('co-map-modal');
  const mapOpenBtn = document.getElementById('co-map-open');
  const mapCloseBtn = document.getElementById('co-map-close');
  const mapCanvas = document.getElementById('co-map-canvas');
  const mapStatus = document.getElementById('co-map-status');
  const mapUseBtn = document.getElementById('co-map-use');
  const mapboxConfigUrl = mapModal?.dataset.googleMapsConfigUrl || '/api/integrations/google-maps/config';

  let mapboxClientConfigPromise = null;
  let mapboxPromise = null;
  let mapboxToken = '';
  let checkoutMap = null;
  let checkoutMarker = null;
  let selectedLocation = null;

  function setMapStatus(message, tone) {
    if (!mapStatus) return;
    mapStatus.textContent = message;
    mapStatus.classList.toggle('is-success', tone === 'success');
    mapStatus.classList.toggle('is-error', tone === 'error');
  }

  function showMapCanvasError(title, description) {
    if (!mapCanvas) return;

    mapCanvas.querySelector('.co-map-error-panel')?.remove();

    const panel = document.createElement('div');
    panel.className = 'co-map-error-panel';
    panel.setAttribute('role', 'alert');
    panel.innerHTML = `
      <strong>${title}</strong>
      <span>${description}</span>
    `;

    mapCanvas.appendChild(panel);
  }

  function clearMapCanvasError() {
    mapCanvas?.querySelector('.co-map-error-panel')?.remove();
  }

  async function loadMapboxConfig() {
    if (mapboxClientConfigPromise) return mapboxClientConfigPromise;

    mapboxClientConfigPromise = fetch(mapboxConfigUrl, {
      headers: { Accept: 'application/json' }
    })
      .then(async (response) => {
        const payload = await response.json().catch(() => null);
        if (!response.ok || !payload?.isConfigured || !payload?.apiKey) {
          throw new Error('missing-api-key');
        }

        return payload;
      });

    return mapboxClientConfigPromise;
  }

  async function loadMapboxApi() {
    if (window.mapboxgl) return Promise.resolve(window.mapboxgl);
    if (mapboxPromise) return mapboxPromise;

    const clientConfig = await loadMapboxConfig();
    mapboxToken = clientConfig.apiKey;

    mapboxPromise = new Promise((resolve, reject) => {
      const link = document.createElement('link');
      link.href = 'https://api.mapbox.com/mapbox-gl-js/v3.4.0/mapbox-gl.css';
      link.rel = 'stylesheet';
      document.head.appendChild(link);

      const script = document.createElement('script');
      script.id = 'mapbox-js-api';
      script.src = 'https://api.mapbox.com/mapbox-gl-js/v3.4.0/mapbox-gl.js';
      script.async = true;
      script.defer = true;
      script.onload = () => {
        window.mapboxgl.accessToken = mapboxToken;
        resolve(window.mapboxgl);
      };
      script.onerror = () => reject(new Error('mapbox-load-failed'));
      document.head.appendChild(script);
    });

    return mapboxPromise;
  }

  async function initCheckoutMap() {
    clearMapCanvasError();
    await loadMapboxApi();
    if (checkoutMap || !mapCanvas) return;

    const center = selectedLocation
      ? [selectedLocation.lng, selectedLocation.lat]
      : [DEFAULT_MAP_CENTER.lng, DEFAULT_MAP_CENTER.lat];

    checkoutMap = new mapboxgl.Map({
      container: mapCanvas,
      style: 'mapbox://styles/mapbox/streets-v12',
      center: center,
      zoom: 13
    });

    checkoutMarker = new mapboxgl.Marker({
      draggable: true
    })
      .setLngLat(center)
      .addTo(checkoutMap);

    checkoutMap.on('click', (event) => {
      selectMapLocation(event.lngLat);
    });

    checkoutMarker.on('dragend', () => {
      const lngLat = checkoutMarker.getLngLat();
      selectMapLocation(lngLat);
    });
  }

  function parseMapboxAddress(feature) {
    const context = feature.context || [];
    let province = '';
    let district = '';
    let ward = '';

    context.forEach(item => {
      if (item.id.startsWith('region')) {
        province = item.text;
      } else if (item.id.startsWith('district') || item.id.startsWith('locality') || item.id.startsWith('place')) {
        if (!district) district = item.text;
      } else if (item.id.startsWith('neighborhood') || item.id.startsWith('postcode') || item.id.startsWith('ward')) {
        if (!ward) ward = item.text;
      }
    });

    if (feature.place_type.includes('neighborhood')) {
      ward = feature.text;
    } else if (feature.place_type.includes('district') || feature.place_type.includes('place')) {
      district = feature.text;
    } else if (feature.place_type.includes('region')) {
      province = feature.text;
    }

    let route = '';
    let streetNumber = '';
    if (feature.place_type.includes('address')) {
      route = feature.text;
      streetNumber = feature.address || '';
    }

    return {
      province,
      district,
      ward,
      streetNumber,
      route,
      formattedAddress: feature.place_name
    };
  }

  async function fillAddressForm(parsedAddress) {
    if (!provinceSelect || !districtSelect || !wardSelect) return;

    await loadProvinces();
    const province = findByName(provincesCache, parsedAddress.province);

    if (province) {
      provinceSelect.value = province.code;
      resetSelect(districtSelect, '-- Chọn Quận / Huyện --');
      resetSelect(wardSelect, '-- Chọn Phường / Xã --');

      const districts = await loadDistricts(province.code);
      const district = findByName(districts, parsedAddress.district);

      if (district) {
        districtSelect.value = district.code;

        const wards = await loadWards(district.code);
        const ward = findByName(wards, parsedAddress.ward);
        if (ward) {
          wardSelect.value = ward.code;
        }
      }
    }

    const addressDetail = [parsedAddress.streetNumber, parsedAddress.route]
      .filter(Boolean)
      .join(' ')
      .trim() || parsedAddress.formattedAddress.split(',')[0];

    setFieldValue(addressDetailInput, addressDetail);
  }

  async function reverseGeocodeLocation(lngLat) {
    const url = `https://api.mapbox.com/geocoding/v5/mapbox.places/${lngLat.lng},${lngLat.lat}.json?access_token=${mapboxToken}&language=vi`;
    const response = await fetch(url);
    if (!response.ok) {
      throw new Error('reverse-geocode-failed');
    }
    const data = await response.json();
    if (!data.features || data.features.length === 0) {
      throw new Error('reverse-geocode-failed');
    }
    return parseMapboxAddress(data.features[0]);
  }

  async function selectMapLocation(lngLat) {
    if (!checkoutMarker) return;

    selectedLocation = lngLat;
    checkoutMarker.setLngLat(lngLat);
    checkoutMap.panTo(lngLat);
    if (mapUseBtn) mapUseBtn.disabled = true;
    setMapStatus('Đang xác định địa chỉ từ vị trí đã chọn...', null);

    try {
      const parsedAddress = await reverseGeocodeLocation(lngLat);
      await fillAddressForm(parsedAddress);
      if (mapUseBtn) mapUseBtn.disabled = false;
      setMapStatus('Đã tự động điền địa chỉ. Bạn có thể dùng vị trí này hoặc chọn lại trên bản đồ.', 'success');
    } catch (error) {
      setMapStatus('Không thể xác định địa chỉ từ Mapbox. Vui lòng thử lại hoặc nhập thủ công.', 'error');
    }
  }

  function openMap() {
    if (!mapModal) return;

    mapModal.classList.add('is-open');
    document.body.style.overflow = 'hidden';
    mapCloseBtn?.focus();
    setMapStatus('Đang tải bản đồ Mapbox...', null);

    initCheckoutMap()
      .then(() => {
        window.setTimeout(() => {
          checkoutMap.resize();

          // Tự động định vị vị trí người dùng (chỉ hoạt động trên HTTPS hoặc localhost)
          if (navigator.geolocation && !selectedLocation) {
            setMapStatus('Đang xác định vị trí hiện tại của bạn...', null);
            navigator.geolocation.getCurrentPosition(
              (position) => {
                const userLngLat = {
                  lng: position.coords.longitude,
                  lat: position.coords.latitude
                };
                selectMapLocation(userLngLat);
              },
              (error) => {
                // Nếu người dùng chặn quyền hoặc lỗi, định vị theo tâm mặc định
                const center = [DEFAULT_MAP_CENTER.lng, DEFAULT_MAP_CENTER.lat];
                checkoutMap.setCenter(center);
                setMapStatus('Kéo ghim hoặc click trên bản đồ để chọn vị trí giao hàng.', null);
              },
              { enableHighAccuracy: true, timeout: 6000 }
            );
          } else {
            const center = selectedLocation
              ? [selectedLocation.lng, selectedLocation.lat]
              : [DEFAULT_MAP_CENTER.lng, DEFAULT_MAP_CENTER.lat];
            checkoutMap.setCenter(center);
            setMapStatus('Kéo ghim hoặc click trên bản đồ để chọn vị trí giao hàng.', null);
          }
        }, 200);
      })
      .catch((error) => {
        setMapStatus('Không thể tải bản đồ Mapbox. Vui lòng kiểm tra cấu hình.', 'error');
        showMapCanvasError('Lỗi tải bản đồ', 'Hãy kiểm tra lại Mapbox Access Token.');
      });
  }

  function closeMap() {
    if (!mapModal) return;
    mapModal.classList.remove('is-open');
    document.body.style.overflow = '';
    mapOpenBtn?.focus();
  }

  mapOpenBtn?.addEventListener('click', openMap);
  mapCloseBtn?.addEventListener('click', closeMap);
  mapUseBtn?.addEventListener('click', closeMap);
  mapModal?.addEventListener('click', (event) => {
    if (event.target === mapModal) closeMap();
  });
  document.addEventListener('keydown', (event) => {
    if (event.key === 'Escape' && mapModal?.classList.contains('is-open')) {
      closeMap();
    }
  });

  /* ── Payment method switching ────────────────────────────────── */
  const paymentRadios = document.querySelectorAll('[name="PaymentMethodId"]');
  const codInfoBlock = document.getElementById('co-cod-info');

  function syncPaymentInfo() {
    const selected = document.querySelector('[name="PaymentMethodId"]:checked');
    if (!codInfoBlock) return;
    codInfoBlock.hidden = selected?.dataset.paymentKind !== 'cod';
  }

  paymentRadios.forEach((radio) => radio.addEventListener('change', syncPaymentInfo));
  syncPaymentInfo();
})();
