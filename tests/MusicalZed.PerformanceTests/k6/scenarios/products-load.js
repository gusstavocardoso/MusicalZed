/**
 * Musical Zed - Teste de Carga: Produtos
 * Listagem, busca e detalhe de produto
 */
import http from 'k6/http';
import { group } from 'k6';
import { API_BASE, checkResponse, productLoadTime, jsonHeaders, randomSleep } from '../utils/helpers.js';
import { defaultThresholds } from '../config/thresholds.js';

export const options = {
  scenarios: {
    constant_load: {
      executor: 'constant-vus', vus: 10, duration: '30s',
    },
    spike: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: '10s', target: 30 },
        { duration: '20s', target: 50 },
        { duration: '10s', target: 0 },
      ],
      startTime: '35s',
    },
  },
  thresholds: {
    ...defaultThresholds,
    product_load_time: ['p(95)<1500'],
  },
};

export default function () {
  let categories = [];

  group('Listagem de Produtos', () => {
    const start = Date.now();
    const r = http.get(`${API_BASE}/api/products`, { headers: jsonHeaders });
    productLoadTime.add(Date.now() - start);
    checkResponse(r, 'GET /api/products');
    if (r.status === 200) {
      const products = JSON.parse(r.body);
      categories = [...new Set(products.map(p => p.categoryId))];
    }
  });

  randomSleep(0.5, 1);

  group('Busca Textual', () => {
    const terms = ['Guitarra', 'Fender', 'Piano', 'Baixo', 'Bateria', 'Roland'];
    const term = terms[Math.floor(Math.random() * terms.length)];
    const r = http.get(`${API_BASE}/api/products?search=${encodeURIComponent(term)}`, { headers: jsonHeaders });
    checkResponse(r, `GET /api/products?search=${term}`);
  });

  randomSleep(0.5, 1);

  group('Filtro por Categoria', () => {
    if (categories.length > 0) {
      const catId = categories[Math.floor(Math.random() * categories.length)];
      const r = http.get(`${API_BASE}/api/products?categoryId=${catId}`, { headers: jsonHeaders });
      checkResponse(r, `GET /api/products?categoryId=${catId}`);
    }
  });

  randomSleep(0.5, 1);

  group('Detalhe do Produto', () => {
    const listR = http.get(`${API_BASE}/api/products`, { headers: jsonHeaders });
    if (listR.status === 200) {
      const prods = JSON.parse(listR.body);
      if (prods.length > 0) {
        const p = prods[Math.floor(Math.random() * prods.length)];
        const start = Date.now();
        const r = http.get(`${API_BASE}/api/products/${p.id}`, { headers: jsonHeaders });
        productLoadTime.add(Date.now() - start);
        checkResponse(r, `GET /api/products/${p.id}`);
      }
    }
  });

  randomSleep(1, 2);
}
