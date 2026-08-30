/**
 * Musical Zed - Teste de Performance: Carrinho
 * Fluxo: ver carrinho → add → update → delete → limpar
 */
import http from 'k6/http';
import { group, check } from 'k6';
import { API_BASE, checkResponse, cartOperationTime, generateSessionId, jsonHeaders, randomSleep } from '../utils/helpers.js';
import { defaultThresholds } from '../config/thresholds.js';

export const options = {
  scenarios: {
    cart_users: {
      executor: 'ramping-vus',
      startVUs: 1,
      stages: [
        { duration: '15s', target: 15 },
        { duration: '30s', target: 15 },
        { duration: '15s', target: 0 },
      ],
    },
  },
  thresholds: {
    ...defaultThresholds,
    cart_operation_time: ['p(95)<1000'],
  },
};

export default function () {
  const sessionId = generateSessionId();
  let productId = null;

  group('Obter Produto', () => {
    const r = http.get(`${API_BASE}/api/products`, { headers: jsonHeaders });
    if (r.status === 200) {
      const prods = JSON.parse(r.body);
      if (prods.length > 0) productId = prods[Math.floor(Math.random() * prods.length)].id;
    }
  });

  if (!productId) return;
  randomSleep(0.3, 0.7);

  group('Carrinho Vazio', () => {
    const r = http.get(`${API_BASE}/api/carts/${sessionId}`, { headers: jsonHeaders });
    checkResponse(r, 'GET cart (empty)');
  });

  randomSleep(0.3, 0.7);

  group('Adicionar Item', () => {
    const start = Date.now();
    const r = http.post(
      `${API_BASE}/api/carts/${sessionId}/items`,
      JSON.stringify({ productId, quantity: 1 }),
      { headers: jsonHeaders }
    );
    cartOperationTime.add(Date.now() - start);
    check(r, {
      'add to cart 200': (r) => r.status === 200,
      'cart has items':  (r) => r.status === 200 && JSON.parse(r.body).items?.length > 0,
    });
  });

  randomSleep(0.5, 1);

  group('Atualizar Quantidade', () => {
    const start = Date.now();
    const r = http.put(
      `${API_BASE}/api/carts/${sessionId}/items/${productId}`,
      JSON.stringify({ quantity: 3 }),
      { headers: jsonHeaders }
    );
    cartOperationTime.add(Date.now() - start);
    checkResponse(r, 'PUT cart item');
  });

  randomSleep(0.5, 1);

  group('Limpar Carrinho', () => {
    const start = Date.now();
    const r = http.del(`${API_BASE}/api/carts/${sessionId}`, null, { headers: jsonHeaders });
    cartOperationTime.add(Date.now() - start);
    check(r, { 'clear cart 204': (r) => r.status === 204 });
  });

  randomSleep(1, 2);
}
